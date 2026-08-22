using System.Diagnostics;
using VideoTool.Core.Services;

namespace VideoTool
{
    public partial class DependenciesForm : Form
    {
        private readonly DependencyManager _manager;
        private CancellationTokenSource? _activeCts;

        public DependenciesForm(DependencyManager manager)
        {
            InitializeComponent();
            _manager = manager;
            RefreshStatus();
        }

        private void RefreshStatus()
        {
            var status = _manager.GetStatus();

            var ffmpegOk = status.FfmpegAvailable && status.FfprobeAvailable;
            lblFfmpegStatus.Text = ffmpegOk ? "설치됨" : "미설치";
            lblFfmpegStatus.ForeColor = ffmpegOk ? Color.SeaGreen : Color.Firebrick;
            btnDownloadFfmpeg.Text = ffmpegOk ? "다시 받기" : "다운로드";

            lblRealesrganStatus.Text = status.RealEsrganAvailable ? "설치됨" : "미설치";
            lblRealesrganStatus.ForeColor = status.RealEsrganAvailable ? Color.SeaGreen : Color.Firebrick;
            btnDownloadRealesrgan.Text = status.RealEsrganAvailable ? "다시 받기" : "다운로드";

            var pythonStatus = _manager.GetPythonEngineStatus();
            lblPyTorchStatus.Text = pythonStatus.Available ? "설치됨" : "미설치";
            lblPyTorchStatus.ForeColor = pythonStatus.Available ? Color.SeaGreen : Color.Firebrick;
            btnDownloadPyTorch.Text = pythonStatus.Available ? "다시 받기" : "다운로드";
        }

        private async void btnDownloadFfmpeg_Click(object sender, EventArgs e)
        {
            await RunDownloadAsync("FFmpeg", progFfmpeg, _manager.DownloadFfmpegAsync);
        }

        private async void btnDownloadRealesrgan_Click(object sender, EventArgs e)
        {
            await RunDownloadAsync("Real-ESRGAN", progRealesrgan, _manager.DownloadRealEsrganAsync);
        }

        private async Task RunDownloadAsync(string name, ProgressBar bar, Func<IProgress<double>, CancellationToken, Task> download)
        {
            SetBusy(true);
            AppendLog($"{name} 다운로드를 시작합니다...");
            var progress = new Progress<double>(p => bar.Value = Math.Clamp((int)(p * 100), 0, 100));

            using var cts = new CancellationTokenSource();
            _activeCts = cts;
            try
            {
                await download(progress, cts.Token);
                AppendLog($"{name} 설치 완료.");
            }
            catch (OperationCanceledException)
            {
                AppendLog($"{name} 다운로드가 취소되었습니다.");
            }
            catch (Exception ex)
            {
                AppendLog($"{name} 다운로드 실패: {ex.Message}");
                MessageBox.Show(this,
                    $"{name} 다운로드에 실패했습니다.\n\n{ex.Message}\n\n네트워크 연결을 확인하거나, 공식 페이지에서 직접 받아 폴더에 넣어주세요.",
                    "다운로드 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _activeCts = null;
                bar.Value = 0;
                SetBusy(false);
                RefreshStatus();
            }
        }

        private async void btnDownloadPyTorch_Click(object sender, EventArgs e)
        {
            var confirm = MessageBox.Show(this,
                "PyTorch-CUDA 엔진은 약 3GB를 다운로드합니다(Python 런타임 + PyTorch + AI 모델). NVIDIA GPU가 있는 경우에만 유효합니다.\n계속할까요?",
                "PyTorch-CUDA 설치", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
                return;

            SetBusy(true);
            AppendLog("PyTorch-CUDA 엔진 설치를 시작합니다...");
            var progress = new Progress<double>(p => progPyTorch.Value = Math.Clamp((int)(p * 100), 0, 100));

            using var cts = new CancellationTokenSource();
            _activeCts = cts;
            try
            {
                await _manager.DownloadPythonEngineAsync(AppendLog, progress, AppendLog, cts.Token);
                AppendLog("PyTorch-CUDA 엔진 설치 완료.");
            }
            catch (OperationCanceledException)
            {
                AppendLog("PyTorch-CUDA 설치가 취소되었습니다.");
            }
            catch (Exception ex)
            {
                AppendLog($"PyTorch-CUDA 설치 실패: {ex.Message}");
                MessageBox.Show(this,
                    $"PyTorch-CUDA 설치에 실패했습니다.\n\n{ex.Message}",
                    "설치 실패", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                _activeCts = null;
                progPyTorch.Value = 0;
                SetBusy(false);
                RefreshStatus();
            }
        }

        private void btnCancelDownload_Click(object sender, EventArgs e) => _activeCts?.Cancel();

        private void SetBusy(bool busy)
        {
            btnDownloadFfmpeg.Enabled = !busy;
            btnDownloadRealesrgan.Enabled = !busy;
            btnDownloadPyTorch.Enabled = !busy;
            btnCancelDownload.Enabled = busy;
        }

        private void AppendLog(string line) => txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {line}{Environment.NewLine}");

        private void btnClose_Click(object sender, EventArgs e) => Close();

        private void linkFfmpegSource_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) =>
            OpenUrl("https://github.com/BtbN/FFmpeg-Builds");

        private void linkRealEsrganSource_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) =>
            OpenUrl("https://github.com/xinntao/Real-ESRGAN/releases/tag/v0.2.5.0");

        private void linkPyTorchSource_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) =>
            OpenUrl("https://pytorch.org");

        private static void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            catch
            {
                // 브라우저 실행 실패는 치명적이지 않으므로 무시
            }
        }
    }
}
