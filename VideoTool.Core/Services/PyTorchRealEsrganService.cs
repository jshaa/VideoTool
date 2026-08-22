namespace VideoTool.Core.Services;

/// <summary>PyTorch + CUDA로 realesr-general-x4v3(SRVGGNetCompact) 모델을 구동해 이미지 폴더를 4배 업스케일한다.
/// NVIDIA GPU 전용이지만 ncnn-vulkan 대비 훨씬 빠르고, 애니메이션 전용 모델과 달리 실사 영상에도 적합하다.</summary>
public sealed class PyTorchRealEsrganService
{
    private readonly string _pythonExePath;
    private readonly string _inferScriptPath;
    private readonly string _modelPath;

    public PyTorchRealEsrganService(string pythonExePath, string inferScriptPath, string modelPath)
    {
        _pythonExePath = pythonExePath;
        _inferScriptPath = inferScriptPath;
        _modelPath = modelPath;
    }

    public async Task UpscaleFolderAsync(
        string inputDir,
        string outputDir,
        string imageFormat,
        bool halfPrecision,
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
            _inferScriptPath,
            "-i", inputDir,
            "-o", outputDir,
            "-m", _modelPath,
            "--device", "cuda",
            "--quality", "95",
        };
        if (halfPrecision)
            args.Add("--half");

        // realesrgan-ncnn-vulkan과 동일하게, stdout 형식에 의존하지 않도록 결과 폴더의 파일 수를 폴링해 진행률을 계산한다.
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
            result = await ProcessRunner.RunAsync(_pythonExePath, args, onStdErr: onLog, onStdOut: onLog, cancellationToken: ct).ConfigureAwait(false);
        }
        finally
        {
            pollCts.Cancel();
            await pollTask.ConfigureAwait(false);
        }

        if (result.ExitCode != 0)
            throw new InvalidOperationException($"AI 업스케일(PyTorch) 실패: {string.Join('\n', result.StdErrLines.TakeLast(20))}");

        onProgress?.Invoke((totalInputs, totalInputs));
    }
}
