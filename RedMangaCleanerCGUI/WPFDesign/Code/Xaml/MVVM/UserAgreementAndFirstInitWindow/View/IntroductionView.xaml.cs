using RedMangaCleanerCGUI.WPFDesign.Code.Xaml.MVVM.UserAgreementAndFirstInitWindow.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace RedMangaCleanerCGUI.WPFDesign.Code.Xaml.MVVM.UserAgreementAndFirstInitWindow.View
{
    /// <summary>
    /// Interaction logic for IntroductionView.xaml
    /// </summary>
    public partial class IntroductionView : UserControl
    {
        public IntroductionView()
        {
            InitializeComponent();
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            PV.Window.UserAgreementAndFirstInit.UserAgreement_Localize_Content.IsChecked = true;
            MainViewModel.CurrentMainViewModel.UserAgreementViewCommand.Execute("");
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            PV.UserControl.IntroductionView = this;
            P.LanguageManager.Localize(this, MainGrid);
        }
    }
}
