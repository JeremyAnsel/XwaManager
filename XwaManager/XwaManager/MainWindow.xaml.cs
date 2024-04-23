using System;
using System.IO;
using System.Windows;

namespace XwaManager;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        this.Loaded += Window_Loaded;
    }

    private ViewModel ViewModel
    {
        get { return (ViewModel)this.DataContext; }
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
        {
            return;
        }

        try
        {
            if (!File.Exists(ManagerSettings.SettingsFileName))
            {
                ViewModel.OpenSettingsWindowCommand.Execute(null);
            }

            ManagerSettings.Default.ReadSettings();

            ViewModel.SelectedTheme = ManagerSettings.Default.Theme;
            ViewModel.BaseDirectory = ManagerSettings.Default.BaseDirectory;
            ViewModel.RefreshDirectoriesCommand.Execute(null);

            if (ManagerSettings.Default.CheckUpdatesOnStartup)
            {
                ViewModel.UpdateAllModVersionDataCommand.Execute(null);
            }
        }
        catch (Exception ex)
        {
            iNKORE.UI.WPF.Modern.Controls.MessageBox.Show(
                Application.Current.MainWindow,
                ex.ToString(),
                "Press Ctrl+C to copy the text",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Environment.Exit(0);
        }
    }
}
