using System.Windows;
using System;
using System.IO;

namespace XwaManager;

internal static class FolderHelpers
{
    public static void CopyFolderDialog(string source, string destination, Xceed.Wpf.Toolkit.BusyIndicator busyIndicator, Action<string> getResult = null)
    {
        if (busyIndicator is null)
        {
            throw new ArgumentNullException(nameof(busyIndicator));
        }

        if (!Directory.Exists(source))
        {
            return;
        }

        if (Directory.Exists(destination))
        {
            return;
        }

        source = Path.GetFullPath(source);
        destination = Path.GetFullPath(destination);

        //getResult?.Invoke(string.Empty);
        //getResult?.Invoke(source);

        if (string.Equals(source, destination, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        BusyAction.Run(busyIndicator, dispatcher =>
        {
            Directory.CreateDirectory(destination);

            IOExtensions.TransferResult result = IOExtensions.FileTransferManager.CopyWithProgress(
                source,
                destination,
                progress => dispatcher.Invoke(() =>
                {
                    busyIndicator.BusyContent = $"Copy\nSource: {source}\nDestination: {destination}\nProgress: {Helpers.StrFormatByteSize(progress.BytesTransferred)} / {Helpers.StrFormatByteSize(progress.Total)} {progress.Percentage:F2}%";
                }),
                false,
                true);

            dispatcher.Invoke(() => iNKORE.UI.WPF.Modern.Controls.MessageBox.Show(
                $"Copy folder:\nSource is {source}\nDestination is {destination}\nResult is {result}",
                Application.Current.MainWindow.Title,
                MessageBoxButton.OK,
                result == IOExtensions.TransferResult.Failed ? MessageBoxImage.Error : MessageBoxImage.Information));

            if (result == IOExtensions.TransferResult.Success)
            {
                dispatcher.Invoke(() => getResult?.Invoke(destination));
            }
        });
    }
}
