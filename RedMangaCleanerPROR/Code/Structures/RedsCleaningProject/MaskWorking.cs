using RedsCleaningProject.EditableObjects;
using RedsCleaningProject.Settings;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedsCleaningProject
{
    namespace MaskWorking
    {
        public internal class MaskWork
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
        }
    }
}
