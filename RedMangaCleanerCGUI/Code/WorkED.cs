using Newtonsoft.Json;
using RedsCleaningProjects.RedImages;
using RedsTools.Images;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;

class Work
{
    public static List<RedImageFull> RedImageFulls { get; set; }

    public static void MainVoid()
    {
        RedImageFulls = new List<RedImageFull>();
        List<RedImageCore> redImageCores = new List<RedImageCore>();
        List<ImageData> ImageDatas = new List<ImageData>();

        using (TimeLogger tl = new TimeLogger($"ImageData: File.ReadAllText = [{P.CleaningProjectDirs.ObjectsData}] and JsonConvert.DeserializeObject", LogLevel.Information, P.Logger, 1))
        {
            string serialized = File.ReadAllText(P.CleaningProjectDirs.ObjectsData);
            ImageDatas = JsonConvert.DeserializeObject<List<ImageData>>(serialized);
        }

        using (TimeLogger tl = new TimeLogger("RedImageCore: Formating.GetRedImageCoreFromImgDataObjList", LogLevel.Information, P.Logger, 1))
        {
            for (int i = 0; i < ImageDatas.Count; i++)
            {
                redImageCores.Add(new RedImageCore(ImageDatas[i]));
            }
        }

        using (TimeLogger tl = new TimeLogger("Formating.PrecompileRedImageFullsFromRedImageCoresMultithreading and sort", LogLevel.Information, P.Logger, 1))
        {
            RedImageFulls = Formating.PrecompileRedImageFullsFromRedImageCoresMultithreading(redImageCores);
            RedImageFulls.Sort();
        }

        PV.UserControl.ImagesProcessingView.ImagesProcessingProgressInfo.IsFinished = true;
    }

    public static class Cleaning
    {
        public static DirectBitmap CleanRGBImage(ImageData inputImageData)
        {
            Color[,] inputBaWImageAsColorArray = Images.RGB.BitmapToColorArray(new Bitmap(inputImageData.ImageFilePath));
            List<MyYoloItem> textBoxesAsMyYoloItems = inputImageData.MyYoloItemsList;
            List<byte[,]> textBoxesAsByteArrays = new List<byte[,]>();

            for (int i = 0; i < textBoxesAsMyYoloItems.Count; i++)
            {
                Color[,] imagePartOcupiedByMyYoloItem = RGB.GetImagePartOccupiedByMyYoloItem(inputBaWImageAsColorArray, textBoxesAsMyYoloItems[i]);
                byte[,] textBoxPixels = RGB.GetTextBoxPixels(imagePartOcupiedByMyYoloItem, 250); //TODO1 out
                textBoxPixels = FillGapsInsideGrid(textBoxPixels);

                textBoxesAsByteArrays.Add(textBoxPixels);
            }

            Color[,] inputBaWImageAsColorArrayFinall = RGB.FillTextboxesInMainImage(inputBaWImageAsColorArray, textBoxesAsByteArrays, textBoxesAsMyYoloItems, Color.Red);
            DirectBitmap result = Images.RGB.ColorArrayToDirectBitmap(inputBaWImageAsColorArrayFinall);

            return result;
        }

        public static DirectBitmap CleanBaWImage(ImageData inputImageData)
        {
            byte[,] inputBaWImageAsByteArray = Images.BlackAndWhite.ByteArrayFromBitmap(new Bitmap(inputImageData.ImageFilePath));
            List<MyYoloItem> textBoxesAsMyYoloItems = inputImageData.MyYoloItemsList;
            List<byte[,]> textBoxesAsByteArrays = new List<byte[,]>();

            for (int i = 0; i < textBoxesAsMyYoloItems.Count; i++)
            {
                byte[,] imagePartOcupiedByMyYoloItem = BlackAndWhite.GetImagePartOccupiedByMyYoloItem(inputBaWImageAsByteArray, textBoxesAsMyYoloItems[i]);
                byte[,] textBoxPixels = BlackAndWhite.GetTextBoxPixels(imagePartOcupiedByMyYoloItem, 250);
                textBoxPixels = FillGapsInsideGrid(textBoxPixels);

                textBoxesAsByteArrays.Add(textBoxPixels);
            }

            byte[,] inputBaWImageAsByteArrayFinall = BlackAndWhite.FillTextboxesInMainImage(inputBaWImageAsByteArray, textBoxesAsByteArrays, textBoxesAsMyYoloItems, 0);
            DirectBitmap result = Images.BlackAndWhite.ByteArrayToDirectBitmap(inputBaWImageAsByteArrayFinall);

            return result;
        }

        private static class BlackAndWhite
        {
            public static byte[,] GetImagePartOccupiedByMyYoloItem(byte[,] inputImageAsByteArray, MyYoloItem inputYoloItem)
            {
                int counterX = 0, counterY = 0;
                //byte[,] textBoxAsByteArray = new Bitmap(inputYoloItem.Width, inputYoloItem.Height);
                byte[,] textBoxAsByteArray = new byte[inputYoloItem.Width, inputYoloItem.Height];

                int endPointOfX = inputYoloItem.X + inputYoloItem.Width;
                if (endPointOfX > inputImageAsByteArray.GetLength(0))
                    endPointOfX = inputImageAsByteArray.GetLength(0);

                int endPointOfY = inputYoloItem.Y + inputYoloItem.Height;
                if (endPointOfY > inputImageAsByteArray.GetLength(1))
                    endPointOfY = inputImageAsByteArray.GetLength(1);

                for (int x = inputYoloItem.X; x < endPointOfX; x++)
                {
                    for (int y = inputYoloItem.Y; y < endPointOfY; y++)
                    {
                        textBoxAsByteArray[counterX, counterY] = inputImageAsByteArray[x, y];
                        //image.SetPixel(counterX, counterY, inputImageAsByteArray[x, y]);
                        counterY++;
                    }
                    counterX++;
                    counterY = 0;
                }

                return textBoxAsByteArray;
            }
            public static byte[,] GetTextBoxPixels(byte[,] inputImageAsByteArray, int inputGrayScaleLimit)
            {
                Queue<Point> bufferQueue = new Queue<Point>();
                int width = inputImageAsByteArray.GetLength(0);
                int height = inputImageAsByteArray.GetLength(1);
                byte[,] result = new byte[width, height];
                int counter = 0;

                int pgm = inputGrayScaleLimit; //pixelGrayScaleLimit
                pgm = 245; //dev
                int increaseOn = 10;

                Point center = new Point(width / 2, height / 2);
                bufferQueue.Enqueue(center);

                for (int i = 0; i < increaseOn; i++)
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

                    if (bufferPoint.X + 1 < width && bufferPoint.X - 1 > 0 && bufferPoint.Y + 1 < height && bufferPoint.Y - 1 > 0)
                    {
                        if (inputImageAsByteArray[bufferPoint.X + 1, bufferPoint.Y] > pgm)
                        {
                            if (result[bufferPoint.X + 1, bufferPoint.Y] == 0)
                            {
                                result[bufferPoint.X + 1, bufferPoint.Y] = 1;
                                bufferQueue.Enqueue(new Point(bufferPoint.X + 1, bufferPoint.Y));
                            }
                        }

                        if (inputImageAsByteArray[bufferPoint.X - 1, bufferPoint.Y] > pgm)
                        {
                            if (result[bufferPoint.X - 1, bufferPoint.Y] == 0)
                            {
                                result[bufferPoint.X - 1, bufferPoint.Y] = 1;
                                bufferQueue.Enqueue(new Point(bufferPoint.X - 1, bufferPoint.Y));
                            }
                        }

                        if (inputImageAsByteArray[bufferPoint.X, bufferPoint.Y + 1] > pgm)
                        {
                            if (result[bufferPoint.X, bufferPoint.Y + 1] == 0)
                            {
                                result[bufferPoint.X, bufferPoint.Y + 1] = 1;
                                bufferQueue.Enqueue(new Point(bufferPoint.X, bufferPoint.Y + 1));
                            }
                        }

                        if (inputImageAsByteArray[bufferPoint.X, bufferPoint.Y - 1] > pgm)
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
            public static byte[,] FillTextboxesInMainImage(byte[,] inputMainImage, List<byte[,]> inputTextBoxesAsByteArray, List<MyYoloItem> inputTextboxesAsYoloItems, byte fillColorValue)
            {
                for (int i = 0; i < inputTextBoxesAsByteArray.Count; i++)
                {
                    int width = inputTextboxesAsYoloItems[i].Width;
                    int height = inputTextboxesAsYoloItems[i].Height;

                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            if (inputTextBoxesAsByteArray[i][x, y] == 1)
                            {
                                inputMainImage[x + inputTextboxesAsYoloItems[i].X, y + inputTextboxesAsYoloItems[i].Y] = fillColorValue;
                            }
                        }
                    }
                }

                return inputMainImage;
            }
        }
        private static class RGB
        {
            public static Color[,] GetImagePartOccupiedByMyYoloItem(Color[,] inputImageAsColorArray, MyYoloItem inputYoloItem)
            {
                int counterX = 0, counterY = 0;
                Color[,] textBoxAsColorArray = new Color[inputYoloItem.Width, inputYoloItem.Height];

                int endPointOfX = inputYoloItem.X + inputYoloItem.Width;
                if (endPointOfX > inputImageAsColorArray.GetLength(0))
                    endPointOfX = inputImageAsColorArray.GetLength(0);

                int endPointOfY = inputYoloItem.Y + inputYoloItem.Height;
                if (endPointOfY > inputImageAsColorArray.GetLength(1))
                    endPointOfY = inputImageAsColorArray.GetLength(1);

                for (int x = inputYoloItem.X; x < endPointOfX; x++)
                {
                    for (int y = inputYoloItem.Y; y < endPointOfY; y++)
                    {
                        textBoxAsColorArray[counterX, counterY] = inputImageAsColorArray[x, y];
                        counterY++;
                    }
                    counterX++;
                    counterY = 0;
                }

                return textBoxAsColorArray;
            }
            public static byte[,] GetTextBoxPixels(Color[,] inputImageAsColorArray, int inputGrayScaleLimit) //TODO1 in Kavo?
            {
                Queue<Point> bufferQueue = new Queue<Point>();
                int width = inputImageAsColorArray.GetLength(0);
                int height = inputImageAsColorArray.GetLength(1);
                byte[,] result = new byte[width, height];
                int counter = 0;

                int pgm = inputGrayScaleLimit; //pixelGrayScaleLimit
                pgm = 245; //dev
                int increaseOn = 10;

                Point center = new Point(width / 2, height / 2);
                bufferQueue.Enqueue(center);

                for (int i = 0; i < increaseOn; i++)
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
            public static Color[,] FillTextboxesInMainImage(Color[,] inputMainImage, List<byte[,]> inputTextBoxesAsByteArrays, List<MyYoloItem> inputTextboxesAsYoloItems, Color fillColorValue)
            {
                for (int i = 0; i < inputTextBoxesAsByteArrays.Count; i++)
                {
                    int width = inputTextboxesAsYoloItems[i].Width;
                    int height = inputTextboxesAsYoloItems[i].Height;

                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            if (inputTextBoxesAsByteArrays[i][x, y] == 1)
                            {
                                inputMainImage[x + inputTextboxesAsYoloItems[i].X, y + inputTextboxesAsYoloItems[i].Y] = fillColorValue;
                            }
                        }
                    }
                }

                return inputMainImage;
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
    }

    public static class Formating
    {
        private static int CompiledImagesCount { get; set; }

        private static void PrecompileRedImageFullFromRedImageCoreThread(object inputObj)
        {
            RedImageCore inputRedImageCore = (RedImageCore)inputObj;

            #region TryDebLog

            try
            {
                P.Logger.Log($"Precompile [{inputRedImageCore.ImageFileName}] - Try", LogLevel.Information, 1);
            }
            catch (Exception)
            { }

            #endregion

            try
            {
                RedImageFullsResultList.Add(new RedImageFull(inputRedImageCore));
                P.Floats.Threads.ThreadsEndetSuccessfully++;
            }
            catch (Exception ex)
            {
                P.Floats.Threads.ThreadsEndetWithError++;

                P.Logger.Log($"ERROR:Thread for [{inputRedImageCore.ImageFileName}] catch exeption ex=[{ex}]", LogLevel.Error, 2);
            }

            #region TryDebLog

            try
            {
                P.Logger.Log($"Precompile [{inputRedImageCore.ImageFileName}] - Success", LogLevel.Information, 3);
            }
            catch (Exception)
            { }

            #endregion

            CompiledImagesCount++;

            PV.UserControl.ImagesProcessingView.ImagesProcessingProgressInfo.ProcessedImages = CompiledImagesCount;

            P.Floats.Threads.ThreadCounter++;
        }
        public static List<RedImageFull> PrecompileRedImageFullsFromRedImageCoresMultithreading(List<RedImageCore> inputRedImageCores)
        {
            CompiledImagesCount = 0;
            RedImageFullsResultList = new List<RedImageFull>();

            Queue<RedImageCore> redImageCoresToProcess = new Queue<RedImageCore>();
            int startedThreds = 0;

            for (int i = 0; i < inputRedImageCores.Count; i++)
                redImageCoresToProcess.Enqueue(inputRedImageCores[i]);

            P.Floats.Threads.ClearNew(P.Settings.SettingsList.PrecompileRedImageFullsThreadsCount);

            P.Logger.Log($"Formating.PrecompileRedImageFullsFromRedImageCoresMultithreading: Try", LogLevel.Information, 2);

            PV.UserControl.ImagesProcessingView.ImagesProcessingProgressInfo.Set(Operation.IsPrecompiling, inputRedImageCores.Count);

            while (redImageCoresToProcess.Count > 0)
            {
                if (P.Floats.Threads.ThreadCounter > 0)
                {
                    P.Logger.Log($"Formating.PrecompileRedImageFullsFromRedImageCoresMultithreading: Multithreading: ThreadCounter=[{P.Floats.Threads.ThreadCounter}], Starting thread for File=[{redImageCoresToProcess.Peek().ImageFileName}]-Try", LogLevel.Information, 3);

                    Thread thread = new Thread(new ParameterizedThreadStart(Formating.PrecompileRedImageFullFromRedImageCoreThread));
                    thread.Start(redImageCoresToProcess.Peek());
                    startedThreds++;

                    redImageCoresToProcess.Dequeue();
                    P.Floats.Threads.ThreadCounter--;

                    P.Logger.Log($"Formating.PrecompileRedImageFullsFromRedImageCoresMultithreading: Multithreading: Starting thread-Success", LogLevel.Information, 4);
                }
                Thread.Sleep(10);
            }

            while (!(P.Floats.Threads.ThreadsEndetSuccessfully + P.Floats.Threads.ThreadsEndetWithError == startedThreds))
                Thread.Sleep(10);

            if (P.Floats.Threads.ThreadsEndetWithError > 0)
            {
                P.Logger.Log($"Formating.PrecompileRedImageFullsFromRedImageCoresMultithreading: All threads is Completed, Count of Errors=[{P.Floats.Threads.ThreadsEndetWithError}]", LogLevel.Error, 5);
            }
            else
            {
                P.Logger.Log($"Formating.PrecompileRedImageFullsFromRedImageCoresMultithreading: All threads is Completed-Success", LogLevel.Information, 5);
            }

            return RedImageFullsResultList;
        }

        private static List<RedImageFull> RedImageFullsResultList { get; set; }
    }

    public static void CopyImages(string inputFrom, string inputTo)
    {
        string[] usableImageExtensions = { ".png", ".jpg", ".bmp", ".jpe" };
        string[] files = Directory.GetFiles(inputFrom);

        for (int i = 0; i < files.Length; i++)
        {
            bool filePassCheck = false;
            string copyAs = "";
            for (int j = 0; j < usableImageExtensions.Length; j++)
            {
                if (Path.GetExtension(files[i]).ToLower() == usableImageExtensions[j])
                {
                    filePassCheck = true;

                    copyAs = inputTo + @"\" + Path.GetFileName(files[i]);

                    File.Copy(files[i], copyAs);

                    break;
                }
            }

            if (filePassCheck == true)
            {
                P.Logger.Log($"File=[{files[i]}] pass check as usableImage and copied as=[{copyAs}]", LogLevel.Information);
            }
            else
            {
                P.Logger.Log($"File=[{files[i]}] don't pass usableImage check FileExtension=[{Path.GetExtension(files[i])}]", LogLevel.Warning);
            }
        }
    }
}