using Newtonsoft.Json;
using RedMangaCleanerPROR.Code.Structures;
using RedsCleaningProject.CleaningConfigs;
using RedsCleaningProject.MasksAndEditableObjects;
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
    public static void MainVoid()
    {
        List<BasicImageData> BasicImageDataList = new List<BasicImageData>();
        List<RedImageCleaningConfig> redImageCleaningConfigs = new List<RedImageCleaningConfig>();
        List<string> Filenames = new List<string>();


        using (TimeLogger tl = new TimeLogger($"Copying images From=[{P.StartArguments.InputPath}] To=[{P.CleaningProject.CleaningProjectDirs.SourceImages}]", LogLevel.Information, P.Logger, 1))
        {
            Filenames = CopyImages(P.StartArguments.InputPath, P.CleaningProject.CleaningProjectDirs.SourceImages);
        }

        using (TimeLogger tl = new TimeLogger("Grayscale.ConvertRGBImagesToGrayscaleMultithreading", LogLevel.Information, P.Logger, 1))
        {
            Grayscale.ConvertRGBImagesToGrayscaleMultithreading(Filenames, P.CleaningProject.CleaningProjectDirs.SourceImages);
        }

        using (TimeLogger tl = new TimeLogger("ImageRecog.DetectObjectsOnAllImagesInDir", LogLevel.Information, P.Logger, 1))
        {
            BasicImageDataList = ObjectRecognition.DetectObjectsOnImages(Filenames, P.CleaningProject.CleaningProjectDirs.BlackAndWhiteImages);
        }

        using (TimeLogger tl = new TimeLogger($"RedImageCleaningConfigs: Creating and Saving", LogLevel.Information, P.Logger, 1))
        {
            for (int i = 0; i < BasicImageDataList.Count; i++)
                redImageCleaningConfigs.Add(new RedImageCleaningConfig(BasicImageDataList[i]));
            P.Logger.Log("Created", LogLevel.Information, 2);

            string serialized = JsonConvert.SerializeObject(redImageCleaningConfigs);
            File.WriteAllText(P.CleaningProject.CleaningProjectDirs.CleaningConfigs, serialized);
            P.Logger.Log("Saved", LogLevel.Information, 2);
        }

        using (TimeLogger tl = new TimeLogger("JsonConvert.SerializeObject(ImageDataList)", LogLevel.Information, P.Logger, 1))
        {
            P.ProjectProcessingStatus.Set(Operation.IsJsonSerializeing);
            P.ProjectProcessingStatus.Save();

            string ImageDataListAsString = JsonConvert.SerializeObject(BasicImageDataList); //TODO Check JsonConvert.SerializeObject(ImageDataList, Formatting.);
            File.WriteAllText(P.CleaningProject.CleaningProjectDirs.ObjectDetectionData, ImageDataListAsString);
        }

        P.CleaningProject.CleaningProjectInfo.IsPRORFinished = true;
        P.CleaningProject.Save(P.CleaningProject.CleaningProjectDirs.CleaningProjectInfo);

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

            resulImage.Save(outputPath + @"\" + Path.GetFileNameWithoutExtension(inputImagePath) + ".png", System.Drawing.Imaging.ImageFormat.Png);
        }


        private static void ConvertRGBImageToGrayscaleThread(object inputObj)
        {
            string inputImagePath = (string)inputObj;

            try
            {
                ConvertRGBImageToGrayscale(inputImagePath, P.CleaningProject.CleaningProjectDirs.BlackAndWhiteImages);
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
        public static void ConvertRGBImagesToGrayscaleMultithreading(List<string> filenames, string inputPath)
        {
            Queue<string> filesToProcess = new Queue<string>();
            int startedThreds = 0;

            for (int i = 0; i < filenames.Count; i++)
            {
                filesToProcess.Enqueue(inputPath + @"\" + filenames[i]);
            }

            P.ProjectProcessingStatus.Set(Operation.IsConvertingToGrayscale, filenames.Count, 0);
            P.ProjectProcessingStatus.Save();

            P.Floats.Threads.ThreadsEndetWithError = 0;
            P.Floats.Threads.ThreadsEndetSuccessfully = 0;
            P.Floats.Threads.ThreadCounter = P.Settings.SettingsList.ImagesToBlackAndWhiteThreadsCount;

            P.Logger.Log($"Multithreading converting of RGB images to BaW-Try:", LogLevel.Information, 4);
            while (filesToProcess.Count > 0)
            {
                if (P.Floats.Threads.ThreadCounter > 0)
                {
                    P.Logger.Log($"Multithreading: ThreadCounter=[{P.Floats.Threads.ThreadCounter}], Starting thread for File=[{filesToProcess.Peek()}]-Try", LogLevel.Information, 5);

                    Thread thread = new Thread(new ParameterizedThreadStart(Work.Grayscale.ConvertRGBImageToGrayscaleThread));
                    thread.Start(filesToProcess.Peek());
                    startedThreds++;

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
        public static List<BasicImageData> DetectObjectsOnImages(List<string> filenames, string inputPath)
        {
            List<BasicImageData> result = new List<BasicImageData>();

            P.ProjectProcessingStatus.Set(Operation.IsDetectingObjects, filenames.Count, 0);
            P.ProjectProcessingStatus.Save();

            string config = File.ReadAllText(P.CurrentYoloConfiguration.ModelConfigPath);
            RedsYoloConfig redsYoloConfig = JsonConvert.DeserializeObject<RedsYoloConfig>(config);
            YoloScorer<RedsGeneralModel> scorer = new YoloScorer<RedsGeneralModel>(P.CurrentYoloConfiguration.WeightsPath, redsYoloConfig);

            Image image = null;
            for (int i = 0; i < filenames.Count; i++)
            {
                image = Image.FromFile(inputPath + @"\" + filenames[i]);
                P.Logger.Log($"Detect Objects on image=[{filenames[i]}]-Try", LogLevel.Information, 2);
                List<YoloPrediction> predictions = scorer.Predict(image);
                P.Logger.Log($"Success", LogLevel.Information, 3);

                result.Add(new BasicImageData(inputPath + @"\" + filenames[i], image.Width, image.Height, ImageType.BaW, DetectedObject.ConvertYPLToDetectedObjectList(predictions))); //TODO ImageType.BaW > from start arguments(proj creation args)

                P.ProjectProcessingStatus.Set(i);
                P.ProjectProcessingStatus.Save();
            }

            image.Dispose();
            scorer.Dispose();
            GC.Collect();

            return result;
        }
    }

    public static List<string> CopyImages(string inputFrom, string inputTo)
    {
        List<string> filenames = new List<string>();

        string[] usableImageExtensions = { ".png", ".jpg", ".bmp", ".jpe" };
        string[] files = Directory.GetFiles(inputFrom);

        P.ProjectProcessingStatus.Set(Status.IsRunning, Operation.IsCopyingImages, files.Length, 0);
        P.ProjectProcessingStatus.Save();

        Bitmap imageBuffer;
        for (int i = 0; i < files.Length; i++)
        {
            bool checkPassed = false;
            string saveAs = "";
            for (int j = 0; j < usableImageExtensions.Length; j++)
            {

                if (Path.GetExtension(files[i]).ToLower() == usableImageExtensions[j])
                {
                    saveAs = inputTo + @"\" + Path.GetFileNameWithoutExtension(files[i]) + ".png";

                    imageBuffer = new Bitmap(files[i]);
                    imageBuffer.Save(saveAs, System.Drawing.Imaging.ImageFormat.Png);

                    filenames.Add(Path.GetFileName(saveAs));

                    checkPassed = true;

                    break;
                }
            }

            if (checkPassed)
            {
                P.Logger.Log($"File=[{files[i]}] pass check as usableImage and copied as=[{saveAs}]", LogLevel.Information, 2);
            }
            else
            {
                P.Logger.Log($"File=[{files[i]}] don't pass usableImage check FileExtension=[{Path.GetExtension(files[i])}]", LogLevel.Error, 2);
            }

            P.ProjectProcessingStatus.Set(i);
            P.ProjectProcessingStatus.Save();
        }

        return filenames;
    }
}