using System.Runtime.CompilerServices;
using System;
using System.Windows;

namespace XwaManager;

internal static class AppMain
{
    [STAThread]
    static void Main()
    {
        string[] args = Environment.GetCommandLineArgs();

        if (string.Equals(args[^1], "autoupdate", StringComparison.OrdinalIgnoreCase))
        {
            Updater.SelfUpdate();
            return;
        }

        RunApp();
    }

    [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
    static void RunApp()
    {
        var app = new App();
        app.InitializeComponent();
        app.Run();
    }
}
