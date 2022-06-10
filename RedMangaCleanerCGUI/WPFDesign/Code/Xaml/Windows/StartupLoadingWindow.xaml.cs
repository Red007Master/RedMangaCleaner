using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace RedMangaCleanerCGUI.Code.Xaml.Windows
{
    /// <summary>
    /// Interaction logic for StartupLoadingWindow.xaml
    /// </summary>
    public partial class StartupLoadingWindow : Window
    {
        public StartupLoadingWindow()
        {
            InitializeComponent();

            PV.Window.StartupLoadingWindow = this;

            BitmapImage splashScreenImageSource = new BitmapImage();
            splashScreenImageSource.BeginInit();
            splashScreenImageSource.UriSource = new Uri(@"\WPFDesign\Data\Images\RedMangaCleanerLogo.png", UriKind.Relative);
            splashScreenImageSource.EndInit();
            splashScreenImage.Source = splashScreenImageSource;

            LoadingStatusBox.Text = "Booting...";
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Run(() => Pseudo.PseudoMain());
        }

        public void SetLoadingStatusBox(string inputMessage)
        {
            LoadingStatusBox.Text = inputMessage;
        }
    }
}
