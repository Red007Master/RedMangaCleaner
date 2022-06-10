using RedMangaCleanerCGUI.WPFDesign.Code.Classes;

namespace RedMangaCleanerCGUI.WPFDesign.Code.Xaml.MVVM.StartMenuView.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public static MainViewModel CurrentMainViewModel { get; set; }

        public RelayCommand HomePageViewCommand { get; set; }
        public RelayCommand CreateCleaningProjectViewCommand { get; set; }
        public RelayCommand LoadCleaningProjectViewCommand { get; set; }
        public RelayCommand ImageProcessingStatusViewCommand { get; set; }


        public HomePageViewModel HomePageVM { get; set; }
        public CreateCleaningProjectViewModel CreateCleaningProjectVM { get; set; }
        public LoadCleaningProjectViewModel LoadCleaningProjectVM { get; set; }
        public ImageProcessingStatusViewModel ImageProcessingStatusVM { get; set; }


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
            HomePageVM = new HomePageViewModel();
            CreateCleaningProjectVM = new CreateCleaningProjectViewModel();
            LoadCleaningProjectVM = new LoadCleaningProjectViewModel();
            ImageProcessingStatusVM = new ImageProcessingStatusViewModel();
            CurrentView = HomePageVM;

            CurrentMainViewModel = this;

            HomePageViewCommand = new RelayCommand(o =>
            {
                CurrentView = HomePageVM;
            });

            CreateCleaningProjectViewCommand = new RelayCommand(o =>
            {
                CurrentView = CreateCleaningProjectVM;
            });

            LoadCleaningProjectViewCommand = new RelayCommand(o =>
            {
                CurrentView = LoadCleaningProjectVM;
            });

            ImageProcessingStatusViewCommand = new RelayCommand(o =>
            {
                CurrentView = ImageProcessingStatusVM;
            });
        }
    }
}
