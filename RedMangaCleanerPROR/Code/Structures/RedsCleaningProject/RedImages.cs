using RedMangaCleanerPROR.Code.Structures;
using RedsCleaningProject.MasksAndEditableObjects;
using RedsCleaningProject.MaskWorking;
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
            public string FileName { get; set; }
            public ImageType ImageType { get; set; }


            public int Width { get; set; }
            public int Height { get; set; }

            public List<DetectedObject> DetectedObjects { get; set; }

            public BasicImageData(string imagePath, int width, int height, ImageType imageType, List<DetectedObject> detectedObjects)
            {
                FileName = Path.GetFileName(imagePath);
                Width = width;
                Height = height;
                ImageType = imageType;
                DetectedObjects = detectedObjects;
            }

            public BasicImageData() { }
        }

        public class RedImageCore : BasicImageData, IComparable
        {
            public bool ImageProcessingModeAsBlackAndWhite { get; set; }

            //public List<FillPoint> FillPointsUserInput { get; set; } = new List<FillPoint>();
            //public List<FillPoint> FillPointsObjectDetection { get; set; } = new List<FillPoint>();
            //public List<FillPoint> FillPointsProgramOther { get; set; } = new List<FillPoint>();

            public List<EditableObject> BaseEditableObjects { get; set; } = new List<EditableObject>();

            public RedImageCore(BasicImageData basicImageData)
            {
                TypeConverter.ChildParentSetTo<BasicImageData, RedImageCore>(basicImageData, this);

                //string imagePath = "";

                //if (ImageType == ImageType.RGB)
                //{
                //    imagePath = Path.Combine(P.CleaningProject.CleaningProjectDirs.Images, FileName + NativeExtension); //TODO NativeExtension>WorkExtension (rework image copier)
                //}
                //else if (ImageType == ImageType.BaW)
                //{
                //    imagePath = Path.Combine(P.CleaningProject.CleaningProjectDirs.BlackAndWhiteImages, FileName + WorkExtension);
                //}

                //Bitmap image = new Bitmap(imagePath);
                //Width = image.Width;
                //Height = image.Height;

                for (int i = 0; i < DetectedObjects.Count; i++)
                {
                    BaseEditableObjects.Add(new EditableObject(DetectedObjects[i]));
                }

                //image.Dispose();
            }

            public RedImageCore()
            {

            }

            public virtual int CompareTo(object obj)
            {
                if (obj is RedImageCore)
                {
                    RedImageCore buffer = (RedImageCore)obj;
                    return String.Compare(FileName, buffer.FileName);
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

            public byte[,] ImageAsByteArray { get; set; }
            public DirectBitmap BaWImageAsDirectBitmap { get; set; }

            public Color[,] ImageAsColorArray { get; set; }
            public DirectBitmap RGBImageAsDirectBitmap { get; set; }

            public List<EditableObject> EditableObjects { get; set; } = new List<EditableObject>();

            public void CompileImageAsDirectBitmap()
            {
                BaWImageAsDirectBitmap = Images.DirectBitmapFromPath(P.CleaningProject.CleaningProjectDirs.BlackAndWhiteImages + @"\" + FileName);
                //RGBImageAsDirectBitmap = Images.DirectBitmapFromPath(P.CleaningProjectDirs.SourceImages + @"\" + ImageFileName);
            }
            public void CompileImageAsByteArray()
            {
                ImageAsByteArray = Images.BlackAndWhite.ByteArrayFromBitmap(BaWImageAsDirectBitmap.Bitmap);
            }
            public void CompileImageAsColorArray()
            {
                if (ImageType == ImageType.BaW)
                {
                    //TODO?
                    ImageAsColorArray = Images.RGB.BitmapToColorArray(BaWImageAsDirectBitmap.Bitmap);
                }
                else
                {
                    ImageAsColorArray = Images.RGB.BitmapToColorArray(BaWImageAsDirectBitmap.Bitmap);
                }
            }

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
                    return String.Compare(FileName, buffer.FileName);
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

                DisplayDirectBitmap = new DirectBitmap(BaWImageAsDirectBitmap);

                for (int i = 0; i < BaseEditableObjects.Count; i++)
                {
                    if (BaseEditableObjects[i].ObjectType == ObjectType.TextBox)
                    {
                        EditableObjects.Add(new TextBox(BaseEditableObjects[i].DetectedObject, RGBImageAsDirectBitmap, ImageAsColorArray));
                    }
                }

                for (int i = 0; i < EditableObjects.Count; i++)
                {
                    EditableObjects[i].RectangleMask = MaskWork.DrawRectangleOnMask(EditableObjects[i].DetectedObject, EditableObjects[i].RectangleConfig);
                    EditableObjects[i].TextMask = MaskWork.DrawTextOnMask(EditableObjects[i].DetectedObject, EditableObjects[i].TextConfig);

                    if (EditableObjects[i] is TextBox)
                    {
                        TextBox textBox = (TextBox)EditableObjects[i];

                        textBox.FillingMask = MaskWork.DrawFillingOnMask(textBox, ImageAsColorArray, textBox.FillingConfig);

                        MaskWork.FillByMask(DisplayDirectBitmap, textBox.FillingMask);
                    }

                    MaskWork.FillByMask(DisplayDirectBitmap, EditableObjects[i].RectangleMask);
                    MaskWork.FillByMask(DisplayDirectBitmap, EditableObjects[i].TextMask);
                }
            }
        }

        public enum ImageType
        {
            RGB = 0,
            BaW = 1
        }
    }
}
