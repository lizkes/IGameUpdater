using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;

using IGameUpdater.Helper;

namespace IGameUpdater
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Title = $"IGame更新器 v{App.Version}";
            InitializeComponent();
        }

        private async void Window_ContentRendered(object sender, EventArgs e)
        {
            try
            {
                await MoveFilesAsync(App.updateDirPath, Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), new string[] { "IGameUpdater.exe" });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "IGame更新器错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }

            var args = "";
            for(var i = 0; i < App.exeArgs.Count; i++)
            {
                if (i != 0) args += " ";
                args += $"\"{App.exeArgs[i]}\"";
            }
            ProcessHelper.StartProcess(App.startExePath, args, false, false);
            Environment.Exit(0);
        }

        private async Task MoveFilesAsync(string srcDirPath, string destDirPath, string[] excludeFileNames)
        {
            void MoveFilesTask(IProgress<(string, int)> p)
            {
                Directory.CreateDirectory(destDirPath);
                var srcFilePaths = FileHelper.GetFilePaths(srcDirPath);
                var totalFileAmount = srcFilePaths.Count;
                for (var i = 0; i < totalFileAmount; i++)
                {
                    var srcFilePath = srcFilePaths[i];
                    var srcFileName = srcFilePath.Replace($@"{srcDirPath}\", "");
                    if (Array.Exists(excludeFileNames, fileName => fileName == srcFileName))
                    {
                        continue;
                    }
                    var destFilePath = Path.Combine(destDirPath, srcFileName);
                    var progressValue = (int)Math.Round((i + 1) / (double)totalFileAmount * 100, 0);
                    p.Report(($"正在更新文件: {srcFileName}", progressValue));
                    if (File.Exists(destFilePath))
                    {
                        FileHelper.Retry(() => File.Delete(destFilePath));
                    }
                    Directory.CreateDirectory(Path.GetDirectoryName(destFilePath));
                    FileHelper.Retry(() => File.Move(srcFilePath, destFilePath));
                }
                FileHelper.Retry(() => Directory.Delete(srcDirPath, true));
            }

            var progress = new Progress<(string, int)>(o =>
            {
                updateLabel.Text = o.Item1;
                updateProgressbar.Value = o.Item2;
            });
            await Task.Run(() => MoveFilesTask(progress)).ContinueWith((t) =>
            {
                if (t.IsFaulted) throw t.Exception;
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
    }
}
