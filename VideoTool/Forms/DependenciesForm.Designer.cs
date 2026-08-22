namespace VideoTool
{
    partial class DependenciesForm
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

            this.lblIntro = new System.Windows.Forms.Label();
            this.table = new System.Windows.Forms.TableLayoutPanel();

            this.lblFfmpegName = new System.Windows.Forms.Label();
            this.lblFfmpegStatus = new System.Windows.Forms.Label();
            this.progFfmpeg = new System.Windows.Forms.ProgressBar();
            this.btnDownloadFfmpeg = new System.Windows.Forms.Button();
            this.linkFfmpegSource = new System.Windows.Forms.LinkLabel();

            this.lblRealesrganName = new System.Windows.Forms.Label();
            this.lblRealesrganStatus = new System.Windows.Forms.Label();
            this.progRealesrgan = new System.Windows.Forms.ProgressBar();
            this.btnDownloadRealesrgan = new System.Windows.Forms.Button();
            this.linkRealEsrganSource = new System.Windows.Forms.LinkLabel();

            this.lblPyTorchName = new System.Windows.Forms.Label();
            this.lblPyTorchStatus = new System.Windows.Forms.Label();
            this.progPyTorch = new System.Windows.Forms.ProgressBar();
            this.btnDownloadPyTorch = new System.Windows.Forms.Button();
            this.linkPyTorchSource = new System.Windows.Forms.LinkLabel();

            this.txtLog = new System.Windows.Forms.TextBox();
            this.buttonPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnCancelDownload = new System.Windows.Forms.Button();

            this.table.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();

            // lblIntro
            this.lblIntro.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblIntro.Padding = new System.Windows.Forms.Padding(10, 10, 10, 4);
            this.lblIntro.Height = 60;
            this.lblIntro.Text = "이 도구는 아래의 무료 오픈소스 프로젝트를 사용합니다. 다운로드를 누르면 각 프로젝트의 공식 " +
                "GitHub/공식 배포처에서 받아 %LOCALAPPDATA%\\VideoTool\\tools 에 설치합니다. " +
                "PyTorch-CUDA는 선택 사항이며 NVIDIA GPU에서만 동작합니다(수 GB 다운로드).";

            // table
            this.table.Dock = System.Windows.Forms.DockStyle.Top;
            this.table.ColumnCount = 5;
            this.table.RowCount = 3;
            this.table.Padding = new System.Windows.Forms.Padding(10, 4, 10, 4);
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.table.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 190F));
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.table.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));

            this.lblFfmpegName.Text = "FFmpeg";
            this.lblFfmpegName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblFfmpegName.AutoSize = true;
            this.lblFfmpegStatus.Text = "확인 중...";
            this.lblFfmpegStatus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblFfmpegStatus.AutoSize = true;
            this.progFfmpeg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progFfmpeg.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.btnDownloadFfmpeg.Text = "다운로드";
            this.btnDownloadFfmpeg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDownloadFfmpeg.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnDownloadFfmpeg.Click += new System.EventHandler(this.btnDownloadFfmpeg_Click);
            this.linkFfmpegSource.Text = "BtbN/FFmpeg-Builds (GitHub)";
            this.linkFfmpegSource.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.linkFfmpegSource.AutoSize = true;
            this.linkFfmpegSource.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkFfmpegSource_LinkClicked);
            this.table.Controls.Add(this.lblFfmpegName, 0, 0);
            this.table.Controls.Add(this.lblFfmpegStatus, 1, 0);
            this.table.Controls.Add(this.progFfmpeg, 2, 0);
            this.table.Controls.Add(this.btnDownloadFfmpeg, 3, 0);
            this.table.Controls.Add(this.linkFfmpegSource, 4, 0);

            this.lblRealesrganName.Text = "Real-ESRGAN";
            this.lblRealesrganName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblRealesrganName.AutoSize = true;
            this.lblRealesrganStatus.Text = "확인 중...";
            this.lblRealesrganStatus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblRealesrganStatus.AutoSize = true;
            this.progRealesrgan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progRealesrgan.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.btnDownloadRealesrgan.Text = "다운로드";
            this.btnDownloadRealesrgan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDownloadRealesrgan.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnDownloadRealesrgan.Click += new System.EventHandler(this.btnDownloadRealesrgan_Click);
            this.linkRealEsrganSource.Text = "xinntao/Real-ESRGAN (GitHub)";
            this.linkRealEsrganSource.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.linkRealEsrganSource.AutoSize = true;
            this.linkRealEsrganSource.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkRealEsrganSource_LinkClicked);
            this.table.Controls.Add(this.lblRealesrganName, 0, 1);
            this.table.Controls.Add(this.lblRealesrganStatus, 1, 1);
            this.table.Controls.Add(this.progRealesrgan, 2, 1);
            this.table.Controls.Add(this.btnDownloadRealesrgan, 3, 1);
            this.table.Controls.Add(this.linkRealEsrganSource, 4, 1);

            this.lblPyTorchName.Text = "PyTorch-CUDA";
            this.lblPyTorchName.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblPyTorchName.AutoSize = true;
            this.lblPyTorchStatus.Text = "확인 중...";
            this.lblPyTorchStatus.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblPyTorchStatus.AutoSize = true;
            this.progPyTorch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progPyTorch.Margin = new System.Windows.Forms.Padding(3, 8, 3, 8);
            this.btnDownloadPyTorch.Text = "다운로드";
            this.btnDownloadPyTorch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnDownloadPyTorch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnDownloadPyTorch.Click += new System.EventHandler(this.btnDownloadPyTorch_Click);
            this.linkPyTorchSource.Text = "pytorch.org (공식)";
            this.linkPyTorchSource.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.linkPyTorchSource.AutoSize = true;
            this.linkPyTorchSource.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkPyTorchSource_LinkClicked);
            this.table.Controls.Add(this.lblPyTorchName, 0, 2);
            this.table.Controls.Add(this.lblPyTorchStatus, 1, 2);
            this.table.Controls.Add(this.progPyTorch, 2, 2);
            this.table.Controls.Add(this.btnDownloadPyTorch, 3, 2);
            this.table.Controls.Add(this.linkPyTorchSource, 4, 2);

            // txtLog
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Multiline = true;
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 9F);

            // buttonPanel
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.buttonPanel.Padding = new System.Windows.Forms.Padding(10);
            this.btnClose.Text = "닫기";
            this.btnClose.AutoSize = true;
            this.btnClose.Padding = new System.Windows.Forms.Padding(10, 4, 10, 4);
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            this.btnCancelDownload.Text = "다운로드 취소";
            this.btnCancelDownload.AutoSize = true;
            this.btnCancelDownload.Enabled = false;
            this.btnCancelDownload.Margin = new System.Windows.Forms.Padding(3, 3, 10, 3);
            this.btnCancelDownload.Padding = new System.Windows.Forms.Padding(10, 4, 10, 4);
            this.btnCancelDownload.Click += new System.EventHandler(this.btnCancelDownload_Click);
            this.buttonPanel.Controls.Add(this.btnClose);
            this.buttonPanel.Controls.Add(this.btnCancelDownload);

            // DependenciesForm
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ClientSize = new System.Drawing.Size(700, 460);
            this.MinimumSize = new System.Drawing.Size(620, 400);
            this.Text = "종속성 확인 / 다운로드";
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.table);
            this.Controls.Add(this.lblIntro);
            this.Controls.Add(this.buttonPanel);

            this.table.ResumeLayout(false);
            this.table.PerformLayout();
            this.buttonPanel.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Label lblIntro;
        private System.Windows.Forms.TableLayoutPanel table;

        private System.Windows.Forms.Label lblFfmpegName;
        private System.Windows.Forms.Label lblFfmpegStatus;
        private System.Windows.Forms.ProgressBar progFfmpeg;
        private System.Windows.Forms.Button btnDownloadFfmpeg;
        private System.Windows.Forms.LinkLabel linkFfmpegSource;

        private System.Windows.Forms.Label lblRealesrganName;
        private System.Windows.Forms.Label lblRealesrganStatus;
        private System.Windows.Forms.ProgressBar progRealesrgan;
        private System.Windows.Forms.Button btnDownloadRealesrgan;
        private System.Windows.Forms.LinkLabel linkRealEsrganSource;

        private System.Windows.Forms.Label lblPyTorchName;
        private System.Windows.Forms.Label lblPyTorchStatus;
        private System.Windows.Forms.ProgressBar progPyTorch;
        private System.Windows.Forms.Button btnDownloadPyTorch;
        private System.Windows.Forms.LinkLabel linkPyTorchSource;

        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.FlowLayoutPanel buttonPanel;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnCancelDownload;
    }
}
