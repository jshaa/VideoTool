# VideoTool - AI 4K 업스케일러

360p/720p/1080p 영상을 AI로 4K(3840x2160)로 업스케일하는 Windows 데스크톱 도구입니다.
UI부터 AI 추론, 인코딩, 저장까지 전부 **무료 오픈소스** 구성 요소만으로 동작합니다.

## 구성 요소

| 역할 | 사용 기술 | 라이선스 |
|---|---|---|
| UI / 앱 셸 | C# .NET 10 WinForms | 이 저장소 코드 |
| AI 업스케일 엔진 A | [Real-ESRGAN (ncnn-vulkan)](https://github.com/xinntao/Real-ESRGAN/releases/tag/v0.2.5.0) | BSD-3-Clause |
| AI 업스케일 엔진 B(선택) | [PyTorch](https://pytorch.org) + [realesr-general-x4v3](https://github.com/xinntao/Real-ESRGAN/releases/tag/v0.2.5.0) | BSD-3-Clause |
| 비디오 디코딩/인코딩 | [FFmpeg](https://ffmpeg.org) (빌드: [BtbN/FFmpeg-Builds](https://github.com/BtbN/FFmpeg-Builds)) | LGPL/GPL |

> Real-ESRGAN 실행 파일 자체는 [xinntao/Real-ESRGAN-ncnn-vulkan](https://github.com/xinntao/Real-ESRGAN-ncnn-vulkan)에서 빌드되지만,
> 그 저장소의 릴리즈에는 모델 가중치가 빠져 있어 모델까지 포함된 완전한 Windows 번들은 원조 [xinntao/Real-ESRGAN](https://github.com/xinntao/Real-ESRGAN) 저장소 릴리즈(v0.2.5.0)에서 받습니다.

**엔진 두 가지를 선택할 수 있습니다:**
- **ncnn-Vulkan**: Python/CUDA 설치 없이 NVIDIA/AMD/Intel 어떤 GPU에서도 동작. 기본값.
- **PyTorch-CUDA**: NVIDIA GPU 전용이지만 실측 기준 **최대 38배** 빠릅니다(RTX 3050, FP16+전처리 축소 기준). 애니메이션 전용이 아닌 `realesr-general-x4v3` 모델을 사용해 실사 영상에도 적합합니다. Portable(embeddable) Python을 앱이 자동으로 받아 `%LOCALAPPDATA%\VideoTool\tools\python`에 격리 설치하므로 시스템 Python을 건드리지 않습니다.

두 엔진 모두 별도 프로세스로 실행(shell-out)만 하며 라이브러리로 링크하지 않으므로 라이선스 충돌이 없습니다.

## 프로젝트 구조

```
VideoTool.sln
VideoTool/            WinForms UI (MainForm, SettingsForm, DependenciesForm)
VideoTool.Core/       파이프라인 로직 (UI 의존성 없는 클래스 라이브러리)
  Models/             VideoInfo, UpscaleSettings, UpscaleJob, JobProgress, UpscaleEngine ...
  Services/
    FfmpegService.cs           ffprobe 분석 / 프레임 추출(구간·전처리 축소 지원) / 오디오 추출 / 인코딩 / 병합
    RealEsrganService.cs       realesrgan-ncnn-vulkan 실행 래퍼
    PyTorchRealEsrganService.cs PyTorch 엔진(infer.py) 실행 래퍼
    DependencyManager.cs       ffmpeg / realesrgan / python+torch 자동 다운로드-설치
    VideoUpscalePipeline.cs    청크 단위 파이프라인 오케스트레이터
PythonScripts/        infer.py, srvgg.py - PyTorch 엔진용 추론 스크립트(앱과 함께 배포됨)
```

## 동작 원리

영상 전체를 한 번에 프레임(PNG/JPG)으로 풀면 4K 기준 수십GB에 달할 수 있습니다.
그래서 영상을 **청크(기본 240프레임)** 단위로 잘라 아래 과정을 반복하며 디스크 사용량을 제한합니다.

1. `ffmpeg`로 해당 구간 프레임 추출
2. `realesrgan-ncnn-vulkan`으로 프레임을 4배 AI 업스케일 (모델은 항상 원본 4x로 실행해 화질을 최대로 뽑고,
   목표 해상도가 4배와 다르면 다음 단계에서 정확히 맞춥니다)
3. `ffmpeg`로 목표 해상도(Lanczos)에 정확히 맞춰 해당 구간을 영상 조각으로 인코딩
4. 프레임 임시파일 삭제 후 다음 구간 반복
5. 모든 조각을 무손실로 병합(`-c copy`) → 원본 오디오와 먹싱 → 최종 저장

## 요구 사항

- Windows 10/11
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) (빌드용) / .NET 10 Desktop Runtime (실행용)
- Vulkan을 지원하는 GPU(2016년 이후 대부분의 NVIDIA/AMD/Intel GPU) — CPU만으로는 매우 느립니다.
- PyTorch-CUDA 엔진을 쓰려면 NVIDIA GPU + 최신 드라이버 필요(약 3GB 추가 다운로드)
- 처리할 영상 길이에 따라 수 GB의 임시 디스크 공간

## 빌드 및 실행

```bash
dotnet build VideoTool.sln
dotnet run --project VideoTool/VideoTool.csproj
```

또는 Visual Studio에서 `VideoTool.sln`을 열어 실행(F5)하면 됩니다.

## 처음 실행 시 - 종속성 설치

FFmpeg와 Real-ESRGAN 실행 파일은 저장소에 포함되어 있지 않습니다(용량 문제 + 항상 최신 공식 빌드를 받기 위함).
앱 실행 후 우측 상단 **"도구 확인/다운로드..."** 버튼을 누르면 각 프로젝트의 **공식 GitHub 릴리즈**에서 자동으로 내려받아
`%LOCALAPPDATA%\VideoTool\tools` 아래에 설치합니다. 다운로드 전 각 도구의 GitHub 링크를 클릭해 출처를 직접 확인할 수 있습니다.

자동 다운로드가 안 되는 환경이라면 아래에서 직접 받아 폴더에 압축을 풀어 넣어도 됩니다.

- FFmpeg: `%LOCALAPPDATA%\VideoTool\tools\ffmpeg\` (ffmpeg.exe, ffprobe.exe가 하위 어딘가에 있으면 됨)
- Real-ESRGAN: `%LOCALAPPDATA%\VideoTool\tools\realesrgan\` (realesrgan-ncnn-vulkan.exe + models 폴더)

## 사용 방법

1. **영상 추가...** 로 업스케일할 파일을 추가 (여러 개 선택 가능, 자동으로 해상도 분석)
2. **출력 설정**에서 AI 엔진 / 모델 / 목표 해상도 / 코덱 / 화질(CRF) / 저장 폴더 지정
   - **AI 엔진**: ncnn-Vulkan(기본, 모든 GPU) 또는 PyTorch-CUDA(NVIDIA 전용, 훨씬 빠름). PyTorch 선택 시 모델은 항상 `realesr-general-x4v3` 하나만 사용합니다.
   - 목표 해상도는 8K/4K/1440p/1080p/720p 프리셋 중 선택하거나 "사용자 지정"으로 임의 해상도 지정 가능(기본값 4K UHD 3840x2160)
   - 코덱별로 CRF 의미와 권장 범위가 다르므로(H.264/H.265: 0~51, AV1: 0~63) 코덱을 바꾸면 CRF 기본값도 자동으로 바뀝니다
   - **AI 처리 구간**: 영상 전체가 아니라 일부 구간(시작/길이, 초 단위)만 처리하고 싶을 때 지정. 0(기본값)이면 전체 처리. 구간 밖은 아예 디코딩하지 않아 처리 시간이 구간 길이에만 비례합니다.
3. 필요하면 **고급 설정...** 에서 임시 폴더, 청크 크기, GPU ID, 타일 크기(VRAM 부족 시 줄이기), 오디오 비트레이트, **빠른 모드**(AI 입력 전 원본을 1/2로 축소, 속도 약 4배↑ 대신 디테일 손실) 조정
4. **업스케일 시작** 클릭 → 대기열이 순서대로 처리되며 진행률/로그가 실시간 표시
5. 언제든 **취소**로 중지 가능(현재 청크까지만 진행 후 정지)

## 속도 참고치 (RTX 3050, 1920x1080 입력 기준 실측)

| 설정 | 프레임당 시간 |
|---|---|
| ncnn x4plus, 원본 해상도 | 17.0초 |
| ncnn x4plus, 빠른 모드 | 4.5초 |
| ncnn animevideov3(애니메이션 전용) | 0.65초 |
| PyTorch-CUDA FP16 | 1.75초 |
| PyTorch-CUDA FP16 + 빠른 모드 | **0.44초** |

## 알려진 제한 사항

- 가변 프레임레이트(VFR) 원본은 고정 프레임레이트로 처리되어 미세한 싱크 오차가 있을 수 있습니다.
- 원본 해상도가 매우 낮아 AI 4배 확대로도 목표 해상도에 못 미치면(예: 240p → 4K) 부족분은 일반(Lanczos) 확대로 보정됩니다.
- GPU VRAM이 부족하면 "고급 설정"에서 타일 크기를 줄여주세요(0=자동이 기본, ncnn 엔진에만 적용).
- PyTorch 엔진은 타일링을 지원하지 않습니다(현재 입력 해상도 범위에서는 문제없이 확인됨). 매우 큰 원본을 PyTorch 엔진에 넣으면 VRAM 부족이 날 수 있습니다 — 그런 경우 "빠른 모드"로 사전 축소하거나 ncnn 엔진을 사용하세요.
