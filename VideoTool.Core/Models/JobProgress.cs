namespace VideoTool.Core.Models;

public enum JobStage
{
    Queued,
    Probing,
    ExtractingAudio,
    Processing,
    Concatenating,
    Muxing,
    Completed,
    Failed,
    Canceled,
}

public sealed class JobProgress
{
    public required JobStage Stage { get; init; }
    public double PercentComplete { get; init; }
    public long FramesDone { get; init; }
    public long TotalFrames { get; init; }
    public int CurrentChunk { get; init; }
    public int TotalChunks { get; init; }
    public string? Message { get; init; }
    public TimeSpan? Eta { get; init; }
}
