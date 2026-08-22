namespace VideoTool.Core.Models;

public enum OutputCodec
{
    H264,
    H265,
    Av1,
}

public static class OutputCodecExtensions
{
    public static string ToFfmpegEncoder(this OutputCodec codec) => codec switch
    {
        OutputCodec.H264 => "libx264",
        OutputCodec.H265 => "libx265",
        OutputCodec.Av1 => "libsvtav1",
        _ => throw new ArgumentOutOfRangeException(nameof(codec)),
    };

    public static string ToDisplayName(this OutputCodec codec) => codec switch
    {
        OutputCodec.H264 => "H.264 (호환성 우선)",
        OutputCodec.H265 => "H.265 / HEVC (용량 절감)",
        OutputCodec.Av1 => "AV1 (최신 코덱, 인코딩 느림)",
        _ => codec.ToString(),
    };

    /// <summary>코덱마다 CRF(화질) 스케일이 달라 기본값과 범위를 분리해서 제공.</summary>
    public static (int Min, int Max, int Default) CrfRange(this OutputCodec codec) => codec switch
    {
        OutputCodec.H264 => (0, 51, 18),
        OutputCodec.H265 => (0, 51, 20),
        OutputCodec.Av1 => (0, 63, 30),
        _ => (0, 51, 18),
    };
}
