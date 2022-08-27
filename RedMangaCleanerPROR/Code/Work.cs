using Newtonsoft.Json;
using RedsCleaningProject.RedImages;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using Yolov5Net.Scorer;
using Yolov5Net.Scorer.Models;

class Work
{
    internal static void MainVoid()
    {
        List<BasicImageData> ImageDataList = new List<BasicImageData>();


        using (TimeLogger tl = new TimeLogger($"Copying images From=[{P.StartArguments.InputPath}] To=[{P.CleaningProjectDirs.SourceImages}]", LogLevel.Information, P.Logger, 1))
        {
            CopyImages(P.StartArguments.InputPath, P.CleaningProjectDirs.SourceImages);
        }

        using (TimeLogger tl = new TimeLogger("Grayscale.ConvertRGBImagesToGrayscaleMultithreading", LogLevel.Information, P.Logger, 1))
        {
            Grayscale.ConvertRGBImagesToGrayscaleMultithreading(P.CleaningProjectDirs.SourceImages);
        }

        using (TimeLogger tl = new TimeLogger("ImageRecog.DetectObjectsOnAllImagesInDir", LogLevel.Information, P.Logger, 1))
        {
            ImageDataList = ObjectRecognition.DetectObjectsOnAllImagesInDir(P.CleaningProjectDirs.BlackAndWhiteImages);
        }

        using (TimeLogger tl = new TimeLogger("JsonConvert.SerializeObject(ImageDataList)", LogLevel.Information, P.Logger, 1))
        {
            P.ProjectProcessingStatus.Set(Operation.IsJsonSerializeing);
            P.ProjectProcessingStatus.Save();

            string ImageDataListAsString = JsonConvert.SerializeObject(ImageDataList); //TODO Check JsonConvert.SerializeObject(ImageDataList, Formatting.);
            File.WriteAllText(P.CleaningProjectDirs.ObjectsData, ImageDataListAsString);
        }

        P.CleaningProjectInfo.IsPRORFinished = true;
        P.CleaningProjectInfo.Save(P.CleaningProjectDirs.CleaningProjectInfo);

        P.ProjectProcessingStatus.FinishPROR();
    }

    public static class Grayscale
    {
        private static void ConvertRGBImageToGrayscale(string inputImagePath, string outputPath)
        {
            Bitmap inputImageAsBitmap = new Bitmap(inputImagePath);

            int width = inputImageAsBitmap.Width;
            int height = inputImageAsBitmap.Height;

            Bitmap resulImage = new Bitmap(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color currentPixel = inputImageAsBitmap.GetPixel(x, y);
                    int buffer = currentPixel.R + currentPixel.G + currentPixel.B;
                    buffer = buffer / 3;

                    resulImage.SetPixel(x, y, Color.FromArgb(currentPixel.A, buffer, buffer, buffer));
                }
            }

            resulImage.Save(outputPath + @"\" + Path.GetFileNameWithoutExtension(inputImagePath) + ".png");
        }


        private static void ConvertRGBImageToGrayscaleThread(object inputObj)
        {
            string inputImagePath = (string)inputObj;

            try
            {
                ConvertRGBImageToGrayscale(inputImagePath, P.CleaningProjectDirs.BlackAndWhiteImages);
                P.Floats.Threads.ThreadsEndetSuccessfully++;

                P.ProjectProcessingStatus.Set(P.Floats.Threads.ThreadsEndetSuccessfully);
                P.ProjectProcessingStatus.Save();
            }
            catch (Exception ex)
            {
                P.Floats.Threads.ThreadsEndetWithError++;
                P.Logger.Log($"Thread for [{inputImagePath}] catch exeption ex=[{ex}]", LogLevel.Error);
            }

            P.Floats.Threads.ThreadCounter++;
        }
        public static void ConvertRGBImagesToGrayscaleMultithreading(string inputPath)
        {
            Queue<string> filesToProcess = new Queue<string>();
            int startedThreds = 0;

            {
                P.Logger.Log($"Grayscale.ConvertRGBImagesToGrayscaleMultithreading: GetFiles from dir, Dir=[{inputPath}]-Try", LogLevel.Information, 2);
                string[] buffer = Directory.GetFiles(inputPath);
                P.Logger.Log($"GetFiles from dir-Success, file count=[{buffer.Length}]", LogLevel.Information, 3);
                for (int i = 0; i < buffer.Length; i++)
                    filesToProcess.Enqueue(buffer[i]);

                P.ProjectProcessingStatus.Set(Operation.IsConvertingToGrayscale, buffer.Length, 0);
                P.ProjectProcessingStatus.Save();
            }

            P.Floats.Threads.ThreadsEndetWithError = 0;
            P.Floats.Threads.ThreadsEndetSuccessfully = 0;
            P.Floats.Threads.ThreadCounter = P.Settings.SettingsList.ImagesToBlackAndWhiteThreadsCount;

            P.Logger.Log($"Multithreading converting of RGB images to BaW-Try:", LogLevel.Information, 4);
            while (filesToProcess.Count > 0)
            {
                if (P.Floats.Threads.ThreadCounter > 0)
                {
                    P.Logger.Log($"Multithreading: ThreadCounter=[{P.Floats.Threads.ThreadCounter}], Starting thread for File=[{filesToProcess.Peek()}]-Try", LogLevel.Information, 5);

                    {
                        Thread thread = new Thread(new ParameterizedThreadStart(Work.Grayscale.ConvertRGBImageToGrayscaleThread));
                        thread.Start(filesToProcess.Peek());
                        startedThreds++;
                    }

                    filesToProcess.Dequeue();
                    P.Floats.Threads.ThreadCounter--;

                    P.Logger.Log($"Starting thread-Success", LogLevel.Information, 6);
                }
                Thread.Sleep(10);
            }

            P.Logger.Log($"Multithreading converting of RGB images to BaW: All threads is Started", LogLevel.Information, 7);

            while (true)
            {
                if (P.Floats.Threads.ThreadsEndetSuccessfully + P.Floats.Threads.ThreadsEndetWithError == startedThreds)
                    break;

                Thread.Sleep(10);
            }

            if (P.Floats.Threads.ThreadsEndetWithError > 0)
            {
                P.Logger.Log($"All threads is Completed, Count of Errors=[{P.Floats.Threads.ThreadsEndetWithError}]", LogLevel.Error, 8);
            }
            else
            {
                P.Logger.Log($"All threads is Completed-Success", LogLevel.Information, 8);
            }
        }
    }

    public static class ObjectRecognition
    {
        public static List<BasicImageData> DetectObjectsOnAllImagesInDir(string inputPath)
        {
            List<BasicImageData> result = new List<BasicImageData>();

            P.Logger.Log($"DetectObjectsOnAllImagesInDir: GetFiles from dir, Dir=[{inputPath}]-Try", LogLevel.Information, 1);
            string[] imagesToProcess = Directory.GetFiles(inputPath);
            P.Logger.Log($"GetFiles from dir-Success, file count=[{imagesToProcess.Length}]", LogLevel.Information, 2);

            P.ProjectProcessingStatus.Set(Operation.IsDetectingObjects, imagesToProcess.Length, 0);
            P.ProjectProcessingStatus.Save();

            string config = File.ReadAllText(P.CurrentYoloConfiguration.ModelConfigPath);
            RedsYoloConfig redsYoloConfig = JsonConvert.DeserializeObject<RedsYoloConfig>(config);
            YoloScorer<RedsGeneralModel> scorer = new YoloScorer<RedsGeneralModel>(P.CurrentYoloConfiguration.WeightsPath, redsYoloConfig);

            Image image = null;
            for (int i = 0; i < imagesToProcess.Length; i++)
            {
                image = Image.FromFile(imagesToProcess[i]);

                P.Logger.Log($"DetectObjectsOnAllImagesInDir: Detect Objects on image=[{imagesToProcess[i]}]-Try", LogLevel.Information, 1);
                List<YoloPrediction> predictions = scorer.Predict(image);
                P.Logger.Log($"Detect Objects on image-Success", LogLevel.Information, 2);

                result.Add(new BasicImageData(imagesToProcess[i], image.Width, image.Height, P.CleaningProjectInfo.ConductTextBoxFillingOnBlackAndWhiteVariants, DetectedObject.ConvertYPLToDetectedObjectList(predictions)));

                P.ProjectProcessingStatus.Set(i);
                P.ProjectProcessingStatus.Save();
            }

            image.Dispose();
            scorer.Dispose();
            GC.Collect();

            return result;
        }
    }

    public static void CopyImages(string inputFrom, string inputTo)
    {
        string[] usableImageExtensions = { ".png", ".jpg", ".bmp", ".jpe" };
        string[] files = Directory.GetFiles(inputFrom);

        P.ProjectProcessingStatus.Set(Status.IsRunning, Operation.IsCopyingImages, files.Length, 0);
        P.ProjectProcessingStatus.Save();

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
                P.Logger.Log($"File=[{files[i]}] pass check as usableImage and copied as=[{copyAs}]", LogLevel.Information, 2);
            }
            else
            {
                P.Logger.Log($"File=[{files[i]}] don't pass usableImage check FileExtension=[{Path.GetExtension(files[i])}]", LogLevel.Error, 2);
            }

            P.ProjectProcessingStatus.Set(i);
            P.ProjectProcessingStatus.Save();
        }
    }
}