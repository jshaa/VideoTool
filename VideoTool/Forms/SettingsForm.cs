using VideoTool.Core.Models;

namespace VideoTool
{
    public partial class SettingsForm : Form
    {
        public UpscaleSettings ResultSettings { get; private set; }

        public SettingsForm(UpscaleSettings initial)
        {
            InitializeComponent();
            ResultSettings = initial.Clone();

            cmbIntermediateFormat.Items.Clear();
            cmbIntermediateFormat.Items.Add("jpg");
            cmbIntermediateFormat.Items.Add("png");

            txtTempDir.Text = initial.TempRootDirectory;
            numChunkFrameCount.Value = Math.Clamp(initial.ChunkFrameCount, (int)numChunkFrameCount.Minimum, (int)numChunkFrameCount.Maximum);
            txtGpuId.Text = initial.GpuId ?? string.Empty;
            numTileSize.Value = Math.Clamp(initial.TileSize, (int)numTileSize.Minimum, (int)numTileSize.Maximum);
            cmbIntermediateFormat.SelectedItem = initial.IntermediateFormat;
            if (cmbIntermediateFormat.SelectedIndex < 0)
                cmbIntermediateFormat.SelectedIndex = 0;
            numAudioBitrate.Value = Math.Clamp(initial.AudioBitrateKbps, (int)numAudioBitrate.Minimum, (int)numAudioBitrate.Maximum);
            chkKeepTempFiles.Checked = initial.KeepTempFiles;
            chkFastPreDownscale.Checked = initial.FastPreDownscale;
        }

        private void btnBrowseTempDir_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtTempDir.Text))
                folderBrowserDialog1.SelectedPath = txtTempDir.Text;
            if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
                txtTempDir.Text = folderBrowserDialog1.SelectedPath;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTempDir.Text))
            {
                MessageBox.Show(this, "임시 작업 폴더를 입력해 주세요.", "확인 필요", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DialogResult = DialogResult.None;
                return;
            }

            ResultSettings.TempRootDirectory = txtTempDir.Text.Trim();
            ResultSettings.ChunkFrameCount = (int)numChunkFrameCount.Value;
            ResultSettings.GpuId = string.IsNullOrWhiteSpace(txtGpuId.Text) ? null : txtGpuId.Text.Trim();
            ResultSettings.TileSize = (int)numTileSize.Value;
            ResultSettings.IntermediateFormat = cmbIntermediateFormat.SelectedItem as string ?? "jpg";
            ResultSettings.AudioBitrateKbps = (int)numAudioBitrate.Value;
            ResultSettings.KeepTempFiles = chkKeepTempFiles.Checked;
            ResultSettings.FastPreDownscale = chkFastPreDownscale.Checked;

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
