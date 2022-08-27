using RedsCleaningProject.EditableObjects;
using RedsCleaningProject.MaskWorking;
using RedsTools.Images;
using RedsTools.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedsCleaningProject
{
    namespace RedImages
    {
        public class BasicImageData
        {
            public string ImageFilePath { get; set; }

            public int Width { get; set; }
            public int Height { get; set; }

            public bool IsBlackAndWhite { get; set; }

            public List<DetectedObject> DetectedObjects { get; set; }

            public BasicImageData(string imagePath, int width, int height, bool isBlackAndWhite, List<DetectedObject> detectedObjects)
            {
                ImageFilePath = imagePath;
                Width = width;
                Height = height;
                IsBlackAndWhite = isBlackAndWhite;
                DetectedObjects = detectedObjects;
            }

            public BasicImageData() { }
        }

        public class RedImageCore : BasicImageData, IComparable
        {
            public bool ImageIsBlackAndWhite { get; set; }
            public bool ImageProcessingModeAsBlackAndWhite { get; set; }

            public string ImageFileName { get; set; }

            //public List<FillPoint> FillPointsUserInput { get; set; } = new List<FillPoint>();
            //public List<FillPoint> FillPointsObjectDetection { get; set; } = new List<FillPoint>();
            //public List<FillPoint> FillPointsProgramOther { get; set; } = new List<FillPoint>();

            public List<EditableObject> EditableObjects { get; set; } = new List<EditableObject>();

            public RedImageCore(BasicImageData basicImageData)
            {
                TypeConverter.ChildParentSetTo<BasicImageData, RedImageCore>(basicImageData, this);

                Bitmap image = new Bitmap(ImageFilePath);
                ImageFileName = Path.GetFileName(ImageFilePath);
                Width = image.Width;
                Height = image.Height;

                for (int i = 0; i < DetectedObjects.Count; i++)
                {
                    EditableObjects.Add(new EditableObject(DetectedObjects[i]));
                }

                image.Dispose();
            }
            public RedImageCore()
            {

            }

            public virtual int CompareTo(object obj)
            {
                if (obj is RedImageCore)
                {
                    RedImageCore buffer = (RedImageCore)obj;
                    return String.Compare(ImageFileName, buffer.ImageFileName);
                }
                else
                {
                    P.Logger.Log("Error in CompareTo/RedImageCore", LogLevel.FatalError);
                    return 0;
                }
            }
        }
        public class RedImageFull : RedImageCore, IDisposable
        {
            public DirectBitmap DisplayDirectBitmap { get; set; }
            public bool RectDrawStatus { get; set; }

            public DirectBitmap BaWImageAsDirectBitmap { get; set; }
            public DirectBitmap RGBImageAsDirectBitmap { get; set; }
            public Color[,] ImageAsColorArray { get; set; }
            public byte[,] ImageAsByteArray { get; set; }

            public List<TextBox> TextBoxes { get; set; } = new List<TextBox>();



            public void CompileImageAsDirectBitmap()
            {
                BaWImageAsDirectBitmap = Images.DirectBitmapFromPath(P.CleaningProjectDirs.BlackAndWhiteImages + @"\" + ImageFileName);
                //RGBImageAsDirectBitmap = Images.DirectBitmapFromPath(P.CleaningProjectDirs.SourceImages + @"\" + ImageFileName);
            }
            public void CompileImageAsByteArray()
            {
                ImageAsByteArray = Images.BlackAndWhite.ByteArrayFromBitmap(BaWImageAsDirectBitmap.Bitmap);
            }
            public void CompileImageAsColorArray()
            {
                if (ImageIsBlackAndWhite)
                {
                    //TODO?
                    ImageAsColorArray = Images.RGB.BitmapToColorArray(BaWImageAsDirectBitmap.Bitmap);
                }
                else
                {
                    ImageAsColorArray = Images.RGB.BitmapToColorArray(BaWImageAsDirectBitmap.Bitmap);
                }
            }

            public void DrawUndrawRectangles()
            {
                if (RectDrawStatus)
                {
                    UndrawRectangles();
                }
                else
                {
                    DrawRectangles();
                }
            } //kill

            public void DrawRectangles()
            {
                for (int i = 0; i < TextBoxes.Count; i++)
                {
                    TextBoxes[i].ApplyFiledTextBoxPixelsTo(DisplayDirectBitmap);
                    TextBoxes[i].ApplyOverlayPixelsTo(DisplayDirectBitmap);
                }
                RectDrawStatus = true;
            }   //kill

            public void UndrawRectangles()
            {
                for (int i = 0; i < TextBoxes.Count; i++)
                {
                    TextBoxes[i].UnApplyFiledTextBoxPixelsTo(DisplayDirectBitmap);
                    TextBoxes[i].UnApplyOverlayPixelsTo(DisplayDirectBitmap);
                }

                RectDrawStatus = false;
            }   //kill

            public void Dispose()
            {
                BaWImageAsDirectBitmap.Dispose();
                RGBImageAsDirectBitmap.Dispose();
                DisplayDirectBitmap.Dispose();
            }

            public override int CompareTo(object obj)
            {
                if (obj is RedImageFull)
                {
                    RedImageFull buffer = (RedImageFull)obj;
                    return String.Compare(ImageFileName, buffer.ImageFileName);
                }
                else
                {
                    P.Logger.Log("Error in CompareTo/RedImageFull", LogLevel.FatalError);
                    return 0;
                }
            }

            public RedImageFull(RedImageCore redImageCore)
            {
                TypeConverter.ChildParentSetTo<RedImageCore, RedImageFull>(redImageCore, this);

                CompileImageAsDirectBitmap();
                CompileImageAsByteArray();
                CompileImageAsColorArray();

                for (int i = 0; i < EditableObjects.Count; i++)
                {
                    TextBoxes.Add(new TextBox(EditableObjects[i].DetectedObject, RGBImageAsDirectBitmap, ImageAsColorArray));
                }

                DisplayDirectBitmap = new DirectBitmap(BaWImageAsDirectBitmap);
                //DisplayDirectBitmap = Work.Cleaning.CleanRGBImage(DetectedObjects, ImageAsColorArray);

                for (int i = 0; i < TextBoxes.Count; i++)
                {

                    TextBoxes[i].CalculateTextBoxFillingMask();
                    TextBoxes[i].ApplyFiledTextBoxPixelsTo(DisplayDirectBitmap);

                    TextBoxes[i].CalculateTextMask();
                    TextBoxes[i].CalculateRectangleMask();
                    TextBoxes[i].ApplyOverlayPixelsTo(DisplayDirectBitmap);
                }

                //DrawRectangles();
            }
        }
    }
}
