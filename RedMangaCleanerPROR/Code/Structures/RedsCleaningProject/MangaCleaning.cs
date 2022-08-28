using RedsCleaningProject.Settings;
using System.Collections.Generic;
using System.Drawing;

namespace RedsCleaningProject
{
    namespace MangaCleaning
    {
        public static class Genral
        {
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
        }

        public static class TextBoxes
        {
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
}
