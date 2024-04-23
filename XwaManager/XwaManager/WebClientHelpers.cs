using System;
using System.IO;
using System.Net;
using System.Text;

namespace XwaManager;

internal static class WebClientHelpers
{
    private static readonly Encoding _encoding = Encoding.GetEncoding("iso-8859-1");

    public static string[] DownloadVersionLines(string directoryUrl, string fileName)
    {
        string contentUrl = new Uri(Path.Combine(new Uri(directoryUrl).AbsoluteUri, fileName)).AbsoluteUri;
        return DownloadVersionLines(contentUrl);
    }

    public static string[] DownloadVersionLines(string contentUrl)
    {
        using var webClient = new WebClient();
        webClient.Encoding = _encoding;

        string content = null;

        try
        {
            content = webClient.DownloadString(contentUrl);
        }
        catch
        {
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            return Array.Empty<string>();
        }

        string[] lines = content.GetLines(true);
        return lines;
    }
}
