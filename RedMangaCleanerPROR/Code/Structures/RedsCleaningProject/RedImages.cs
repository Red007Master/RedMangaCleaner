using RedsCleaningProject.MasksAndEditableObjects;
using RedsTools.Images;
using RedsTools.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace RedsCleaningProject
{
    namespace RedImages
    {
        public class BasicImageData
        {
            public string ImageFilePath { get; set; }

            public int Width { get; set; }
            public int Height { get; set; }

            public bool ImageIsBlackAndWhite { get; set; }

            public List<DetectedObject> DetectedObjects { get; set; }

            public BasicImageData(string imagePath, int width, int height, bool isBlackAndWhite, List<DetectedObject> detectedObjects)
            {
                ImageFilePath = imagePath;
                Width = width;
                Height = height;
                ImageIsBlackAndWhite = isBlackAndWhite;
                DetectedObjects = detectedObjects;
            }

            public BasicImageData() { }
        }

        public class RedImageCore : BasicImageData, IComparable
        {
            public string ImageFileName { get; set; }

            public bool ImageProcessingModeAsBlackAndWhite { get; set; }

            //public List<FillPoint> FillPointsUserInput { get; set; } = new List<FillPoint>();
            //public List<FillPoint> FillPointsObjectDetection { get; set; } = new List<FillPoint>();
            //public List<FillPoint> FillPointsProgramOther { get; set; } = new List<FillPoint>();

            public List<EditableObject> BaseEditableObjects { get; set; } = new List<EditableObject>();

            public RedImageCore(BasicImageData basicImageData)
            {
                TypeConverter.ChildParentSetTo<BasicImageData, RedImageCore>(basicImageData, this);

                ImageFileName = Path.GetFileName(ImageFilePath);
                Bitmap image = new Bitmap(ImageFilePath);
                Width = image.Width;
                Height = image.Height;

                for (int i = 0; i < DetectedObjects.Count; i++)
                {
                    BaseEditableObjects.Add(new EditableObject(DetectedObjects[i]));
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

            public List<EditableObject> EditableObjects { get; set; } = new List<EditableObject>();

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
                for (int i = 0; i < EditableObjects.Count; i++)
                {
                    TextBox textBox = (TextBox)EditableObjects[i];

                    textBox.ApplyFiledTextBoxPixelsTo(DisplayDirectBitmap);
                    EditableObjects[i].ApplyOverlayPixelsTo(DisplayDirectBitmap);
                }
                RectDrawStatus = true;
            }   //kill

            public void UndrawRectangles()
            {
                for (int i = 0; i < EditableObjects.Count; i++)
                {
                    TextBox textBox = (TextBox)EditableObjects[i];

                    textBox.UnApplyFiledTextBoxPixelsTo(DisplayDirectBitmap);
                    EditableObjects[i].UnApplyOverlayPixelsTo(DisplayDirectBitmap);
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

                for (int i = 0; i < BaseEditableObjects.Count; i++)
                {
                    EditableObjects.Add(new TextBox(BaseEditableObjects[i].DetectedObject, RGBImageAsDirectBitmap, ImageAsColorArray));
                }

                DisplayDirectBitmap = new DirectBitmap(BaWImageAsDirectBitmap);
                //DisplayDirectBitmap = Work.Cleaning.CleanRGBImage(DetectedObjects, ImageAsColorArray);

                for (int i = 0; i < EditableObjects.Count; i++)
                {
                    TextBox textBox = (TextBox)EditableObjects[i];

                    textBox.CalculateTextBoxFillingMask();
                    textBox.ApplyFiledTextBoxPixelsTo(DisplayDirectBitmap);

                    EditableObjects[i].CalculateTextMask();
                    EditableObjects[i].CalculateRectangleMask();
                    EditableObjects[i].ApplyOverlayPixelsTo(DisplayDirectBitmap);
                }
            }
        }
    }
}
