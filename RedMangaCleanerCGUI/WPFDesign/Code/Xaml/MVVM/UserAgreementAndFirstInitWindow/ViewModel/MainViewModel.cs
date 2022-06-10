using RedMangaCleanerCGUI.WPFDesign.Code.Classes;

namespace RedMangaCleanerCGUI.WPFDesign.Code.Xaml.MVVM.UserAgreementAndFirstInitWindow.ViewModel
{
    class MainViewModel : ObservableObject
    {
        public static MainViewModel CurrentMainViewModel { get; set; }

        public RelayCommand LanguageViewCommand { get; set; }
        public RelayCommand BaseSettingsViewCommand { get; set; }
        public RelayCommand IntroductionViewCommand { get; set; }
        public RelayCommand UserAgreementViewCommand { get; set; }


        public LanguageViewModel LanguageVM { get; set; }
        public BaseSettingsViewModel BaseSettingsVM { get; set; }
        public IntroductionViewModel IntroductionVM { get; set; }
        public UserAgreementViewModel UserAgreementVM { get; set; }

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
            LanguageVM = new LanguageViewModel();
            BaseSettingsVM = new BaseSettingsViewModel();
            IntroductionVM = new IntroductionViewModel();
            UserAgreementVM = new UserAgreementViewModel();
            CurrentView = LanguageVM;

            CurrentMainViewModel = this;

            LanguageViewCommand = new RelayCommand(o =>
            {
                CurrentView = LanguageVM;
            });

            BaseSettingsViewCommand = new RelayCommand(o =>
            {
                CurrentView = BaseSettingsVM;
            });

            IntroductionViewCommand = new RelayCommand(o =>
            {
                CurrentView = IntroductionVM;
            });

            UserAgreementViewCommand = new RelayCommand(o =>
            {
                CurrentView = UserAgreementVM;
            });
        }
    }
}
