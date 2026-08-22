namespace VideoTool.Core.Models;

public enum UpscaleEngine
{
    /// <summary>realesrgan-ncnn-vulkan.exe. Python 불필요, 모든 GPU 벤더에서 동작.</summary>
    NcnnVulkan,

    /// <summary>PyTorch + CUDA. NVIDIA GPU 전용이지만 훨씬 빠르고, 실사 영상에 적합한
    /// realesr-general-x4v3 모델을 사용한다.</summary>
    PyTorchCuda,
}

public static class UpscaleEngineExtensions
{
    public static string ToDisplayName(this UpscaleEngine engine) => engine switch
    {
        UpscaleEngine.NcnnVulkan => "ncnn-Vulkan (모든 GPU, Python 불필요)",
        UpscaleEngine.PyTorchCuda => "PyTorch-CUDA (NVIDIA 전용, 훨씬 빠름)",
        _ => engine.ToString(),
    };
}
