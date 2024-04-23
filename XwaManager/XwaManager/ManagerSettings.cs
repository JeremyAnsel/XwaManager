using System.IO;
using System.Text;

namespace XwaManager;

internal sealed class ManagerSettings
{
    private static readonly Encoding _encoding = Encoding.GetEncoding("iso-8859-1");

    public static readonly string SettingsFileName = Path.Combine(GlobalSettings.XwaManagerDirectory, "XwaManager.json");

    public static readonly ManagerSettings Default = new();

    public ManagerSettings()
    {
        SetDefaultValues();
    }

    public int Theme { get; set; }

    public string BaseDirectory { get; set; }

    public bool CheckUpdatesOnStartup { get; set; }

    public void SetDefaultValues()
    {
        Theme = 0;
        BaseDirectory = GlobalSettings.DefaultWorkingDirectory;
        CheckUpdatesOnStartup = false;
    }

    public bool ReadSettings()
    {
        if (!File.Exists(SettingsFileName))
        {
            return false;
        }

        string json = File.ReadAllText(SettingsFileName, _encoding);
        ManagerSettings settings = Newtonsoft.Json.JsonConvert.DeserializeObject<ManagerSettings>(json);
        Theme = settings.Theme;
        BaseDirectory = settings.BaseDirectory;
        CheckUpdatesOnStartup = settings.CheckUpdatesOnStartup;
        return true;
    }

    public void SaveSettings()
    {
        string json = Newtonsoft.Json.JsonConvert.SerializeObject(Default, Newtonsoft.Json.Formatting.Indented);
        File.WriteAllText(SettingsFileName, json, _encoding);
    }
}
