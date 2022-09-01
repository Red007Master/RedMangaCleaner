using Newtonsoft.Json;
using RedsCleaningProject.Core;
using RedsTools.Utility.WPF.LanguageManager;
using System.IO;

public static class InitializationPersonalized //CGUI
{
    public static string Title = "RdmMangaCleaner(CleanerGraphicalUserInterface)";
    public static string LogFile = "LogCGUI.txt";
    public static StartArguments StartArguments { get { return GetDebugStartArguments(); } }

    public static void MainInit()
    {
        GeneralUIConfigInit();
        LocalizeGUIInit();
    }

    private static void GeneralUIConfigInit()
    {
        if (File.Exists(P.PathDirs.GeneralUIConfig))
        {
            string serialized = File.ReadAllText(P.PathDirs.GeneralUIConfig);
            GeneralUIConfig generalUIConfig = JsonConvert.DeserializeObject<GeneralUIConfig>(serialized);
            P.GeneralUIConfig = generalUIConfig;
        }
        else
        {
            P.GeneralUIConfig.SetDefault();
            string serialized = JsonConvert.SerializeObject(P.GeneralUIConfig);
            File.WriteAllText(P.PathDirs.GeneralUIConfig, serialized);
        }
    }
    private static void LocalizeGUIInit()
    {
        P.LanguageManager = new LanguageManager(P.PathDirs.Languages, P.Settings.SettingsList.Language);
    }

    private static StartArguments GetDebugStartArguments()
    {
        StartArguments arguments = new StartArguments();

        arguments.UserTag = "TestPrg";
        arguments.CleaningProjectFolderName = "";
        arguments.OutputBlackAndWhiteImages = true;
        arguments.ConductObjectDetectionOnBlackAndWhiteVariants = true;
        arguments.ConductTextBoxFillingOnBlackAndWhiteVariants = true;
        arguments.CleaningProjectId = P.CleaningProjectsGlobalInfo.GetAndIncrementId();
        arguments.InputPath = @"D:\Other\Translate\I Was Caught up in a Hero Summoning, but That World Is at Peace\I Was Caught up in a Hero Summoning, but That World Is at Peace Chapter 9\MangaOUT1";
        arguments.FolderOptions = FolderOptions.CreateNewFolderById;

        P.CleaningProjectNames = new CleaningProjectNames(FolderOptions.CreateNewFolderById, arguments.CleaningProjectId);
        P.CleaningProjectDirs.SetFromPath(P.PathDirs.CleaningProjects, P.CleaningProjectNames);

        return arguments;
    }
}