using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace IGameUpdater.Helper
{
    public class FileHelper
    {
        public static List<string> GetFilePaths(string dirPath)
        {
            var filePaths = new List<string>();
            if (Directory.Exists(dirPath))
            {
                foreach (var childFilePath in Directory.GetFiles(dirPath))
                {
                    filePaths.Add(childFilePath);
                }
                foreach (var childDirPath in Directory.GetDirectories(dirPath))
                {
                    var childFilePaths = GetFilePaths(childDirPath);
                    filePaths.AddRange(childFilePaths);
                }
            }
            return filePaths;
        }

        public static void Retry(Action action, int retryNum = 15, int delay = 200)
        {
            for (int i = 0; i < retryNum; i++)
            {
                try
                {
                    action();
                    return;
                }
                catch (Exception)
                {
                    Thread.Sleep(delay);
                    continue;
                }
            }
            action();
        }
    }
}
