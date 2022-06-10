using RedsTools.Images;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RedMangaCleanerCGUI.WPFDesign.Code.Xaml.MVVM.MainWindow.View
{
    /// <summary>
    /// Interaction logic for ImageEditorView.xaml
    /// </summary>
    public partial class ImageEditorView : UserControl
    {
        public int CurrentImageId { get; set; } = 0;

        public ImageEditorView()
        {
            PV.UserControl.ImageEditorView = this;

            InitializeComponent();
            SetImageSelectButtonsContent();
            ZBorderInit();
            HitTestsInit();
        }

        private void HitTestsInit()
        {
            if (!(Work.RedImageFulls != null && Work.RedImageFulls.Count > 0))
                ImgIdInputTextBox.IsHitTestVisible = false;
        }

        private void ZBorderInit()
        {
            if (Work.RedImageFulls != null && Work.RedImageFulls.Count > 0)
            {
                SetZBorderImageTo(0);
            }
        }

        private void SetImageSelectButtonsContent()
        {
            PrevImgButton.Content = "<";
            NextImgButton.Content = ">";
        }


        private void PrevImgButton_Click(object sender, RoutedEventArgs e)
        {
            if (Work.RedImageFulls != null)
            {
                if (CurrentImageId > 0)
                {
                    CurrentImageId--;
                    SetZBorderImageTo(CurrentImageId);
                }
            }
        }

        private void NextImgButton_Click(object sender, RoutedEventArgs e)
        {
            if (Work.RedImageFulls != null)
            {
                if (CurrentImageId < Work.RedImageFulls.Count - 1)
                {
                    CurrentImageId++;
                    SetZBorderImageTo(CurrentImageId);
                }
            }
        }

        private void ImgIdInputTextBox_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            TextBox senderAsTB = sender as TextBox;

            if (Work.RedImageFulls != null && Work.RedImageFulls.Count > 0)
            {
                Regex regex = new Regex("[^0-9]+");
                e.Handled = regex.IsMatch(e.Text);
            }
            else
            {
                e.Handled = true;
            }

            Task.Run(() => TextBoxValueCheckAndSet(senderAsTB));
        }
        private async void TextBoxValueCheckAndSet(TextBox textBox)
        {
            Thread.Sleep(1);

            PV.UserControl.ImageEditorView.Dispatcher.Invoke((Action)(() =>
            {
                if (textBox.Text != "" && Convert.ToInt32(textBox.Text) > Work.RedImageFulls.Count)
                {
                    textBox.Text = (Work.RedImageFulls.Count - 1).ToString();
                }

                SetZBorderImageTo(Convert.ToInt32(textBox.Text));
            }));
        }

        private void SetZBorderImageTo(int imageId)
        {
            Image img = (Image)zborder.Child;
            img.Source = Images.ImageSourceFromBitmap(Work.RedImageFulls[imageId].BaWImageAsDirectBitmap.Bitmap);

            ImgIdDisplayTextBox.Text = $"{imageId}/{Work.RedImageFulls.Count - 1}";
            ImgIdInputTextBox.Text = imageId.ToString();
        }
    }
}
