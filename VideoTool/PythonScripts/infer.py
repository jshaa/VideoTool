import argparse
import sys
import time
from pathlib import Path

import numpy as np
import torch
from PIL import Image

sys.path.insert(0, str(Path(__file__).resolve().parent))
from srvgg import SRVGGNetCompact


def load_model(model_path: str, device: torch.device, half: bool) -> torch.nn.Module:
    model = SRVGGNetCompact(num_in_ch=3, num_out_ch=3, num_feat=64, num_conv=32, upscale=4, act_type="prelu")
    ckpt = torch.load(model_path, map_location="cpu")
    if isinstance(ckpt, dict):
        state_dict = ckpt.get("params_ema", ckpt.get("params", ckpt))
    else:
        state_dict = ckpt
    model.load_state_dict(state_dict, strict=True)
    model.eval()
    model = model.to(device)
    return model.half() if half else model


def process_image(model: torch.nn.Module, device: torch.device, in_path: Path, out_path: Path, half: bool, quality: int):
    img = Image.open(in_path).convert("RGB")
    arr = np.array(img).astype(np.float32) / 255.0
    tensor = torch.from_numpy(arr).permute(2, 0, 1).unsqueeze(0).to(device)
    if half:
        tensor = tensor.half()

    with torch.no_grad():
        out = model(tensor)

    out = out.clamp(0, 1).float().squeeze(0).permute(1, 2, 0).cpu().numpy()
    out_img = Image.fromarray((out * 255.0).round().astype(np.uint8))
    out_img.save(out_path, quality=quality)


def main() -> int:
    parser = argparse.ArgumentParser(description="PyTorch 기반 폴더 단위 4배 업스케일")
    parser.add_argument("-i", "--input", required=True, help="입력 이미지 폴더")
    parser.add_argument("-o", "--output", required=True, help="출력 이미지 폴더")
    parser.add_argument("-m", "--model", required=True, help="모델 체크포인트(.pth) 경로")
    parser.add_argument("--device", default="cuda", choices=["cuda", "cpu"])
    parser.add_argument("--half", action="store_true", help="fp16 반정밀도 추론(더 빠름)")
    parser.add_argument("--quality", type=int, default=95, help="jpg 저장 품질")
    args = parser.parse_args()

    if args.device == "cuda" and not torch.cuda.is_available():
        print("경고: CUDA를 사용할 수 없어 CPU로 대체합니다 (매우 느립니다).", flush=True)
    device = torch.device(args.device if (args.device == "cpu" or torch.cuda.is_available()) else "cpu")
    print(f"device: {device}" +
          (f" ({torch.cuda.get_device_name(0)})" if device.type == "cuda" else ""), flush=True)

    model = load_model(args.model, device, args.half)

    in_dir = Path(args.input)
    out_dir = Path(args.output)
    out_dir.mkdir(parents=True, exist_ok=True)

    files = sorted(p for p in in_dir.iterdir() if p.is_file())
    for f in files:
        t0 = time.time()
        process_image(model, device, f, out_dir / f.name, args.half, args.quality)
        print(f"{f.name} done in {time.time() - t0:.2f}s", flush=True)

    return 0


if __name__ == "__main__":
    try:
        sys.exit(main())
    except Exception as ex:  # noqa: BLE001 - C# 쪽에 실패 원인을 그대로 전달하기 위해 광범위하게 캐치
        print(f"오류: {ex}", file=sys.stderr, flush=True)
        sys.exit(1)
