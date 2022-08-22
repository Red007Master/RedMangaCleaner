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

        //public class CleaningProject
        //{
        //    public CleaningProjectInfo CleaningProjectInfo { get; set; }

        //    public List<BasicImageData> BasicImageDatas { get; set; }
        //    public List<RedImageCore> RedImageCores { get; set; }
        //    public List<RedImageFull> RedImageFulls { get; set; }

        //    public FillParam DefFillParam { get; set; }
        //}

        public class CleaningProjectInfo : IComparable
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

                    Save();
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
                TypeConverter.ChildParentSetTo<BasicImageData, RedImageCore> (basicImageData, this);

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

            public RedImageFull(RedImageCore redImageCore)
            {
                TypeConverter.ChildParentSetTo<RedImageCore, RedImageFull>(redImageCore, this);

                CompileImageAsDirectBitmap();
                CompileImageAsByteArray();
                CompileImageAsColorArray();

                for (int i = 0; i < EditableObjects.Count; i++)
                {
                    TextBoxes.Add(new TextBox(RGBImageAsDirectBitmap, ImageAsColorArray, EditableObjects[i].DetectedObject));
                }
                
                DisplayDirectBitmap = new DirectBitmap(BaWImageAsDirectBitmap);
                //DisplayDirectBitmap = Work.Cleaning.CleanRGBImage(DetectedObjects, ImageAsColorArray);

                for (int i = 0; i < TextBoxes.Count; i++)
                {
                    TextBoxes[i].CalculateTextBoxPixelsThatMustBeCleaned();
                    TextBoxes[i].ApplyFiledTextBoxPixelsTo(DisplayDirectBitmap);
                }

                //DrawRectangles();
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
                    RectangleDrawOptions rectOptions = new RectangleDrawOptions();
                    rectOptions.Color = Color.Blue;
                    rectOptions.Thickness = 5;

                    rectOptions.Rectangle = EditableObjects[i].DetectedObject.Rectangle;

                    Drawing.DrawRectangleOnDirectBitmap(DisplayDirectBitmap, rectOptions);
                }
                
                for (int i = 0; i < TextBoxes.Count; i++)
                {
                    TextBoxes[i].ApplyFiledTextBoxPixelsTo(DisplayDirectBitmap);
                }
                RectDrawStatus = true;
            }   //kill

            public void UndrawRectangles()
            {
                for (int i = 0; i < EditableObjects.Count; i++)
                {
                    RectangleDrawOptions rectOptions = new RectangleDrawOptions();
                    rectOptions.Color = Color.Red;
                    rectOptions.Thickness = 5;

                    rectOptions.Rectangle = EditableObjects[i].DetectedObject.Rectangle;

                    Drawing.UnDrawRectangleOnDirectBitmap(DisplayDirectBitmap, rectOptions, ImageAsColorArray);
                }

                for (int i = 0; i < TextBoxes.Count; i++)
                {
                    TextBoxes[i].UnFillTextBox(ImageAsColorArray, DisplayDirectBitmap);
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
        }

        public class EditableObject
        {
            public DirectBitmap ParentDirectBitmap { get; set; }
            public Color[,] ParentColorArray { get; set; }

            public DetectedObject DetectedObject { get; set; }
            public byte[,] FilledPixelsOverlay { get; set; }
            public Rectangle WorkZoneRectangle { get; set; }

            public EditableObject(DetectedObject detectedObject)
            {
                DetectedObject = detectedObject;
            }
            public EditableObject(DirectBitmap parentDirectBitmap, Color[,] parentColorArray, DetectedObject detectedObject)
            {
                ParentDirectBitmap = parentDirectBitmap;
                ParentColorArray = parentColorArray;
                DetectedObject = detectedObject;
            }
        }

        public class TextBox : EditableObject
        {
            public byte[,] FilledPixelsCleaning { get; set; }

            public void CalculateTextBoxPixelsThatMustBeCleaned() //TODO add arguments
            {
                FilledPixelsCleaning = Work.Cleaning.CalculateTextBoxPixelsThatMustBeCleaned(this, ParentColorArray); //there 
            }
            
            public void UnFillTextBox(Color[,] parentImage, DirectBitmap directBitmap)
            {
                Work.Cleaning.UnFillTextBox(this, parentImage, directBitmap);
            }

            public void ApplyFiledTextBoxPixelsTo(DirectBitmap targetDirectBitmap)
            {
                Drawing.FillByMask(targetDirectBitmap, FilledPixelsCleaning, Color.Gold, new Point(DetectedObject.Rectangle.X, DetectedObject.Rectangle.Y));
            }

            public TextBox(DetectedObject detectedObject) : base(detectedObject){}
            public TextBox(DirectBitmap parentDirectBitmap, Color[,] parentColorArray, DetectedObject detectedObject) : base(parentDirectBitmap, parentColorArray, detectedObject){}
        }
    }
}
