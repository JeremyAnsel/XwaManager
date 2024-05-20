using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Semver;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace XwaManager;

public sealed partial class ViewModel : ObservableObject
{
    public ViewModel()
    {
        SemVersion.TryParse(
            Assembly
            .GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion,
            SemVersionStyles.Any,
            out SemVersion version);

        AssemblyVersion = version ?? new SemVersion(0);
    }

    public SemVersion AssemblyVersion { get; private set; }

    public SemVersion AssemblyVersionUpdate { get; private set; }

    [ObservableProperty]
    private int selectedTheme;

    [ObservableProperty]
    private string baseDirectory = GlobalSettings.DefaultWorkingDirectory;

    partial void OnBaseDirectoryChanged(string oldValue, string newValue)
    {
        LoadInstallDirectories();
    }

    public ObservableCollection<DirectoryModel> DirectoryModels { get; } = new();

    private void LoadInstallDirectories()
    {
        DirectoryModels.Clear();

        if (!Directory.Exists(BaseDirectory))
        {
            return;
        }

        foreach (string directory in Directory.EnumerateDirectories(BaseDirectory, "*", SearchOption.TopDirectoryOnly))
        {
            if (!XwaExeVersion.IsXwaDirectory(directory))
            {
                continue;
            }

            var directoryModel = new DirectoryModel(directory);
            DirectoryModels.Add(directoryModel);
        }
    }

    [RelayCommand]
    private void RefreshDirectories()
    {
        LoadInstallDirectories();
    }

    [RelayCommand]
    private void BrowseDirectories()
    {
        if (!Directory.Exists(BaseDirectory))
        {
            return;
        }

        Process.Start(BaseDirectory);
    }

    [RelayCommand]
    private void DeleteDirectory(DirectoryModel directory)
    {
        if (directory is null)
        {
            return;
        }

        MessageBoxResult confirmResult = iNKORE.UI.WPF.Modern.Controls.MessageBox.Show(
            Application.Current.MainWindow,
            $"Do you want to delete the \"{directory.DirectoryName}\" directory?",
            "Delete directory",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question,
            MessageBoxResult.No);

        if (confirmResult != MessageBoxResult.Yes)
        {
            return;
        }

        var busyIndicator = (Application.Current.MainWindow as MainWindow)?.busyIndicator;

        BusyAction.Run(busyIndicator, dispatcher =>
        {
            if (Directory.Exists(directory.DirectoryPath))
            {
                Directory.Delete(directory.DirectoryPath, true);
            }

            dispatcher.Invoke(() => iNKORE.UI.WPF.Modern.Controls.MessageBox.Show(
            Application.Current.MainWindow,
                $"The directory \"{directory.DirectoryName}\" is deleted.",
                "Delete directory",
                MessageBoxButton.OK,
                MessageBoxImage.Information));

            dispatcher.Invoke(() => LoadInstallDirectories());
        });
    }

    [RelayCommand]
    private async Task RenameDirectory(DirectoryModel directory)
    {
        if (directory is null)
        {
            return;
        }

        MessageBoxResult confirmResult = iNKORE.UI.WPF.Modern.Controls.MessageBox.Show(
            Application.Current.MainWindow,
            $"Do you want to rename the \"{directory.DirectoryName}\" directory?",
            "Rename directory",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirmResult != MessageBoxResult.Yes)
        {
            return;
        }

        string newName = await iNKORE.UI.WPF.Modern.Controls.InputBox.ShowAsync(
            Application.Current.MainWindow,
            "Rename directory",
            "Enter the new name of the directory.",
            directory.DirectoryName);

        newName = newName?.Trim();

        if (string.IsNullOrEmpty(newName)
            || string.Equals(newName, directory.DirectoryName, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (newName.Any(Path.GetInvalidFileNameChars().Contains))
        {
            iNKORE.UI.WPF.Modern.Controls.MessageBox.Show(
                Application.Current.MainWindow,
                $"The name contains invalid characters.",
                "Rename directory",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            return;
        }

        string newPath = Path.Combine(BaseDirectory, newName);

        if (Directory.Exists(newPath))
        {
            iNKORE.UI.WPF.Modern.Controls.MessageBox.Show(
                Application.Current.MainWindow,
                $"The directory \"{newName}\" already exists.",
                "Rename directory",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            return;
        }

        var busyIndicator = (Application.Current.MainWindow as MainWindow)?.busyIndicator;

        BusyAction.Run(busyIndicator, dispatcher =>
        {
            Directory.Move(directory.DirectoryPath, newPath);

            dispatcher.Invoke(() => iNKORE.UI.WPF.Modern.Controls.MessageBox.Show(
                Application.Current.MainWindow,
                $"The directory \"{directory.DirectoryName}\" is renamed to {newName}.",
                "Rename directory",
                MessageBoxButton.OK,
                MessageBoxImage.Information));

            dispatcher.Invoke(() => LoadInstallDirectories());
        });
    }

    [RelayCommand]
    private async Task CopyDirectory(DirectoryModel directory)
    {
        if (directory is null)
        {
            return;
        }

        MessageBoxResult confirmResult = iNKORE.UI.WPF.Modern.Controls.MessageBox.Show(
            Application.Current.MainWindow,
            $"Do you want to make a copy of the \"{directory.DirectoryName}\" directory?",
            "Copy directory",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirmResult != MessageBoxResult.Yes)
        {
            return;
        }

        string newName = await iNKORE.UI.WPF.Modern.Controls.InputBox.ShowAsync(
            Application.Current.MainWindow,
            "Copy directory",
            "Enter the name of the new directory.",
            "");

        newName = newName?.Trim();

        if (string.IsNullOrEmpty(newName))
        {
            return;
        }

        if (newName.Any(Path.GetInvalidFileNameChars().Contains))
        {
            iNKORE.UI.WPF.Modern.Controls.MessageBox.Show(
                Application.Current.MainWindow,
                $"The name contains invalid characters.",
                "Copy directory",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            return;
        }

        string newPath = Path.Combine(BaseDirectory, newName);

        if (Directory.Exists(newPath))
        {
            iNKORE.UI.WPF.Modern.Controls.MessageBox.Show(
                Application.Current.MainWindow,
                $"The directory \"{newName}\" already exists.",
                "Copy directory",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            return;
        }

        FolderHelpers.CopyFolderDialog(
            directory.DirectoryPath,
            newPath,
            (Application.Current.MainWindow as MainWindow)?.busyIndicator,
            t => LoadInstallDirectories());
    }

    [RelayCommand]
    private void LaunchDirectoryLauncher(DirectoryModel directory)
    {
        if (directory is null)
        {
            return;
        }

        string launcherPath = Path.Combine(directory.DirectoryPath, "Alliance.exe");

        if (!File.Exists(launcherPath))
        {
            iNKORE.UI.WPF.Modern.Controls.MessageBox.Show(
                Application.Current.MainWindow,
                $"The directory \"{directory.DirectoryName}\" doesn't have a launcher.",
                "Launch directory",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        var process = new Process();
        process.StartInfo.FileName = launcherPath;
        process.StartInfo.WorkingDirectory = directory.DirectoryPath;
        process.StartInfo.UseShellExecute = false;
        process.Start();

        Thread.Sleep(1000);
    }

    [RelayCommand]
    private void OpenDirectory(DirectoryModel directory)
    {
        if (directory is null)
        {
            return;
        }

        Process.Start(directory.DirectoryPath);
    }

    [RelayCommand]
    private void NavigateTo(string url)
    {
        bool result = Uri.TryCreate(url, UriKind.Absolute, out Uri link);
        if (!result)
        {
            return;
        }

        Process.Start(link.AbsoluteUri);
    }

    [RelayCommand]
    private void UpdateAllModVersionData()
    {
        bool isManagerUpdateAvailable = CheckingXwaManager();

        if (isManagerUpdateAvailable)
        {
            var busyIndicator = (Application.Current.MainWindow as MainWindow)?.busyIndicator;

            BusyAction.Run(busyIndicator, dispatcher =>
            {
                dispatcher(() => busyIndicator.BusyContent = "Self Update");
                Updater.SelfUpdate();
            });

            return;
        }

        CheckingDirectoryModels();
    }

    private bool CheckingXwaManager()
    {
        string[] lines = WebClientHelpers.DownloadVersionLines(GlobalSettings.XwaManagerUpdateUrl);

        if (lines.Length < 1)
        {
            return false;
        }

        SemVersion currentVersion = AssemblyVersion;
        SemVersion.TryParse(lines[0], SemVersionStyles.Any, out SemVersion updateVersion);
        AssemblyVersionUpdate = updateVersion;

        if (currentVersion is null || updateVersion is null)
        {
            return false;
        }

        bool check = currentVersion.CompareSortOrderTo(updateVersion) < 0;

        if (check)
        {
            MessageBoxResult confirmResult = iNKORE.UI.WPF.Modern.Controls.MessageBox.Show(
                Application.Current.MainWindow,
                $"An update for X-Wing Alliance Manager is available.\n" +
                $"Current version is v{AssemblyVersion}.\n" +
                $"Updated version is v{AssemblyVersionUpdate}.\n" +
                $"Do you want to install it now?",
                "X-Wing Alliance Manager Update",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirmResult == MessageBoxResult.Yes)
            {
                return true;
            }
        }

        return false;
    }

    private void CheckingDirectoryModels()
    {
        var busyIndicator = (Application.Current.MainWindow as MainWindow)?.busyIndicator;

        BusyAction.Run(busyIndicator, dispatcher =>
        {
            foreach (var model in DirectoryModels)
            {
                dispatcher(() => busyIndicator.BusyContent = "Checking\n" + model.DirectoryName + "\n" + model.Version);

                model.UpdateVersionData();
            }
        });
    }

    [RelayCommand]
    private void OpenSettingsWindow()
    {
        var settingsWindow = new SettingsWindow(Application.Current.MainWindow);
        settingsWindow.ShowDialog();
    }
}
