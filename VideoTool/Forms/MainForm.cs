using VideoTool.Core.Models;
using VideoTool.Core.Services;

namespace VideoTool
{
    public partial class MainForm : Form
    {
        private static readonly (string Text, int Width, int Height)[] ResolutionPresets =
        [
            ("8K UHD (7680x4320)", 7680, 4320),
            ("4K UHD (3840x2160)", 3840, 2160),
            ("1440p QHD (2560x1440)", 2560, 1440),
            ("1080p FHD (1920x1080)", 1920, 1080),
            ("720p HD (1280x720)", 1280, 720),
        ];
        private const string CustomResolutionLabel = "사용자 지정";

        private readonly List<UpscaleJob> _jobs = new();
        private readonly Dictionary<Guid, ListViewItem> _rowsByJobId = new();
        private readonly DependencyManager _dependencyManager;

        // 고급 설정(임시폴더/GPU/타일크기/청크크기 등)은 SettingsForm에서만 편집되고,
        // 시작 버튼을 누르는 시점에 화면의 기본 설정(모델/해상도/코덱/CRF)과 합쳐져 각 작업에 적용된다.
        private UpscaleSettings _advancedSettings = new();

        private CancellationTokenSource? _runCts;

        public MainForm()
        {
            InitializeComponent();
            _dependencyManager = new DependencyManager();

            PopulateCombos();
            txtOutputFolder.Text = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "VideoTool_4K");

            FormClosing += MainForm_FormClosing;
            _ = RefreshDependencyStatusAsync();
        }

        private readonly record struct ComboOption<T>(string Text, T Value)
        {
            public override string ToString() => Text;
        }

        private void PopulateCombos()
        {
            cmbEngine.Items.Clear();
            foreach (var e in Enum.GetValues<UpscaleEngine>())
                cmbEngine.Items.Add(new ComboOption<UpscaleEngine>(e.ToDisplayName(), e));
            cmbEngine.SelectedIndex = 0;

            cmbModel.Items.Clear();
            foreach (var m in Enum.GetValues<UpscaleModel>())
                cmbModel.Items.Add(new ComboOption<UpscaleModel>(m.ToDisplayName(), m));
            cmbModel.SelectedIndex = 0;

            cmbCodec.Items.Clear();
            foreach (var c in Enum.GetValues<OutputCodec>())
                cmbCodec.Items.Add(new ComboOption<OutputCodec>(c.ToDisplayName(), c));
            cmbCodec.SelectedIndex = 0;

            cmbResolutionPreset.Items.Clear();
            foreach (var preset in ResolutionPresets)
                cmbResolutionPreset.Items.Add(new ComboOption<(int Width, int Height)>(preset.Text, (preset.Width, preset.Height)));
            cmbResolutionPreset.Items.Add(new ComboOption<(int Width, int Height)>(CustomResolutionLabel, (0, 0)));
            cmbResolutionPreset.SelectedIndex = Array.FindIndex(ResolutionPresets, p => p.Width == 3840 && p.Height == 2160);
        }

        // ---- 큐 관리 ----

        private void btnAddVideo_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "영상 파일|*.mp4;*.mkv;*.mov;*.avi;*.webm;*.wmv;*.flv;*.m4v|모든 파일|*.*";
            openFileDialog1.Multiselect = true;
            openFileDialog1.Title = "업스케일할 영상 선택";
            if (openFileDialog1.ShowDialog(this) != DialogResult.OK)
                return;

            foreach (var path in openFileDialog1.FileNames)
                AddJob(path);
        }

        private void AddJob(string inputPath)
        {
            var job = new UpscaleJob
            {
                InputPath = inputPath,
                OutputPath = MakeUniqueOutputPath(inputPath),
            };
            _jobs.Add(job);

            var item = new ListViewItem(job.FileName) { Tag = job };
            item.SubItems.Add("분석 중...");
            item.SubItems.Add($"{job.Settings.TargetWidth}x{job.Settings.TargetHeight}");
            item.SubItems.Add(ModelColumnText(job.Settings));
            item.SubItems.Add(StageToText(job.Stage, null));
            item.SubItems.Add("0%");
            lvJobs.Items.Add(item);
            _rowsByJobId[job.Id] = item;

            _ = ProbeJobAsync(job);
        }

        private async Task ProbeJobAsync(UpscaleJob job)
        {
            try
            {
                var status = _dependencyManager.GetStatus();
                if (!status.FfprobeAvailable)
                {
                    UiThread(() =>
                    {
                        if (_rowsByJobId.TryGetValue(job.Id, out var item))
                            item.SubItems[1].Text = "ffprobe 필요";
                    });
                    return;
                }

                var probe = new FfmpegService(status.FfmpegPath, status.FfprobePath);
                var info = await probe.ProbeAsync(job.InputPath).ConfigureAwait(false);
                job.SourceInfo = info;
                UpdateJobRow(job);
            }
            catch (Exception ex)
            {
                UiThread(() =>
                {
                    if (_rowsByJobId.TryGetValue(job.Id, out var item))
                        item.SubItems[1].Text = "분석 실패";
                });
                AppendLog($"[{job.FileName}] 분석 실패: {ex.Message}");
            }
        }

        private string MakeUniqueOutputPath(string inputPath)
        {
            var folder = string.IsNullOrWhiteSpace(txtOutputFolder.Text)
                ? Environment.GetFolderPath(Environment.SpecialFolder.MyVideos)
                : txtOutputFolder.Text;
            var baseName = Path.GetFileNameWithoutExtension(inputPath);
            var candidate = Path.Combine(folder, $"{baseName}_4K.mp4");

            var existingPaths = _jobs.Select(j => j.OutputPath).ToHashSet(StringComparer.OrdinalIgnoreCase);
            var counter = 1;
            while (existingPaths.Contains(candidate))
            {
                candidate = Path.Combine(folder, $"{baseName}_4K_{counter}.mp4");
                counter++;
            }
            return candidate;
        }

        private void btnRemoveVideo_Click(object sender, EventArgs e)
        {
            foreach (var item in lvJobs.SelectedItems.Cast<ListViewItem>().ToList())
            {
                var job = (UpscaleJob)item.Tag!;
                if (IsActiveStage(job.Stage))
                    continue; // 진행 중인 작업은 먼저 취소해야 제거 가능

                _jobs.Remove(job);
                _rowsByJobId.Remove(job.Id);
                lvJobs.Items.Remove(item);
            }
        }

        private static bool IsActiveStage(JobStage stage) => stage is JobStage.Probing or JobStage.ExtractingAudio
            or JobStage.Processing or JobStage.Concatenating or JobStage.Muxing;

        private void btnBrowseOutput_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtOutputFolder.Text))
                folderBrowserDialog1.SelectedPath = txtOutputFolder.Text;
            if (folderBrowserDialog1.ShowDialog(this) != DialogResult.OK)
                return;

            txtOutputFolder.Text = folderBrowserDialog1.SelectedPath;
            foreach (var job in _jobs.Where(j => j.Stage == JobStage.Queued))
                job.OutputPath = Path.Combine(folderBrowserDialog1.SelectedPath, Path.GetFileName(job.OutputPath));
        }

        // ---- 설정 컨트롤 ----

        private void cmbEngine_SelectedIndexChanged(object sender, EventArgs e)
        {
            var engine = ((ComboOption<UpscaleEngine>)cmbEngine.SelectedItem!).Value;
            // PyTorch 엔진은 realesr-general-x4v3 모델 하나만 사용하므로 ncnn 전용 모델 선택은 의미가 없다.
            cmbModel.Enabled = engine == UpscaleEngine.NcnnVulkan;
        }

        private void cmbResolutionPreset_SelectedIndexChanged(object sender, EventArgs e)
        {
            var (width, height) = ((ComboOption<(int Width, int Height)>)cmbResolutionPreset.SelectedItem!).Value;
            var isCustom = width == 0 && height == 0;
            numTargetWidth.Enabled = isCustom;
            numTargetHeight.Enabled = isCustom;
            if (!isCustom)
            {
                numTargetWidth.Value = width;
                numTargetHeight.Value = height;
            }
        }

        private bool IsCustomResolutionSelected() =>
            cmbResolutionPreset.SelectedItem is ComboOption<(int Width, int Height)> { Value: (0, 0) };

        // H.264/H.265 등 yuv420p 인코딩은 가로/세로가 짝수여야 하므로, 직접 입력으로 홀수가 들어오면 보정한다.
        private void numTargetDimension_Leave(object? sender, EventArgs e)
        {
            if (sender is NumericUpDown numeric && numeric.Value % 2 != 0)
                numeric.Value -= 1;
        }

        private void cmbCodec_SelectedIndexChanged(object sender, EventArgs e)
        {
            var codec = ((ComboOption<OutputCodec>)cmbCodec.SelectedItem!).Value;
            var (min, max, def) = codec.CrfRange();
            numCrf.Minimum = min;
            numCrf.Maximum = max;
            numCrf.Value = def;
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            using var form = new SettingsForm(_advancedSettings.Clone());
            if (form.ShowDialog(this) == DialogResult.OK)
                _advancedSettings = form.ResultSettings;
        }

        private void btnDependencies_Click(object sender, EventArgs e) => OpenDependenciesDialog();

        private void OpenDependenciesDialog()
        {
            using var form = new DependenciesForm(_dependencyManager);
            form.ShowDialog(this);
            _ = RefreshDependencyStatusAsync();
        }

        private Task RefreshDependencyStatusAsync()
        {
            return Task.Run(() =>
            {
                var status = _dependencyManager.GetStatus();
                var pythonStatus = _dependencyManager.GetPythonEngineStatus();
                UiThread(() =>
                {
                    statusLabelDeps.Text = $"FFmpeg/Real-ESRGAN: {(status.AllAvailable ? "준비 완료" : "미설치")} | " +
                        $"PyTorch-CUDA: {(pythonStatus.Available ? "준비 완료" : "미설치")} - 필요 시 '도구 확인/다운로드'";
                });
            });
        }

        private UpscaleSettings BuildSettingsFromUi()
        {
            var settings = _advancedSettings.Clone();
            settings.Engine = ((ComboOption<UpscaleEngine>)cmbEngine.SelectedItem!).Value;
            settings.Model = ((ComboOption<UpscaleModel>)cmbModel.SelectedItem!).Value;
            settings.TargetWidth = (int)numTargetWidth.Value;
            settings.TargetHeight = (int)numTargetHeight.Value;
            settings.Codec = ((ComboOption<OutputCodec>)cmbCodec.SelectedItem!).Value;
            settings.Crf = (int)numCrf.Value;
            settings.TrimStart = numTrimStart.Value > 0 ? TimeSpan.FromSeconds((double)numTrimStart.Value) : null;
            settings.TrimDuration = numTrimDuration.Value > 0 ? TimeSpan.FromSeconds((double)numTrimDuration.Value) : null;
            return settings;
        }

        // ---- 실행 ----

        private async void btnStart_Click(object sender, EventArgs e)
        {
            var status = _dependencyManager.GetStatus();
            if (!status.FfmpegAvailable || !status.FfprobeAvailable)
            {
                var openSetup = MessageBox.Show(this,
                    "FFmpeg이 아직 준비되지 않았습니다. 지금 다운로드하시겠습니까?",
                    "종속성 필요", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (openSetup == DialogResult.Yes)
                    OpenDependenciesDialog();
                return;
            }

            var engine = ((ComboOption<UpscaleEngine>)cmbEngine.SelectedItem!).Value;
            if (engine == UpscaleEngine.NcnnVulkan && !status.RealEsrganAvailable)
            {
                var openSetup = MessageBox.Show(this,
                    "Real-ESRGAN(ncnn-Vulkan)이 아직 준비되지 않았습니다. 지금 다운로드하시겠습니까?",
                    "종속성 필요", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (openSetup == DialogResult.Yes)
                    OpenDependenciesDialog();
                return;
            }

            var pythonStatus = _dependencyManager.GetPythonEngineStatus();
            if (engine == UpscaleEngine.PyTorchCuda && !pythonStatus.Available)
            {
                var openSetup = MessageBox.Show(this,
                    "PyTorch-CUDA 엔진이 아직 준비되지 않았습니다. 지금 다운로드하시겠습니까?",
                    "종속성 필요", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (openSetup == DialogResult.Yes)
                    OpenDependenciesDialog();
                return;
            }

            var pendingJobs = _jobs.Where(j => j.Stage is JobStage.Queued or JobStage.Failed or JobStage.Canceled).ToList();
            if (pendingJobs.Count == 0)
            {
                MessageBox.Show(this, "대기 중인 작업이 없습니다.", "알림", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SetRunningUiState(true);
            _runCts = new CancellationTokenSource();
            var ffmpeg = new FfmpegService(status.FfmpegPath, status.FfprobePath);
            var realEsrgan = new RealEsrganService(status.RealEsrganPath, status.RealEsrganModelsDir);
            var pyTorchRealEsrgan = new PyTorchRealEsrganService(pythonStatus.PythonExePath, pythonStatus.InferScriptPath, pythonStatus.ModelPath);

            try
            {
                foreach (var job in pendingJobs)
                {
                    if (_runCts.IsCancellationRequested)
                        break;

                    job.Settings = BuildSettingsFromUi();
                    job.ErrorMessage = null;

                    var pipeline = new VideoUpscalePipeline(ffmpeg, realEsrgan, pyTorchRealEsrgan);
                    pipeline.ProgressChanged += p => OnPipelineProgress(job, p);
                    pipeline.LogReceived += line => AppendLog($"[{job.FileName}] {line}");

                    UpdateJobRow(job);

                    try
                    {
                        await pipeline.RunAsync(job, _runCts.Token);
                        AppendLog($"[{job.FileName}] 완료 -> {job.OutputPath}");
                    }
                    catch (OperationCanceledException)
                    {
                        AppendLog($"[{job.FileName}] 취소되었습니다.");
                        UpdateJobRow(job);
                        break;
                    }
                    catch (Exception ex)
                    {
                        AppendLog($"[{job.FileName}] 오류: {ex.Message}");
                    }
                    finally
                    {
                        UpdateJobRow(job);
                    }
                }
            }
            finally
            {
                SetRunningUiState(false);
                _runCts?.Dispose();
                _runCts = null;
                UiThread(() => statusLabelJob.Text = "대기 중");
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _runCts?.Cancel();
            btnCancel.Enabled = false;
            AppendLog("취소 요청됨 - 현재 단계가 끝나는 대로 중지합니다.");
        }

        private void SetRunningUiState(bool running)
        {
            UiThread(() =>
            {
                btnAddVideo.Enabled = !running;
                btnRemoveVideo.Enabled = !running;
                btnStart.Enabled = !running;
                btnSettings.Enabled = !running;
                btnDependencies.Enabled = !running;
                cmbEngine.Enabled = !running;
                cmbModel.Enabled = !running && ((ComboOption<UpscaleEngine>)cmbEngine.SelectedItem!).Value == UpscaleEngine.NcnnVulkan;
                cmbResolutionPreset.Enabled = !running;
                cmbCodec.Enabled = !running;
                numCrf.Enabled = !running;
                btnBrowseOutput.Enabled = !running;
                numTargetWidth.Enabled = !running && IsCustomResolutionSelected();
                numTargetHeight.Enabled = !running && IsCustomResolutionSelected();
                btnCancel.Enabled = running;
                if (!running)
                    progressBarCurrent.Value = 0;
            });
        }

        // ---- 진행률 / 로그 ----

        private void OnPipelineProgress(UpscaleJob job, JobProgress p)
        {
            UiThread(() =>
            {
                UpdateJobRow(job);
                progressBarCurrent.Value = Math.Clamp((int)p.PercentComplete, 0, 100);
                var etaText = p.Eta is { } eta ? $" (남은 시간 약 {FormatEta(eta)})" : "";
                statusLabelJob.Text = $"{job.FileName}: {p.Message}{etaText}";
            });
        }

        private void UpdateJobRow(UpscaleJob job)
        {
            UiThread(() =>
            {
                if (!_rowsByJobId.TryGetValue(job.Id, out var item))
                    return;

                item.SubItems[1].Text = job.SourceInfo is { } info ? $"{info.Width}x{info.Height}" : item.SubItems[1].Text;
                item.SubItems[2].Text = $"{job.Settings.TargetWidth}x{job.Settings.TargetHeight}";
                item.SubItems[3].Text = ModelColumnText(job.Settings);
                item.SubItems[4].Text = StageToText(job.Stage, job.ErrorMessage);
                item.SubItems[5].Text = $"{job.PercentComplete:0}%";
            });
        }

        private static string ModelColumnText(UpscaleSettings settings) => settings.Engine switch
        {
            UpscaleEngine.PyTorchCuda => "PyTorch-CUDA (general-x4v3)",
            _ => settings.Model.ToDisplayName(),
        };

        private static string StageToText(JobStage stage, string? error) => stage switch
        {
            JobStage.Queued => "대기 중",
            JobStage.Probing => "분석 중",
            JobStage.ExtractingAudio => "오디오 추출 중",
            JobStage.Processing => "처리 중",
            JobStage.Concatenating => "병합 중",
            JobStage.Muxing => "저장 중",
            JobStage.Completed => "완료",
            JobStage.Failed => string.IsNullOrEmpty(error) ? "실패" : $"실패: {error}",
            JobStage.Canceled => "취소됨",
            _ => stage.ToString(),
        };

        private static string FormatEta(TimeSpan ts)
        {
            if (ts.TotalHours >= 1)
                return $"{(int)ts.TotalHours}시간 {ts.Minutes}분";
            if (ts.TotalMinutes >= 1)
                return $"{ts.Minutes}분 {ts.Seconds}초";
            return $"{Math.Max(ts.Seconds, 0)}초";
        }

        private void AppendLog(string line)
        {
            UiThread(() => txtLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {line}{Environment.NewLine}"));
        }

        private void MainForm_FormClosing(object? sender, FormClosingEventArgs e)
        {
            if (_runCts is not null && !_runCts.IsCancellationRequested)
            {
                var result = MessageBox.Show(this,
                    "작업이 진행 중입니다. 종료하면 현재 작업이 취소됩니다. 종료할까요?",
                    "종료 확인", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result != DialogResult.Yes)
                {
                    e.Cancel = true;
                    return;
                }
                _runCts.Cancel();
            }
        }

        // realesrgan/ffmpeg 콜백이나 async 라이브러리 코드는 ConfigureAwait(false)로 실행되어
        // 임의의 스레드풀 스레드에서 돌아오므로, WinForms 컨트롤을 만지는 모든 코드는 이 헬퍼를 통해야 한다.
        private void UiThread(Action action)
        {
            if (IsDisposed || Disposing)
                return;
            try
            {
                if (InvokeRequired)
                    BeginInvoke(action);
                else
                    action();
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        }
    }
}
