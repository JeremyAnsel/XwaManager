using iNKORE.UI.WPF.Modern;
using System.Windows;
using System.Windows.Controls;

namespace XwaManager;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class SettingsWindow : Window
{
    public SettingsWindow(Window owner)
    {
        Owner = owner;
        InitializeComponent();

        int theme = ManagerSettings.Default.Theme;

        if (theme >= 0 && theme <= 2)
        {
            ((RadioButton)themePanel.Children[theme]).IsChecked = true;
        }

        baseDirectoryBox.Text = ManagerSettings.Default.BaseDirectory;
        checkUpdatesOnStartupButton.IsChecked = ManagerSettings.Default.CheckUpdatesOnStartup;

        Closed += SettingsWindow_Closed;
    }

    private void SettingsWindow_Closed(object sender, System.EventArgs e)
    {
        ManagerSettings.Default.ReadSettings();

        var viewModel = (ViewModel)Owner.DataContext;
        viewModel.SelectedTheme = ManagerSettings.Default.Theme;
        viewModel.BaseDirectory = ManagerSettings.Default.BaseDirectory;

        if (viewModel.SelectedTheme >= 0 && viewModel.SelectedTheme <= 2)
        {
            var button = (RadioButton)themePanel.Children[viewModel.SelectedTheme];
            ThemePanelButton_Checked(button, null);
        }
    }

    private void ThemePanelButton_Checked(object sender, RoutedEventArgs e)
    {
        var button = (RadioButton)sender;

        switch (button.Tag)
        {
            case "Default":
                ThemeManager.Current.ApplicationTheme = null;
                break;

            case "Light":
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                break;

            case "Dark":
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                break;
        }
    }

    private void BaseDirectoryBrowse_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new FolderBrowserForWPF.Dialog
        {
            Title = "Select a directory containing your X-Wing Alliance installations",
            FileName = baseDirectoryBox.Text
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        if (XwaExeVersion.IsXwaDirectory(dialog.FileName))
        {
            iNKORE.UI.WPF.Modern.Controls.MessageBox.Show(
                this,
                $"A directory containing X-Wing Alliance is selected. Please select the directory containing your X-Wing Alliance installations.",
                "Select directory",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        baseDirectoryBox.Text = dialog.FileName;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        int theme = 0;
        for (int themeIndex = 0; themeIndex <= 2; themeIndex++)
        {
            if (((RadioButton)themePanel.Children[themeIndex]).IsChecked == true)
            {
                theme = themeIndex;
                break;
            }
        }

        ManagerSettings.Default.Theme = theme;
        ManagerSettings.Default.BaseDirectory = baseDirectoryBox.Text;
        ManagerSettings.Default.CheckUpdatesOnStartup = checkUpdatesOnStartupButton.IsChecked == true;

        ManagerSettings.Default.SaveSettings();

        var viewModel = (ViewModel)Owner.DataContext;
        viewModel.SelectedTheme = ManagerSettings.Default.Theme;
        viewModel.BaseDirectory = ManagerSettings.Default.BaseDirectory;

        Close();
    }
}
