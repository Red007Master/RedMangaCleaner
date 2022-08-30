using RedsCleaningProject.MangaCleaning;
using RedsCleaningProject.MasksAndEditableObjects;
using RedsCleaningProject.MaskWorking;
using RedsCleaningProject.Settings;
using System;
using System.Drawing;

namespace RedsCleaningProject
{
    namespace MasksAndEditableObjects
    {
        public class RedsMask
        {
            public bool IsDrawed { get; set; }

            public DirectBitmap DisplayDirectBitmap { get; set; }
            public Color[,] ParentColorArray { get; set; }

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

            public void ApplyMask()
            {
                MaskWork.FillByMask(DisplayDirectBitmap, this);
            }
            public void ApplyMask(DirectBitmap targetDirectBitmap)
            {
                MaskWork.FillByMask(targetDirectBitmap, this);
            }

            public void UnApplyMask()
            {
                MaskWork.UnFillByMask(DisplayDirectBitmap, this, ParentColorArray);
            }
            public void UnApplyMask(DirectBitmap targetDirectBitmap)
            {
                MaskWork.UnFillByMask(targetDirectBitmap, this, ParentColorArray);
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
            public ObjectType ObjectType { get; set; }

            public DirectBitmap ParentDirectBitmap { get; set; }

            public Color[,] ParentColorArray { get; set; }

            public DetectedObject DetectedObject { get; set; }

            public RectangleSettings RectangleSettings { get; set; }
            public RedsMask RectangleMask { get; set; }

            public TextSettings TextSettings { get; set; }
            public RedsMask TextMask { get; set; }

            public void CalculateRectangleMask()
            {
                RectangleMask = MaskWork.DrawRectangleOnMask(DetectedObject, RectangleSettings);
            }

            public void CalculateTextMask()
            {
                TextMask = MaskWork.DrawTextOnMask(DetectedObject, TextSettings);
            }

            public void UnApplyOverlayPixels(DirectBitmap targetDirectBitmap)
            {
                MaskWork.UnFillByMask(targetDirectBitmap, RectangleMask, ParentColorArray);
                MaskWork.UnFillByMask(targetDirectBitmap, TextMask, ParentColorArray);
            }

            public EditableObject(DetectedObject detectedObject)
            {
                DetectedObject = detectedObject;
                RectangleSettings = new RectangleSettings(detectedObject.Rectangle);
                TextSettings = new TextSettings();

                ObjectType = (ObjectType)detectedObject.Id;
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
                FillingMask = MaskWork.DrawFillingOnMask(this, ParentColorArray, FillingSettings);
            }

            public TextBox(DetectedObject detectedObject) : base(detectedObject) { }
            public TextBox(DetectedObject detectedObject, DirectBitmap parentDirectBitmap, Color[,] parentColorArray)
            : base(detectedObject, parentDirectBitmap, parentColorArray) { }
            public TextBox(DetectedObject detectedObject, DirectBitmap parentDirectBitmap, Color[,] parentColorArray, TextBoxFillingSettings textBoxFillingSettings)
            : base(detectedObject, parentDirectBitmap, parentColorArray)
            {
                FillingSettings = textBoxFillingSettings;
            }
        }

        public enum ObjectType 
        {
            TextBox = 0,
            TextWithoutBox = 1,
            Sound = 2
        }
    }

    namespace MaskWorking
    {
        public class MaskWork
        {
            public static RedsMask DrawRectangleOnMask(DetectedObject detectedObject, RectangleSettings rectangleSettings, Point shift = new Point())
            {
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

                result.Palette[1] = rectangleSettings.BorderColor.DisplayColor;

                return result;
            }

            public static RedsMask DrawFillingOnMask(TextBox textBox, Color[,] parentImage, TextBoxFillingSettings textBoxFillingSettings)
            {
                Color[,] imagePartOcupiedByMyYoloItem = Genral.GetImagePartOccupiedByDetectedObject(parentImage, textBox.DetectedObject);
                byte[,] textBoxPixels = TextBoxes.GetTextBoxPixels(imagePartOcupiedByMyYoloItem, textBoxFillingSettings);
                textBoxPixels = Genral.FillGapsInsideGrid(textBoxPixels);

                RedsMask result = new RedsMask(textBoxPixels);
                result.ShiftRelativelyToBitmap = new Point(textBox.DetectedObject.Rectangle.X, textBox.DetectedObject.Rectangle.Y);
                result.Palette[1] = textBoxFillingSettings.FillingColor.DisplayColor;

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

                graphics.Clear(textSettings.BackgroundColor.DisplayColor);

                int shift = 0;

                if (textSettings.ClassNameFontSettings.Draw)
                {
                    graphics.DrawString(type, textSettings.ClassNameFontSettings.Font, new SolidBrush(textSettings.ClassNameFontSettings.FontColor.DisplayColor), new PointF(shift, 0));
                    shift += typeWidth + 5;
                }

                if (textSettings.ConfidenceFontSettings.Draw)
                {
                    graphics.DrawString(confidence, textSettings.ConfidenceFontSettings.Font, new SolidBrush(textSettings.ConfidenceFontSettings.FontColor.DisplayColor), new PointF(shift, 0));
                    shift += typeWidth + 5;
                }

                for (int x = 0; x < result.Width; x++)
                {
                    for (int y = 0; y < result.Height; y++)
                    {
                        result.Mask[x, y] = result.GetOrSetAndGetColorId(tempBitmap.GetPixel(x, y));
                    }
                }

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

                mask.IsDrawed = true;
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

                mask.IsDrawed = false;
            }
        }
    }
}
