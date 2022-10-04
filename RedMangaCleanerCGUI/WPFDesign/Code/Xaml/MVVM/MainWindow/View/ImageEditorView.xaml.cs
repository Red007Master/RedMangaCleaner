using RedsTools.Images;
using System;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Input;

namespace RedMangaCleanerCGUI.WPFDesign.Code.Xaml.MVVM.MainWindow.View
{
    /// <summary>
    /// Interaction logic for ImageEditorView.xaml
    /// </summary>
    public partial class ImageEditorView : UserControl
    {
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);

        public int CurrentImageId { get; set; } = 0;

        public ImageEditorView()
        {
            PV.UserControl.ImageEditorView = this;

            InitializeComponent();
            ZBorderInit();
        }

        private void ZBorderInit()
        {
            if (Work.RedImageFulls != null && Work.RedImageFulls.Count > 0)
            {
                SetZBorderImageTo(0);
            }
        }

        private void SetZBorderImageTo(int imageId)
        {
            Image img = (Image)zborder.Child;
            img.Source = Images.ImageSourceFromBitmap(Work.RedImageFulls[imageId].DisplayDirectBitmap.Bitmap);
        }

        public System.Drawing.Point GetImageCoordsAt(MouseButtonEventArgs e)
        {
            if (zborder != null && zborder.IsMouseOver)
            {
                var controlSpacePosition = e.GetPosition(zborder.Child);
                var imageControl = zborder.Child as Image;
                if (imageControl != null && imageControl.Source != null)
                {
                    // Convert from control space to image space
                    int x = Convert.ToInt32(Math.Floor(controlSpacePosition.X * imageControl.Source.Width / imageControl.ActualWidth));
                    int y = Convert.ToInt32(Math.Floor(controlSpacePosition.Y * imageControl.Source.Height / imageControl.ActualHeight));

                    return new System.Drawing.Point(x, y);
                }
            }
            return new System.Drawing.Point(-1, -1);
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Drawing.Point point = GetImageCoordsAt(e);
            Console.WriteLine(point);

            //for (int i = 0; i < Work.RedImageFulls[CurrentImageId].EditableObjects.Count; i++)
            //{
            //    if (Work.RedImageFulls[CurrentImageId].EditableObjects[i].DetectedObject.Rectangle.Contains(point))
            //    {
            //        RedsCleaningProject.MasksAndEditableObjects.TextBox textBox = (RedsCleaningProject.MasksAndEditableObjects.TextBox)Work.RedImageFulls[CurrentImageId].EditableObjects[i];

            //        textBox.ApplyOverlayPixelsTo(Work.RedImageFulls[CurrentImageId].DisplayDirectBitmap);
            //        textBox.ApplyFiledTextBoxPixelsTo(Work.RedImageFulls[CurrentImageId].DisplayDirectBitmap);
            //        SetZBorderImageTo(CurrentImageId);
            //        break;
            //    }
            //}
        }
    }
}
