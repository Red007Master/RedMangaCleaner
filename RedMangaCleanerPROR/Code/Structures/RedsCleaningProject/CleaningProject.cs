using Newtonsoft.Json;
using RedsCleaningProject.EditableObjects;
using RedsTools.Images;
using RedsTools.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace RedsCleaningProject
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

    namespace EditableObjects
    {
        public class RedsMask
        {
            public Point ShiftRelativelyToBitmap { get; set; }

            public int Width { get; set; }
            public int Height { get; set; }

            public byte[,] Mask { get; set; }

            private int PaletteMaxIndex { get; set; }
            public Color[] Palette { get; set; }

            public byte GetOrSetAndGetColorId(Color iColor)
            {
                int result = 0;

                byte iR = iColor.R;
                byte iG = iColor.G;
                byte iB = iColor.B;

                bool isAlredyExist = false;

                for (int i = 0; i < PaletteMaxIndex + 1; i++)
                {
                    byte pR = Palette[i].R;
                    byte pG = Palette[i].G;
                    byte pB = Palette[i].B;

                    if (iR == pR && iG == pG && iB == pB)
                    {
                        isAlredyExist = true;
                        result = i;
                        break;
                    }
                }

                if (!isAlredyExist)
                {
                    PaletteMaxIndex++;
                    Palette[PaletteMaxIndex] = Color.FromArgb(iR, iG, iB);
                    result = PaletteMaxIndex;
                }

                return Convert.ToByte(result);
            }

            public RedsMask(int width, int height, Point shiftRelativelyToBitmap = new Point())
            {
                Width = width;
                Height = height;

                Mask = new byte[Width, Height];

                PaletteMaxIndex = 0;
                Palette = new Color[255];
                Palette[0] = Color.Empty;

                ShiftRelativelyToBitmap = shiftRelativelyToBitmap;
            }
            public RedsMask(Rectangle rectangle) : this(rectangle.Width, rectangle.Height, new Point(rectangle.X, rectangle.Y)) { }
            public RedsMask(byte[,] mask) : this(mask.GetLength(0), mask.GetLength(1)) { Mask = mask; }
        }

        public class EditableObject
        {
            public DirectBitmap ParentDirectBitmap { get; set; }
            public Color[,] ParentColorArray { get; set; }

            public DetectedObject DetectedObject { get; set; }

            public RectangleSettings RectangleSettings { get; set; }
            public RedsMask RectangleMask { get; set; }

            public TextSettings TextSettings { get; set; }
            public RedsMask TextMask { get; set; }

            public void CalculateRectangleMask()
            {
                RectangleMask = Drawing.DrawRectangleOnMask(DetectedObject, RectangleSettings);
            }
            public void CalculateRectangleMask(RectangleSettings rectangleSettings)
            {
                RectangleMask = Drawing.DrawRectangleOnMask(DetectedObject, rectangleSettings);
            }

            public void CalculateTextMask()
            {
                TextMask = Drawing.DrawTextOnMask(DetectedObject, TextSettings);
            }
            public void CalculateTextMask(TextSettings textSettings)
            {
                TextMask = Drawing.DrawTextOnMask(DetectedObject, textSettings);
            }

            public void ApplyOverlayPixelsTo(DirectBitmap targetDirectBitmap)
            {
                Drawing.FillByMask(targetDirectBitmap, RectangleMask);
                Drawing.FillByMask(targetDirectBitmap, TextMask);
            }
            public void UnApplyOverlayPixelsTo(DirectBitmap targetDirectBitmap)
            {
                Drawing.UnFillByMask(targetDirectBitmap, RectangleMask, ParentColorArray);
                Drawing.UnFillByMask(targetDirectBitmap, TextMask, ParentColorArray);
            }

            public EditableObject(DetectedObject detectedObject)
            {
                DetectedObject = detectedObject;
                RectangleSettings = new RectangleSettings(detectedObject.Rectangle);
                TextSettings = new TextSettings();
            }
            public EditableObject(DetectedObject detectedObject, DirectBitmap parentDirectBitmap, Color[,] parentColorArray) : this(detectedObject)
            {
                ParentDirectBitmap = parentDirectBitmap;
                ParentColorArray = parentColorArray;
            }
        }
        public class TextBox : EditableObject
        {
            public RedsMask FillingMask { get; set; }

            public TextBoxFillingSettings FillingSettings { get; set; } = new TextBoxFillingSettings();

            public void CalculateTextBoxFillingMask()
            {
                FillingMask = Drawing.DrawTextBoxFillingOnMask(this, ParentColorArray, FillingSettings);
            }
            public void CalculateTextBoxFillingMask(TextBoxFillingSettings textBoxFillingSettings)
            {
                FillingMask = Drawing.DrawTextBoxFillingOnMask(this, ParentColorArray, textBoxFillingSettings);
            }

            public void ApplyFiledTextBoxPixelsTo(DirectBitmap targetDirectBitmap)
            {
                Drawing.FillByMask(targetDirectBitmap, FillingMask);
            }
            public void UnApplyFiledTextBoxPixelsTo(DirectBitmap targetDirectBitmap)
            {
                Drawing.UnFillByMask(targetDirectBitmap, FillingMask, ParentColorArray);
            }

            public TextBox(DetectedObject detectedObject) : base(detectedObject) { }
            public TextBox(DetectedObject detectedObject, DirectBitmap parentDirectBitmap, Color[,] parentColorArray) : base(detectedObject, parentDirectBitmap, parentColorArray) { }
            public TextBox(DetectedObject detectedObject, DirectBitmap parentDirectBitmap, Color[,] parentColorArray, TextBoxFillingSettings textBoxFillingSettings)
            : base(detectedObject, parentDirectBitmap, parentColorArray)
            {
                FillingSettings = textBoxFillingSettings;
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

        public class RectangleSettings
        {
            public bool Draw { get; set; }
            public Rectangle Rectangle { get; set; }

            public bool AutoRectangleBorderThickness { get; set; }
            public int RectangleBorderThickness { get; set; }

            public bool AutoBorderColor { get; set; }
            public Color BorderColor { get; set; }

            public RectangleSettings()
            {
                Draw = true;

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

        public class TextSettings
        {
            public bool DrawConfidence { get; set; }
            public FontSettings ConfidenceFontSettings { get; set; }

            public bool DrawClassName { get; set; }
            public FontSettings ClassNameFontSettings { get; set; }

            public bool DrawBackground { get; set; }
            public Color BackgroundColor { get; set; }

            public TextSettings()
            {
                DrawConfidence = true;
                ConfidenceFontSettings = new FontSettings();

                DrawClassName = true;
                ClassNameFontSettings = new FontSettings();

                DrawBackground = true;
                BackgroundColor = Color.DarkRed;
            }
        }
        public class FontSettings
        {
            public Font Font { get { return new Font(FontName, FontSize, GraphicsUnit.Pixel); } set { } }

            public bool Draw { get; set; }

            public string FontName { get; set; }

            public bool AutoFontSize { get; set; }
            public int FontSize { get; set; }

            public bool AutoFontColor { get; set; }
            public Color FontColor { get; set; }

            public FontSettings()
            {
                Draw = true;

                FontName = "Consolas";

                AutoFontColor = true;
                FontSize = 30;

                AutoFontColor = false;
                FontColor = Color.White;
            }
        }

        public static class Drawing
        {
            public static RedsMask DrawTextBoxFillingOnMask(TextBox textBox, Color[,] parentImage, TextBoxFillingSettings textBoxFillingSettings)
            {
                Color[,] imagePartOcupiedByMyYoloItem = GetImagePartOccupiedByDetectedObject(parentImage, textBox.DetectedObject);
                byte[,] textBoxPixels = GetTextBoxPixels(imagePartOcupiedByMyYoloItem, textBoxFillingSettings);
                textBoxPixels = FillGapsInsideGrid(textBoxPixels);

                RedsMask result = new RedsMask(textBoxPixels);
                result.ShiftRelativelyToBitmap = new Point(textBox.DetectedObject.Rectangle.X, textBox.DetectedObject.Rectangle.Y);
                result.Palette[1] = textBoxFillingSettings.FillingDisplayColor;

                return result;
            }
            internal static RedsMask DrawRectangleOnMask(DetectedObject detectedObject, RectangleSettings rectangleSettings, Point shift = new Point())
            {
                int width = rectangleSettings.Rectangle.Width;
                int height = rectangleSettings.Rectangle.Height;

                RedsMask result = new RedsMask(detectedObject.Rectangle);

                for (int i = 0; i < rectangleSettings.Rectangle.Width; i++)
                {
                    for (int j = 0; j < rectangleSettings.RectangleBorderThickness; j++)
                    {
                        result.Mask[i, j + shift.Y] = 1;
                    }

                    for (int j = rectangleSettings.RectangleBorderThickness; j > 0; j--)
                    {
                        result.Mask[i, rectangleSettings.Rectangle.Height - j + shift.Y] = 1;
                    }
                }

                for (int i = shift.Y; i < rectangleSettings.Rectangle.Height + shift.Y; i++)
                {
                    for (int j = 0; j < rectangleSettings.RectangleBorderThickness; j++)
                    {
                        result.Mask[j, i] = 1;
                    }

                    for (int j = rectangleSettings.RectangleBorderThickness; j > 0; j--)
                    {
                        result.Mask[rectangleSettings.Rectangle.Width - j, i] = 1;
                    }
                }

                result.Palette[1] = rectangleSettings.BorderColor;

                return result;
            }
            public static RedsMask DrawTextOnMask(DetectedObject detectedObject, TextSettings textSettings)
            {
                int maxFontSize = textSettings.ClassNameFontSettings.FontSize;
                if (maxFontSize < textSettings.ConfidenceFontSettings.FontSize)
                    maxFontSize = textSettings.ConfidenceFontSettings.FontSize;

                string type = detectedObject.Type;
                string confidence = Convert.ToString(Math.Round(detectedObject.Confidence, 2));

                int typeWidth = System.Windows.Forms.TextRenderer.MeasureText(type, textSettings.ClassNameFontSettings.Font).Width;
                int confWidth = System.Windows.Forms.TextRenderer.MeasureText(confidence, textSettings.ConfidenceFontSettings.Font).Width;

                int totalWidth = 0;

                if (textSettings.ClassNameFontSettings.Draw)
                    totalWidth += typeWidth;

                if (textSettings.ConfidenceFontSettings.Draw)
                    totalWidth += confWidth;

                int width = totalWidth;
                int height = maxFontSize + 6;

                RedsMask result = new RedsMask(width, height);
                result.ShiftRelativelyToBitmap = new Point(detectedObject.Rectangle.X, detectedObject.Rectangle.Y - maxFontSize - 6);

                Bitmap tempBitmap = new Bitmap(result.Width, result.Height);
                Graphics graphics = Graphics.FromImage(tempBitmap);

                //graphics.Clear(textSettings.BackgroundColor);
                graphics.Clear(Color.Green);

                int shift = 0;

                if (textSettings.ClassNameFontSettings.Draw)
                {
                    graphics.DrawString(type, textSettings.ClassNameFontSettings.Font, new SolidBrush(textSettings.ClassNameFontSettings.FontColor), new PointF(shift, 0));
                    shift += typeWidth + 5;
                }

                if (textSettings.ConfidenceFontSettings.Draw)
                {
                    graphics.DrawString(confidence, textSettings.ConfidenceFontSettings.Font, new SolidBrush(textSettings.ConfidenceFontSettings.FontColor), new PointF(shift, 0));
                    shift += typeWidth + 5;
                }

                for (int x = 0; x < result.Width; x++)
                {
                    for (int y = 0; y < result.Height; y++)
                    {
                        result.Mask[x, y] = result.GetOrSetAndGetColorId(tempBitmap.GetPixel(x, y));
                    }
                }

                //tempBitmap.Save(@"D:\img.png");

                graphics.Dispose();
                tempBitmap.Dispose();

                return result;
            }

            public static void FillByMask(DirectBitmap targetDirectBitmap, RedsMask mask)
            {
                for (int x = 0; x < mask.Width; x++)
                {
                    for (int y = 0; y < mask.Height; y++)
                    {
                        if (mask.Mask[x, y] != 0)
                        {
                            int targetX = x + mask.ShiftRelativelyToBitmap.X;
                            int targetY = y + mask.ShiftRelativelyToBitmap.Y;

                            bool coordinatesIsInBounds = targetDirectBitmap.Width > targetX && targetDirectBitmap.Height > targetY && targetX >= 0 && targetY >= 0;

                            if (coordinatesIsInBounds)
                                targetDirectBitmap.SetPixel(targetX, targetY, mask.Palette[mask.Mask[x, y]]);
                        }
                    }
                }
            }
            public static void UnFillByMask(DirectBitmap targetDirectBitmap, RedsMask mask, Color[,] sourceImage)
            {
                for (int x = 0; x < mask.Width; x++)
                {
                    for (int y = 0; y < mask.Height; y++)
                    {
                        if (mask.Mask[x, y] != 0)
                        {
                            int targetX = x + mask.ShiftRelativelyToBitmap.X;
                            int targetY = y + mask.ShiftRelativelyToBitmap.Y;

                            bool coordinatesIsInBounds = targetDirectBitmap.Width > targetX && targetDirectBitmap.Height > targetY && targetX >= 0 && targetY >= 0;

                            if (coordinatesIsInBounds)
                            {
                                targetDirectBitmap.SetPixel(targetX, targetY, sourceImage[targetX, targetY]);
                            }
                        }
                    }
                }
            }

            public static byte[,] FillGapsInsideGrid(byte[,] inputGrid)
            {
                int width = inputGrid.GetLength(0);
                int height = inputGrid.GetLength(1);

                bool filedPixelIsFound;
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        if (inputGrid[i, j] == 0)
                        {
                            filedPixelIsFound = false;
                            for (int l = i; l < width; l++)
                            {
                                if (inputGrid[l, j] == 1)
                                {
                                    filedPixelIsFound = true;
                                    break;
                                }
                            }

                            if (!filedPixelIsFound)
                            {
                                continue;
                            }

                            filedPixelIsFound = false;
                            for (int l = i; l > 0; l--)
                            {
                                if (inputGrid[l, j] == 1)
                                {
                                    filedPixelIsFound = true;
                                    break;
                                }
                            }

                            if (!filedPixelIsFound)
                            {
                                continue;
                            }

                            filedPixelIsFound = false;
                            for (int l = j; l < height; l++)
                            {
                                if (inputGrid[i, l] == 1)
                                {
                                    filedPixelIsFound = true;
                                    break;
                                }
                            }

                            if (!filedPixelIsFound)
                            {
                                continue;
                            }

                            filedPixelIsFound = false;
                            for (int l = j; l > 0; l--)
                            {
                                if (inputGrid[i, l] == 1)
                                {
                                    filedPixelIsFound = true;
                                    break;
                                }
                            }

                            if (!filedPixelIsFound)
                            {
                                continue;
                            }

                            inputGrid[i, j] = 1;
                        }
                    }
                }

                return inputGrid;
            }

            public static Color[,] GetImagePartOccupiedByDetectedObject(Color[,] iImageAsColorArray, DetectedObject iDetectedObject)
            {
                int counterX = 0, counterY = 0;
                Color[,] textBoxAsColorArray = new Color[iDetectedObject.Rectangle.Width, iDetectedObject.Rectangle.Height];

                int endPointOfX = iDetectedObject.Rectangle.X + iDetectedObject.Rectangle.Width;
                if (endPointOfX > iImageAsColorArray.GetLength(0))
                    endPointOfX = iImageAsColorArray.GetLength(0);

                int endPointOfY = iDetectedObject.Rectangle.Y + iDetectedObject.Rectangle.Height;
                if (endPointOfY > iImageAsColorArray.GetLength(1))
                    endPointOfY = iImageAsColorArray.GetLength(1);

                for (int x = iDetectedObject.Rectangle.X; x < endPointOfX; x++)
                {
                    for (int y = iDetectedObject.Rectangle.Y; y < endPointOfY; y++)
                    {
                        textBoxAsColorArray[counterX, counterY] = iImageAsColorArray[x, y];
                        counterY++;
                    }
                    counterX++;
                    counterY = 0;
                }

                return textBoxAsColorArray;
            }
            public static byte[,] GetTextBoxPixels(Color[,] inputImageAsColorArray, TextBoxFillingSettings textBoxFillingSettings)
            {
                Queue<Point> bufferQueue = new Queue<Point>();
                int width = inputImageAsColorArray.GetLength(0);
                int height = inputImageAsColorArray.GetLength(1);
                byte[,] result = new byte[width, height];
                int counter = 0;

                int pgm = textBoxFillingSettings.PixelGrayScaleLimit;
                int croshairLineLenght = 0;

                if (true)
                {
                    croshairLineLenght = textBoxFillingSettings.CroshairLineLenght;
                } //TODO textBoxFillingSettings.AutomaticCroshairLineLenght

                Point center = new Point(width / 2, height / 2);
                bufferQueue.Enqueue(center);

                for (int i = 0; i < textBoxFillingSettings.CroshairLineLenght; i++)
                {
                    bufferQueue.Enqueue(new Point(center.X + i, center.Y));
                    bufferQueue.Enqueue(new Point(center.X, center.Y + i));
                    bufferQueue.Enqueue(new Point(center.X - i, center.Y));
                    bufferQueue.Enqueue(new Point(center.X, center.Y - i));
                }

                while (bufferQueue.Count > 0)
                {
                    counter++;

                    Point bufferPoint = bufferQueue.Peek();

                    if (bufferPoint.X + 1 < width && bufferPoint.X - 1 > 0 && bufferPoint.Y + 1 < height && bufferPoint.Y - 1 > 0) //TODO finall rep pgm to color? and rep .G
                    {
                        if (inputImageAsColorArray[bufferPoint.X + 1, bufferPoint.Y].G > pgm)
                        {
                            if (result[bufferPoint.X + 1, bufferPoint.Y] == 0)
                            {
                                result[bufferPoint.X + 1, bufferPoint.Y] = 1;
                                bufferQueue.Enqueue(new Point(bufferPoint.X + 1, bufferPoint.Y));
                            }
                        }

                        if (inputImageAsColorArray[bufferPoint.X - 1, bufferPoint.Y].G > pgm)
                        {
                            if (result[bufferPoint.X - 1, bufferPoint.Y] == 0)
                            {
                                result[bufferPoint.X - 1, bufferPoint.Y] = 1;
                                bufferQueue.Enqueue(new Point(bufferPoint.X - 1, bufferPoint.Y));
                            }
                        }

                        if (inputImageAsColorArray[bufferPoint.X, bufferPoint.Y + 1].G > pgm)
                        {
                            if (result[bufferPoint.X, bufferPoint.Y + 1] == 0)
                            {
                                result[bufferPoint.X, bufferPoint.Y + 1] = 1;
                                bufferQueue.Enqueue(new Point(bufferPoint.X, bufferPoint.Y + 1));
                            }
                        }

                        if (inputImageAsColorArray[bufferPoint.X, bufferPoint.Y - 1].G > pgm)
                        {
                            if (result[bufferPoint.X, bufferPoint.Y - 1] == 0)
                            {
                                result[bufferPoint.X, bufferPoint.Y - 1] = 1;
                                bufferQueue.Enqueue(new Point(bufferPoint.X, bufferPoint.Y - 1));
                            }
                        }
                    }

                    bufferQueue.Dequeue();
                }

                return result;
            }
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
