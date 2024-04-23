using iNKORE.UI.WPF.Modern;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace XwaManager;

/// <summary>
/// Logique d'interaction pour ThemeSelectorControl.xaml
/// </summary>
[DependencyPropertyGenerator.DependencyProperty<int>("SelectedTheme")]
public partial class ThemeSelectorControl : UserControl
{
    public ThemeSelectorControl()
    {
        InitializeComponent();
    }

    private void ThemeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selector = (Selector)sender;

        switch (selector.SelectedIndex)
        {
            case 0:
                ThemeManager.Current.ApplicationTheme = null;
                break;

            case 1:
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Light;
                break;

            case 2:
                ThemeManager.Current.ApplicationTheme = ApplicationTheme.Dark;
                break;
        }
    }
}
