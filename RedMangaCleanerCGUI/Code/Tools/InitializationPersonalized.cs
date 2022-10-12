using Newtonsoft.Json;
using RedMangaCleanerPROR.Code.Structures;
using RedsCleaningProject.Core;
using RedsTools.Utility.WPF.LanguageManager;
using System.IO;

public static class InitializationPersonalized //CGUI
{
    public static string Title = "RdmMangaCleaner(CleanerGraphicalUserInterface)";
    public static string LogFile = "LogCGUI.txt";
    public static CleaningProjectCreationArguments StartArguments { get { return GetDebugStartArguments(); } }

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
            P.GeneralUIConfig = JsonConvert.DeserializeObject<GeneralUIConfig>(serialized);
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

    private static CleaningProjectCreationArguments GetDebugStartArguments()
    {
        CleaningProjectCreationArguments arguments = new CleaningProjectCreationArguments();

        arguments.UserTag = "TestPrg";
        arguments.OutputBlackAndWhiteImages = true;
        arguments.ConductObjectDetectionOnBlackAndWhiteVariants = true;
        arguments.ConductTextBoxFillingOnBlackAndWhiteVariants = true;
        arguments.CleaningProjectId = P.CleaningProjectsGlobalInfo.GetAndIncrementId();
        arguments.InputPath = @"D:\Other\Translate\I Was Caught up in a Hero Summoning, but That World Is at Peace\I Was Caught up in a Hero Summoning, but That World Is at Peace Chapter 9\MangaOUT1";
        arguments.FolderOptions = FolderOptions.CreateNewFolderById;

        P.CleaningProject = new CleaningProject(arguments, P.PathDirs.CleaningProjects);

        return arguments;
    }
}