using System.Diagnostics;
using System.IO;

namespace XwaManager;

internal static class GlobalSettings
{
    public static readonly string XwaManagerDirectory = GetXwaManagerDirectory();

    public static readonly string DefaultWorkingDirectory = GetDefaultWorkingDirectory();

    public static readonly string XwaManagerRepository = @"https://github.com/JeremyAnsel/XwaManager/raw/main/XwaManager/zip/";
    public static readonly string XwaManagerUpdateUrl = XwaManagerRepository + "XwaManager-version.txt";
    public static readonly string XwaManagerSetupUrl = XwaManagerRepository + "XwaManager.zip";

    public static readonly string ModUpdateUrl = @"https://www.xwaupgrade.com/version";

    private static string GetXwaManagerDirectory()
    {
        using var process = Process.GetCurrentProcess();
        using var module = process.MainModule;
        string path = module.FileName;
        string directory = Path.GetDirectoryName(path);
        return directory;
    }

    private static string GetDefaultWorkingDirectory()
    {
        string directory = GetXwaManagerDirectory();
        directory = Path.GetDirectoryName(directory);

        // todo
#if DEBUG
        directory = @"C:\xwa\_ManagerTest";
#endif

        return directory;
    }
}
