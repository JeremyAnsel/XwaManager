using System;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace XwaManager;

internal static class ArchiveHelpers
{
    public static void UpdateZipLastWriteTime(string path)
    {
        DateTimeOffset date;

        using (var archiveFile = File.OpenRead(path))
        using (var archive = new ZipArchive(archiveFile, ZipArchiveMode.Read))
        {
            date = archive.Entries.Max(t => t.LastWriteTime);
        }

        File.SetLastWriteTimeUtc(path, date.UtcDateTime);
    }
}
