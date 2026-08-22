namespace VideoTool
{
    partial class SettingsForm
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
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();

            this.table = new System.Windows.Forms.TableLayoutPanel();
            this.lblTempDir = new System.Windows.Forms.Label();
            this.txtTempDir = new System.Windows.Forms.TextBox();
            this.btnBrowseTempDir = new System.Windows.Forms.Button();
            this.lblChunkFrameCount = new System.Windows.Forms.Label();
            this.numChunkFrameCount = new System.Windows.Forms.NumericUpDown();
            this.lblGpuId = new System.Windows.Forms.Label();
            this.txtGpuId = new System.Windows.Forms.TextBox();
            this.lblTileSize = new System.Windows.Forms.Label();
            this.numTileSize = new System.Windows.Forms.NumericUpDown();
            this.lblIntermediateFormat = new System.Windows.Forms.Label();
            this.cmbIntermediateFormat = new System.Windows.Forms.ComboBox();
            this.lblAudioBitrate = new System.Windows.Forms.Label();
            this.numAudioBitrate = new System.Windows.Forms.NumericUpDown();
            this.chkKeepTempFiles = new System.Windows.Forms.CheckBox();
            this.chkFastPreDownscale = new System.Windows.Forms.CheckBox();

            this.lblHint = new System.Windows.Forms.Label();
            this.buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();

            this.table.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numChunkFrameCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTileSize)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAudioBitrate)).BeginInit();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();

            // table
            this.table.Dock = System.Windows.Forms.DockStyle.Top;
            this.table.ColumnCount = 3;
            this.table.RowCount = 8;
            this.table.Padding = new System.Windows.Forms.Padding(10);
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 160F));
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            for (int i = 0; i < 8; i++)
                this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 34F));

            this.lblTempDir.Text = "임시 작업 폴더:";
            this.lblTempDir.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTempDir.AutoSize = true;
            this.txtTempDir.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtTempDir.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.btnBrowseTempDir.Text = "찾아보기...";
            this.btnBrowseTempDir.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnBrowseTempDir.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnBrowseTempDir.Click += new System.EventHandler(this.btnBrowseTempDir_Click);
            this.table.Controls.Add(this.lblTempDir, 0, 0);
            this.table.Controls.Add(this.txtTempDir, 1, 0);
            this.table.Controls.Add(this.btnBrowseTempDir, 2, 0);

            this.lblChunkFrameCount.Text = "청크 크기(프레임 수):";
            this.lblChunkFrameCount.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblChunkFrameCount.AutoSize = true;
            this.numChunkFrameCount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numChunkFrameCount.Minimum = 30;
            this.numChunkFrameCount.Maximum = 3000;
            this.numChunkFrameCount.Increment = 30;
            this.numChunkFrameCount.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.table.Controls.Add(this.lblChunkFrameCount, 0, 1);
            this.table.Controls.Add(this.numChunkFrameCount, 1, 1);

            this.lblGpuId.Text = "GPU ID(비우면 자동):";
            this.lblGpuId.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblGpuId.AutoSize = true;
            this.txtGpuId.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtGpuId.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.table.Controls.Add(this.lblGpuId, 0, 2);
            this.table.Controls.Add(this.txtGpuId, 1, 2);

            this.lblTileSize.Text = "타일 크기(0=자동):";
            this.lblTileSize.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblTileSize.AutoSize = true;
            this.numTileSize.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numTileSize.Minimum = 0;
            this.numTileSize.Maximum = 4096;
            this.numTileSize.Increment = 32;
            this.numTileSize.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.table.Controls.Add(this.lblTileSize, 0, 3);
            this.table.Controls.Add(this.numTileSize, 1, 3);

            this.lblIntermediateFormat.Text = "중간 프레임 형식:";
            this.lblIntermediateFormat.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblIntermediateFormat.AutoSize = true;
            this.cmbIntermediateFormat.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbIntermediateFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbIntermediateFormat.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.table.Controls.Add(this.lblIntermediateFormat, 0, 4);
            this.table.Controls.Add(this.cmbIntermediateFormat, 1, 4);

            this.lblAudioBitrate.Text = "오디오 비트레이트(kbps):";
            this.lblAudioBitrate.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblAudioBitrate.AutoSize = true;
            this.numAudioBitrate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numAudioBitrate.Minimum = 64;
            this.numAudioBitrate.Maximum = 320;
            this.numAudioBitrate.Increment = 32;
            this.numAudioBitrate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.table.Controls.Add(this.lblAudioBitrate, 0, 5);
            this.table.Controls.Add(this.numAudioBitrate, 1, 5);

            this.chkKeepTempFiles.Text = "임시 프레임 파일 보존(디버그용, 디스크 많이 사용)";
            this.chkKeepTempFiles.AutoSize = true;
            this.chkKeepTempFiles.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkKeepTempFiles.Margin = new System.Windows.Forms.Padding(3, 6, 3, 4);
            this.table.Controls.Add(this.chkKeepTempFiles, 0, 6);
            this.table.SetColumnSpan(this.chkKeepTempFiles, 3);

            this.chkFastPreDownscale.Text = "빠른 모드: AI 처리 전 원본을 1/2 해상도로 축소(속도 약 4배↑, 디테일 손실 있음)";
            this.chkFastPreDownscale.AutoSize = true;
            this.chkFastPreDownscale.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkFastPreDownscale.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.table.Controls.Add(this.chkFastPreDownscale, 0, 7);
            this.table.SetColumnSpan(this.chkFastPreDownscale, 3);

            // lblHint
            this.lblHint.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblHint.Padding = new System.Windows.Forms.Padding(10, 4, 10, 4);
            this.lblHint.Height = 60;
            this.lblHint.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblHint.Text = "청크 크기가 클수록 진행 중 임시 디스크 사용량이 늘어나지만 처리 단위가 커집니다.\n" +
                "타일 크기를 줄이면 VRAM 사용량이 줄어드는 대신 속도가 느려집니다(그래픽 메모리 부족 오류 시 조정).";

            // buttonPanel
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.buttonPanel.Padding = new System.Windows.Forms.Padding(10);
            this.btnCancel.Text = "취소";
            this.btnCancel.AutoSize = true;
            this.btnCancel.Padding = new System.Windows.Forms.Padding(10, 4, 10, 4);
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnOk.Text = "확인";
            this.btnOk.AutoSize = true;
            this.btnOk.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.btnOk.Padding = new System.Windows.Forms.Padding(10, 4, 10, 4);
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            this.buttonPanel.Controls.Add(this.btnCancel);
            this.buttonPanel.Controls.Add(this.btnOk);

            // SettingsForm
            this.AcceptButton = this.btnOk;
            this.CancelButton = this.btnCancel;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ClientSize = new System.Drawing.Size(480, 454);
            this.Text = "고급 설정";
            this.Controls.Add(this.lblHint);
            this.Controls.Add(this.table);
            this.Controls.Add(this.buttonPanel);

            this.table.ResumeLayout(false);
            this.table.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numChunkFrameCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTileSize)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numAudioBitrate)).EndInit();
            this.buttonPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TableLayoutPanel table;
        private System.Windows.Forms.Label lblTempDir;
        private System.Windows.Forms.TextBox txtTempDir;
        private System.Windows.Forms.Button btnBrowseTempDir;
        private System.Windows.Forms.Label lblChunkFrameCount;
        private System.Windows.Forms.NumericUpDown numChunkFrameCount;
        private System.Windows.Forms.Label lblGpuId;
        private System.Windows.Forms.TextBox txtGpuId;
        private System.Windows.Forms.Label lblTileSize;
        private System.Windows.Forms.NumericUpDown numTileSize;
        private System.Windows.Forms.Label lblIntermediateFormat;
        private System.Windows.Forms.ComboBox cmbIntermediateFormat;
        private System.Windows.Forms.Label lblAudioBitrate;
        private System.Windows.Forms.NumericUpDown numAudioBitrate;
        private System.Windows.Forms.CheckBox chkKeepTempFiles;
        private System.Windows.Forms.CheckBox chkFastPreDownscale;
        private System.Windows.Forms.Label lblHint;
        private System.Windows.Forms.FlowLayoutPanel buttonPanel;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}
