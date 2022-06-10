using System.Windows;

namespace RedMangaCleanerCGUI.WPFDesign.Code.Xaml.Windows
{
    /// <summary>
    /// Interaction logic for FirstStartWindow.xaml
    /// </summary>
    public partial class UserAgreementAndFirstInit : Window
    {
        public UserAgreementAndFirstInit()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            P.LanguageManager.Localize(this, MainGrid);
        }
    }
}
