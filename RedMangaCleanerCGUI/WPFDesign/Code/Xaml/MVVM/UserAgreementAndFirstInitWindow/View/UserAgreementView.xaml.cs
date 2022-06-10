using System.Windows;
using System.Windows.Controls;

namespace RedMangaCleanerCGUI.WPFDesign.Code.Xaml.MVVM.UserAgreementAndFirstInitWindow.View
{
    /// <summary>
    /// Interaction logic for UserAgreementView.xaml
    /// </summary>
    public partial class UserAgreementView : UserControl
    {
        public UserAgreementView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            PV.UserControl.UserAgreementView = this;
            P.LanguageManager.Localize(this, MainGrid);

        }

        private void UserAgreementBtn_Click(object sender, RoutedEventArgs e)
        {
            P.Config.UserAgreementIsAccepted = true;
            //RedTools.ConfigM.Save();
            PV.Window.UserAgreementAndFirstInit.UserAgreement_Localize_Content.Visibility = Visibility.Hidden;
        }
    }
}
