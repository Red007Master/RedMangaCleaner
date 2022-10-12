using Newtonsoft.Json;
using RedsCleaningProject.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RedMangaCleanerCGUI.WPFDesign.Code.Xaml.MVVM.StartMenuView.View
{
    /// <summary>
    /// Interaction logic for LoadCleaningProjectView.xaml
    /// </summary>
    public partial class LoadCleaningProjectView : UserControl
    {
        private List<CleaningProject> CleaningProjects { get; set; } = new List<CleaningProject>();

        public LoadCleaningProjectView()
        {
            InitializeComponent();

            string[] projectsDirs = System.IO.Directory.GetDirectories(P.PathDirs.CleaningProjects);
            CleaningProjectNames names = new CleaningProjectNames("Empty");

            for (int i = 0; i < projectsDirs.Length; i++)
            {
                string cleaningProjectInfoPath = projectsDirs[i] + "\\" + names.CleaningProjectInfo;
                string serialized = System.IO.File.ReadAllText(cleaningProjectInfoPath);
                CleaningProject cleaningProject = JsonConvert.DeserializeObject<CleaningProject>(serialized);
                CleaningProjects.Add(cleaningProject);
            }

            CleaningProjects.Sort();

            for (int i = 0; i < CleaningProjects.Count; i++)
            {
                Button radioButton = new Button();
                radioButton.Content = $"ID:[{CleaningProjects[i].CleaningProjectInfo.Id}], UserTag:[{CleaningProjects[i].CleaningProjectInfo.UserTag}]";
                radioButton.Click += LoadButtonPressed;
                radioButton.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#19000000"));
                radioButton.Margin = new Thickness(5);
                radioButton.FontSize = 15;
                radioButton.HorizontalContentAlignment = HorizontalAlignment.Left;

                this.ButtonsStackPanel.Children.Add(radioButton);
            }
        }

        private void LoadButtonPressed(object sender, System.Windows.RoutedEventArgs e)
        {
            Button button = (Button)sender;

            string content = button.Content.ToString();

            int id = Convert.ToInt32(RemoveAllNonDigits(content.Split(',')[0]));

            P.Floats.Logic.IsLoad = true;

            for (int i = 0; i < CleaningProjects.Count; i++)
            {
                if (CleaningProjects[i].CleaningProjectInfo.Id == id)
                {
                    P.CleaningProject = CleaningProjects[i];
                    break;
                }
            }

            PV.UserControl.StartManuView.SetViewToImageProcessingStatus();
        }

        public static string RemoveAllNonDigits(string str)
        {
            return new string(str.Where(c => char.IsDigit(c)).ToArray());
        }
    }
}
