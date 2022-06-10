using RedMangaCleanerCGUI.WPFDesign.Code.Xaml.MVVM.StartMenuView.ViewModel;
using RedsTools.WPF;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace RedMangaCleanerCGUI.WPFDesign.Code.Xaml.MVVM.MainWindow.View
{
    /// <summary>
    /// Interaction logic for StartMenuView.xaml
    /// </summary>
    public partial class StartMenuView : UserControl
    {
        public bool Colum1Expandet = true;
        public int Colum1ExpandShrinkAnimationDevider = 50;
        public int Colum1ExpandShrinkAnimationTime = 5;

        public List<TextBlock> TextBlocks;
        public List<Button> Buttons;

        public StartMenuView()
        {
            InitializeComponent();
            P.LanguageManager.Localize(this, MainGrid);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            PV.UserControl.StartManuView = this;
            HomePageBtn_Localize_Content.IsHitTestVisible = false;
        }

        #region Colum1
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Colum1Expandet)
            {
                int stackPanelWidth = Convert.ToInt32(ButtonStackPanel.ActualWidth);
                int windowWidth = Convert.ToInt32(StartMenuWindow.ActualWidth);
                int avabileSizeForColum1 = windowWidth - stackPanelWidth - 60;

                if (avabileSizeForColum1 > 0)
                {
                    Colum1.Width = new GridLength(avabileSizeForColum1);
                }
            }
        }


        private void ShrinkUnShrinkColum1()
        {
            if (!Colum1Expandet)
            {
                ExpandColum1();
            }
            else
            {
                ShrinkColum1();
            }
        }

        private void ShrinkColum1()
        {
            SetTextBlocksWraping(TextWrapping.NoWrap);

            int stackPanelWidth = Convert.ToInt32(ButtonStackPanel.ActualWidth);
            int windowWidth = Convert.ToInt32(StartMenuWindow.ActualWidth);
            int avabileSizeForColum1 = windowWidth - stackPanelWidth - 60;

            int animationDevider = avabileSizeForColum1 / Colum1ExpandShrinkAnimationDevider;

            int iIterator = 0;
            int sizeIterator = avabileSizeForColum1;
            while (iIterator < Colum1ExpandShrinkAnimationDevider)
            {
                iIterator++;
                sizeIterator -= animationDevider;

                PV.Window.MainWindow.Dispatcher.Invoke((Action)(() =>
                {
                    Colum1.Width = new GridLength(Convert.ToDouble(sizeIterator));
                }));

                Thread.Sleep(Colum1ExpandShrinkAnimationTime);
            }

            PV.Window.MainWindow.Dispatcher.Invoke((Action)(() =>
            {
                Colum1.Width = new GridLength(Convert.ToDouble(0));
            }));

            Colum1Expandet = false;
        }
        private void ExpandColum1()
        {
            int stackPanelWidth = Convert.ToInt32(ButtonStackPanel.ActualWidth);
            int windowWidth = Convert.ToInt32(StartMenuWindow.ActualWidth);
            int avabileSizeForColum1 = windowWidth - stackPanelWidth - 60;

            int animationDevider = avabileSizeForColum1 / Colum1ExpandShrinkAnimationDevider;

            int iIterator = 0;
            int sizeIterator = 0;
            while (iIterator < Colum1ExpandShrinkAnimationDevider)
            {
                iIterator++;
                sizeIterator += animationDevider;

                PV.Window.MainWindow.Dispatcher.Invoke((Action)(() =>
                {
                    Colum1.Width = new GridLength(Convert.ToDouble(sizeIterator));
                }));

                Thread.Sleep(Colum1ExpandShrinkAnimationTime);
            }

            PV.Window.MainWindow.Dispatcher.Invoke((Action)(() =>
            {
                Colum1.Width = new GridLength(Convert.ToDouble(avabileSizeForColum1));
            }));

            Colum1Expandet = true;

            SetTextBlocksWraping(TextWrapping.Wrap);
        }

        private void SetTextBlocksWraping(TextWrapping inputWraping)
        {
            if (/*TextBlocks == null*/ true)
            {
                List<Visual> visuals = WPFTypes.GetChildrens(PV.UserControl.HomePageView, true);
                TextBlocks = new List<TextBlock>();

                for (int i = 0; i < visuals.Count; i++)
                {
                    if (visuals[i] is TextBlock)
                    {
                        TextBlocks.Add((TextBlock)visuals[i]);
                    }
                }
            }

            PV.UserControl.HomePageView.Dispatcher.Invoke((Action)(() =>
            {
                for (int i = 0; i < TextBlocks.Count; i++)
                {
                    TextBlock buffer = TextBlocks[i];
                    buffer.TextWrapping = inputWraping;
                }
            }));
        }


        #endregion


        private void CreateCleaningProjectButton_Click(object sender, RoutedEventArgs e)
        {
            SetViewAndChangeButtonStateTo(MainViewModel.CurrentMainViewModel.CreateCleaningProjectVM, CreateCleaningProjectBtn_Localize_Content);
        }
        private void LoadCleaningProjectButton_Click(object sender, RoutedEventArgs e)
        {
            SetViewAndChangeButtonStateTo(MainViewModel.CurrentMainViewModel.LoadCleaningProjectVM, LoadCleaningProjectBtn_Localize_Content);
        }
        private void OpenListofExistingProjectsButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void OpenCleaningProjectButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void HomePageButton_Click(object sender, RoutedEventArgs e)
        {
            SetViewAndChangeButtonStateTo(MainViewModel.CurrentMainViewModel.HomePageVM, HomePageBtn_Localize_Content);
        }

        private async void SetViewAndChangeButtonStateTo(object inputView, Button inputAsButton)
        {
            if (/*Buttons == null*/true)
            {
                List<Visual> visuals = WPFTypes.GetChildrens(ButtonStackPanel, true);
                Buttons = new List<Button>();

                for (int i = 0; i < visuals.Count; i++)
                {
                    if (visuals[i] is Button)
                    {
                        Buttons.Add((Button)visuals[i]);
                    }
                }
            }

            for (int i = 0; i < Buttons.Count; i++)
            {
                Buttons[i].IsHitTestVisible = true;
            }

            inputAsButton.IsHitTestVisible = false;

            await Task.Run(() => ShrinkColum1());

            MainViewModel.CurrentMainViewModel.CurrentView = inputView;

            await Task.Run(() => ExpandColum1());
        }
        public async void SetViewToImageProcessingStatus()
        {
            await Task.Run(() => ShrinkColum1());

            MainViewModel.CurrentMainViewModel.CurrentView = MainViewModel.CurrentMainViewModel.ImageProcessingStatusVM;

            await Task.Run(() => ExpandColum1());
        }
    }
}
