using Microsoft.WindowsAPICodePack.Dialogs;
using RedsCleaningProjects.Core;
using System.Windows;
using System.Windows.Controls;


namespace RedMangaCleanerCGUI.WPFDesign.Code.Xaml.MVVM.StartMenuView.View
{
    /// <summary>
    /// Interaction logic for CreateCleaningProjectView.xaml
    /// </summary>
    public partial class CreateCleaningProjectView : UserControl
    {
        public bool Grid0Row1IsExpandet = true;
        public bool Grid1Row1IsExpandet = false;
        public bool Grid2Row1IsExpandet = false;

        public CreateCleaningProjectView()
        {
            InitializeComponent();

            P.LanguageManager.Localize(this, MainGrid);
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Grid1Row1.Height = new GridLength(0);
            Grid1DropDownToggleButton.IsChecked = true;

            Grid2Row1.Height = new GridLength(0);
            Grid2DropDownToggleButton.IsChecked = true;

            CreateNewCleaningProjectBTN_Localize_Content.Visibility = Visibility.Collapsed;
        }

        #region DropDowm0

        private void DropDown0_Click(object sender, RoutedEventArgs e)
        {
            if (Grid0Row1IsExpandet)
            {
                Grid0Row1.Height = new GridLength(0);
                Grid0Row1IsExpandet = false;
            }
            else
            {
                Grid0Row1.Height = new GridLength();
                Grid0Row1IsExpandet = true;
            }
        }
        private void DropDown0SelectFolderButton_Click(object sender, RoutedEventArgs e)
        {
            using (CommonOpenFileDialog dialog = new CommonOpenFileDialog())
            {
                dialog.IsFolderPicker = true;
                dialog.Title = "Select folder with images that you wanna to process";
                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    InputImagePathPreviewTB_Localize_Tag.Text = dialog.FileName;
                }
            }
        }
        private void DropDown0ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            //InputArguments.InputPath = InputImagePathPreviewTextBox.Text;

            Grid0Row1.Height = new GridLength(0);
            Grid0Row1IsExpandet = false;
            Grid0ToggleButtonCheckbox.IsChecked = true;
            Grid0DropDownToggleButton.IsChecked = true;

            Grid1Row1.Height = new GridLength();
            Grid1DropDownToggleButton.IsChecked = false;
            Grid1Row1IsExpandet = true;

            CreateNewCleaningProjectBTN_Localize_Content.Visibility = Visibility.Visible;
        }

        #endregion

        #region DropDowm1

        private void DropDown1_Click(object sender, RoutedEventArgs e)
        {
            if (Grid1Row1IsExpandet)
            {
                Grid1Row1.Height = new GridLength(0);
                Grid1Row1IsExpandet = false;
            }
            else
            {
                Grid1Row1.Height = new GridLength();
                Grid1Row1IsExpandet = true;
            }
        }
        private void DropDown1ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            P.StartArguments.UserTag = UserTagTB_Localize_Tag.Text;

            Grid1Row1.Height = new GridLength(0);
            Grid1Row1IsExpandet = false;
            Grid1ToggleButtonCheckbox.IsChecked = true;
            Grid1DropDownToggleButton.IsChecked = true;

            Grid2Row1.Height = new GridLength();
            Grid2DropDownToggleButton.IsChecked = false;
            Grid2Row1IsExpandet = true;
        }

        #endregion

        #region DropDowm2
        private void DropDown2_Click(object sender, RoutedEventArgs e)
        {
            if (Grid2Row1IsExpandet)
            {
                Grid2Row1.Height = new GridLength(0);
                Grid2Row1IsExpandet = false;
            }
            else
            {
                Grid2Row1.Height = new GridLength();
                Grid2Row1IsExpandet = true;
            }
        }
        private void DropDown2ConfirmButton_Click(object sender, RoutedEventArgs e)
        {
            //InputArguments.OutputBlackAndWhiteImages = (bool)OutputBlackAndWhiteImagesCheckbox.IsChecked;
            //InputArguments.ConductObjectDetectionOnBlackAndWhiteVariants = (bool)ConductObjectDetectionOnBlackAndWhiteVariantsCheckbox.IsChecked;
            //InputArguments.ConductTextBoxFillingOnBlackAndWhiteVariants = (bool)ConductTextBoxFillingOnBlackAndWhiteVariantsCheckbox.IsChecked;

            Grid2Row1.Height = new GridLength(0);
            Grid2Row1IsExpandet = false;
            Grid2ToggleButtonCheckbox.IsChecked = true;
            Grid2DropDownToggleButton.IsChecked = true;

            MainScrollViewer.ScrollToEnd();
        }

        #endregion

        private void CreatenewProjectButton_Click(object sender, RoutedEventArgs e)
        {
            P.StartArguments.InputPath = InputImagePathPreviewTB_Localize_Tag.Text;
            P.StartArguments.UserTag = UserTagTB_Localize_Tag.Text;
            P.StartArguments.OutputBlackAndWhiteImages = (bool)OutputBlackAndWhiteImagesCheckbox.IsChecked;
            P.StartArguments.ConductObjectDetectionOnBlackAndWhiteVariants = (bool)ConductObjectDetectionOnBlackAndWhiteVariantsCheckbox.IsChecked;
            P.StartArguments.ConductTextBoxFillingOnBlackAndWhiteVariants = (bool)ConductTextBoxFillingOnBlackAndWhiteVariantsCheckbox.IsChecked;
            P.StartArguments.FolderOptions = FolderOptions.CreateNewFolderById;
            P.StartArguments.CleaningProjectId = P.CleaningProjectsGlobalInfo.GetAndIncrementId();

            P.CleaningProjectNames = new CleaningProjectNames(FolderOptions.CreateNewFolderById, P.StartArguments.CleaningProjectId);
            P.CleaningProjectDirs.SetFromPath(P.PathDirs.CleaningProjects, P.CleaningProjectNames);

            PV.UserControl.StartManuView.SetViewToImageProcessingStatus();
        }

        private void InfoBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            Button senderAsButton = (Button)sender;
            string text = InfoButtonTextByName(senderAsButton.Name);
            MessageBox.Show(text);
        }
        private string InfoButtonTextByName(string inputName)
        {
            //string result = "";
            //string name = inputName.Replace("Btn", "").Replace("BSBTN", "");
            //var property = typeof(ControlsText).GetProperty(name);

            //result = (string)property.GetValue(LanguageManager.Local.Control);

            //return result;

            return "";
        }
    }
}