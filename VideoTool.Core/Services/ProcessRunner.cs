using System.Diagnostics;

namespace VideoTool.Core.Services;

public sealed class ProcessResult
{
    public int ExitCode { get; init; }
    public List<string> StdOutLines { get; init; } = new();
    public List<string> StdErrLines { get; init; } = new();
}

/// <summary>외부 실행 파일(ffmpeg, ffprobe, realesrgan-ncnn-vulkan)을 실행하고
/// 표준 출력/에러를 실시간으로 콜백하며, 취소 시 프로세스 트리를 종료한다.</summary>
public static class ProcessRunner
{
    public static async Task<ProcessResult> RunAsync(
        string fileName,
        IEnumerable<string> arguments,
        string? workingDirectory = null,
        Action<string>? onStdOut = null,
        Action<string>? onStdErr = null,
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(fileName))
            throw new FileNotFoundException($"실행 파일을 찾을 수 없습니다: {fileName}", fileName);

        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            WorkingDirectory = workingDirectory ?? Path.GetDirectoryName(fileName) ?? string.Empty,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        foreach (var arg in arguments)
            psi.ArgumentList.Add(arg);

        using var process = new Process { StartInfo = psi, EnableRaisingEvents = true };
        var stdOutLines = new List<string>();
        var stdErrLines = new List<string>();

        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data is null) return;
            stdOutLines.Add(e.Data);
            onStdOut?.Invoke(e.Data);
        };
        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data is null) return;
            stdErrLines.Add(e.Data);
            onStdErr?.Invoke(e.Data);
        };

        if (!process.Start())
            throw new InvalidOperationException($"프로세스를 시작할 수 없습니다: {fileName}");

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();

        using var registration = cancellationToken.Register(() => TryKillTree(process));

        // 실제 프로세스 종료(취소로 인한 kill 포함)까지 대기한 뒤 취소 예외를 던진다.
        // 그래야 호출자가 임시 파일을 지울 때 파일 잠금이 남아있지 않다.
        await process.WaitForExitAsync(CancellationToken.None).ConfigureAwait(false);

        cancellationToken.ThrowIfCancellationRequested();

        return new ProcessResult
        {
            ExitCode = process.ExitCode,
            StdOutLines = stdOutLines,
            StdErrLines = stdErrLines,
        };
    }

    private static void TryKillTree(Process process)
    {
        try
        {
            if (!process.HasExited)
                process.Kill(entireProcessTree: true);
        }
        catch
        {
            // 이미 종료된 경우 등은 무시
        }
    }
}
