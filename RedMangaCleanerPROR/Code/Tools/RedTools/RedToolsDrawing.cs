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
                        mask.Mask[i, j] = 1;
                    }
                }

                for (int i = 0; i < rectangleSettings.Rectangle.Height; i++)
                {

                }

                mask.Palette[1] = rectangleSettings.BorderColor;
                return mask;
            }


            public static void OverlayRectangles()
            {
                //TODO
            }

            public static void FillByMask(DirectBitmap targetDirectBitmap, RedsMask mask, Point shift = new Point())
            {
                for (int x = 0; x < mask.Width; x++)
                {
                    for (int y = 0; y < mask.Height; y++)
                    {
                        if (mask.Mask[x, y] != 0)
                        {
                            targetDirectBitmap.SetPixel(x + shift.X, y + shift.Y, mask.Palette[mask.Mask[x, y]]);
                        }
                    }
                }
            }

            //public static void FillByMaskFromPalette(DirectBitmap targetDirectBitmap, byte[,] mask, Color[] colors, Point shift = new Point())
            //{
            //    //int tWidth = targetDirectBitmap.Width;
            //    //int tHeight = targetDirectBitmap.Height;
            //    int mWidth = mask.GetLength(0);
            //    int mHeight = mask.GetLength(1);

            //    for (int x = 0; x < mWidth; x++)
            //    {
            //        for (int y = 0; y < mHeight; y++)
            //        {
            //            if (mask[x, y] != 0)
            //            {
            //                targetDirectBitmap.SetPixel(x + shift.X, y + shift.Y, colors[mask[x, y]]);
            //            }
            //        }
            //    }
            //}
            public static void UnFillByMask(DirectBitmap targetDirectBitmap, RedsMask mask, Color[,] sourceImage, Point shift = new Point())
            {
                for (int x = 0; x < mask.Width; x++)
                {
                    for (int y = 0; y < mask.Height; y++)
                    {
                        if (mask.Mask[x, y] != 0)
                        {
                            targetDirectBitmap.SetPixel(x + shift.X, y + shift.Y, sourceImage[x + shift.X, y + shift.Y]);
                        }
                    }
                }
            }
        }
    }
}

