using Newtonsoft.Json;
using RedsCleaningProject.Core;
using System;
using System.IO;
using System.Net;

class Initalization
{
    public static void Start()
    {
        ConsoleInit();

        VersionInit();

        PathDirsInit();

        StartArgumentsInit();

        P.Logger = new Logger(P.PathDirs.Log, new LogSettings(LogLevel.Debug));
        P.Settings = new Settings(P.PathDirs.MainSettings);
        P.ProjectProcessingStatus = new ProjectProcessingStatus(P.PathDirs.ProjectProcessingStatus);
        P.CleaningProjectsGlobalInfo = new CleaningProjectsGlobalInfo(P.PathDirs.CleaningProjectsGlobalInfo);

        P.ProjectProcessingStatus.Status = Status.IsBooting;
        P.ProjectProcessingStatus.Save();

        FileValidation();

        YoloConfigInit();
        InitializationPersonalized.MainInit();
    }


    private static void StartArgumentsInit()
    {
        if (File.Exists(P.PathDirs.StartArguments))
        {
            string serialized = File.ReadAllText(P.PathDirs.StartArguments);
            P.StartArguments = JsonConvert.DeserializeObject<StartArguments>(serialized);
            //File.Delete(P.PathDirs.StartArguments);
        }
        else
        {
            StartArguments devSetInputArgs = new StartArguments();

            devSetInputArgs.FolderOptions = FolderOptions.AutoCreateById;
            devSetInputArgs.CleaningProjectId = 7;
            devSetInputArgs.OutputBlackAndWhiteImages = true;
            devSetInputArgs.ConductObjectDetectionOnBlackAndWhiteVariants = true;
            devSetInputArgs.ConductTextBoxFillingOnBlackAndWhiteVariants = true;
            devSetInputArgs.InputPath =
            @"E:\Other\Translate\I Was Caught up in a Hero Summoning, but That World Is at Peace\I Was Caught up in a Hero Summoning, but That World Is at Peace Chapter 6\MangaOUT1";
            devSetInputArgs.CleaningProjectFolderName = @"CleaningProject_ID-[0]";

            string devSetInputArgsString = JsonConvert.SerializeObject(devSetInputArgs);
            //string dewSetInputArgsString = File.ReadAllText(@"D:\args.json");
            P.StartArguments = JsonConvert.DeserializeObject<StartArguments>(devSetInputArgsString);
        }
    }

    private static void ConsoleInit()
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Title = InitializationPersonalized.Title;

        try
        {
            Console.WindowWidth = 200;
        }
        catch (Exception)
        {
            P.Logger.Log("Console.WindowWidth = 200 can't be set", LogLevel.Warning);
        }
    }
    internal static void VersionInit()
    {
        P.ConstantData.VersionName = "Beta_3.0.0";
        P.ConstantData.VersionId = 3;
    }
    private static void PathDirsInit()
    {
        string currentPath = Environment.CurrentDirectory;
        currentPath = @"D:\Development\RedsSoft\RdmMangaCleaner"; //DEV

        P.PathDirs.SetFromExecutionPath(currentPath, P.PathNames);
        Dir.CreateAllDirsInObject(P.PathDirs);
    }

    private static void FileValidation()
    {
        ReadmeValidation();
    }
    private static void ReadmeValidation()
    {
        //string webData = "";
        //using (WebClient webClient = new WebClient())
        //{
        //    webData = webClient.DownloadString("https://raw.githubusercontent.com/Red007Master/RedMangaCleaner/master/README.md");
        //}

        //Console.Write(webData);

        //if (!File.Exists(P.PathDirs.Readme))
        //{
        //    //https://raw.githubusercontent.com/Red007Master/RedMangaCleaner/master/README.md
        //}

        bool rewrite = false;
        string oldReadmeContent = "";
        string newReadmeContent = "";

        if (File.Exists(P.PathDirs.Readme))
        {
            oldReadmeContent = File.ReadAllText(P.PathDirs.Readme);
        }

        try
        {

        }
        catch (Exception)
        {
            if (!File.Exists(P.PathDirs.Readme))
            {
                rewrite = true;

                newReadmeContent = "So... Network unavabile so we can't get last redme from [https://raw.githubusercontent.com/Red007Master/RedMangaCleaner/master/README.md]";
            }
        }

        if (rewrite)
            File.WriteAllText(P.PathDirs.Readme, newReadmeContent);
    }

    private static void YoloConfigInit()
    {
        P.PathNames.YoloConfig = P.Settings.SettingsList.YoloConfig;
        P.PathNames.YoloWeight = P.Settings.SettingsList.YoloWeights;

        P.PathDirs.YoloConfig = P.PathDirs.YoloData + @"\" + P.PathNames.YoloConfig;
        P.PathDirs.YoloWeight = P.PathDirs.YoloData + @"\" + P.PathNames.YoloWeight;

        P.CurrentYoloConfiguration.WeightsPath = P.PathDirs.YoloWeight;
        P.CurrentYoloConfiguration.ModelConfigPath = P.PathDirs.YoloConfig;
    }
}
