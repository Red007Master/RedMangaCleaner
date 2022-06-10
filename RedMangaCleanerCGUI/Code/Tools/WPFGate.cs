using RedMangaCleanerCGUI.WPFDesign.Code.Xaml.Windows;
using System;

class WPFGate
{
    public static void SetLoadingTextBox(string inputText)
    {
        PV.Window.StartupLoadingWindow.Dispatcher.Invoke((Action)(() =>
        {
            PV.Window.StartupLoadingWindow.LoadingStatusBox.Text = inputText;
        }));
    }

    public static void AsUserAgreementDontAccepted()
    {
        PV.Window.StartupLoadingWindow.Dispatcher.Invoke((Action)(() =>
        {
            PV.Window.UserAgreementAndFirstInit = new UserAgreementAndFirstInit();
            PV.Window.UserAgreementAndFirstInit.Show();
            PV.Window.StartupLoadingWindow.Close();
        }));
    }

    public static void OpenMainWindow()
    {
        PV.Window.StartupLoadingWindow.Dispatcher.Invoke((Action)(() =>
        {
            PV.Window.MainWindow = new MainWindow();
            PV.Window.MainWindow.Show();

            if (PV.Window.UserAgreementAndFirstInit != null)
                PV.Window.UserAgreementAndFirstInit.Close();

            if (PV.Window.StartupLoadingWindow != null)
                PV.Window.StartupLoadingWindow.Close();
        }));
    }
}
