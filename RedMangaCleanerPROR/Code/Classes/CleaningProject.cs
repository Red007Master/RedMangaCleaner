using Newtonsoft.Json;
using RedsCleaningProjects.RedImages;
using RedsTools.Drawing;
using RedsTools.Images;
using RedsTools.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace RedsCleaningProjects
{
    namespace Core
    {
        public class StartArguments
        {
            public FolderOptions FolderOptions { get; set; }

            public int CleaningProjectId { get; set; }
            public string CleaningProjectFolderName { get; set; }

            public bool OutputBlackAndWhiteImages { get; set; }
            public bool ConductObjectDetectionOnBlackAndWhiteVariants { get; set; }
            public bool ConductTextBoxFillingOnBlackAndWhiteVariants { get; set; }

            public string UserTag { get; set; }
            public string InputPath { get; set; }
        }

        internal class CleaningProject
        {
            public CleaningProjectInfo CleaningProjectInfo { get; set; }

            public List<ImageData> ImageDatas { get; set; }
            public List<RedImageCore> RedImageCores { get; set; }
            public List<RedImageFull> RedImageFulls { get; set; }

            public FillParam DefFillParam { get; set; }
        }

        class CleaningProjectInfo : IComparable
        {
            public int Id { get; set; }
            public bool OutputBlackAndWhiteImages { get; set; }
            public bool ConductObjectDetectionOnBlackAndWhiteVariants { get; set; }
            public bool ConductTextBoxFillingOnBlackAndWhiteVariants { get; set; }
            public int ImageCount { get; set; }
            public string ImageSourcePath { get; set; }
            public string UserTag { get; set; }
            public DateTime CreateTime { get; set; }
            public bool IsPRORFinished { get; set; }

            public CleaningProjectInfo(StartArguments inputArguments, int id)
            {
                Id = id;
                OutputBlackAndWhiteImages = inputArguments.OutputBlackAndWhiteImages;
                ConductTextBoxFillingOnBlackAndWhiteVariants = inputArguments.ConductTextBoxFillingOnBlackAndWhiteVariants;
                ConductObjectDetectionOnBlackAndWhiteVariants = inputArguments.ConductObjectDetectionOnBlackAndWhiteVariants;
                ImageCount = Directory.GetFiles(inputArguments.InputPath).Length;
                ImageSourcePath = inputArguments.InputPath;
                UserTag = inputArguments.UserTag;
                CreateTime = DateTime.Now;
                IsPRORFinished = false;
            }

            public CleaningProjectInfo(StartArguments inputArguments)
            {
                Id = inputArguments.CleaningProjectId;
                OutputBlackAndWhiteImages = inputArguments.OutputBlackAndWhiteImages;
                ConductTextBoxFillingOnBlackAndWhiteVariants = inputArguments.ConductTextBoxFillingOnBlackAndWhiteVariants;
                ConductObjectDetectionOnBlackAndWhiteVariants = inputArguments.ConductObjectDetectionOnBlackAndWhiteVariants;
                ImageCount = Directory.GetFiles(inputArguments.InputPath).Length;
                ImageSourcePath = inputArguments.InputPath;
                UserTag = inputArguments.UserTag;
                CreateTime = DateTime.Now;
                IsPRORFinished = false;
            }

            //crate newtons json contructor
            public CleaningProjectInfo() { }

            public void Save(string iPath)
            {
                string serialized = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(iPath, serialized);
            }

            public int CompareTo(object obj)
            {
                if (obj is CleaningProjectInfo)
                {
                    CleaningProjectInfo buffer = (CleaningProjectInfo)obj;

                    if (Id > buffer.Id)
                    {
                        return 1;
                    }
                    else if (Id < buffer.Id)
                    {
                        return -1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    P.Logger.Log("Error in CompareTo/CleaningProjectInfo", LogLevel.FatalError);
                    return 0;
                }
            }
        }
        public class CleaningProjectsGlobalInfo
        {
            private string Path { get; set; }

            public int IdCounter { get; set; }
            public int CriticalErrorsCounter { get; set; }

            public void Load()
            {
                if (File.Exists(Path))
                {
                    string serialized = File.ReadAllText(Path);

                    CleaningProjectsGlobalInfo buffer = JsonConvert.DeserializeObject<CleaningProjectsGlobalInfo>(serialized);

                    foreach (PropertyInfo propertyInfo in this.GetType().GetProperties())
                    {
                        var property = typeof(CleaningProjectsGlobalInfo).GetProperty(propertyInfo.Name);
                        var value = property.GetValue(buffer);

                        property.SetValue(this, value);
                    }
                }
                else
                {
                    CleaningProjectsGlobalInfo buffer = new CleaningProjectsGlobalInfo();

                    string serialized = JsonConvert.SerializeObject(buffer);
                    File.WriteAllText(Path, serialized);
                }
            }
            public void Save()
            {
                string serialized = JsonConvert.SerializeObject(this);
                File.WriteAllText(Path, serialized);
            }

            public int GetAndIncrementId()
            {
                Load();

                int id = IdCounter;
                IdCounter++;

                Save();

                return id;
            }

            public CleaningProjectsGlobalInfo()
            {
                IdCounter = 0;
                CriticalErrorsCounter = 0;
            }
            public CleaningProjectsGlobalInfo(string path)
            {
                Path = path;

                Load();
            }
        }

        public enum FolderOptions
        {
            CreateNewFolderById = 0,
            CreateNewFolderByName = 1,
            AutoCreateById = 2,
        }
    }

    namespace RedImages
    {
        public class ImageData
        {
            public string ImageFilePath { get; set; }
            public bool IsBlackAndWhite { get; set; }
            public List<MyYoloItem> MyYoloItemsList { get; set; }

            public ImageData(string imagePath, bool isBlackAndWhite, List<MyYoloItem> yoloItemsList)
            {
                ImageFilePath = imagePath;
                IsBlackAndWhite = isBlackAndWhite;
                MyYoloItemsList = yoloItemsList;
            }
        }

        public class RedImageCore : IComparable
        {
            public bool ImageIsBlackAndWhite { get; set; }
            public bool ImageProcessingModeAsBlackAndWhite { get; set; }

            public string ImageFileName { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }

            public List<FillPoint> FillPointsUserInput { get; set; } = new List<FillPoint>();
            public List<FillPoint> FillPointsObjectDetection { get; set; } = new List<FillPoint>();
            public List<FillPoint> FillPointsProgramOther { get; set; } = new List<FillPoint>();

            public List<TextBoxInfo> TextBoxes { get; set; } = new List<TextBoxInfo>();

            public RedImageCore(ImageData imageData)
            {
                Bitmap image = new Bitmap(imageData.ImageFilePath);

                ImageIsBlackAndWhite = imageData.IsBlackAndWhite;
                ImageFileName = Path.GetFileName(imageData.ImageFilePath);
                Width = image.Width;
                Height = image.Height;

                for (int i = 0; i < imageData.MyYoloItemsList.Count; i++)
                {
                    TextBoxes.Add(new TextBoxInfo(imageData.MyYoloItemsList[i]));
                    FillPointsObjectDetection.Add(new FillPoint(imageData.MyYoloItemsList[i]));
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
            public DirectBitmap BaWImageAsDirectBitmap { get; set; }
            public DirectBitmap RGBImageAsDirectBitmap { get; set; }
            public Color[,] ImageAsColorArray { get; set; }
            public byte[,] ImageAsByteArray { get; set; }

            public RedImageFull(RedImageCore redImageCore)
            {
                TypeConverter.ChildParentSetTo<RedImageCore, RedImageFull>(redImageCore, this);

                CompileImageAsDirectBitmap();
                CompileImageAsByteArray();
                CompileImageAsColorArray();
                //FillAllTextBoxes();

                for (int i = 0; i < TextBoxes.Count; i++)
                {
                    RectangleDrawOptions rectOptions = new RectangleDrawOptions();
                    rectOptions.Color = Color.Red;
                    rectOptions.Thickness = 10;
                    rectOptions.DrawPositioningOptions = DrawPositioningOptions.ByWidthAndHeightThrueXandY;

                    rectOptions.FirstPoint = new RedsPoint(TextBoxes[i].X, TextBoxes[i].Y);
                    rectOptions.Width = TextBoxes[i].Width;
                    rectOptions.Height = TextBoxes[i].Height;

                    Drawing.DrawRectangleOnDirectBitmap(BaWImageAsDirectBitmap, rectOptions);
                }
            }

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
                }
                else
                {
                    ImageAsColorArray = Images.RGB.BitmapToColorArray(BaWImageAsDirectBitmap.Bitmap);
                }
            }
            //public void FillAllTextBoxes()
            //{
            //    Inpainter inpainter = new Inpainter();

            //    Bitmap mask = new Bitmap(Width, Height);
            //    Graphics graphics = Graphics.FromImage(mask);

            //    foreach (var item in TextBoxes)
            //    {
            //        int x = item.X;
            //        int y = item.Y;
            //        int hight = item.Height;
            //        int width = item.Width;

            //        System.Drawing.Rectangle rect = new System.Drawing.Rectangle(x, y, width, hight);

            //        SolidBrush brush = new SolidBrush(Color.Red);
            //        graphics.FillRectangle(brush, rect);
            //    };

            //    Color[,] col = Images.RGB.BitmapToColorArray(RGBImageAsDirectBitmap.Bitmap);
            //    Bitmap bmp = Images.RGB.ColorArrayToDirectBitmap(col).Bitmap;

            //    InpaintSettings inpaintSettings = new InpaintSettings();
            //    inpaintSettings.MaxInpaintIterations = 50;
            //    var result = inpainter.Inpaint(BitmapToArgb(bmp), BitmapToArgb(mask), inpaintSettings);
            //    Bitmap resAsBitmap = result.FromArgbToBitmap();

            //    for (int x = 0; x < Width; x++)
            //    {
            //        for (int y = 0; y < Height; y++)
            //        {
            //            RGBImageAsDirectBitmap.SetPixel(x, y, resAsBitmap.GetPixel(x, y));
            //        }
            //    }
            //}

            public static Point Center(Rectangle rect)
            {
                return new System.Drawing.Point(rect.Left + rect.Width / 2, rect.Top + rect.Height / 2);
            }

            public RedImageFull()
            {

            }

            public void Dispose()
            {
                BaWImageAsDirectBitmap.Dispose();
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
        }

        public class TextBoxInfo
        {
            public MyYoloItem MyYoloItem { get; set; }

            private byte[,] FilledPixelsAsByteArray { get; set; }

            public int X { get; set; }
            public int Y { get; set; }
            public int Width { get; set; }
            public int Height { get; set; }

            public byte[,] GetFilledPixelsAsByteArray()
            {
                return FilledPixelsAsByteArray;
            }
            public void SetFilledPixelsAsByteArray(byte[,] inputArray)
            {
                FilledPixelsAsByteArray = inputArray;
            }

            public TextBoxInfo(MyYoloItem myYoloItem)
            {
                SetFromMyYoloItem(myYoloItem);
            }

            public void SetFromMyYoloItem(MyYoloItem myYoloItem)
            {
                MyYoloItem = myYoloItem;
                X = myYoloItem.X;
                Y = myYoloItem.Y;
                Height = myYoloItem.Height;
                Width = myYoloItem.Width;
            }
        }
    }
}
