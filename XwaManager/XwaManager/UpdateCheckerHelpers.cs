using System.Globalization;
using System.Net;
using System;

namespace XwaManager;

internal static class UpdateCheckerHelpers
{
    public static DateTime GetGithubDate(string dataUrl)
    {
        string data = GetGithubString(dataUrl);
        DateTime date = GetGithubCommitsDate(data);
        return date;
    }

    public static Version GetGithubRcFileVersion(string dataUrl)
    {
        string data = GetGithubString(dataUrl);

        if (string.IsNullOrEmpty(data))
        {
            return null;
        }

        string key = "FILEVERSION ";
        int keyIndex = data.IndexOf(key);

        if (keyIndex == -1)
        {
            return null;
        }

        int valueStartIndex = keyIndex + key.Length;
        int valueIndex = data.IndexOfAny(new char[] { '\r', '\n' }, valueStartIndex);

        if (valueIndex == -1)
        {
            return null;
        }

        string versionString = data[valueStartIndex..valueIndex];

        if (string.IsNullOrEmpty(versionString))
        {
            return null;
        }

        versionString = versionString.Replace(',', '.');

        if (!Version.TryParse(versionString, out Version version))
        {
            return null;
        }

        return version;
    }

    public static string GetGithubString(string dataUrl)
    {
        string data;

        try
        {
            using var client = new WebClient();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            data = client.DownloadString(dataUrl);
        }
        catch
        {
            data = string.Empty;
        }

        return data;
    }

    public static DateTime GetGithubCommitsDate(string commitsData)
    {
        if (string.IsNullOrEmpty(commitsData))
        {
            return DateTime.MinValue;
        }

        string key = "\"committedDate\":\"";
        int keyIndex = commitsData.IndexOf(key);

        if (keyIndex == -1)
        {
            return DateTime.MinValue;
        }

        int valueStartIndex = keyIndex + key.Length;
        int valueIndex = commitsData.IndexOf('"', valueStartIndex);

        if (valueIndex == -1)
        {
            return DateTime.MinValue;
        }

        string dateString = commitsData[valueStartIndex..valueIndex];

        if (string.IsNullOrEmpty(dateString))
        {
            return DateTime.MinValue;
        }

        if (!DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime date))
        {
            return DateTime.MinValue;
        }

        return date;
    }

    public static bool CompareDate(DateTime left, DateTime right, double delayInMinutes = 5)
    {
        if (left == DateTime.MinValue)
        {
            return false;
        }

        if (right == DateTime.MaxValue)
        {
            return false;
        }

        bool result = left > right.AddMinutes(delayInMinutes);
        return result;
    }
}
