using Newtonsoft.Json;
using RedMangaCleanerPROR.Code.Structures;
using RedsCleaningProject.CleaningConfigs;
using RedsCleaningProject.RedImages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

class Work
{
    public static List<RedImageFull> RedImageFulls { get; set; }

    public static void MainVoid()
    {
        RedImageFulls = new List<RedImageFull>();

        List<BasicImageData> BasicImageDatas = new List<BasicImageData>();
        List<RedImageCleaningConfig> redImageCleaningConfigs = new List<RedImageCleaningConfig>();

        List<RedImageCore> redImageCores = new List<RedImageCore>();

        using (TimeLogger tl = new TimeLogger($"ImageData: File.ReadAllText = [{P.CleaningProject.CleaningProjectDirs.ObjectDetectionData}] and JsonConvert.DeserializeObject", LogLevel.Information, P.Logger, 1))
        {
            string serialized = File.ReadAllText(P.CleaningProject.CleaningProjectDirs.ObjectDetectionData);
            BasicImageDatas = JsonConvert.DeserializeObject<List<BasicImageData>>(serialized);
        }

        using (TimeLogger tl = new TimeLogger($"RedImageCleaningConfigs: Loading", LogLevel.Information, P.Logger, 1))
        {
            if (File.Exists(P.CleaningProject.CleaningProjectDirs.CleaningConfigs))
            {
                P.Logger.Log("Config found, loading", LogLevel.Information, 2);
                redImageCleaningConfigs = JsonConvert.DeserializeObject<List<RedImageCleaningConfig>>(File.ReadAllText(P.CleaningProject.CleaningProjectDirs.CleaningConfigs));
                P.Logger.Log("Loaded", LogLevel.Information, 3);
            }
            else
            {
                P.Logger.Log("Config not found, creating new", LogLevel.Warning, 2);

                for (int i = 0; i < BasicImageDatas.Count; i++)
                {
                    redImageCleaningConfigs.Add(new RedImageCleaningConfig(BasicImageDatas[i]));
                }

                P.Logger.Log($"Created, saving to [{P.CleaningProject.CleaningProjectDirs.CleaningConfigs}]", LogLevel.Information, 3);
                string serialized = JsonConvert.SerializeObject(redImageCleaningConfigs);
                File.WriteAllText(P.CleaningProject.CleaningProjectDirs.CleaningConfigs, serialized);
                P.Logger.Log("Saved", LogLevel.Information, 4);
            }
        }

        using (TimeLogger tl = new TimeLogger("RedImageCore: Formating.GetRedImageCoreFromImgDataObjList", LogLevel.Information, P.Logger, 1))
        {
            for (int i = 0; i < BasicImageDatas.Count; i++)
            {
                redImageCores.Add(new RedImageCore(BasicImageDatas[i], redImageCleaningConfigs[i]));
            }
        }

        using (TimeLogger tl = new TimeLogger("Formating.PrecompileRedImageFullsFromRedImageCoresMultithreading and sort", LogLevel.Information, P.Logger, 1))
        {
            RedImageFulls = Formating.PrecompileRedImageFullsFromRedImageCoresMultithreading(redImageCores);

            RedImageFulls.Sort();
        }

        //redImageCleaningConfigs[0].EditableObjectCleaningConfigs[2].RectangleConfig.RectangleShift = new System.Drawing.Rectangle(69, 69, 69, 69);

        PV.UserControl.ImagesProcessingView.ImagesProcessingProgressInfo.IsFinished = true;
    }

    public static class Formating
    {
        private static int CompiledImagesCount { get; set; }

        private static void PrecompileRedImageFullFromRedImageCoreThread(object inputObj)
        {
            RedImageCore inputRedImageCore = (RedImageCore)inputObj;

            try
            {
                P.Logger.Log($"Precompile [{inputRedImageCore.FileName}] - Try", LogLevel.Information, 1);
            }
            catch (Exception)
            { }

            try
            {
                RedImageFullsResultList.Add(new RedImageFull(inputRedImageCore));
                P.Floats.Threads.ThreadsEndetSuccessfully++;
            }
            catch (Exception ex)
            {
                P.Floats.Threads.ThreadsEndetWithError++;

                P.Logger.Log($"ERROR:Thread for [{inputRedImageCore.FileName}] catch exeption ex=[{ex}]", LogLevel.Error, 2);
            }

            try
            {
                P.Logger.Log($"Precompile [{inputRedImageCore.FileName}] - Success", LogLevel.Information, 3);
            }
            catch (Exception)
            { }

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
                    P.Logger.Log($"Formating.PrecompileRedImageFullsFromRedImageCoresMultithreading: Multithreading: ThreadCounter=[{P.Floats.Threads.ThreadCounter}], Starting thread for File=[{redImageCoresToProcess.Peek().FileName}]-Try", LogLevel.Information, 3);

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