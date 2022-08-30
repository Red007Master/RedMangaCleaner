﻿using RedsCleaningProject.MasksAndEditableObjects;
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
            public string ImageFilePath { get; set; }

            public int Width { get; set; }
            public int Height { get; set; }

            public bool ProcessAsBaW { get; set; }

            public List<DetectedObject> DetectedObjects { get; set; }

            public BasicImageData(string imagePath, int width, int height, bool isBlackAndWhite, List<DetectedObject> detectedObjects)
            {
                ImageFilePath = imagePath;
                Width = width;
                Height = height;
                ProcessAsBaW = isBlackAndWhite;
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

            public byte[,] ImageAsByteArray { get; set; }
            public DirectBitmap BaWImageAsDirectBitmap { get; set; }

            public Color[,] ImageAsColorArray { get; set; }
            public DirectBitmap RGBImageAsDirectBitmap { get; set; }

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
                if (ProcessAsBaW)
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
                    EditableObjects[i].RectangleMask = MaskWork.DrawRectangleOnMask(EditableObjects[i].DetectedObject, EditableObjects[i].RectangleSettings);
                    EditableObjects[i].TextMask = MaskWork.DrawTextOnMask(EditableObjects[i].DetectedObject, EditableObjects[i].TextSettings);

                    if (EditableObjects[i] is TextBox)
                    {
                        TextBox textBox = (TextBox)EditableObjects[i];

                        textBox.FillingMask = MaskWork.DrawFillingOnMask(textBox, ImageAsColorArray, textBox.FillingSettings);

                        MaskWork.FillByMask(DisplayDirectBitmap, textBox.FillingMask);
                    }

                    MaskWork.FillByMask(DisplayDirectBitmap, EditableObjects[i].RectangleMask);
                    MaskWork.FillByMask(DisplayDirectBitmap, EditableObjects[i].TextMask);
                }
            }
        }
    }
}
