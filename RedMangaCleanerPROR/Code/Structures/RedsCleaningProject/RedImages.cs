using RedMangaCleanerPROR.Code.Structures;
using RedsCleaningProject.CleaningConfigs;
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

            public RedImageCore(BasicImageData basicImageData, RedImageCleaningConfig redImageCleaningConfig)
            {
                TypeConverter.ChildParentSetTo<BasicImageData, RedImageCore>(basicImageData, this);

                for (int i = 0; i < DetectedObjects.Count; i++)
                {
                    BaseEditableObjects.Add(new EditableObject(DetectedObjects[i], redImageCleaningConfig.EditableObjectCleaningConfigs[i]));
                }
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

            public void PrecalculateImageAs(bool DirectBitmap, bool ByteArray, bool ColorArray)
            {
                if (DirectBitmap)
                {
                    BaWImageAsDirectBitmap = Images.DirectBitmapFromPath(P.CleaningProject.CleaningProjectDirs.BlackAndWhiteImages + @"\" + FileName);
                    //RGBImageAsDirectBitmap = Images.DirectBitmapFromPath(P.CleaningProjectDirs.SourceImages + @"\" + ImageFileName);
                }

                if (ByteArray)
                {
                    ImageAsByteArray = Images.BlackAndWhite.ByteArrayFromBitmap(BaWImageAsDirectBitmap.Bitmap);
                }

                if (ColorArray)
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

                PrecalculateImageAs(true, true, true);

                DisplayDirectBitmap = new DirectBitmap(BaWImageAsDirectBitmap);

                for (int i = 0; i < BaseEditableObjects.Count; i++)
                {
                    if (BaseEditableObjects[i].ObjectType == ObjectType.TextBox)
                    {
                        EditableObjects.Add(new TextBox(BaseEditableObjects[i], RGBImageAsDirectBitmap, ImageAsColorArray));
                    }
                    else
                    {
                        EditableObjects.Add(BaseEditableObjects[i]);
                    }
                }

                for (int i = 0; i < EditableObjects.Count; i++)
                {
                    if (EditableObjects[i] is TextBox)
                    {
                        TextBox textBox = (TextBox)EditableObjects[i];

                        textBox.PrecalculateMasks(true, true, true);

                        textBox.FillMasksTo(DisplayDirectBitmap, true, true, true);
                    }
                    else
                    {
                        EditableObjects[i].PrecalculateMasks(true, true);
                        EditableObjects[i].FillMasksTo(DisplayDirectBitmap, true, true);
                    }
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
