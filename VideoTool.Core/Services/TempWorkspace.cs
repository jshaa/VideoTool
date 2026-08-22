namespace VideoTool.Core.Services;

/// <summary>작업 1건에 대한 임시 작업 폴더(프레임/업스케일 결과/인코딩 조각/오디오)를 관리한다.
/// 청크 단위로 처리하므로 ResetChunkDirs()를 청크마다 호출해 디스크 사용량을 제한한다.</summary>
public sealed class TempWorkspace : IDisposable
{
    public string RootDir { get; }
    public string ChunkFramesDir => Path.Combine(RootDir, "frames");
    public string ChunkUpscaledDir => Path.Combine(RootDir, "upscaled");
    public string PartsDir => Path.Combine(RootDir, "parts");
    public string AudioPath => Path.Combine(RootDir, "audio.m4a");

    private readonly bool _keepOnDispose;

    public TempWorkspace(string tempRoot, bool keepOnDispose = false)
    {
        RootDir = Path.Combine(tempRoot, "job-" + Guid.NewGuid().ToString("N"));
        _keepOnDispose = keepOnDispose;
        Directory.CreateDirectory(RootDir);
        Directory.CreateDirectory(PartsDir);
    }

    public void ResetChunkDirs()
    {
        SafeDeleteDir(ChunkFramesDir);
        SafeDeleteDir(ChunkUpscaledDir);
        Directory.CreateDirectory(ChunkFramesDir);
        Directory.CreateDirectory(ChunkUpscaledDir);
    }

    private static void SafeDeleteDir(string dir)
    {
        if (Directory.Exists(dir))
            Directory.Delete(dir, recursive: true);
    }

    public void Dispose()
    {
        if (_keepOnDispose)
            return;
        try
        {
            if (Directory.Exists(RootDir))
                Directory.Delete(RootDir, recursive: true);
        }
        catch
        {
            // 임시 파일이 잠시 잠겨 있을 수 있음 - 다음 실행 시 정리되도록 무시
        }
    }
}
