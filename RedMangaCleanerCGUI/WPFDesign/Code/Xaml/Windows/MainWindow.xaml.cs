using System.Windows;

namespace RedMangaCleanerCGUI.WPFDesign.Code.Xaml.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            P.LanguageManager.Localize(this, MainGrid);
        }
    }
}
