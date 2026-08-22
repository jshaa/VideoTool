using System.Globalization;
using System.Text;
using System.Text.Json;
using VideoTool.Core.Models;

namespace VideoTool.Core.Services;

/// <summary>ffmpeg/ffprobe 실행 파일을 감싸서 비디오 분석, 프레임 추출, 오디오 추출,
/// 인코딩, 병합/먹싱 기능을 제공한다.</summary>
public sealed class FfmpegService
{
    private readonly string _ffmpegPath;
    private readonly string _ffprobePath;

    public FfmpegService(string ffmpegPath, string ffprobePath)
    {
        _ffmpegPath = ffmpegPath;
        _ffprobePath = ffprobePath;
    }

    public async Task<VideoInfo> ProbeAsync(string inputPath, CancellationToken ct = default)
    {
        // 필요한 필드만 명시적으로 요청한다(-show_format/-show_streams 대신 -show_entries).
        // 자유 형식 메타데이터 태그(title/comment/encoder 등)에는 일부 실제 영상 파일에서
        // ffprobe가 JSON으로 제대로 이스케이프하지 못하는 값이 들어있는 경우가 있어(JsonException 유발),
        // 애초에 그런 태그를 요청하지 않는 편이 안전하다.
        string[] args =
        [
            "-v", "error",
            "-print_format", "json",
            "-show_entries", "stream=index,codec_type,codec_name,width,height,r_frame_rate,avg_frame_rate,duration:format=duration",
            inputPath,
        ];

        var result = await ProcessRunner.RunAsync(_ffprobePath, args, cancellationToken: ct).ConfigureAwait(false);
        if (result.ExitCode != 0)
            throw new InvalidOperationException($"ffprobe 실행 실패 (종료 코드 {result.ExitCode}): {string.Join('\n', result.StdErrLines)}");

        var json = string.Join('\n', result.StdOutLines);
        using var doc = ParseProbeJson(json);
        var root = doc.RootElement;

        JsonElement? videoStream = null;
        var hasAudio = false;
        foreach (var stream in root.GetProperty("streams").EnumerateArray())
        {
            var codecType = stream.TryGetProperty("codec_type", out var ct2) ? ct2.GetString() : null;
            if (codecType == "video" && videoStream is null)
                videoStream = stream;
            else if (codecType == "audio")
                hasAudio = true;
        }

        if (videoStream is null)
            throw new InvalidOperationException("입력 파일에서 비디오 스트림을 찾을 수 없습니다.");

        var vs = videoStream.Value;
        var width = vs.GetProperty("width").GetInt32();
        var height = vs.GetProperty("height").GetInt32();

        var frameRate = ParseFrameRate(GetStringOrNull(vs, "avg_frame_rate"))
                         ?? ParseFrameRate(GetStringOrNull(vs, "r_frame_rate"))
                         ?? 30.0;
        if (frameRate <= 0)
            frameRate = 30.0;

        var codecName = GetStringOrNull(vs, "codec_name");

        double durationSeconds = 0;
        if (root.TryGetProperty("format", out var fmt) && TryParseDouble(GetStringOrNull(fmt, "duration"), out var d))
            durationSeconds = d;
        else if (TryParseDouble(GetStringOrNull(vs, "duration"), out var vd))
            durationSeconds = vd;

        return new VideoInfo
        {
            FilePath = inputPath,
            Width = width,
            Height = height,
            FrameRate = frameRate,
            Duration = TimeSpan.FromSeconds(durationSeconds),
            HasAudio = hasAudio,
            VideoCodec = codecName,
        };
    }

    /// <summary>preScale을 지정하면 AI에 넘기기 전 프레임을 해당 해상도로 미리 축소한다.
    /// AI 4배 확대 연산량은 입력 픽셀 수에 비례하므로, 예컨대 원본을 1/2로 축소해서 넣으면
    /// 처리 시간이 대략 1/4로 줄어든다(대신 AI가 살릴 수 있는 원본 디테일도 그만큼 줄어듦).</summary>
    public async Task ExtractFramesAsync(
        string inputPath,
        TimeSpan startTime,
        TimeSpan duration,
        string outputDir,
        string imageFormat,
        Action<string>? onLog,
        (int Width, int Height)? preScale = null,
        CancellationToken ct = default)
    {
        Directory.CreateDirectory(outputDir);
        var pattern = Path.Combine(outputDir, $"f%08d.{imageFormat}");

        var args = new List<string> { "-y", "-hide_banner", "-loglevel", "error" };
        args.AddRange(["-ss", startTime.TotalSeconds.ToString(CultureInfo.InvariantCulture)]);
        args.AddRange(["-i", inputPath]);
        args.AddRange(["-t", duration.TotalSeconds.ToString(CultureInfo.InvariantCulture)]);
        if (preScale is { } scale)
            args.AddRange(["-vf", $"scale={scale.Width}:{scale.Height}:flags=lanczos"]);
        if (string.Equals(imageFormat, "jpg", StringComparison.OrdinalIgnoreCase))
            args.AddRange(["-qscale:v", "2"]);
        args.AddRange(["-fps_mode", "passthrough", pattern]);

        var result = await ProcessRunner.RunAsync(_ffmpegPath, args, onStdErr: onLog, cancellationToken: ct).ConfigureAwait(false);
        if (result.ExitCode != 0)
            throw new InvalidOperationException($"프레임 추출 실패: {string.Join('\n', result.StdErrLines.TakeLast(20))}");
    }

    /// <summary>오디오 트랙 중 [startTime, startTime+duration) 구간만 AAC로 추출한다.
    /// 오디오가 없거나 추출에 실패하면 false를 반환한다.</summary>
    public async Task<bool> ExtractAudioAsync(
        string inputPath,
        TimeSpan startTime,
        TimeSpan duration,
        string outputAudioPath,
        int bitrateKbps,
        Action<string>? onLog,
        CancellationToken ct = default)
    {
        string[] args =
        [
            "-y", "-hide_banner", "-loglevel", "error",
            "-ss", startTime.TotalSeconds.ToString(CultureInfo.InvariantCulture),
            "-i", inputPath,
            "-t", duration.TotalSeconds.ToString(CultureInfo.InvariantCulture),
            "-vn",
            "-c:a", "aac",
            "-b:a", $"{bitrateKbps}k",
            outputAudioPath,
        ];
        var result = await ProcessRunner.RunAsync(_ffmpegPath, args, onStdErr: onLog, cancellationToken: ct).ConfigureAwait(false);
        return result.ExitCode == 0 && File.Exists(outputAudioPath);
    }

    /// <summary>업스케일된 프레임 폴더 하나(청크)를 목표 해상도로 정확히 맞춰 영상 조각으로 인코딩한다.</summary>
    public async Task EncodeChunkAsync(
        string frameDir,
        string imageFormat,
        double fps,
        int targetWidth,
        int targetHeight,
        OutputCodec codec,
        int crf,
        string outputPath,
        Action<string>? onLog,
        CancellationToken ct = default)
    {
        var pattern = Path.Combine(frameDir, $"f%08d.{imageFormat}");
        var scaleFilter =
            $"scale={targetWidth}:{targetHeight}:flags=lanczos:force_original_aspect_ratio=decrease," +
            $"pad={targetWidth}:{targetHeight}:(ow-iw)/2:(oh-ih)/2,setsar=1";

        string[] args =
        [
            "-y", "-hide_banner", "-loglevel", "error",
            "-framerate", fps.ToString(CultureInfo.InvariantCulture),
            "-i", pattern,
            "-vf", scaleFilter,
            "-c:v", codec.ToFfmpegEncoder(),
            "-preset", codec == OutputCodec.Av1 ? "8" : "medium",
            "-crf", crf.ToString(CultureInfo.InvariantCulture),
            "-pix_fmt", "yuv420p",
            outputPath,
        ];

        var result = await ProcessRunner.RunAsync(_ffmpegPath, args, onStdErr: onLog, cancellationToken: ct).ConfigureAwait(false);
        if (result.ExitCode != 0)
            throw new InvalidOperationException($"청크 인코딩 실패: {string.Join('\n', result.StdErrLines.TakeLast(20))}");
    }

    /// <summary>인코딩된 청크 조각들을 재인코딩 없이(-c copy) 하나로 이어붙인다.</summary>
    public async Task ConcatAsync(IReadOnlyList<string> partFilePaths, string outputPath, Action<string>? onLog, CancellationToken ct = default)
    {
        if (partFilePaths.Count == 0)
            throw new InvalidOperationException("병합할 영상 조각이 없습니다.");

        var listPath = outputPath + ".concat.txt";
        var sb = new StringBuilder();
        foreach (var part in partFilePaths)
        {
            var escaped = part.Replace("'", "'\\''");
            sb.AppendLine($"file '{escaped}'");
        }
        await File.WriteAllTextAsync(listPath, sb.ToString(), ct).ConfigureAwait(false);

        try
        {
            string[] args =
            [
                "-y", "-hide_banner", "-loglevel", "error",
                "-f", "concat", "-safe", "0",
                "-i", listPath,
                "-c", "copy",
                outputPath,
            ];
            var result = await ProcessRunner.RunAsync(_ffmpegPath, args, onStdErr: onLog, cancellationToken: ct).ConfigureAwait(false);
            if (result.ExitCode != 0)
                throw new InvalidOperationException($"청크 병합 실패: {string.Join('\n', result.StdErrLines.TakeLast(20))}");
        }
        finally
        {
            File.Delete(listPath);
        }
    }

    /// <summary>영상과(있다면) 오디오를 재인코딩 없이 최종 파일로 합친다.</summary>
    public async Task MuxAsync(string videoPath, string? audioPath, string outputPath, Action<string>? onLog, CancellationToken ct = default)
    {
        var args = new List<string> { "-y", "-hide_banner", "-loglevel", "error", "-i", videoPath };
        if (audioPath is not null)
            args.AddRange(["-i", audioPath]);

        args.AddRange(["-map", "0:v:0"]);
        if (audioPath is not null)
            args.AddRange(["-map", "1:a:0"]);

        args.Add("-c");
        args.Add("copy");
        if (audioPath is not null)
            args.Add("-shortest");

        args.Add(outputPath);

        var result = await ProcessRunner.RunAsync(_ffmpegPath, args, onStdErr: onLog, cancellationToken: ct).ConfigureAwait(false);
        if (result.ExitCode != 0)
            throw new InvalidOperationException($"최종 저장(먹싱) 실패: {string.Join('\n', result.StdErrLines.TakeLast(20))}");
    }

    private static JsonDocument ParseProbeJson(string json)
    {
        try
        {
            return JsonDocument.Parse(json);
        }
        catch (JsonException ex)
        {
            throw new InvalidOperationException($"ffprobe 출력(JSON)을 해석할 수 없습니다: {ex.Message}", ex);
        }
    }

    private static string? GetStringOrNull(JsonElement element, string propertyName) =>
        element.TryGetProperty(propertyName, out var prop) ? prop.GetString() : null;

    private static bool TryParseDouble(string? raw, out double value)
    {
        if (raw is null)
        {
            value = 0;
            return false;
        }
        return double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    private static double? ParseFrameRate(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;

        var parts = raw.Split('/');
        if (parts.Length == 2
            && double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out var num)
            && double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out var den)
            && den != 0)
        {
            return num / den;
        }

        return double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var direct) ? direct : null;
    }
}
