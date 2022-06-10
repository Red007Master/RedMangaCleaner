using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RedMangaCleanerCGUI.WPFDesign.Code.Xaml.MVVM.StartMenuView.View
{
    /// <summary>
    /// Interaction logic for ImagesProcessingView.xaml
    /// </summary>
    public partial class ImagesProcessingView : UserControl
    {
        public ProjectProcessingStatus ProjectProcessingStatus { get; set; }
        public ImagesProcessingProgressInfo ImagesProcessingProgressInfo { get; set; }

        public ImagesProcessingView()
        {
            InitializeComponent();
        }

        private async void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            ProjectProcessingStatus = new ProjectProcessingStatus(P.PathDirs.ProjectProcessingStatus);
            ImagesProcessingProgressInfo = new ImagesProcessingProgressInfo();

            PV.UserControl.ImagesProcessingView = this;

            if (P.Floats.Logic.IsLoad)
            {
                Grid0.Visibility = Visibility.Collapsed;
                Grid1.Visibility = Visibility.Collapsed;
                Grid2.Visibility = Visibility.Collapsed;

                P.Floats.Logic.IsLoad = false;
            }
            else
            {
                PRORLauncher pRORLauncher = new PRORLauncher(P.PathDirs.PROR, P.PathDirs.StartArguments, P.StartArguments);
                pRORLauncher = new PRORLauncher(@"C:\Users\Red007Master\source\Red007MasterProjects\!Tools!\RedMangaCleaner\RedMangaCleanerPROR\bin\Debug\RedMangaCleanerPROR.exe", P.PathDirs.StartArguments, P.StartArguments); //DEV

                pRORLauncher.Start();

                Grid1Row1.Height = new GridLength(0);
                Grid2Row1.Height = new GridLength(0);
                Grid3Row1.Height = new GridLength(0);

                await Task.Run(() => PRORUI());
            }

            Task.Run(() => Work.MainVoid());

            await Task.Run(() => CGUIUI());

            //PV.Window.MainWindow.OpenImagePro();            
        }

        private void PRORUI()
        {
            while (!ProjectProcessingStatus.IsPRORFinished)
            {
                PRORProcessingProgressUIUpdate();
                Thread.Sleep(100);
            }

            Thread.Sleep(1000);
        }
        private void CGUIUI()
        {
            while (!ImagesProcessingProgressInfo.IsFinished)
            {
                ImagesProcessingProgressInfo.UpdateUIIn(this);
                Thread.Sleep(100);
            }
            ImagesProcessingProgressInfo.UpdateUIIn(this);
        }

        private void PRORProcessingProgressUIUpdate()
        {
            ProjectProcessingStatus.Load();
            ImagesProcessingProgressInfo.SetFrom(ProjectProcessingStatus);
            ImagesProcessingProgressInfo.UpdateUIIn(this);
        }
    }
}
