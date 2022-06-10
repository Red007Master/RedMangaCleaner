using System.Windows;
using System.Windows.Controls;

namespace RedMangaCleanerCGUI.WPFDesign.Code.Xaml.MVVM.StartMenuView.View
{
    /// <summary>
    /// Interaction logic for DefaultWindowView.xaml
    /// </summary>
    public partial class HomePageView : UserControl
    {
        public HomePageView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            PV.UserControl.HomePageView = this;
        }
    }
}
