using RedMangaCleanerCGUI.Code.Xaml.Windows;
using RedMangaCleanerCGUI.WPFDesign.Code.Xaml.MVVM.MainWindow.View;
using RedMangaCleanerCGUI.WPFDesign.Code.Xaml.MVVM.StartMenuView.View;
using RedMangaCleanerCGUI.WPFDesign.Code.Xaml.MVVM.UserAgreementAndFirstInitWindow.View;
using RedMangaCleanerCGUI.WPFDesign.Code.Xaml.Windows;

public static class PV
{
    public static class Window
    {
        public static StartupLoadingWindow StartupLoadingWindow { get; set; }
        public static UserAgreementAndFirstInit UserAgreementAndFirstInit { get; set; }
        public static MainWindow MainWindow { get; set; }
    }

    public static class UserControl
    {
        public static UserAgreementView UserAgreementView { get; set; }
        public static BaseSettingsView BaseSettingsView { get; internal set; }
        public static IntroductionView IntroductionView { get; internal set; }
        public static LanguageView LanguageView { get; internal set; }
        public static StartMenuView StartManuView { get; internal set; }
        public static HomePageView HomePageView { get; internal set; }
        public static ImagesProcessingView ImagesProcessingView { get; internal set; }
        public static ImageEditorView ImageEditorView { get; internal set; }
    }
}