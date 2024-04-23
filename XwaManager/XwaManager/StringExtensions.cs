using System;

namespace XwaManager;

internal static class StringExtensions
{
    public static string[] GetLines(this string str, bool removeEmptyLines = false)
    {
        return str.Split(
            new[] { "\r\n", "\r", "\n" },
            removeEmptyLines ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
    }
}
