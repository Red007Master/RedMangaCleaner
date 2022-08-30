using RedMangaCleanerCGUI.WPFDesign.Code.Xaml.MVVM.UserAgreementAndFirstInitWindow.ViewModel;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace RedMangaCleanerCGUI.WPFDesign.Code.Xaml.MVVM.UserAgreementAndFirstInitWindow.View
{
    /// <summary>
    /// Interaction logic for BaseSettingsView.xaml
    /// </summary>
    public partial class BaseSettingsView : UserControl
    {
        public BaseSettingsView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            PV.UserControl.BaseSettingsView = this;
            P.LanguageManager.Localize(this, MainGrid);
            LoadSettingsToUI();
        }

        private void NextBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            PV.Window.UserAgreementAndFirstInit.Introduction_Localize_Content.IsChecked = true;
            MainViewModel.CurrentMainViewModel.IntroductionViewCommand.Execute("");
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

            //result = (string)property.GetValue(LanguageManager.Local.Control); TODO

            //return result;

            return "";
        }

        public void SaveSettingsFromUI()
        {
            P.Settings.SettingsList.UseYoloV5ObjectRecognition = (bool)PV.UserControl.BaseSettingsView.UseAlturosYoloObjectRecognitionCheckBox.IsChecked;

            //if (PV.UserControl.BaseSettingsView.YoloProcessingModeComboBox.SelectedIndex == 0)
            //{
            //    P.Settings.SettingsList.YoloProcessingMode = "CPU";
            //}
            //else if (PV.UserControl.BaseSettingsView.YoloProcessingModeComboBox.SelectedIndex == 1)
            //{
            //    P.Settings.SettingsList.YoloProcessingMode = "GPU";
            //}

            P.Settings.SettingsList.ProcessingBufferSize = Convert.ToInt32(PV.UserControl.BaseSettingsView.ProcessingBufferSizeTextBox.Text);
            P.Settings.SettingsList.PrecompileRedImageFullsThreadsCount = Convert.ToInt32(PV.UserControl.BaseSettingsView.PrecompileRedImageFullsThreadsCountTextBox.Text);
            P.Settings.SettingsList.ImagesToBlackAndWhiteThreadsCount = Convert.ToInt32(PV.UserControl.BaseSettingsView.ImagesToBlackAndWhiteThreadsCountTextBox.Text);

            P.Settings.WriteSettings();
        }
        public void LoadSettingsToUI()
        {
            PV.UserControl.BaseSettingsView.UseAlturosYoloObjectRecognitionCheckBox.IsChecked = P.Settings.SettingsList.UseYoloV5ObjectRecognition;

            //if (P.Settings.SettingsList.YoloProcessingMode.ToLower() == "cpu")
            //{
            //    PV.UserControl.BaseSettingsView.YoloProcessingModeComboBox.SelectedIndex = 0;
            //}
            //else if (P.Settings.SettingsList.YoloProcessingMode.ToLower() == "gpu")
            //{
            //    PV.UserControl.BaseSettingsView.YoloProcessingModeComboBox.SelectedIndex = 1;
            //}

            PV.UserControl.BaseSettingsView.ProcessingBufferSizeTextBox.Text = P.Settings.SettingsList.ProcessingBufferSize.ToString();
            PV.UserControl.BaseSettingsView.PrecompileRedImageFullsThreadsCountTextBox.Text = P.Settings.SettingsList.PrecompileRedImageFullsThreadsCount.ToString();
            PV.UserControl.BaseSettingsView.ImagesToBlackAndWhiteThreadsCountTextBox.Text = P.Settings.SettingsList.ImagesToBlackAndWhiteThreadsCount.ToString();
        }
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            SaveSettingsFromUI();
        }
        private void LoadBtn_Click(object sender, RoutedEventArgs e)
        {
            LoadSettingsToUI();
        }


        private static readonly Regex _regex = new Regex("[^0-9.-]+");
        private static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }
        private void PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsTextAllowed(e.Text);
        }
    }
}
