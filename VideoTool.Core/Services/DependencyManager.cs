using System.IO.Compression;
using System.Net.Http;

namespace VideoTool.Core.Services;

public sealed record DependencyStatus(
    bool FfmpegAvailable, string FfmpegPath,
    bool FfprobeAvailable, string FfprobePath,
    bool RealEsrganAvailable, string RealEsrganPath, string RealEsrganModelsDir)
{
    public bool AllAvailable => FfmpegAvailable && FfprobeAvailable && RealEsrganAvailable;
}

public sealed record PythonEngineStatus(bool Available, string PythonExePath, string InferScriptPath, string ModelPath);

/// <summary>ffmpeg와 realesrgan-ncnn-vulkan 실행 파일의 존재 여부를 확인하고,
/// 없으면 공식 GitHub 릴리즈에서 내려받아 로컬(%LOCALAPPDATA%\VideoTool\tools)에 설치한다.
/// 두 도구 모두 무료 오픈소스 프로젝트의 공식 배포판이다.</summary>
public sealed class DependencyManager
{
    public static readonly string ToolsRoot = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "VideoTool", "tools");

    private static readonly string FfmpegDir = Path.Combine(ToolsRoot, "ffmpeg");
    private static readonly string RealEsrganDir = Path.Combine(ToolsRoot, "realesrgan");
    private static readonly string PythonDir = Path.Combine(ToolsRoot, "python");
    // 모델 가중치는 PythonDir '밖'에 둬야 한다. Python의 site 모듈은 시작 시 자신의 설치 루트를
    // "경로 설정 파일(.pth)" 대상으로 스캔하는데, PyTorch 체크포인트도 관례상 .pth 확장자를 쓰다 보니
    // PythonDir 안에 두면 site가 바이너리 체크포인트를 텍스트 설정 파일로 오인해 파싱하다가 깨진다.
    private static readonly string PythonModelsDir = Path.Combine(ToolsRoot, "python-models");

    // BtbN/FFmpeg-Builds: ffmpeg 공식 소스를 GitHub Actions로 자동 빌드/배포하는 오픈소스 프로젝트.
    // "latest" 릴리즈 태그의 고정 파일명을 사용하므로 버전이 올라가도 URL이 그대로 유효하다.
    private const string FfmpegDownloadUrl =
        "https://github.com/BtbN/FFmpeg-Builds/releases/latest/download/ffmpeg-master-latest-win64-gpl.zip";

    // xinntao/Real-ESRGAN-ncnn-vulkan(래퍼 저장소)의 릴리즈는 실행 파일만 있고 모델 가중치가 빠져 있다.
    // 모델이 포함된 완전한 Windows 번들은 원조 xinntao/Real-ESRGAN 저장소 릴리즈에 첨부되어 있으며,
    // 이후 릴리즈(v0.3.0)에는 해당 첨부파일이 없어 "latest"가 아닌 마지막으로 번들이 첨부된 태그를 고정 사용한다.
    private const string RealEsrganDownloadUrl =
        "https://github.com/xinntao/Real-ESRGAN/releases/download/v0.2.5.0/realesrgan-ncnn-vulkan-20220424-windows.zip";

    // Python 공식 배포 "embeddable package" - 설치 없이 폴더에 풀어서 쓰는 최소 런타임(무료/오픈소스, PSF 라이선스).
    private const string PythonEmbedUrl = "https://www.python.org/ftp/python/3.10.11/python-3.10.11-embed-amd64.zip";
    private const string GetPipUrl = "https://bootstrap.pypa.io/get-pip.py";
    // PyTorch 공식 CUDA 12.8 휠 인덱스(무료/오픈소스, BSD 라이선스).
    private const string TorchCudaIndexUrl = "https://download.pytorch.org/whl/cu128";
    private const string GeneralX4v3ModelUrl =
        "https://github.com/xinntao/Real-ESRGAN/releases/download/v0.2.5.0/realesr-general-x4v3.pth";
    private const string GeneralX4v3ModelFileName = "realesr-general-x4v3.pth";

    private readonly HttpClient _http;
    private readonly string _scriptsDir;

    public DependencyManager(HttpClient? httpClient = null, string? scriptsDir = null)
    {
        _http = httpClient ?? new HttpClient();
        if (_http.DefaultRequestHeaders.UserAgent.Count == 0)
            _http.DefaultRequestHeaders.UserAgent.ParseAdd("VideoTool-OpenSource-Client/1.0");
        // PythonScripts(infer.py/srvgg.py)는 다운로드 대상이 아니라 앱 자체에 포함되어 실행 파일과 같이 배포된다.
        _scriptsDir = scriptsDir ?? Path.Combine(AppContext.BaseDirectory, "PythonScripts");
    }

    public DependencyStatus GetStatus()
    {
        var ffmpegExe = FindFile(FfmpegDir, "ffmpeg.exe");
        var ffprobeExe = FindFile(FfmpegDir, "ffprobe.exe");
        var realesrganExe = FindFile(RealEsrganDir, "realesrgan-ncnn-vulkan.exe");
        var modelsDir = Path.Combine(Path.GetDirectoryName(realesrganExe) ?? RealEsrganDir, "models");

        return new DependencyStatus(
            File.Exists(ffmpegExe), ffmpegExe,
            File.Exists(ffprobeExe), ffprobeExe,
            File.Exists(realesrganExe) && Directory.Exists(modelsDir), realesrganExe, modelsDir);
    }

    public PythonEngineStatus GetPythonEngineStatus()
    {
        var pythonExe = Path.Combine(PythonDir, "python.exe");
        var modelPath = Path.Combine(PythonModelsDir, GeneralX4v3ModelFileName);
        var inferScript = Path.Combine(_scriptsDir, "infer.py");
        var torchInstalled = Directory.Exists(Path.Combine(PythonDir, "Lib", "site-packages", "torch"));

        return new PythonEngineStatus(
            File.Exists(pythonExe) && torchInstalled && File.Exists(modelPath) && File.Exists(inferScript),
            pythonExe, inferScript, modelPath);
    }

    /// <summary>Python 임베디드 런타임 다운로드 -> pip 구성 -> PyTorch(CUDA)/numpy/Pillow 설치 -> AI 모델 다운로드까지
    /// 한 번에 진행한다. 전체 수 GB 다운로드가 발생하며 몇 분 정도 걸릴 수 있다.</summary>
    public async Task DownloadPythonEngineAsync(
        Action<string>? onStatus,
        IProgress<double>? onProgress,
        Action<string>? onLog,
        CancellationToken ct = default)
    {
        Directory.CreateDirectory(PythonDir);

        onStatus?.Invoke("Python 런타임 다운로드 중...");
        var pyZip = Path.Combine(ToolsRoot, "python-embed.zip");
        try
        {
            await DownloadFileAsync(PythonEmbedUrl, pyZip, onProgress, ct).ConfigureAwait(false);
            ExtractZip(pyZip, PythonDir);
        }
        finally
        {
            if (File.Exists(pyZip))
                File.Delete(pyZip);
        }

        onStatus?.Invoke("Python 환경 구성 중...");
        EnableSitePackages(PythonDir);

        onStatus?.Invoke("pip 설치 중...");
        var getPipPath = Path.Combine(PythonDir, "get-pip.py");
        try
        {
            await DownloadFileAsync(GetPipUrl, getPipPath, null, ct).ConfigureAwait(false);
            await RunPythonAsync(PythonDir, [getPipPath, "--no-warn-script-location"], onLog, ct).ConfigureAwait(false);
        }
        finally
        {
            if (File.Exists(getPipPath))
                File.Delete(getPipPath);
        }

        onStatus?.Invoke("PyTorch(CUDA) 설치 중... (약 3GB, 시간이 걸릴 수 있습니다)");
        await RunPythonAsync(PythonDir,
            ["-m", "pip", "install", "torch", "--index-url", TorchCudaIndexUrl, "--no-warn-script-location"],
            onLog, ct).ConfigureAwait(false);

        onStatus?.Invoke("필수 패키지(numpy, Pillow) 설치 중...");
        await RunPythonAsync(PythonDir,
            ["-m", "pip", "install", "numpy", "Pillow", "--no-warn-script-location"],
            onLog, ct).ConfigureAwait(false);

        onStatus?.Invoke("AI 모델 다운로드 중...");
        Directory.CreateDirectory(PythonModelsDir);
        var modelPath = Path.Combine(PythonModelsDir, GeneralX4v3ModelFileName);
        await DownloadFileAsync(GeneralX4v3ModelUrl, modelPath, onProgress, ct).ConfigureAwait(false);

        onStatus?.Invoke("완료");
    }

    /// <summary>임베디드 Python은 기본적으로 site-packages 로딩이 꺼져 있어(성능/보안상 이유) pip로 설치한
    /// 패키지를 import할 수 없다. python3XX._pth 파일의 "#import site" 주석을 해제해 활성화한다.</summary>
    private static void EnableSitePackages(string pythonDir)
    {
        var pthFile = Directory.GetFiles(pythonDir, "python*._pth").FirstOrDefault();
        if (pthFile is null)
            return;

        var lines = File.ReadAllLines(pthFile);
        for (var i = 0; i < lines.Length; i++)
        {
            if (lines[i].TrimStart('#').Trim() == "import site")
                lines[i] = "import site";
        }
        File.WriteAllLines(pthFile, lines);
    }

    private static async Task RunPythonAsync(string pythonDir, IEnumerable<string> args, Action<string>? onLog, CancellationToken ct)
    {
        var pythonExe = Path.Combine(pythonDir, "python.exe");
        var result = await ProcessRunner.RunAsync(pythonExe, args, onStdOut: onLog, onStdErr: onLog, cancellationToken: ct).ConfigureAwait(false);
        if (result.ExitCode != 0)
            throw new InvalidOperationException($"Python 환경 설정 실패: {string.Join('\n', result.StdErrLines.TakeLast(30))}");
    }

    /// <summary>압축 해제 후 실행 파일이 배포판마다 다른 하위 폴더 구조를 가질 수 있어 재귀 탐색한다.</summary>
    private static string FindFile(string rootDir, string fileName)
    {
        if (!Directory.Exists(rootDir))
            return Path.Combine(rootDir, fileName);

        var found = Directory.EnumerateFiles(rootDir, fileName, SearchOption.AllDirectories).FirstOrDefault();
        return found ?? Path.Combine(rootDir, fileName);
    }

    public async Task DownloadFfmpegAsync(IProgress<double>? progress, CancellationToken ct = default)
    {
        Directory.CreateDirectory(FfmpegDir);
        var zipPath = Path.Combine(ToolsRoot, "ffmpeg-download.zip");
        try
        {
            await DownloadFileAsync(FfmpegDownloadUrl, zipPath, progress, ct).ConfigureAwait(false);
            ExtractZip(zipPath, FfmpegDir);
        }
        finally
        {
            if (File.Exists(zipPath))
                File.Delete(zipPath);
        }
    }

    public async Task DownloadRealEsrganAsync(IProgress<double>? progress, CancellationToken ct = default)
    {
        Directory.CreateDirectory(RealEsrganDir);
        var zipPath = Path.Combine(ToolsRoot, "realesrgan-download.zip");
        try
        {
            await DownloadFileAsync(RealEsrganDownloadUrl, zipPath, progress, ct).ConfigureAwait(false);
            ExtractZip(zipPath, RealEsrganDir);
        }
        finally
        {
            if (File.Exists(zipPath))
                File.Delete(zipPath);
        }
    }

    private async Task DownloadFileAsync(string url, string destinationPath, IProgress<double>? progress, CancellationToken ct)
    {
        using var response = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var totalBytes = response.Content.Headers.ContentLength;

        var httpStream = await response.Content.ReadAsStreamAsync(ct).ConfigureAwait(false);
        await using (httpStream.ConfigureAwait(false))
        {
            var fileStream = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None, 81920, useAsync: true);
            await using (fileStream.ConfigureAwait(false))
            {
                var buffer = new byte[81920];
                long totalRead = 0;
                int read;
                while ((read = await httpStream.ReadAsync(buffer, ct).ConfigureAwait(false)) > 0)
                {
                    await fileStream.WriteAsync(buffer.AsMemory(0, read), ct).ConfigureAwait(false);
                    totalRead += read;
                    if (totalBytes is > 0)
                        progress?.Report((double)totalRead / totalBytes.Value);
                }
            }
        }
    }

    private static void ExtractZip(string zipPath, string destinationDir)
    {
        using var archive = ZipFile.OpenRead(zipPath);
        foreach (var entry in archive.Entries)
        {
            if (string.IsNullOrEmpty(entry.Name))
                continue; // 디렉터리 항목은 건너뜀

            var destPath = Path.Combine(destinationDir, entry.FullName);
            var destDir = Path.GetDirectoryName(destPath);
            if (destDir is not null)
                Directory.CreateDirectory(destDir);
            entry.ExtractToFile(destPath, overwrite: true);
        }
    }
}
