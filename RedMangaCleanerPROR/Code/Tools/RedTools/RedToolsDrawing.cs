using System;
using System.Drawing;

namespace RedsTools
{
    namespace Drawing
    {
        public static class Drawing
        {
            public static void DrawRectangleOnDirectBitmap(DirectBitmap iDirectBitmap, RectangleDrawOptions iRectangleDrawOptions)
            {
                if (iRectangleDrawOptions.DrawPositioningOptions == DrawPositioningOptions.ByTwoPoints)
                {
                    DrawRecByTwoPoints(iDirectBitmap, iRectangleDrawOptions);
                }
                else if (iRectangleDrawOptions.DrawPositioningOptions == DrawPositioningOptions.ByCenterAndSize)
                {
                    DrawRecByCenterAndSize(iDirectBitmap, iRectangleDrawOptions);
                }
                else if (iRectangleDrawOptions.DrawPositioningOptions == DrawPositioningOptions.ByWidthAndHeightThrueXandY)
                {
                    DrawRecByWidthAndHeightThrueXandY(iDirectBitmap, iRectangleDrawOptions);
                }
            }

            private static void DrawRecByTwoPoints(DirectBitmap iDirectBitmap, RectangleDrawOptions iRectangleDrawOptions)
            {
                throw new NotImplementedException();
            }
            private static void DrawRecByCenterAndSize(DirectBitmap iDirectBitmap, RectangleDrawOptions iRectangleDrawOptions)
            {
                throw new NotImplementedException();
            }
            private static void DrawRecByWidthAndHeightThrueXandY(DirectBitmap iDirectBitmap, RectangleDrawOptions iRectangleDrawOptions)
            {
                int imageWidth = iDirectBitmap.Width;
                int imageHeight = iDirectBitmap.Height;
                int borderThickness = iRectangleDrawOptions.Thickness;

                bool isWidthInBounds = iRectangleDrawOptions.FirstPoint.X + iRectangleDrawOptions.Width + borderThickness < imageWidth;
                bool isHeightInBounds = iRectangleDrawOptions.FirstPoint.Y + iRectangleDrawOptions.Height + borderThickness < imageHeight;

                for (int i = 0; i < iRectangleDrawOptions.Width; i++)
                {
                    RedsPoint point1 = new RedsPoint(iRectangleDrawOptions.FirstPoint.X + i, iRectangleDrawOptions.FirstPoint.Y);
                    RedsPoint point2 = new RedsPoint(iRectangleDrawOptions.FirstPoint.X + i, iRectangleDrawOptions.FirstPoint.Y + iRectangleDrawOptions.Height - 1);

                    SetPixels(point1, iDirectBitmap, iRectangleDrawOptions.Color, borderThickness, Orientation.Vertical);
                    SetPixels(point2, iDirectBitmap, iRectangleDrawOptions.Color, borderThickness, Orientation.Vertical);
                }

                for (int i = 0; i < iRectangleDrawOptions.Height; i++)
                {
                    RedsPoint point1 = new RedsPoint(iRectangleDrawOptions.FirstPoint.X, iRectangleDrawOptions.FirstPoint.Y + i);
                    RedsPoint point2 = new RedsPoint(iRectangleDrawOptions.FirstPoint.X + iRectangleDrawOptions.Width - 1, iRectangleDrawOptions.FirstPoint.Y + i);

                    SetPixels(point1, iDirectBitmap, iRectangleDrawOptions.Color, borderThickness, Orientation.Horizontal);
                    SetPixels(point2, iDirectBitmap, iRectangleDrawOptions.Color, borderThickness, Orientation.Horizontal);
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
            public DrawPositioningOptions DrawPositioningOptions { get; set; }

            public RedsPoint FirstPoint { get; set; }
            public RedsPoint LastBottomPoint { get; set; }
            public RedsPoint CenterPoint { get; set; }

            public int Width { get; set; }
            public int Height { get; set; }

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

        public enum DrawPositioningOptions
        {
            ByTwoPoints = 0,
            ByCenterAndSize = 1,
            ByWidthAndHeightThrueXandY = 2,
        }
    }
}

