using RedMangaCleanerCGUI.WPFDesign.Code.Classes;

namespace RedMangaCleanerCGUI.WPFDesign.Code.Xaml.MVVM.MainWindow.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public static MainViewModel CurrentMainViewModel { get; set; }

        public RelayCommand StartMenuViewCommand { get; set; }
        public RelayCommand ImageEditorViewCommand { get; set; }

        public StartMenuViewModel StartMenuVM { get; set; }
        public ImageEditorViewModel ImageEditorVM { get; set; }

        private object currentView;

        public object CurrentView
        {
            get { return currentView; }
            set
            {
                currentView = value;
                OnPropertyChange();
            }
        }

        public MainViewModel()
        {
            StartMenuVM = new StartMenuViewModel();
            ImageEditorVM = new ImageEditorViewModel();
            CurrentView = StartMenuVM;

            CurrentMainViewModel = this;

            StartMenuViewCommand = new RelayCommand(o =>
            {
                CurrentView = StartMenuVM;
            });

            ImageEditorViewCommand = new RelayCommand(o =>
            {
                CurrentView = ImageEditorVM;
            });
        }
    }
}
