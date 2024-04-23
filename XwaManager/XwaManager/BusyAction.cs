using System.Threading.Tasks;
using System;
using System.Windows;

namespace XwaManager;

public static class BusyAction
{
    public static void Run(Xceed.Wpf.Toolkit.BusyIndicator busyIndicator, Action action)
    {
        Run(busyIndicator, dispatcher => action());
    }

    public static void Run(Xceed.Wpf.Toolkit.BusyIndicator busyIndicator, Action<Action<Action>> action)
    {
        if (busyIndicator is null)
        {
            throw new ArgumentNullException(nameof(busyIndicator));
        }

        busyIndicator.BusyContent = string.Empty;
        busyIndicator.IsBusy = true;

        Action<Action> dispatcherAction = a =>
        {
            busyIndicator.Dispatcher.Invoke(a);
        };

        Task.Factory.StartNew(state =>
        {
            var disp = (Action<Action>)state;
            disp(() => { busyIndicator.IsBusy = true; });

            try
            {
                action(disp);
            }
            catch (Exception ex)
            {
                disp(() => Xceed.Wpf.Toolkit.MessageBox.Show(Application.Current.MainWindow, ex.ToString(), "Press Ctrl+C to copy the text", MessageBoxButton.OK, MessageBoxImage.Error));
            }

            disp(() => { busyIndicator.IsBusy = false; });
        }, dispatcherAction);
    }
}
