namespace VideoTool.Core.Models;

public sealed class VideoInfo
{
    public required string FilePath { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
    public double FrameRate { get; init; } = 30.0;
    public TimeSpan Duration { get; init; }
    public bool HasAudio { get; init; }
    public string? VideoCodec { get; init; }

    public long EstimatedFrameCount => Math.Max(1, (long)Math.Round(Duration.TotalSeconds * FrameRate));
}
