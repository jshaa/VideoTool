using System.Globalization;
using VideoTool.Core.Models;

namespace VideoTool.Core.Services;

/// <summary>realesrgan-ncnn-vulkan 실행 파일을 감싸서 이미지 폴더 단위 AI 업스케일을 수행한다.
/// Vulkan 기반이라 별도의 Python/CUDA 설치 없이 NVIDIA/AMD/Intel GPU에서 동작한다.</summary>
public sealed class RealEsrganService
{
    private readonly string _exePath;
    private readonly string _modelsDir;

    public RealEsrganService(string exePath, string modelsDir)
    {
        _exePath = exePath;
        _modelsDir = modelsDir;
    }

    public async Task UpscaleFolderAsync(
        string inputDir,
        string outputDir,
        UpscaleModel model,
        string imageFormat,
        string? gpuId,
        int tileSize,
        Action<string>? onLog,
        Action<(int Done, int Total)>? onProgress,
        CancellationToken ct = default)
    {
        Directory.CreateDirectory(outputDir);
        var totalInputs = Directory.EnumerateFiles(inputDir).Count();
        if (totalInputs == 0)
            return;

        var args = new List<string>
        {
            "-i", inputDir,
            "-o", outputDir,
            "-n", model.ToModelName(),
            "-s", "4",
            "-f", imageFormat,
            "-m", _modelsDir,
        };
        if (!string.IsNullOrWhiteSpace(gpuId))
            args.AddRange(["-g", gpuId]);
        if (tileSize > 0)
            args.AddRange(["-t", tileSize.ToString(CultureInfo.InvariantCulture)]);

        // realesrgan-ncnn-vulkan의 콘솔 진행률 출력 형식은 버전마다 달라질 수 있어,
        // 결과 폴더에 실제로 쌓이는 파일 수를 폴링하는 방식으로 진행률을 계산한다.
        using var pollCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        var pollTask = Task.Run(async () =>
        {
            try
            {
                while (!pollCts.IsCancellationRequested)
                {
                    var done = Directory.Exists(outputDir) ? Directory.EnumerateFiles(outputDir).Count() : 0;
                    onProgress?.Invoke((done, totalInputs));
                    await Task.Delay(300, pollCts.Token).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                // 폴링 종료 - 정상
            }
        }, CancellationToken.None);

        ProcessResult result;
        try
        {
            result = await ProcessRunner.RunAsync(_exePath, args, onStdErr: onLog, onStdOut: onLog, cancellationToken: ct).ConfigureAwait(false);
        }
        finally
        {
            pollCts.Cancel();
            await pollTask.ConfigureAwait(false);
        }

        if (result.ExitCode != 0)
            throw new InvalidOperationException($"AI 업스케일 실패: {string.Join('\n', result.StdErrLines.TakeLast(20))}");

        onProgress?.Invoke((totalInputs, totalInputs));
    }
}
