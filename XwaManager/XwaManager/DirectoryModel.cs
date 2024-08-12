using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System;
using Semver;

namespace XwaManager;

public sealed partial class DirectoryModel : ObservableObject
{
    private static readonly Encoding _encoding = Encoding.GetEncoding("iso-8859-1");

    private const string ImageFilename = "Alliance.jpg";

    private const string VanillaType = "Vanilla";

    private const string VanillaVersion = "Vanilla";

    private static readonly string[] VersionTypes = new[]
    {
        "XWAU",
        "TFTC",
        "EMBER",
    };

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DirectoryName))]
    [NotifyPropertyChangedFor(nameof(Image))]
    private string _directoryPath;

    public string DirectoryName => Path.GetFileName(DirectoryPath);

    public ImageSource Image
    {
        get
        {
            string imagePath = Path.Combine(DirectoryPath, ImageFilename);
            using var fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read);
            BitmapFrame image = BitmapFrame.Create(fs, BitmapCreateOptions.None, BitmapCacheOption.OnLoad);
            image.Freeze();
            return image;
        }
    }

    public bool IsVanilla => string.Equals(VersionType, VanillaType, System.StringComparison.OrdinalIgnoreCase);

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsVanilla))]
    private string _versionType;

    [ObservableProperty]
    private string _version;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsUpdateAvailable))]
    [NotifyPropertyChangedFor(nameof(DoesUpdateRequireReset))]
    private ModVersionData _modVersionData;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsUpdateAvailable))]
    [NotifyPropertyChangedFor(nameof(DoesUpdateRequireReset))]
    private ModVersionData _modUpdateVersionData;

    [ObservableProperty]
    private bool _isHooksUpdateAvailable;

    [ObservableProperty]
    private bool _isGoldenDDrawUpdateAvailable;

    [ObservableProperty]
    private bool _isEffectsDDrawUpdateAvailable;

    public bool IsUpdateAvailable
    {
        get
        {
            if (ModVersionData is null
                || !ModVersionData.IsFilled
                || ModUpdateVersionData is null
                || !ModUpdateVersionData.IsFilled)
            {
                return false;
            }

            SemVersion currentVersion = ModVersionData.ModVersion;
            SemVersion updateVersion = ModUpdateVersionData.ModVersion;

            if (currentVersion is null || updateVersion is null)
            {
                return false;
            }

            bool check = currentVersion.CompareSortOrderTo(updateVersion) < 0;
            return check;
        }
    }

    public bool DoesUpdateRequireReset
    {
        get
        {
            if (!IsUpdateAvailable)
            {
                return false;
            }

            SemVersion currentVersion = ModVersionData.ModVersion;
            SemVersion updateVersion = ModUpdateVersionData.ModVersion;

            bool check = currentVersion.Major != updateVersion.Major;
            return check;
        }
    }

    public DirectoryModel(string directoryPath)
    {
        _directoryPath = directoryPath;
        ReadVersion();
        ReadModVersion();

        if (IsVanilla)
        {
            ReadVersionDataFromCredits();
        }

        if (!IsVanilla && (ModVersionData is null || !ModVersionData.IsFilled))
        {
            ReadVersionDataFromVersionName();
        }
    }

    private void ReadVersion()
    {
        VersionType = VanillaType;
        Version = VanillaVersion;

        for (int index = VersionTypes.Length - 1; index >= 0; index--)
        {
            string type = VersionTypes[index];
            string version = ReadVersionName(type);

            if (version is null)
            {
                continue;
            }

            VersionType = type;
            Version = version;
            break;
        }
    }

    private string ReadVersionName(string type)
    {
        string name = type + "Version.txt";
        string path = Path.Combine(DirectoryPath, name);

        if (!File.Exists(path))
        {
            return null;
        }

        string[] lines = File.ReadLines(path, _encoding)
            .Where(t => !string.IsNullOrEmpty(t))
            .ToArray();

        string version = lines.LastOrDefault();
        return version;
    }

    private void ReadModVersion()
    {
        if (IsVanilla)
        {
            return;
        }

        ModVersionData = new ModVersionData(DirectoryPath, VersionTypes[0], VersionType);
    }

    private void ReadVersionDataFromCredits()
    {
        string name = "Credits.txt";
        string path = Path.Combine(DirectoryPath, name);

        if (!File.Exists(path))
        {
            return;
        }

        string content = File.ReadAllText(path, _encoding);

        if (content.IndexOf("Upgrade 2020", StringComparison.InvariantCultureIgnoreCase) != -1)
        {
            VersionType = "XWAU";
            Version = "XWAU Mega Patch 2020";

            ModVersionData = new ModVersionData("XWAU_2020");
        }
    }

    private void ReadVersionDataFromVersionName()
    {
        ModVersionData = new ModVersionData(
            VersionTypes[0],
            ReadVersionName(VersionTypes[0]),
            VersionType,
            ReadVersionName(VersionType));
    }

    public void UpdateVersionData()
    {
        IsHooksUpdateAvailable = UpdateCheckerHooks.CheckHooksVersion(DirectoryPath);
        IsGoldenDDrawUpdateAvailable = UpdateCheckerDDraw.CheckGoldenDDrawVersion(DirectoryPath);
        IsEffectsDDrawUpdateAvailable = UpdateCheckerDDraw.CheckEffectsDDrawVersion(DirectoryPath);

        if (IsVanilla || ModVersionData is null || !ModVersionData.IsFilled)
        {
            return;
        }

        ModUpdateVersionData = new ModVersionData(GlobalSettings.ModUpdateUrl, VersionTypes[0], VersionType);
    }
}
