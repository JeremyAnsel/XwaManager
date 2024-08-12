using System;
using System.IO;
using System.Net;

namespace XwaManager;

internal static class UpdateCheckerHooks
{
    private const string XwaHooksSetupDataUrl = @"https://github.com/JeremyAnsel/XwaHooksSetup/commits/master/";
    private const string XwaHooksSetupPath = @"XwaHooksSetup\XwaHooksSetup.exe";
    private const string XwaHooksMainCommitsDataUrl = @"https://github.com/JeremyAnsel/xwa_hooks/commits/master/";

    public static bool CheckHooksVersion(string xwaDirectory)
    {
        DateTime githubHooksSetupLastUpdateDate = GetGithubHooksSetupLastUpdateDate();
        DateTime xwaHooksSetupLastUpdateDate = GetXwaHooksSetupLastUpdateDate(xwaDirectory);
        bool hooksSetupUpdate = UpdateCheckerHelpers.CompareDate(githubHooksSetupLastUpdateDate, xwaHooksSetupLastUpdateDate);

        DateTime githubHooksMainLastUpdateDate = GetGithubHooksMainLastUpdateDate();
        DateTime xwaHooksMainLastUpdateDate = GetXwaHooksMainLastUpdateDate(xwaDirectory);
        bool hooksMainUpdate = UpdateCheckerHelpers.CompareDate(githubHooksMainLastUpdateDate, xwaHooksMainLastUpdateDate);

        bool update = hooksSetupUpdate || hooksMainUpdate;
        return update;
    }

    public static DateTime GetGithubHooksSetupLastUpdateDate()
    {
        return UpdateCheckerHelpers.GetGithubDate(XwaHooksSetupDataUrl);
    }

    public static DateTime GetXwaHooksSetupLastUpdateDate(string xwaDirectory)
    {
        if (!Directory.Exists(xwaDirectory))
        {
            return DateTime.MaxValue;
        }

        string path = Path.Combine(xwaDirectory, XwaHooksSetupPath);

        if (!File.Exists(path))
        {
            return DateTime.MaxValue;
        }

        DateTime date = File.GetLastWriteTime(path);
        return date;
    }

    public static DateTime GetGithubHooksMainLastUpdateDate()
    {
        return UpdateCheckerHelpers.GetGithubDate(XwaHooksMainCommitsDataUrl);
    }

    public static DateTime GetXwaHooksMainLastUpdateDate(string xwaDirectory)
    {
        if (!Directory.Exists(xwaDirectory))
        {
            return DateTime.MaxValue;
        }

        DateTime date = DateTime.MinValue;

        foreach (string hook in Directory.EnumerateFiles(xwaDirectory, "hook_*.dll"))
        {
            FileInfo hookFileInfo = new(hook);
            DateTime lastHookDate = hookFileInfo.LastWriteTime;

            if (lastHookDate > date)
            {
                date = lastHookDate;
            }
        }

        if (date == DateTime.MinValue)
        {
            return DateTime.MaxValue;
        }

        return date;
    }
}
