using System.Text;

namespace XwaManager;

internal static class Helpers
{
    public static string StrFormatByteSize(long filesize)
    {
        var sb = new StringBuilder(11);
        NativeMethods.StrFormatByteSize(filesize, sb, sb.Capacity);
        return sb.ToString();
    }

    public static string GetSubstringVersion(this string str)
    {
        if (str is null)
        {
            return string.Empty;
        }

        int index = str.LastIndexOf(' ');

        if (index == -1)
        {
            return string.Empty;
        }

        return str.Substring(index + 1);
    }
}
