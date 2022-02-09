using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;

namespace IGameUpdater
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        public static string updateDirPath;
        public static string startExePath;
        public static List<string> exeArgs = new List<string>();
        public static Version Version { get; set; } = new Version(FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion);

        void AppStartup(object s, StartupEventArgs e)
        {
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length >= 3)
            {
                updateDirPath = args[1];
                var exeName = args[2];
                startExePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), exeName);
                for (int i = 3; i < args.Length; i++)
                {
                    exeArgs.Add(args[i]);
                }
            }
            else
            {
                MessageBox.Show("传入参数不正确", "IGame更新器错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(1);
            }

            if (!Directory.Exists(updateDirPath))
            {
                MessageBox.Show($"指定的更新文件夹{updateDirPath}不存在", "IGame更新器错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(2);
            }
            if (!File.Exists(startExePath))
            {
                MessageBox.Show($"指定的启动程序{startExePath}不存在", "IGame更新器错误", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(3);
            }
        }
    }
}
