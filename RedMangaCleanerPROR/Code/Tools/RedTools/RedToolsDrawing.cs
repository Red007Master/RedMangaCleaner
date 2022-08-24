using RedsCleaningProjects.Core;
using System;
using System.Drawing;

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

                            bool coordinatesIsInBounds = targetDirectBitmap.Width > targetX && targetDirectBitmap.Height > targetY;

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

