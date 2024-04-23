using System;
using System.Collections.Generic;

namespace XwaManager;

public sealed class ModVersionData
{
    public static readonly Dictionary<string, string> VersionUrls = new()
    {
        {"XWAU", "https://forums.xwaupgrade.com/viewforum.php?f=34" },
        {"TFTC", "https://sites.google.com/view/tie-fighter-total-conversion" },
        {"EMBER", "https://forums.xwaupgrade.com/viewtopic.php?t=12968" },
    };

    private static string GetVersionUrl(string versionType)
    {
        if (!VersionUrls.TryGetValue(versionType, out string url))
        {
            return null;
        }

        return url;
    }

    public ModVersionData(string versionKey)
    {
        switch (versionKey)
        {
            case "XWAU_2020":
                MainType = "XWAU";
                MainVersionString = "0.0.1";
                MainFullName = "XWAU Mega Patch 2020";
                MainInfoUrl = GetVersionUrl("XWAU");
                MainInfoUrlHost = GetHostFromUrl(MainInfoUrl);
                ModType = "XWAU";
                ModVersionString = "0.0.1";
                ModFullName = "XWAU Mega Patch 2020";
                ModInfoUrl = GetVersionUrl("XWAU");
                ModInfoUrlHost = GetHostFromUrl(ModInfoUrl);
                IsFilled = true;
                break;
        }
    }

    public ModVersionData(string mainType, string mainVersion, string modType, string modVersion)
    {
        MainType = mainType;
        MainVersionString = Helpers.GetSubstringVersion(mainVersion);
        MainFullName = mainVersion;
        MainInfoUrl = GetVersionUrl(mainType);
        MainInfoUrlHost = GetHostFromUrl(MainInfoUrl);
        ModType = modType;
        ModVersionString = Helpers.GetSubstringVersion(modVersion);
        ModFullName = modVersion;
        ModInfoUrl = GetVersionUrl(modType);
        ModInfoUrlHost = GetHostFromUrl(ModInfoUrl);

        if (!MainVersionString.Contains("."))
        {
            MainVersionString += ".0";
        }

        if (!ModVersionString.Contains("."))
        {
            ModVersionString += ".0";
        }

        IsFilled = true;
    }

    public ModVersionData(string directoryUrl, string mainType, string modType)
    {
        ModVersionFileName = "mod-version-" + mainType + "-" + modType + ".txt";
        DirectoryUrl = directoryUrl;

        string[] lines = WebClientHelpers.DownloadVersionLines(DirectoryUrl, ModVersionFileName);

        if (lines.Length < 6)
        {
            return;
        }

        MainType = mainType;
        MainVersionString = lines[0];
        MainFullName = lines[1];
        MainInfoUrl = lines[2];
        MainInfoUrlHost = GetHostFromUrl(MainInfoUrl);

        ModType = modType;
        ModVersionString = lines[3];
        ModFullName = lines[4];
        ModInfoUrl = lines[5];
        ModInfoUrlHost = GetHostFromUrl(ModInfoUrl);

        if (!MainVersionString.Contains("."))
        {
            MainVersionString += ".0";
        }

        if (!ModVersionString.Contains("."))
        {
            ModVersionString += ".0";
        }

        IsFilled = true;
    }

    private static string GetHostFromUrl(string url)
    {
        if (Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
        {
            return uri.Host;
        }

        return null;
    }

    public bool IsFilled { get; }

    public string ModVersionFileName { get; }

    public string DirectoryUrl { get; }

    public string MainType { get; }

    public string MainVersionString { get; }

    public Version MainVersion
    {
        get
        {
            if (string.IsNullOrEmpty(MainVersionString) || !Version.TryParse(MainVersionString, out Version result))
            {
                return null;
            }

            return result;
        }
    }

    public string MainFullName { get; }

    public string MainInfoUrl { get; }

    public string MainInfoUrlHost { get; }

    public string ModType { get; }

    public string ModVersionString { get; }

    public Version ModVersion
    {
        get
        {
            if (string.IsNullOrEmpty(ModVersionString) || !Version.TryParse(ModVersionString, out Version result))
            {
                return null;
            }

            return result;
        }
    }

    public string ModFullName { get; }

    public string ModInfoUrl { get; }

    public string ModInfoUrlHost { get; }
}
