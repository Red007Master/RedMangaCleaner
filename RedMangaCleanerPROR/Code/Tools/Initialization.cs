using Newtonsoft.Json;
using RedMangaCleanerPROR.Code.Structures;
using RedsCleaningProject.Core;
using RedsCleaningProject.DrawingConfigs;
using System;
using System.IO;
using System.Net;

class Initalization
{
    public static void Start()
    {
        ConsoleInit();

        ConstantVariablesInit();

        PathDirsInit();

        StartArgumentsInit();

        P.Logger = new Logger(P.PathDirs.Log, new LogSettings(LogLevel.Debug));
        P.Settings = new Settings(P.PathDirs.MainSettings);
        P.ProjectProcessingStatus = new ProjectProcessingStatus(P.PathDirs.ProjectProcessingStatus);
        P.CleaningProjectsGlobalInfo = new CleaningProjectsGlobalInfo(P.PathDirs.CleaningProjectsGlobalInfo);

        P.ProjectProcessingStatus.Status = Status.IsBooting;
        P.ProjectProcessingStatus.Save();

        using (TimeLogger tl = new TimeLogger("FileValidation", LogLevel.Debug, P.Logger, 1))
        {
            FileValidation();
        }

        InitializationPersonalized.MainInit();
    }


    private static void StartArgumentsInit()
    {
        if (File.Exists(P.PathDirs.StartArguments))
        {
            string serialized = File.ReadAllText(P.PathDirs.StartArguments);
            P.StartArguments = JsonConvert.DeserializeObject<CleaningProjectCreationArguments>(serialized);
            //File.Delete(P.PathDirs.StartArguments);
        }
        else
        {
            CleaningProjectCreationArguments devSetInputArgs = new CleaningProjectCreationArguments();

            devSetInputArgs.FolderOptions = FolderOptions.AutoCreateById;
            devSetInputArgs.CleaningProjectId = 7;
            devSetInputArgs.OutputBlackAndWhiteImages = true;
            devSetInputArgs.ConductObjectDetectionOnBlackAndWhiteVariants = true;
            devSetInputArgs.ConductTextBoxFillingOnBlackAndWhiteVariants = true;
            devSetInputArgs.InputPath =
            @"C:\Users\Red007Master\Desktop\SourceImages";

            string devSetInputArgsString = JsonConvert.SerializeObject(devSetInputArgs);
            //string dewSetInputArgsString = File.ReadAllText(@"D:\args.json");
            P.StartArguments = JsonConvert.DeserializeObject<CleaningProjectCreationArguments>(devSetInputArgsString);
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
    internal static void ConstantVariablesInit()
    {
        P.ConstantData.VersionName = "Beta_3.0.0";
        P.ConstantData.VersionId = 3;

        P.ConstantData.ProjectLocation = "Red007Master/RedMangaCleaner";
        P.ConstantData.GithubCoreUrl = "https://github.com";
        P.ConstantData.GuthubRawCoreUrl = "https://raw.githubusercontent.com";
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
        using (TimeLogger tl = new TimeLogger("ReadmeValidationAndUpdate", LogLevel.Debug, P.Logger, 2))
        {
            ReadmeValidationAndUpdate();
        }

        using (TimeLogger tl = new TimeLogger("YoloConfigAndWeightsValidation", LogLevel.Debug, P.Logger, 2))
        {
            YoloConfigAndWeightsValidation();
        }

        using (TimeLogger tl = new TimeLogger("DrawingConfigsValidation", LogLevel.Debug, P.Logger, 2))
        {
            DrawingConfigsValidation();
        }
    }
    private static void ReadmeValidationAndUpdate()
    {
        bool write = false;
        string oldReadmeContent = "";
        string newReadmeContent = "";

        if (File.Exists(P.PathDirs.Readme))
            oldReadmeContent = File.ReadAllText(P.PathDirs.Readme);

        try
        {
            using (WebClient webClient = new WebClient())
            {
                string readmeCore = webClient.DownloadString($"{P.ConstantData.GuthubRawCoreUrl}/{P.ConstantData.ProjectLocation}/master/Files/Readme/core.txt");
                string readmeVer = webClient.DownloadString($"{P.ConstantData.GuthubRawCoreUrl}/{P.ConstantData.ProjectLocation}/master/Files/Readme/ver{P.ConstantData.VersionId}.txt");

                newReadmeContent = readmeCore + "\n" + readmeVer;

                if (newReadmeContent != oldReadmeContent)
                    write = true;
            }
        }
        catch (Exception ex)
        {
            P.Logger.Log($"Can't get readme from github Ex=[{ex}]", LogLevel.Error);

            if (!File.Exists(P.PathDirs.Readme))
            {
                write = true;

                newReadmeContent = $"So... Network unavabile (or some other problems, you can check: {InitializationPersonalized.LogFile}), basically we can't get last readme from [{P.ConstantData.GuthubRawCoreUrl}/{P.ConstantData.ProjectLocation}/master/Files/Readme]\n" +
                                   "TODO there placeholder readme";
            }
        }

        if (write)
            File.WriteAllText(P.PathDirs.Readme, newReadmeContent);
    }
    private static void YoloConfigAndWeightsValidation()
    {
        string modelConfigPath = P.PathDirs.YoloData + @"\" + P.Settings.SettingsList.ModelConfig;
        string weightsPath = P.PathDirs.YoloData + @"\" + P.Settings.SettingsList.YoloWeights;

        if (!File.Exists(modelConfigPath))
            P.Logger.Log($"ModelConfig ({modelConfigPath}) don't exist, edit settings to correct values or create/add file", LogLevel.FatalError);

        if (!File.Exists(weightsPath))
            P.Logger.Log($"YoloWeights ({weightsPath}) don't exist, edit settings to correct values or create/add file", LogLevel.FatalError);

        P.CurrentYoloConfiguration.ModelConfigPath = modelConfigPath;
        P.CurrentYoloConfiguration.WeightsPath = weightsPath;
    }
    private static void DrawingConfigsValidation()
    {
        string defaultName = "default.json";

        string defaultRectangleConfigPath = P.PathDirs.RectangleConfigs + @"\" + defaultName;
        string defaultTextConfigPath = P.PathDirs.TextConfigs + @"\" + defaultName;
        string defaultTextBoxFillingConfigPath = P.PathDirs.TextBoxFillingConfigs + @"\" + defaultName;

        if (!File.Exists(defaultRectangleConfigPath))
            HandleMissingConfigFile<RectangleConfig>(defaultRectangleConfigPath, new RectangleConfig());

        if (!File.Exists(defaultTextConfigPath))
            HandleMissingConfigFile<TextConfig>(defaultTextConfigPath, new TextConfig());

        if (!File.Exists(defaultTextBoxFillingConfigPath))
            HandleMissingConfigFile<TextBoxFillingConfig>(defaultTextBoxFillingConfigPath, new TextBoxFillingConfig());
    }
    private static void HandleMissingConfigFile<T>(string path, T fileObject)
    {
        P.Logger.Log($"Missing default [{fileObject.GetType()}] file on address [{path}], creating new", LogLevel.Warning);
        string jsonContent = JsonConvert.SerializeObject(fileObject, Formatting.Indented);
        File.WriteAllText(path, jsonContent);
    }
}
