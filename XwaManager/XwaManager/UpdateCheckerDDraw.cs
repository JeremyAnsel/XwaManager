using System;
using System.Diagnostics;
using System.IO;

namespace XwaManager;

internal static class UpdateCheckerDDraw
{
    private const string XwaMainDDrawDataUrl = @"https://raw.githubusercontent.com/JeremyAnsel/xwa_ddraw_d3d11/master/ddraw_main/ddraw/ddraw.rc";
    private const string XwaGoldenDDrawDataUrl = @"https://raw.githubusercontent.com/JeremyAnsel/xwa_ddraw_d3d11/master/impl11/ddraw/ddraw.rc";
    private const string XwaEffectsDDrawDataUrl = @"https://raw.githubusercontent.com/Prof-Butts/xwa_ddraw_d3d11/master/impl11/ddraw/ddraw.rc";

    public static bool CheckGoldenDDrawVersion(string xwaDirectory)
    {
        Version githubMainDDrawVersion = GetGithubMainDDrawFileVersion();
        Version xwaMainDDrawVersion = GetXwaMainDDrawFileVersion(xwaDirectory);
        bool mainDDrawUpdate = githubMainDDrawVersion is not null && xwaMainDDrawVersion is not null && githubMainDDrawVersion > xwaMainDDrawVersion;

        Version githubGoldenDDrawVersion = GetGithubGoldenDDrawFileVersion();
        Version xwaGoldenDDrawVersion = GetXwaGoldenDDrawFileVersion(xwaDirectory);
        bool goldenDDrawUpdate = githubGoldenDDrawVersion is not null && xwaGoldenDDrawVersion is not null && githubGoldenDDrawVersion > xwaGoldenDDrawVersion;

        bool mainDDrawAvailable = xwaGoldenDDrawVersion is not null && xwaMainDDrawVersion is null;

        bool update = mainDDrawAvailable || mainDDrawUpdate || goldenDDrawUpdate;
        return update;
    }

    public static bool CheckEffectsDDrawVersion(string xwaDirectory)
    {
        Version githubEffectsDDrawVersion = GetGithubEffectsDDrawFileVersion();
        Version xwaEffectsDDrawVersion = GetXwaEffectsDDrawFileVersion(xwaDirectory);
        bool effectsDDrawUpdate = xwaEffectsDDrawVersion is not null && githubEffectsDDrawVersion > xwaEffectsDDrawVersion;

        bool update = effectsDDrawUpdate;
        return update;
    }

    public static Version GetGithubMainDDrawFileVersion()
    {
        return UpdateCheckerHelpers.GetGithubRcFileVersion(XwaMainDDrawDataUrl);
    }

    public static Version GetXwaMainDDrawFileVersion(string xwaDirectory)
    {
        Version xwaMainVersion = GetFileVersion(Path.Combine(xwaDirectory, "ddraw.dll"));

        if (xwaMainVersion is null)
        {
            return null;
        }

        if (xwaMainVersion.Major != 0)
        {
            return null;
        }

        return xwaMainVersion;
    }

    public static Version GetGithubGoldenDDrawFileVersion()
    {
        return UpdateCheckerHelpers.GetGithubRcFileVersion(XwaGoldenDDrawDataUrl);
    }

    public static Version GetXwaGoldenDDrawFileVersion(string xwaDirectory)
    {
        Version xwaMainVersion = GetFileVersion(Path.Combine(xwaDirectory, "ddraw.dll"));

        if (xwaMainVersion is null)
        {
            return null;
        }

        Version version;

        if (xwaMainVersion.Major == 0)
        {
            version = GetFileVersion(Path.Combine(xwaDirectory, "ddraw_golden.dll"));
        }
        else if (xwaMainVersion < new Version(1, 4))
        {
            version = xwaMainVersion;
        }
        else
        {
            version = null;
        }

        return version;
    }

    public static Version GetGithubEffectsDDrawFileVersion()
    {
        return UpdateCheckerHelpers.GetGithubRcFileVersion(XwaEffectsDDrawDataUrl);
    }

    public static Version GetXwaEffectsDDrawFileVersion(string xwaDirectory)
    {
        Version xwaMainVersion = GetFileVersion(Path.Combine(xwaDirectory, "ddraw.dll"));

        if (xwaMainVersion is null)
        {
            return null;
        }

        Version version;

        if (xwaMainVersion.Major == 0)
        {
            version = GetFileVersion(Path.Combine(xwaDirectory, "ddraw_effects.dll"));
        }
        else if (xwaMainVersion >= new Version(1, 4))
        {
            version = xwaMainVersion;
        }
        else
        {
            version = null;
        }

        return version;
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
