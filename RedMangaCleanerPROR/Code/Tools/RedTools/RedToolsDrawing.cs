using RedsCleaningProjects.EditableObjects;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RedsTools
{
    namespace Drawing
    {
        public static class Drawing
        {
            internal static RedsMask DrawRectangleOnMask(RedsMask mask, RectangleSettings rectangleSettings, Point shift = new Point())
            {
                for (int i = 0; i < rectangleSettings.Rectangle.Width; i++)
                {
                    for (int j = 0; j < rectangleSettings.RectangleBorderThickness; j++)
                    {
                        mask.Mask[i, j + shift.Y] = 1;
                    }

                    for (int j = rectangleSettings.RectangleBorderThickness; j > 0; j--)
                    {
                        mask.Mask[i, rectangleSettings.Rectangle.Height - j + shift.Y] = 1;
                    }
                }

                for (int i = shift.Y; i < rectangleSettings.Rectangle.Height + shift.Y; i++)
                {
                    for (int j = 0; j < rectangleSettings.RectangleBorderThickness; j++)
                    {
                        mask.Mask[j, i] = 1;
                    }

                    for (int j = rectangleSettings.RectangleBorderThickness; j > 0; j--)
                    {
                        mask.Mask[rectangleSettings.Rectangle.Width - j, i] = 1;
                    }
                }

                mask.Palette[1] = rectangleSettings.BorderColor;
                return mask;
            }

            public static RedsMask DrawTextOnMask(DetectedObject detectedObject, TextSettings textSettings)
            {
                Color fillColor1 = Color.FromArgb(255, 0, 0);
                Color fillColor2 = Color.FromArgb(0, 255, 0);

                int maxFontSize = textSettings.ClassNameFontSettings.FontSize;
                if (maxFontSize < textSettings.ConfidenceFontSettings.FontSize)
                    maxFontSize = textSettings.ConfidenceFontSettings.FontSize;

                string type = detectedObject.Type;
                string confidence = Convert.ToString(Math.Round(detectedObject.Confidence, 2));

                int typeWidth = TextRenderer.MeasureText(type, textSettings.ClassNameFontSettings.Font).Width;
                int confWidth = TextRenderer.MeasureText(confidence, textSettings.ConfidenceFontSettings.Font).Width;

                int totalWidth = 0;

                if (textSettings.ClassNameFontSettings.Draw)
                    totalWidth += typeWidth;

                if (textSettings.ConfidenceFontSettings.Draw)
                    totalWidth += confWidth;

                int width = totalWidth;
                int height = maxFontSize + 5;

                RedsMask result = new RedsMask(width, height);
                result.ShiftRelativelyToBitmap = new Point(detectedObject.Rectangle.X, detectedObject.Rectangle.Y - maxFontSize - 5);

                Bitmap tempBitmap = new Bitmap(result.Width, result.Height);
                Graphics graphics = Graphics.FromImage(tempBitmap);

                int shift = 0;

                if (textSettings.ClassNameFontSettings.Draw)
                {
                    graphics.DrawString(type, textSettings.ClassNameFontSettings.Font, new SolidBrush(fillColor1), new PointF(shift, 0));
                    shift += typeWidth + 5;
                }

                if (textSettings.ConfidenceFontSettings.Draw)
                {
                    graphics.DrawString(confidence, textSettings.ConfidenceFontSettings.Font, new SolidBrush(fillColor2), new PointF(shift, 0));
                }

                for (int x = 0; x < result.Width; x++)
                {
                    for (int y = 0; y < result.Height; y++)
                    {
                        Color getColor = tempBitmap.GetPixel(x, y);
                        byte R = getColor.R;
                        byte G = getColor.G;
                        byte B = getColor.B;

                        if (R == 255)
                        {
                            result.Mask[x, y] = 1;
                        }
                        else if (G == 255)
                        {
                            result.Mask[x, y] = 2;
                        }
                        else
                        {
                            result.Mask[x, y] = 3;
                        }
                    }
                }

                result.Palette[1] = textSettings.ClassNameFontSettings.FontColor;
                result.Palette[2] = textSettings.ConfidenceFontSettings.FontColor;
                result.Palette[3] = textSettings.BackgroundColor;

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

