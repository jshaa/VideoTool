namespace VideoTool
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();

            this.toolbarPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAddVideo = new System.Windows.Forms.Button();
            this.btnRemoveVideo = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            this.btnDependencies = new System.Windows.Forms.Button();

            this.settingsGroupBox = new System.Windows.Forms.GroupBox();
            this.settingsTable = new System.Windows.Forms.TableLayoutPanel();
            this.lblEngine = new System.Windows.Forms.Label();
            this.cmbEngine = new System.Windows.Forms.ComboBox();
            this.lblModel = new System.Windows.Forms.Label();
            this.cmbModel = new System.Windows.Forms.ComboBox();
            this.lblResolution = new System.Windows.Forms.Label();
            this.cmbResolutionPreset = new System.Windows.Forms.ComboBox();
            this.numTargetWidth = new System.Windows.Forms.NumericUpDown();
            this.lblX = new System.Windows.Forms.Label();
            this.numTargetHeight = new System.Windows.Forms.NumericUpDown();
            this.lblCodec = new System.Windows.Forms.Label();
            this.cmbCodec = new System.Windows.Forms.ComboBox();
            this.lblCrf = new System.Windows.Forms.Label();
            this.numCrf = new System.Windows.Forms.NumericUpDown();
            this.lblTrim = new System.Windows.Forms.Label();
            this.numTrimStart = new System.Windows.Forms.NumericUpDown();
            this.lblTrimTilde = new System.Windows.Forms.Label();
            this.numTrimDuration = new System.Windows.Forms.NumericUpDown();
            this.lblTrimHint = new System.Windows.Forms.Label();
            this.lblOutputFolder = new System.Windows.Forms.Label();
            this.txtOutputFolder = new System.Windows.Forms.TextBox();
            this.btnBrowseOutput = new System.Windows.Forms.Button();

            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.lvJobs = new System.Windows.Forms.ListView();
            this.colFileName = new System.Windows.Forms.ColumnHeader();
            this.colInputRes = new System.Windows.Forms.ColumnHeader();
            this.colTargetRes = new System.Windows.Forms.ColumnHeader();
            this.colModel = new System.Windows.Forms.ColumnHeader();
            this.colStatus = new System.Windows.Forms.ColumnHeader();
            this.colProgress = new System.Windows.Forms.ColumnHeader();
            this.logPanel = new System.Windows.Forms.Panel();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.progressBarCurrent = new System.Windows.Forms.ProgressBar();

            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabelDeps = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusLabelJob = new System.Windows.Forms.ToolStripStatusLabel();

            this.settingsGroupBox.SuspendLayout();
            this.settingsTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTargetWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTargetHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCrf)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTrimStart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTrimDuration)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.logPanel.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();

            // toolbarPanel
            this.toolbarPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolbarPanel.FlowDirection = System.Windows.Forms.FlowDirection.LeftToRight;
            this.toolbarPanel.Padding = new System.Windows.Forms.Padding(8);
            this.toolbarPanel.AutoSize = true;
            this.toolbarPanel.Controls.Add(this.btnAddVideo);
            this.toolbarPanel.Controls.Add(this.btnRemoveVideo);
            this.toolbarPanel.Controls.Add(this.btnStart);
            this.toolbarPanel.Controls.Add(this.btnCancel);
            this.toolbarPanel.Controls.Add(this.btnSettings);
            this.toolbarPanel.Controls.Add(this.btnDependencies);

            // btnAddVideo
            this.btnAddVideo.Text = "영상 추가...";
            this.btnAddVideo.AutoSize = true;
            this.btnAddVideo.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.btnAddVideo.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this.btnAddVideo.Click += new System.EventHandler(this.btnAddVideo_Click);

            // btnRemoveVideo
            this.btnRemoveVideo.Text = "선택 제거";
            this.btnRemoveVideo.AutoSize = true;
            this.btnRemoveVideo.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.btnRemoveVideo.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this.btnRemoveVideo.Click += new System.EventHandler(this.btnRemoveVideo_Click);

            // btnStart
            this.btnStart.Text = "업스케일 시작";
            this.btnStart.AutoSize = true;
            this.btnStart.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.btnStart.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this.btnStart.Font = new System.Drawing.Font(this.Font, System.Drawing.FontStyle.Bold);
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);

            // btnCancel
            this.btnCancel.Text = "취소";
            this.btnCancel.AutoSize = true;
            this.btnCancel.Margin = new System.Windows.Forms.Padding(0, 0, 20, 0);
            this.btnCancel.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this.btnCancel.Enabled = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // btnSettings
            this.btnSettings.Text = "고급 설정...";
            this.btnSettings.AutoSize = true;
            this.btnSettings.Margin = new System.Windows.Forms.Padding(0, 0, 6, 0);
            this.btnSettings.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this.btnSettings.Click += new System.EventHandler(this.btnSettings_Click);

            // btnDependencies
            this.btnDependencies.Text = "도구 확인/다운로드...";
            this.btnDependencies.AutoSize = true;
            this.btnDependencies.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this.btnDependencies.Click += new System.EventHandler(this.btnDependencies_Click);

            // settingsTable
            this.settingsTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.settingsTable.ColumnCount = 5;
            this.settingsTable.RowCount = 6;
            this.settingsTable.Padding = new System.Windows.Forms.Padding(4);
            this.settingsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.settingsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.settingsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.settingsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.settingsTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            for (int i = 0; i < 6; i++)
            {
                this.settingsTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            }

            // Row 0: 모델
            this.lblModel.Text = "AI 모델:";
            this.lblModel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblModel.AutoSize = true;
            this.cmbModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbModel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbModel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.settingsTable.Controls.Add(this.lblModel, 0, 0);
            this.settingsTable.Controls.Add(this.cmbModel, 1, 0);
            this.settingsTable.SetColumnSpan(this.cmbModel, 4);

            // Row 1: 목표 해상도
            this.lblResolution.Text = "목표 해상도:";
            this.lblResolution.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblResolution.AutoSize = true;
            this.cmbResolutionPreset.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbResolutionPreset.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbResolutionPreset.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbResolutionPreset.SelectedIndexChanged += new System.EventHandler(this.cmbResolutionPreset_SelectedIndexChanged);
            this.numTargetWidth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numTargetWidth.Minimum = 16;
            this.numTargetWidth.Maximum = 16384;
            this.numTargetWidth.Increment = 2;
            this.numTargetWidth.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numTargetWidth.Enabled = false;
            this.numTargetWidth.Leave += new System.EventHandler(this.numTargetDimension_Leave);
            this.lblX.Text = "x";
            this.lblX.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblX.AutoSize = true;
            this.lblX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.numTargetHeight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numTargetHeight.Minimum = 16;
            this.numTargetHeight.Maximum = 16384;
            this.numTargetHeight.Increment = 2;
            this.numTargetHeight.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numTargetHeight.Enabled = false;
            this.numTargetHeight.Leave += new System.EventHandler(this.numTargetDimension_Leave);
            this.settingsTable.Controls.Add(this.lblResolution, 0, 1);
            this.settingsTable.Controls.Add(this.cmbResolutionPreset, 1, 1);
            this.settingsTable.Controls.Add(this.numTargetWidth, 2, 1);
            this.settingsTable.Controls.Add(this.lblX, 3, 1);
            this.settingsTable.Controls.Add(this.numTargetHeight, 4, 1);

            // Row 2: 코덱 / CRF
            this.lblCodec.Text = "코덱:";
            this.lblCodec.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCodec.AutoSize = true;
            this.cmbCodec.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbCodec.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCodec.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbCodec.SelectedIndexChanged += new System.EventHandler(this.cmbCodec_SelectedIndexChanged);
            this.lblCrf.Text = "화질(CRF):";
            this.lblCrf.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCrf.AutoSize = true;
            this.numCrf.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numCrf.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.settingsTable.Controls.Add(this.lblCodec, 0, 2);
            this.settingsTable.Controls.Add(this.cmbCodec, 1, 2);
            this.settingsTable.SetColumnSpan(this.cmbCodec, 2);
            this.settingsTable.Controls.Add(this.lblCrf, 3, 2);
            this.settingsTable.Controls.Add(this.numCrf, 4, 2);

            // Row 3: 저장 폴더
            this.lblOutputFolder.Text = "저장 폴더:";
            this.lblOutputFolder.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblOutputFolder.AutoSize = true;
            this.txtOutputFolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtOutputFolder.ReadOnly = true;
            this.txtOutputFolder.Margin = new System.Windows.Forms.Padding(3, 6, 3, 4);
            this.btnBrowseOutput.Text = "찾아보기...";
            this.btnBrowseOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBrowseOutput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnBrowseOutput.Click += new System.EventHandler(this.btnBrowseOutput_Click);
            this.settingsTable.Controls.Add(this.lblOutputFolder, 0, 3);
            this.settingsTable.Controls.Add(this.txtOutputFolder, 1, 3);
            this.settingsTable.SetColumnSpan(this.txtOutputFolder, 3);
            this.settingsTable.Controls.Add(this.btnBrowseOutput, 4, 3);

            // Row 4: AI 처리 구간 (전체 영상 중 일부만 AI 업스케일하고 싶을 때 사용)
            this.lblTrim.Text = "AI 처리 구간:";
            this.lblTrim.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTrim.AutoSize = true;
            this.numTrimStart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numTrimStart.Minimum = 0;
            this.numTrimStart.Maximum = 86400;
            this.numTrimStart.Increment = 5;
            this.numTrimStart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lblTrimTilde.Text = "~ 길이(초):";
            this.lblTrimTilde.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTrimTilde.AutoSize = true;
            this.numTrimDuration.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numTrimDuration.Minimum = 0;
            this.numTrimDuration.Maximum = 86400;
            this.numTrimDuration.Increment = 5;
            this.numTrimDuration.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.lblTrimHint.Text = "(초 단위, 0=전체 영상)";
            this.lblTrimHint.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTrimHint.AutoSize = true;
            this.lblTrimHint.ForeColor = System.Drawing.SystemColors.GrayText;
            this.settingsTable.Controls.Add(this.lblTrim, 0, 4);
            this.settingsTable.Controls.Add(this.numTrimStart, 1, 4);
            this.settingsTable.Controls.Add(this.lblTrimTilde, 2, 4);
            this.settingsTable.Controls.Add(this.numTrimDuration, 3, 4);
            this.settingsTable.Controls.Add(this.lblTrimHint, 4, 4);

            // Row 5: AI 엔진
            this.lblEngine.Text = "AI 엔진:";
            this.lblEngine.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblEngine.AutoSize = true;
            this.cmbEngine.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbEngine.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbEngine.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbEngine.SelectedIndexChanged += new System.EventHandler(this.cmbEngine_SelectedIndexChanged);
            this.settingsTable.Controls.Add(this.lblEngine, 0, 5);
            this.settingsTable.Controls.Add(this.cmbEngine, 1, 5);
            this.settingsTable.SetColumnSpan(this.cmbEngine, 4);

            // settingsGroupBox
            this.settingsGroupBox.Text = "출력 설정";
            this.settingsGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.settingsGroupBox.Height = 239;
            this.settingsGroupBox.Padding = new System.Windows.Forms.Padding(6);
            this.settingsGroupBox.Controls.Add(this.settingsTable);

            // lvJobs
            this.lvJobs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvJobs.View = System.Windows.Forms.View.Details;
            this.lvJobs.FullRowSelect = true;
            this.lvJobs.GridLines = true;
            this.lvJobs.HideSelection = false;
            this.lvJobs.MultiSelect = true;
            this.colFileName.Text = "파일명";
            this.colFileName.Width = 260;
            this.colInputRes.Text = "입력 해상도";
            this.colInputRes.Width = 110;
            this.colTargetRes.Text = "목표 해상도";
            this.colTargetRes.Width = 110;
            this.colModel.Text = "모델";
            this.colModel.Width = 170;
            this.colStatus.Text = "상태";
            this.colStatus.Width = 130;
            this.colProgress.Text = "진행률";
            this.colProgress.Width = 80;
            this.lvJobs.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
                this.colFileName, this.colInputRes, this.colTargetRes, this.colModel, this.colStatus, this.colProgress});

            // progressBarCurrent
            this.progressBarCurrent.Dock = System.Windows.Forms.DockStyle.Top;
            this.progressBarCurrent.Height = 20;
            this.progressBarCurrent.Minimum = 0;
            this.progressBarCurrent.Maximum = 100;

            // txtLog
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Multiline = true;
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtLog.BackColor = System.Drawing.Color.Black;
            this.txtLog.ForeColor = System.Drawing.Color.Gainsboro;

            // logPanel
            this.logPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.logPanel.Controls.Add(this.txtLog);
            this.logPanel.Controls.Add(this.progressBarCurrent);

            // splitContainer
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainer.Panel1.Controls.Add(this.lvJobs);
            this.splitContainer.Panel2.Controls.Add(this.logPanel);
            this.splitContainer.SplitterDistance = 260;
            this.splitContainer.SplitterWidth = 6;

            // statusStrip
            this.statusLabelDeps.Text = "종속성 확인 중...";
            this.statusLabelDeps.Spring = false;
            this.statusLabelJob.Text = "";
            this.statusLabelJob.Spring = true;
            this.statusLabelJob.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.statusStrip.Items.Add(this.statusLabelJob);
            this.statusStrip.Items.Add(this.statusLabelDeps);

            // MainForm
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1040, 784);
            this.MinimumSize = new System.Drawing.Size(860, 624);
            this.Text = "VideoTool - AI 4K 업스케일러 (Real-ESRGAN + FFmpeg)";
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.settingsGroupBox);
            this.Controls.Add(this.toolbarPanel);
            this.Controls.Add(this.statusStrip);

            this.settingsGroupBox.ResumeLayout(false);
            this.settingsTable.ResumeLayout(false);
            this.settingsTable.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numTargetWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTargetHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCrf)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTrimStart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTrimDuration)).EndInit();
            this.logPanel.ResumeLayout(false);
            this.logPanel.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;

        private System.Windows.Forms.FlowLayoutPanel toolbarPanel;
        private System.Windows.Forms.Button btnAddVideo;
        private System.Windows.Forms.Button btnRemoveVideo;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.Button btnDependencies;

        private System.Windows.Forms.GroupBox settingsGroupBox;
        private System.Windows.Forms.TableLayoutPanel settingsTable;
        private System.Windows.Forms.Label lblEngine;
        private System.Windows.Forms.ComboBox cmbEngine;
        private System.Windows.Forms.Label lblModel;
        private System.Windows.Forms.ComboBox cmbModel;
        private System.Windows.Forms.Label lblResolution;
        private System.Windows.Forms.ComboBox cmbResolutionPreset;
        private System.Windows.Forms.NumericUpDown numTargetWidth;
        private System.Windows.Forms.Label lblX;
        private System.Windows.Forms.NumericUpDown numTargetHeight;
        private System.Windows.Forms.Label lblCodec;
        private System.Windows.Forms.ComboBox cmbCodec;
        private System.Windows.Forms.Label lblCrf;
        private System.Windows.Forms.NumericUpDown numCrf;
        private System.Windows.Forms.Label lblTrim;
        private System.Windows.Forms.NumericUpDown numTrimStart;
        private System.Windows.Forms.Label lblTrimTilde;
        private System.Windows.Forms.NumericUpDown numTrimDuration;
        private System.Windows.Forms.Label lblTrimHint;
        private System.Windows.Forms.Label lblOutputFolder;
        private System.Windows.Forms.TextBox txtOutputFolder;
        private System.Windows.Forms.Button btnBrowseOutput;

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ListView lvJobs;
        private System.Windows.Forms.ColumnHeader colFileName;
        private System.Windows.Forms.ColumnHeader colInputRes;
        private System.Windows.Forms.ColumnHeader colTargetRes;
        private System.Windows.Forms.ColumnHeader colModel;
        private System.Windows.Forms.ColumnHeader colStatus;
        private System.Windows.Forms.ColumnHeader colProgress;
        private System.Windows.Forms.Panel logPanel;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.ProgressBar progressBarCurrent;

        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelDeps;
        private System.Windows.Forms.ToolStripStatusLabel statusLabelJob;
    }
}
