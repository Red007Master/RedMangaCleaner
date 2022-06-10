using RedMangaCleanerCGUI.WPFDesign.Code.Xaml.MVVM.UserAgreementAndFirstInitWindow.ViewModel;
using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace RedMangaCleanerCGUI.WPFDesign.Code.Xaml.MVVM.UserAgreementAndFirstInitWindow.View
{
    /// <summary>
    /// Interaction logic for LanguageView.xaml
    /// </summary>
    public partial class LanguageView : UserControl
    {
        public LanguageView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            PV.UserControl.LanguageView = this;
            P.LanguageManager.Localize(this, MainGrid);
            AddLanguageButtonsFromDictionary();
        }

        private void AddLanguageButtonsFromDictionary()
        {
            try
            {
                foreach (var language in P.LanguageManager.Languages)
                {
                    RadioButton radioButton = new RadioButton();
                    if (P.Settings.SettingsList.Language.ToLower() == language.FullName)
                    {
                        radioButton.IsChecked = true;
                    }

                    StringBuilder buttonName = new StringBuilder(language.FullName);
                    buttonName[0] = Char.ToUpper(buttonName[0]);
                    radioButton.Content = "   " + buttonName.ToString() + "   ";
                    radioButton.Style = this.FindResource("RedsRadioButtonLanguageStyle") as Style;
                    radioButton.Checked += LanguageButtonPressed;

                    this.WrapPanelLanguages.Children.Add(radioButton);
                }
            }
            catch (System.Exception)
            { } //VS rand exeption kastil/fix lol))) (don't ideal) TODO?
        }

        private void LanguageButtonPressed(object sender, RoutedEventArgs e)
        {
            RadioButton senderRadioButton = (RadioButton)sender;

            string languageName = senderRadioButton.Content.ToString();
            languageName = languageName.Replace(" ", "");

            P.LanguageManager.SetLanguageTo(languageName);

            P.Settings.SettingsList.Language = languageName;
            P.Settings.WriteSettings();

            P.LanguageManager.Localize(PV.Window.UserAgreementAndFirstInit, MainGrid);
        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            PV.Window.UserAgreementAndFirstInit.BaseSettings_Localize_Content.IsChecked = true;
            MainViewModel.CurrentMainViewModel.BaseSettingsViewCommand.Execute("");
        }
    }
}
