namespace VideoTool.Core.Models;

public sealed class UpscaleSettings
{
    public UpscaleEngine Engine { get; set; } = UpscaleEngine.NcnnVulkan;

    /// <summary>Engine이 PyTorchCuda일 때 fp16(반정밀도) 추론 사용 여부. 속도가 더 빠르고
    /// 이 모델 규모에서는 화질 손실이 거의 없다.</summary>
    public bool PyTorchHalfPrecision { get; set; } = true;

    public UpscaleModel Model { get; set; } = UpscaleModel.GeneralPhoto;

    public int TargetWidth { get; set; } = 3840;
    public int TargetHeight { get; set; } = 2160;

    public OutputCodec Codec { get; set; } = OutputCodec.H264;
    public int Crf { get; set; } = 18;

    /// <summary>청크당 프레임 수. 클수록 임시 디스크 사용량과 진행률 단위가 커짐.</summary>
    public int ChunkFrameCount { get; set; } = 240;

    public string TempRootDirectory { get; set; } = Path.Combine(Path.GetTempPath(), "VideoTool");

    /// <summary>null/빈 문자열이면 realesrgan이 자동으로 GPU를 선택.</summary>
    public string? GpuId { get; set; }

    /// <summary>0이면 자동(VRAM에 맞게 realesrgan이 결정).</summary>
    public int TileSize { get; set; } = 0;

    public bool KeepTempFiles { get; set; } = false;

    /// <summary>중간 프레임 저장 형식: "jpg"(빠르고 용량 작음) 또는 "png"(무손실, 용량 큼).</summary>
    public string IntermediateFormat { get; set; } = "jpg";

    public int AudioBitrateKbps { get; set; } = 192;

    /// <summary>null/0이면 처음부터 처리. 영상 전체가 아니라 일부 구간만 AI 업스케일하고 싶을 때 사용
    /// (예: 20분짜리 영상 중 앞 3분만 처리해서 소요 시간을 가늠해보는 경우).</summary>
    public TimeSpan? TrimStart { get; set; }

    /// <summary>null/0이면 구간 시작부터 영상 끝까지 처리.</summary>
    public TimeSpan? TrimDuration { get; set; }

    /// <summary>true면 AI에 넘기기 전 원본을 1/2 해상도로 축소한다. AI 연산량이 픽셀 수에 비례하므로
    /// 처리 시간이 대략 1/4로 줄어드는 대신, AI가 살릴 수 있는 원본 디테일도 줄어든다.
    /// (실사 영상에서 애니메이션 전용 고속 모델 대신 화질을 유지하며 속도를 절충하고 싶을 때 사용)</summary>
    public bool FastPreDownscale { get; set; } = false;

    public UpscaleSettings Clone() => (UpscaleSettings)MemberwiseClone();
}
