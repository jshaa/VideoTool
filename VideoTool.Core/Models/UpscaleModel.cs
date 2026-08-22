namespace VideoTool.Core.Models;

public enum UpscaleModel
{
    GeneralPhoto,
    AnimeVideo,
    AnimePhoto,
}

public static class UpscaleModelExtensions
{
    /// <summary>realesrgan-ncnn-vulkan의 -n 옵션에 전달할 모델 이름.
    /// 배포되는 models 폴더에는 realesrgan-x4plus / realesrgan-x4plus-anime / realesr-animevideov3(-x2/-x3/-x4)만
    /// 들어있으므로 이 세 가지만 지원한다.</summary>
    public static string ToModelName(this UpscaleModel model) => model switch
    {
        UpscaleModel.GeneralPhoto => "realesrgan-x4plus",
        UpscaleModel.AnimeVideo => "realesr-animevideov3",
        UpscaleModel.AnimePhoto => "realesrgan-x4plus-anime",
        _ => throw new ArgumentOutOfRangeException(nameof(model)),
    };

    public static string ToDisplayName(this UpscaleModel model) => model switch
    {
        UpscaleModel.GeneralPhoto => "일반 영상 - 고화질 (느림)",
        UpscaleModel.AnimeVideo => "애니메이션 영상 - 빠름",
        UpscaleModel.AnimePhoto => "애니메이션/일러스트 - 고화질",
        _ => model.ToString(),
    };
}
