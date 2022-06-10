using Newtonsoft.Json;
using RedsCleaningProjects.Core;
using System;
using System.IO;

class Initalization
{
    public static void Start()
    {
        ConsoleInit();

        VersionInit();

        PathDirsInit();

        StartArgumentsInit();

        P.Logger = new Logger(P.PathDirs.Log, new LogSettings(LogLevel.Debug));
        P.Settings = new Settings(P.PathDirs.Settings);
        P.ProjectProcessingStatus = new ProjectProcessingStatus(P.PathDirs.ProjectProcessingStatus);
        P.CleaningProjectsGlobalInfo = new CleaningProjectsGlobalInfo(P.PathDirs.CleaningProjectsGlobalInfo);

        P.ProjectProcessingStatus.Status = Status.IsBooting;
        P.ProjectProcessingStatus.Save();

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
            StartArguments dewSetInputArgs = new StartArguments();

            dewSetInputArgs.FolderOptions = FolderOptions.AutoCreateById;
            dewSetInputArgs.CleaningProjectId = 7;
            dewSetInputArgs.OutputBlackAndWhiteImages = true;
            dewSetInputArgs.ConductObjectDetectionOnBlackAndWhiteVariants = true;
            dewSetInputArgs.ConductTextBoxFillingOnBlackAndWhiteVariants = true;
            dewSetInputArgs.InputPath = @"D:\Other\Translate\I Was Caught up in a Hero Summoning, but That World Is at Peace\I Was Caught up in a Hero Summoning, but That World Is at Peace Chapter 9\MangaOUT1";
            dewSetInputArgs.CleaningProjectFolderName = @"CleaningProject_ID-[0]";

            string dewSetInputArgsString = JsonConvert.SerializeObject(dewSetInputArgs);
            //string dewSetInputArgsString = File.ReadAllText(@"D:\args.json");
            P.StartArguments = JsonConvert.DeserializeObject<StartArguments>(dewSetInputArgsString);
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
        P.ConstantData.VersionName = "Beta_2.0.0";
        P.ConstantData.VersionId = 2;
    }
    private static void PathDirsInit()
    {
        string currentPath = Environment.CurrentDirectory;
        currentPath = @"D:\RdmMangaCleaner\Data\Packages\ObjectDetection";

        P.PathDirs.SetFromExecutionPath(currentPath, P.PathNames);
        Dir.CreateAllDirsInObject(P.PathDirs);
    }
    private static void YoloConfigInit()
    {
        P.PathNames.YoloConfig = P.Settings.SettingsList.YoloConfig;
        P.PathNames.YoloNames = P.Settings.SettingsList.YoloNames;
        P.PathNames.YoloWeight = P.Settings.SettingsList.YoloWeights;

        P.PathDirs.YoloConfig = P.PathDirs.YoloData + @"\" + P.PathNames.YoloConfig;
        P.PathDirs.YoloNames = P.PathDirs.YoloData + @"\" + P.PathNames.YoloNames;
        P.PathDirs.YoloWeight = P.PathDirs.YoloData + @"\" + P.PathNames.YoloWeight;

        P.CurrentYoloConfiguration.ConfigFile = P.PathDirs.YoloConfig;
        P.CurrentYoloConfiguration.NamesFile = P.PathDirs.YoloNames;
        P.CurrentYoloConfiguration.WeightsFile = P.PathDirs.YoloWeight;
    }
}
