using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Drawing;
using System.Windows.Forms;
using System.IO.Compression;


namespace Invokerator
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string downloadUrl = "https://github.com/PowerShell/PowerShell/releases/download/v7.3.6/PowerShell-7.3.6-win-x64.zip";
            string exeDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            string downloadPath = Path.Combine(exeDirectory, "pwsh.zip");
            string extractionDirectory = Path.Combine(exeDirectory, "pwsh");
            string pwshExePath = Path.Combine(extractionDirectory, "pwsh.exe");

            if (File.Exists(pwshExePath))
            {
                progressBar1.Value = 100;
                MessageBox.Show("PowerShell is already downloaded and ready to use", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadProgressChanged += (s, args) =>
                    {
                        progressBar1.Value = args.ProgressPercentage;
                    };

                    webClient.DownloadFileCompleted += (s, args) =>
                    {
                        if (args.Error == null)
                        {
                            try
                            {
                                Directory.CreateDirectory(extractionDirectory);

                                ZipFile.ExtractToDirectory(downloadPath, extractionDirectory);

                                MessageBox.Show("Powershell downloaded successfully!", "Success!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Extraction or execution failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Download failed: " + args.Error.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    };

                    try
                    {
                        webClient.DownloadFileAsync(new Uri(downloadUrl), downloadPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Download failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            string exeDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            string extractionDirectory = Path.Combine(exeDirectory, "pwsh");
            string pwshExePath = Path.Combine(extractionDirectory, "pwsh.exe");

            if (File.Exists(pwshExePath))
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Filter = "Executable Files|*.exe";
                    openFileDialog.Title = "Select an .exe file to launch";

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string selectedExePath = openFileDialog.FileName;
                        string powershellCommand = $"$env:__COMPAT_LAYER='RunAsInvoker'; & '{selectedExePath}'";
                        ProcessStartInfo startInfo = new ProcessStartInfo(pwshExePath, $"-Command {powershellCommand}");
                        Process.Start(startInfo);
                    }
                }
            }
            else
            {
                MessageBox.Show("PowerShell is not downloaded yet. Please download it first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}