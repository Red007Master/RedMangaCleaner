using Newtonsoft.Json;
using RedsCleaningProjects.RedImages;
using RedsCleaningProjects.Core;
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


        public class RedsMask
        {
            public Point ShiftRelativelyToBitmap { get; set; } 

            public int Width { get; set; }
            public int Height { get; set; }

            public byte[,] Mask { get; set; }
            public Color[] Palette { get; set; }

            public RedsMask(int width, int height, Point shiftRelativelyToBitmap = new Point())
            {
                Width = width;
                Height = height;

                Mask = new byte[Width, Height];

                Palette = new Color[255];
                Palette[0] = Color.Empty;

                ShiftRelativelyToBitmap = shiftRelativelyToBitmap;
            }
            public RedsMask(byte[,] mask, Point shiftRelativelyToBitmap = new Point())
            {
                Width = mask.GetLength(0);
                Height = mask.GetLength(1);

                Mask = mask;

                Palette = new Color[255];
                Palette[0] = Color.Empty;

                ShiftRelativelyToBitmap = shiftRelativelyToBitmap;
            }
        }

        public class RectangleSettings
        {
            public Rectangle Rectangle { get; set; }

            public bool AutoRectangleBorderThickness { get; set; }
            public int RectangleBorderThickness { get; set; }

            public bool AutoBorderColor { get; set; }
            public Color BorderColor { get; set; }

            public RectangleSettings()
            {
                AutoRectangleBorderThickness = false;
                RectangleBorderThickness = 5;

                AutoBorderColor = false;
                BorderColor = Color.Red;
            }
            public RectangleSettings(Rectangle rectangle) : this()
            {
                Rectangle = rectangle;
            }
        }

        public class FontSettings
        {
            public string FontName { get; set; }

            public bool AutoFontSize { get; set; }
            public int FontSize { get; set; }

            public bool AutoFontColor { get; set; }
            public Color FontColor { get; set; }

            public FontSettings()
            {
                FontName = "Consolas";

                AutoFontColor = true;
                FontSize = 11;

                AutoFontColor = false;
                FontColor = Color.Red;
            }
        }

        public class TextBoxFillingSettings
        {
            public bool FillAsBlackAndWhite { get; set; }

            public byte PixelGrayScaleLimit { get; set; }

            public bool AutomaticCroshairLineLenght { get; set; }
            public int CroshairLineLenght { get; set; }

            public Color FillingDisplayColor { get; set; }
            public Color FillingFinalColor { get; set; }


            public TextBoxFillingSettings()
            {
                FillAsBlackAndWhite = true;

                PixelGrayScaleLimit = 245;

                AutomaticCroshairLineLenght = false;
                CroshairLineLenght = 10;

                FillingDisplayColor = Color.DarkGreen;
                FillingFinalColor = Color.White;
            }
        }
        public class EditableObjectInfoOverlayConfiguration
        {
            public bool DrawRectangle { get; set; }
            public RectangleSettings RectangleSettings { get; set; }

            public bool DrawConfidence { get; set; }
            public FontSettings ConfidenceFontSettings { get; set; }

            public bool DrawClassName { get; set; }
            public FontSettings ClassNameFontSettings { get; set; }

            public EditableObjectInfoOverlayConfiguration()
            {
                DrawRectangle = true;
                RectangleSettings = new RectangleSettings();

                DrawConfidence = true;
                ConfidenceFontSettings = new FontSettings();

                DrawClassName = true;
                ClassNameFontSettings = new FontSettings();
            }
            public EditableObjectInfoOverlayConfiguration(Rectangle rectangle)
            {
                DrawRectangle = true;
                RectangleSettings = new RectangleSettings(rectangle);

                DrawConfidence = true;
                ConfidenceFontSettings = new FontSettings();

                DrawClassName = true;
                ClassNameFontSettings = new FontSettings();
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
                    TextBoxes.Add(new TextBox(EditableObjects[i].DetectedObject, RGBImageAsDirectBitmap, ImageAsColorArray));
                }
                
                DisplayDirectBitmap = new DirectBitmap(BaWImageAsDirectBitmap);
                //DisplayDirectBitmap = Work.Cleaning.CleanRGBImage(DetectedObjects, ImageAsColorArray);

                for (int i = 0; i < TextBoxes.Count; i++)
                {
                    TextBoxes[i].CalculateTextBoxPixelsThatMustBeCleaned();
                    TextBoxes[i].ApplyFiledTextBoxPixelsTo(DisplayDirectBitmap);

                    TextBoxes[i].CalculateInfoOverlay();
                    TextBoxes[i].ApplyOverlayPixelsTo(DisplayDirectBitmap);
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
        }

        public class EditableObject
        {
            public DirectBitmap ParentDirectBitmap { get; set; }
            public Color[,] ParentColorArray { get; set; }

            public DetectedObject DetectedObject { get; set; }
            public RedsMask FilledPixelsInfoOverlay { get; set; }


            public EditableObjectInfoOverlayConfiguration OverlaySettings { get; set; }
            public void CalculateInfoOverlay()
            {
                FilledPixelsInfoOverlay = Work.Cleaning.CalculateInfoOverlayPixels(DetectedObject, OverlaySettings);
            }
            public void CalculateInfoOverlay(EditableObjectInfoOverlayConfiguration editableObjectInfoOverlayConfiguration)
            {
                FilledPixelsInfoOverlay = Work.Cleaning.CalculateInfoOverlayPixels(DetectedObject, editableObjectInfoOverlayConfiguration);
            }

            public void ApplyOverlayPixelsTo(DirectBitmap targetDirectBitmap)
            {
                Drawing.FillByMask(targetDirectBitmap, FilledPixelsInfoOverlay);
            }
            public void UnApplyOverlayPixelsTo(DirectBitmap targetDirectBitmap)
            {
                Drawing.UnFillByMask(targetDirectBitmap, FilledPixelsInfoOverlay, ParentColorArray);
            }

            public EditableObject(DetectedObject detectedObject)
            {
                DetectedObject = detectedObject;
                OverlaySettings = new EditableObjectInfoOverlayConfiguration(detectedObject.Rectangle);
            }
            public EditableObject(DetectedObject detectedObject, DirectBitmap parentDirectBitmap, Color[,] parentColorArray) : this(detectedObject)
            {
                ParentDirectBitmap = parentDirectBitmap;
                ParentColorArray = parentColorArray;
            }
        }

        public class TextBox : EditableObject
        {
            public RedsMask FilledPixelsCleaning { get; set; }

            public TextBoxFillingSettings FillingSettings { get; set; } = new TextBoxFillingSettings();
            public void CalculateTextBoxPixelsThatMustBeCleaned()
            {
                FilledPixelsCleaning = Work.Cleaning.CalculateTextBoxPixelsThatMustBeCleaned(this, ParentColorArray, FillingSettings);
            }
            public void CalculateTextBoxPixelsThatMustBeCleaned(TextBoxFillingSettings textBoxFillingSettings)
            {
                FilledPixelsCleaning = Work.Cleaning.CalculateTextBoxPixelsThatMustBeCleaned(this, ParentColorArray, textBoxFillingSettings);
            }

            public void ApplyFiledTextBoxPixelsTo(DirectBitmap targetDirectBitmap)
            {
                Drawing.FillByMask(targetDirectBitmap, FilledPixelsCleaning);
            }
            public void UnApplyFiledTextBoxPixelsTo(DirectBitmap targetDirectBitmap)
            {
                Drawing.UnFillByMask(targetDirectBitmap, FilledPixelsCleaning, ParentColorArray);
            }

            public TextBox(DetectedObject detectedObject) : base(detectedObject){}
            public TextBox(DetectedObject detectedObject, DirectBitmap parentDirectBitmap, Color[,] parentColorArray) : base(detectedObject, parentDirectBitmap, parentColorArray){}
            public TextBox(DetectedObject detectedObject, DirectBitmap parentDirectBitmap, Color[,] parentColorArray, TextBoxFillingSettings textBoxFillingSettings)
            : base(detectedObject, parentDirectBitmap, parentColorArray)
            {
                FillingSettings = textBoxFillingSettings;
            }
        }
    }
}
