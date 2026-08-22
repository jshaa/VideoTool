using VideoTool.Core.Models;

namespace VideoTool.Core.Services;

/// <summary>단일 업스케일 작업을 처음부터 끝까지 실행하는 오케스트레이터.
/// 전체 영상을 한 번에 프레임으로 풀면 4K 기준 수십GB에 달할 수 있으므로,
/// 청크(수백 프레임) 단위로 [추출 -> AI 업스케일 -> 인코딩 -> 정리]를 반복해 디스크 사용량을 제한한다.</summary>
public sealed class VideoUpscalePipeline
{
    private readonly FfmpegService _ffmpeg;
    private readonly RealEsrganService _realEsrgan;
    private readonly PyTorchRealEsrganService _pyTorchRealEsrgan;

    public event Action<JobProgress>? ProgressChanged;
    public event Action<string>? LogReceived;

    public VideoUpscalePipeline(FfmpegService ffmpeg, RealEsrganService realEsrgan, PyTorchRealEsrganService pyTorchRealEsrgan)
    {
        _ffmpeg = ffmpeg;
        _realEsrgan = realEsrgan;
        _pyTorchRealEsrgan = pyTorchRealEsrgan;
    }

    public async Task RunAsync(UpscaleJob job, CancellationToken ct = default)
    {
        var settings = job.Settings;
        using var workspace = new TempWorkspace(settings.TempRootDirectory, settings.KeepTempFiles);

        // Report()가 지역 함수라 try 블록 밖에서도 참조되므로, ETA 계산용 상태는 try 블록 바깥(메서드 스코프)에 둔다.
        DateTime? lastSampleAt = null;
        long lastSampleFrames = 0;
        double? smoothedFramesPerSecond = null;

        try
        {
            Report(JobStage.Probing, 0, "입력 파일 분석 중...");
            var info = await _ffmpeg.ProbeAsync(job.InputPath, ct).ConfigureAwait(false);
            job.SourceInfo = info;

            if (info.Width * 4 < settings.TargetWidth || info.Height * 4 < settings.TargetHeight)
            {
                Log($"경고: 원본 해상도({info.Width}x{info.Height})가 낮아 AI 4배 확대만으로는 목표 해상도({settings.TargetWidth}x{settings.TargetHeight})에 못 미칩니다. 부족분은 일반(Lanczos) 업스케일로 보정됩니다.");
            }

            // 빠른 모드: AI에 넣기 전 프레임을 1/2로 축소해서 연산량(≈픽셀 수)을 1/4로 줄인다.
            (int Width, int Height)? preScale = null;
            if (settings.FastPreDownscale)
            {
                var halfWidth = Math.Max(2, info.Width / 2 / 2 * 2);
                var halfHeight = Math.Max(2, info.Height / 2 / 2 * 2);
                preScale = (halfWidth, halfHeight);
                Log($"빠른 모드: AI 처리 전 {info.Width}x{info.Height} -> {halfWidth}x{halfHeight}로 축소합니다 (처리 시간 약 1/4로 단축, 디테일은 다소 손실).");
            }

            // 영상 전체가 아니라 일부 구간만 처리하고 싶을 때(예: 긴 영상의 일부만 테스트) 사용.
            // 지정 안 하면(둘 다 null/0) 기존과 동일하게 영상 전체를 처리한다.
            var segmentStart = settings.TrimStart is { } ts && ts > TimeSpan.Zero ? ts : TimeSpan.Zero;
            if (segmentStart > info.Duration)
                segmentStart = info.Duration;
            var availableAfterStart = info.Duration - segmentStart;
            var segmentDuration = settings.TrimDuration is { } td && td > TimeSpan.Zero && td < availableAfterStart
                ? td
                : availableAfterStart;

            if (segmentDuration <= TimeSpan.Zero)
                throw new InvalidOperationException($"처리할 구간이 없습니다 (구간 시작 {segmentStart:hh\\:mm\\:ss}이 영상 길이 {info.Duration:hh\\:mm\\:ss}를 벗어남).");

            if (segmentStart > TimeSpan.Zero || segmentDuration < info.Duration)
                Log($"AI 처리 구간: {segmentStart:hh\\:mm\\:ss} ~ {(segmentStart + segmentDuration):hh\\:mm\\:ss} (전체 길이 {info.Duration:hh\\:mm\\:ss} 중 {segmentDuration:hh\\:mm\\:ss}만 처리)");

            string? audioPath = null;
            if (info.HasAudio)
            {
                Report(JobStage.ExtractingAudio, 0, "오디오 추출 중...");
                var ok = await _ffmpeg.ExtractAudioAsync(job.InputPath, segmentStart, segmentDuration, workspace.AudioPath, settings.AudioBitrateKbps, Log, ct).ConfigureAwait(false);
                audioPath = ok ? workspace.AudioPath : null;
                if (!ok)
                    Log("경고: 오디오 추출에 실패해 무음 영상으로 진행합니다.");
            }

            var totalFrames = Math.Max(1, (long)Math.Round(segmentDuration.TotalSeconds * info.FrameRate));
            var chunkSeconds = Math.Max(settings.ChunkFrameCount, 1) / info.FrameRate;
            var totalChunks = Math.Max(1, (int)Math.Ceiling(segmentDuration.TotalSeconds / chunkSeconds));

            var partPaths = new List<string>();
            long framesDone = 0;

            for (var chunkIndex = 0; chunkIndex < totalChunks; chunkIndex++)
            {
                ct.ThrowIfCancellationRequested();

                var chunkOffset = TimeSpan.FromSeconds(chunkIndex * chunkSeconds);
                var remaining = segmentDuration - chunkOffset;
                if (remaining <= TimeSpan.Zero)
                    break;
                var thisChunkDuration = remaining.TotalSeconds < chunkSeconds ? remaining : TimeSpan.FromSeconds(chunkSeconds);
                var chunkStart = segmentStart + chunkOffset;

                workspace.ResetChunkDirs();

                Report(JobStage.Processing, PercentFor(framesDone, totalFrames),
                    $"프레임 추출 중 (청크 {chunkIndex + 1}/{totalChunks})", chunkIndex + 1, totalChunks, framesDone, totalFrames);
                await _ffmpeg.ExtractFramesAsync(job.InputPath, chunkStart, thisChunkDuration, workspace.ChunkFramesDir, settings.IntermediateFormat, Log, preScale, ct)
                    .ConfigureAwait(false);

                var chunkFrameCount = Directory.GetFiles(workspace.ChunkFramesDir).Length;
                if (chunkFrameCount == 0)
                {
                    Log($"청크 {chunkIndex + 1}에서 추출된 프레임이 없어 건너뜁니다.");
                    continue;
                }

                Action<(int Done, int Total)> onUpscaleProgress = p => Report(JobStage.Processing, PercentFor(framesDone + p.Done, totalFrames),
                    $"AI 업스케일 중 (청크 {chunkIndex + 1}/{totalChunks}, {p.Done}/{p.Total} 프레임)",
                    chunkIndex + 1, totalChunks, framesDone + p.Done, totalFrames);

                if (settings.Engine == UpscaleEngine.PyTorchCuda)
                {
                    await _pyTorchRealEsrgan.UpscaleFolderAsync(
                        workspace.ChunkFramesDir, workspace.ChunkUpscaledDir,
                        settings.IntermediateFormat, settings.PyTorchHalfPrecision, Log,
                        onUpscaleProgress, ct).ConfigureAwait(false);
                }
                else
                {
                    await _realEsrgan.UpscaleFolderAsync(
                        workspace.ChunkFramesDir, workspace.ChunkUpscaledDir,
                        settings.Model, settings.IntermediateFormat,
                        settings.GpuId, settings.TileSize, Log,
                        onUpscaleProgress, ct).ConfigureAwait(false);
                }

                Report(JobStage.Processing, PercentFor(framesDone, totalFrames),
                    $"청크 인코딩 중 ({chunkIndex + 1}/{totalChunks})", chunkIndex + 1, totalChunks, framesDone, totalFrames);
                var partPath = Path.Combine(workspace.PartsDir, $"part_{chunkIndex:D5}.mp4");
                await _ffmpeg.EncodeChunkAsync(
                    workspace.ChunkUpscaledDir, settings.IntermediateFormat, info.FrameRate,
                    settings.TargetWidth, settings.TargetHeight, settings.Codec, settings.Crf,
                    partPath, Log, ct).ConfigureAwait(false);

                partPaths.Add(partPath);
                framesDone += chunkFrameCount;
            }

            Report(JobStage.Concatenating, 95, "영상 조각 병합 중...");
            var videoOnlyPath = Path.Combine(workspace.RootDir, "video_only.mp4");
            await _ffmpeg.ConcatAsync(partPaths, videoOnlyPath, Log, ct).ConfigureAwait(false);

            Report(JobStage.Muxing, 98, "오디오 결합 및 최종 저장 중...");
            var outputDir = Path.GetDirectoryName(job.OutputPath);
            if (!string.IsNullOrEmpty(outputDir))
                Directory.CreateDirectory(outputDir);
            await _ffmpeg.MuxAsync(videoOnlyPath, audioPath, job.OutputPath, Log, ct).ConfigureAwait(false);

            Report(JobStage.Completed, 100, "완료");
        }
        catch (OperationCanceledException)
        {
            Report(JobStage.Canceled, job.PercentComplete, "취소됨");
            throw;
        }
        catch (Exception ex)
        {
            job.ErrorMessage = ex.Message;
            Report(JobStage.Failed, job.PercentComplete, $"오류: {ex.Message}");
            throw;
        }

        void Report(JobStage stage, double percent, string message, int currentChunk = 0, int totalChunks = 0, long framesDone = 0, long totalFrames = 0)
        {
            // 작업 시작 시점부터의 누적 평균 대신, 최근 진행 구간의 처리 속도(지수이동평균)로 ETA를 계산한다.
            // 누적 평균은 청크 첫 프레임 추출 등 초반의 느린/정지 구간이 평균을 오래 오염시켜
            // "남은 시간"이 실제보다 훨씬 크게 나오는 문제가 있었다.
            var now = DateTime.UtcNow;
            if (framesDone > lastSampleFrames)
            {
                if (lastSampleAt is not null)
                {
                    var intervalSeconds = (now - lastSampleAt.Value).TotalSeconds;
                    if (intervalSeconds > 0.05)
                    {
                        var instantRate = (framesDone - lastSampleFrames) / intervalSeconds;
                        const double alpha = 0.3;
                        smoothedFramesPerSecond = smoothedFramesPerSecond is null
                            ? instantRate
                            : (alpha * instantRate) + ((1 - alpha) * smoothedFramesPerSecond.Value);
                    }
                }
                lastSampleAt = now;
                lastSampleFrames = framesDone;
            }

            TimeSpan? eta = null;
            if (smoothedFramesPerSecond is > 0 && totalFrames > 0)
            {
                var remainingFrames = Math.Max(totalFrames - framesDone, 0);
                eta = TimeSpan.FromSeconds(remainingFrames / smoothedFramesPerSecond.Value);
            }

            job.Stage = stage;
            job.PercentComplete = percent;
            job.LastMessage = message;

            ProgressChanged?.Invoke(new JobProgress
            {
                Stage = stage,
                PercentComplete = percent,
                FramesDone = framesDone,
                TotalFrames = totalFrames,
                CurrentChunk = currentChunk,
                TotalChunks = totalChunks,
                Message = message,
                Eta = eta,
            });
        }
    }

    private static double PercentFor(long done, long total) => total <= 0 ? 0 : Math.Clamp(done * 100.0 / total, 0, 99);

    private void Log(string line) => LogReceived?.Invoke(line);
}
