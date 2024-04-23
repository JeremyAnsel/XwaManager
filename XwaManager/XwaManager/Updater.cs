using System;
using System.Diagnostics;
using System.IO.Compression;
using System.IO;
using System.Net;

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
            return;
        }

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

            File.Delete(file);
        }

        ZipFile.ExtractToDirectory(zipFilePath, managerDirectory);

        using (var process = Process.GetCurrentProcess())
        {
            Process.Start(process.MainModule.FileName, process.StartInfo.Arguments);
        }

        Environment.Exit(0);
    }

    public static void Restart()
    {
        using (var process = Process.GetCurrentProcess())
        {
            Process.Start(process.MainModule.FileName, process.StartInfo.Arguments);
        }

        Environment.Exit(0);
    }
}
