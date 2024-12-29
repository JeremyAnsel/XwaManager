using System;
using System.Diagnostics;
using System.IO;

namespace XwaManager;

internal static class UpdateCheckerLauncher
{
    private const string XwaLauncherDataUrl = @"https://github.com/JeremyAnsel/XwaAlliance/releases/latest";

    public static bool CheckLauncherVersion(string xwaDirectory)
    {
        Version githubLauncherVersion = GetGithubLauncherFileVersion();
        Version xwaLauncherVersion = GetXwaLauncherFileVersion(xwaDirectory);
        bool update = githubLauncherVersion is not null && xwaLauncherVersion is not null && githubLauncherVersion > xwaLauncherVersion;
        return update;
    }

    public static Version GetGithubLauncherFileVersion()
    {
        return UpdateCheckerHelpers.GetGithubLatestReleaseVersion(XwaLauncherDataUrl);
    }

    public static Version GetXwaLauncherFileVersion(string xwaDirectory)
    {
        Version xwaLauncherVersion = GetFileVersion(Path.Combine(xwaDirectory, "Alliance.exe"));

        if (xwaLauncherVersion is null)
        {
            return null;
        }

        return xwaLauncherVersion;
    }

    private static Version GetFileVersion(string path)
    {
        if (!File.Exists(path))
        {
            return null;
        }

        FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(path);
        Version version = new(versionInfo.FileMajorPart, versionInfo.FileMinorPart, versionInfo.FileBuildPart, versionInfo.FilePrivatePart);
        return version;
    }
}
