namespace VideoTool.Core.Models;

public sealed class UpscaleJob
{
    public Guid Id { get; } = Guid.NewGuid();

    public required string InputPath { get; init; }
    public required string OutputPath { get; set; }

    public UpscaleSettings Settings { get; set; } = new();
    public VideoInfo? SourceInfo { get; set; }

    public JobStage Stage { get; set; } = JobStage.Queued;
    public double PercentComplete { get; set; }
    public string? LastMessage { get; set; }
    public string? ErrorMessage { get; set; }

    public string FileName => Path.GetFileName(InputPath);
}
