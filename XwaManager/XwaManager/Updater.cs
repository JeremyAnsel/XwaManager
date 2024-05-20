using System;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Net;
using System.Windows;
using System.Threading;

namespace XwaManager;

internal static class Updater
{
    public static void SelfUpdate()
    {
        string managerDirectory = GlobalSettings.XwaManagerDirectory;
        string directoryName = Path.GetFileName(managerDirectory);

        if (directoryName.IndexOf("_wip_", StringComparison.OrdinalIgnoreCase) != -1)
        {
            return;
        }

        string bakFilePath = Path.Combine(managerDirectory, "XwaManager.bak");
        string zipFilePath = Path.Combine(managerDirectory, "XwaManager.zip");
        string exeFilePath = Path.Combine(managerDirectory, "XwaManager.exe");

        if (File.Exists(bakFilePath))
        {
            File.Delete(bakFilePath);
            return;
        }

        File.Delete(zipFilePath);

        try
        {
            using var client = new WebClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            client.DownloadFile(GlobalSettings.XwaManagerSetupUrl, zipFilePath);
            ArchiveHelpers.UpdateZipLastWriteTime(zipFilePath);
        }
        catch
        {
            Restart();
            return;
        }

        try
        {
            File.Delete(bakFilePath);
            File.Move(exeFilePath, bakFilePath);

            var keepExtensions = new string[]
            {
                ".zip",
                ".bak",
                ".json",
            };

            foreach (string file in Directory.EnumerateFiles(managerDirectory))
            {
                string fileName = Path.GetFileName(file);

                bool found = false;

                foreach (string ext in keepExtensions)
                {
                    if (string.Equals(fileName, "XwaManager" + ext, StringComparison.OrdinalIgnoreCase))
                    {
                        found = true;
                        break;
                    }
                }

                if (found)
                {
                    continue;
                }

                int retryCount = 10;

                for (int i = 0; i <= retryCount; i++)
                {
                    try
                    {
                        File.Delete(file);
                        break;
                    }
                    catch
                    {
                        if (i == retryCount)
                        {
                            throw;
                        }

                        Thread.Sleep(1000);
                    }
                }
            }

            ZipFile.ExtractToDirectory(zipFilePath, managerDirectory);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.ToString());

            if (File.Exists(bakFilePath))
            {
                File.Delete(bakFilePath);
            }
        }

        Restart();
    }

    public static void RestartAutoUpdate()
    {
        using (var process = Process.GetCurrentProcess())
        {
            string arguments = process.StartInfo.Arguments + " autoupdate";
            Process.Start(process.MainModule.FileName, arguments);
        }

        Environment.Exit(0);
    }

    public static void Restart()
    {
        MessageBox.Show("Restart()");

        using (var process = Process.GetCurrentProcess())
        {
            string arguments = process.StartInfo.Arguments;

            if (arguments.EndsWith(" autoupdate", StringComparison.OrdinalIgnoreCase))
            {
                arguments = arguments[..^" autoupdate".Length];
            }

            Process.Start(process.MainModule.FileName, arguments);
        }

        Environment.Exit(0);
    }
}
