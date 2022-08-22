using System;
using System.Drawing;

namespace RedsTools
{
    namespace Drawing
    {
        public static class Drawing
        {
            public static void UnDrawRectangleOnDirectBitmap(DirectBitmap iDirectBitmap, RectangleDrawOptions iRectangleDrawOptions, Color[,] iImageAsColorArray)
            {
                int imageWidth = iDirectBitmap.Width;
                int imageHeight = iDirectBitmap.Height;
                int borderThickness = iRectangleDrawOptions.Thickness;

                bool isWidthInBounds = iRectangleDrawOptions.Rectangle.X + iRectangleDrawOptions.Rectangle.Width + borderThickness < imageWidth;
                bool isHeightInBounds = iRectangleDrawOptions.Rectangle.Y + iRectangleDrawOptions.Rectangle.Height + borderThickness < imageHeight;

                for (int i = 0; i < iRectangleDrawOptions.Rectangle.Width; i++)
                {
                    RedsPoint point1 = new RedsPoint(iRectangleDrawOptions.Rectangle.X + i, iRectangleDrawOptions.Rectangle.Y);
                    RedsPoint point2 = new RedsPoint(iRectangleDrawOptions.Rectangle.X + i, iRectangleDrawOptions.Rectangle.Y + iRectangleDrawOptions.Rectangle.Height - 1);

                    UnsetPixels(point1, iDirectBitmap, iImageAsColorArray, borderThickness, Orientation.Vertical);
                    UnsetPixels(point2, iDirectBitmap, iImageAsColorArray, borderThickness, Orientation.Vertical);
                }

                for (int i = 0; i < iRectangleDrawOptions.Rectangle.Height; i++)
                {
                    RedsPoint point1 = new RedsPoint(iRectangleDrawOptions.Rectangle.X, iRectangleDrawOptions.Rectangle.Y + i);
                    RedsPoint point2 = new RedsPoint(iRectangleDrawOptions.Rectangle.X + iRectangleDrawOptions.Rectangle.Width - 1, iRectangleDrawOptions.Rectangle.Y + i);

                    UnsetPixels(point1, iDirectBitmap, iImageAsColorArray, borderThickness, Orientation.Horizontal);
                    UnsetPixels(point2, iDirectBitmap, iImageAsColorArray, borderThickness, Orientation.Horizontal);
                }
            }
            //тоже переделат

            public static void DrawRectangleOnDirectBitmap(DirectBitmap iDirectBitmap, RectangleDrawOptions iRectangleDrawOptions)
            {
                int imageWidth = iDirectBitmap.Width;
                int imageHeight = iDirectBitmap.Height;
                int borderThickness = iRectangleDrawOptions.Thickness;

                bool isWidthInBounds = iRectangleDrawOptions.Rectangle.X + iRectangleDrawOptions.Rectangle.Width + borderThickness < imageWidth;
                bool isHeightInBounds = iRectangleDrawOptions.Rectangle.Y + iRectangleDrawOptions.Rectangle.Height + borderThickness < imageHeight;

                for (int i = 0; i < iRectangleDrawOptions.Rectangle.Width; i++)
                {
                    RedsPoint point1 = new RedsPoint(iRectangleDrawOptions.Rectangle.X + i, iRectangleDrawOptions.Rectangle.Y);
                    RedsPoint point2 = new RedsPoint(iRectangleDrawOptions.Rectangle.X + i, iRectangleDrawOptions.Rectangle.Y + iRectangleDrawOptions.Rectangle.Height - 1);

                    SetPixels(point1, iDirectBitmap, iRectangleDrawOptions.Color, borderThickness, Orientation.Vertical);
                    SetPixels(point2, iDirectBitmap, iRectangleDrawOptions.Color, borderThickness, Orientation.Vertical);
                }

                for (int i = 0; i < iRectangleDrawOptions.Rectangle.Height; i++)
                {
                    RedsPoint point1 = new RedsPoint(iRectangleDrawOptions.Rectangle.X, iRectangleDrawOptions.Rectangle.Y + i);
                    RedsPoint point2 = new RedsPoint(iRectangleDrawOptions.Rectangle.X + iRectangleDrawOptions.Rectangle.Width - 1, iRectangleDrawOptions.Rectangle.Y + i);

                    SetPixels(point1, iDirectBitmap, iRectangleDrawOptions.Color, borderThickness, Orientation.Horizontal);
                    SetPixels(point2, iDirectBitmap, iRectangleDrawOptions.Color, borderThickness, Orientation.Horizontal);
                }
            }
            //тоже переделат
                
            public static void OverlayRectangles()
            {
                //TODO
            }

            public static void FillByMask(DirectBitmap targetDirectBitmap, byte[,] mask, Color color, Point shift = new Point())
            {
                //int tWidth = targetDirectBitmap.Width;
                //int tHeight = targetDirectBitmap.Height;
                int mWidth = mask.GetLength(0);
                int mHeight = mask.GetLength(1);

                for (int x = 0; x < mWidth; x++)
                {
                    for (int y = 0; y < mHeight; y++)
                    {
                        if (mask[x, y] == 1)
                        {
                            targetDirectBitmap.SetPixel(x + shift.X, y + shift.Y, color);
                        }
                    }
                }
            }
            public static void FillByMaskFromPalette(DirectBitmap targetDirectBitmap, byte[,] mask, Color[] colors, Point shift = new Point())
            {
                //int tWidth = targetDirectBitmap.Width;
                //int tHeight = targetDirectBitmap.Height;
                int mWidth = mask.GetLength(0);
                int mHeight = mask.GetLength(1);

                for (int x = 0; x < mWidth; x++)
                {
                    for (int y = 0; y < mHeight; y++)
                    {
                        if (mask[x, y] != 0)
                        {
                            targetDirectBitmap.SetPixel(x + shift.X, y + shift.Y, colors[mask[x, y]]);
                        }
                    }
                }
            }

            private static void SetPixels(RedsPoint iPoint, DirectBitmap iDirectBitmap, Color iColor, int iLenght, Orientation iOrientation)
            {
                iLenght = CheckFixLenght(iLenght);

                int halfLenghtShift = iLenght / 2;

                int x, y;

                for (int i = 0; i < iLenght; i++)
                {
                    if (iOrientation == Orientation.Horizontal)
                    {
                        x = iPoint.X - halfLenghtShift + i;
                        y = iPoint.Y;

                        SaveSetPixel(iDirectBitmap, x, y, iColor);
                    }
                    else if (iOrientation == Orientation.Vertical)
                    {
                        x = iPoint.X;
                        y = iPoint.Y - halfLenghtShift + i;

                        SaveSetPixel(iDirectBitmap, x, y, iColor);
                    }
                }
            }
            private static void UnsetPixels(RedsPoint iPoint, DirectBitmap iDirectBitmap, Color[,] iImageAsColorArray, int iLenght, Orientation iOrientation)
            {
                iLenght = CheckFixLenght(iLenght);

                int halfLenghtShift = iLenght / 2;

                int x, y;

                for (int i = 0; i < iLenght; i++)
                {
                    if (iOrientation == Orientation.Horizontal)
                    {
                        x = iPoint.X - halfLenghtShift + i;
                        y = iPoint.Y;

                        SaveSetPixel(iDirectBitmap, x, y, iImageAsColorArray);
                    }
                    else if (iOrientation == Orientation.Vertical)
                    {
                        x = iPoint.X;
                        y = iPoint.Y - halfLenghtShift + i;

                        SaveSetPixel(iDirectBitmap, x, y, iImageAsColorArray);
                    }
                }
            }

            private static int CheckFixLenght(int iLenght)
            {
                if (!(iLenght % 2 == 0) && iLenght >= 2)
                {
                    iLenght--;
                }

                return iLenght;
            }

            private static void SaveSetPixel(DirectBitmap iDirectBitmap, int iX, int iY, Color iColor)
            {
                if (iX > 0 && iY > 0)
                    if (iDirectBitmap.Width > iX && iDirectBitmap.Height > iY)
                        iDirectBitmap.SetPixel(iX, iY, iColor);
            }
            private static void SaveSetPixel(DirectBitmap iDirectBitmap, int iX, int iY, Color[,] iImageAsColorArray)
            {
                if (iX > 0 && iY > 0)
                    if (iDirectBitmap.Width > iX && iDirectBitmap.Height > iY)
                        iDirectBitmap.SetPixel(iX, iY, iImageAsColorArray[iX, iY]);
            }

            public enum Orientation
            {
                Horizontal = 0,
                Vertical = 1,
            }
        }

        public class RectangleDrawOptions
        {
            public Rectangle Rectangle { get; set; }

            public Color Color { get; set; }
            public int Thickness { get; set; }
            public bool IsThicknessIsAuto { get; set; }
        }
        public class RedsPoint
        {
            public int X { get; set; }
            public int Y { get; set; }

            public RedsPoint(int x, int y)
            {
                X = x;
                Y = y;
            }
        }
    }
}

