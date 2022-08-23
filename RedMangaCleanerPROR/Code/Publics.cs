using RedsCleaningProjects.Core;
using RedsTools.Utility.WPF.LanguageManager;
using System;
using System.Windows.Forms;

class P
{
    public static Logger Logger { get; set; }
    public static Settings Settings { get; set; }
    public static LanguageManager LanguageManager { get; set; }

    public static PathNames PathNames { get; set; } = new PathNames();
    public static PathDirs PathDirs { get; set; } = new PathDirs();
    public static CleaningProjectNames CleaningProjectNames { get; set; }
    public static CleaningProjectDirs CleaningProjectDirs { get; set; } = new CleaningProjectDirs();

    public static StartArguments StartArguments { get; set; }
    public static Config Config { get; set; } = new Config();
    public static YoloConfiguration CurrentYoloConfiguration { get; set; } = new YoloConfiguration();


    public static CleaningProjectInfo CleaningProjectInfo { get; set; }
    public static CleaningProjectsGlobalInfo CleaningProjectsGlobalInfo { get; set; } = new CleaningProjectsGlobalInfo();
    public static ProjectProcessingStatus ProjectProcessingStatus { get; set; }

    public class Floats
    {
        public static class Threads
        {
            public static int ThreadCounter { get; set; }
            public static int ThreadsEndetSuccessfully { get; set; }
            public static int ThreadsEndetWithError { get; set; }

            public static void ClearNew(int iThreadCounter)
            {
                ThreadCounter = iThreadCounter;
                ThreadsEndetSuccessfully = 0;
                ThreadsEndetWithError = 0;
            }
        }

        public static class Logic
        {
            public static bool IsLoad { get; set; } = false;
            public static int Id { get; set; }
        }
    }
    public class ConstantData
    {
        public static string VersionName { get; set; }
        public static int VersionId { get; set; }
    }
}


public class PathCoreClass
{
    public string Core { get; set; }

    public string Settings { get; set; }
    public string Log { get; set; }

    public string Data { get; set; }
    public string YoloData { get; set; }
    public string Executables { get; set; }
    public string Temp { get; set; }
    public string Variables { get; set; }
    public string Languages { get; set; }
    public string TextBoxFillingArguments { get; set; }
    public string ObjectDetection { get; set; }
    public string UserInterface { get; set; }
    public string CleaningProjects { get; set; }

    public string PROR { get; set; }
    public string CGUI { get; set; }
    public string CleaningProjectsGlobalInfo { get; set; }
    public string ProjectProcessingStatus { get; set; }
    public string StartArguments { get; set; }
    public string LastRetrievedWebData { get; set; }
    public string Config { get; set; }

    public string YoloConfig { get; set; }
    public string YoloWeight { get; set; }
}
public class CleaningProjectPathCoreClass
{
    public string Core { get; set; }

    public string ObjectsData { get; set; }
    public string CleaningProjectInfo { get; set; }

    public string Images { get; set; }
    public string Data { get; set; }

    public string BlackAndWhiteImages { get; set; }
    public string SourceImages { get; set; }
    public string ResultImages { get; set; }
}

public class PathNames : PathCoreClass
{
    public PathNames()
    {
        Core = "RdmMangaCleaner";

        Settings = "Settings.txt";
        Log = InitializationPersonalized.LogFile;

        PROR = "PROR.exe";
        CGUI = "CGUI.exe";
        CleaningProjectsGlobalInfo = "CleaningProjectsGlobalInfo.json";
        ProjectProcessingStatus = "ProjectProcessingStatus.json";
        StartArguments = "StartArguments.json";
        LastRetrievedWebData = "LastRetrievedWebData.json";
        Config = "Config.json";

        Data = "Data";
        YoloData = "YoloData";
        Executables = "Executables";
        Temp = "Temp";
        UserInterface = "UserInterface";
        ObjectDetection = "ObjectDetection";
        CleaningProjects = "CleaningProjects";
        Variables = "Variables";
        Languages = "Languages";
        TextBoxFillingArguments = "TextBoxFillingArguments";
    }
}
public class CleaningProjectNames : CleaningProjectPathCoreClass
{
    public FolderOptions FolderOptions { get; set; }

    public CleaningProjectNames(FolderOptions folderOptions, object iValue = null)
    {
        FolderOptions = folderOptions;

        if (folderOptions == FolderOptions.CreateNewFolderById)
        {
            if (iValue is int)
            {
                int newFolderId = (int)iValue;

                ConstructorCore($@"CleaningProject_ID-[{newFolderId}]");
            }
            else
            {
                P.Logger.Log("CleaningProjectNames:CleaningProjectNames iValue not int", LogLevel.FatalError);
            }
        }
        else if (folderOptions == FolderOptions.CreateNewFolderByName)
        {
            if (iValue is string)
            {
                string newFolderName = (string)iValue;

                ConstructorCore(newFolderName);
            }
            else
            {
                P.Logger.Log("CleaningProjectNames:CleaningProjectNames iValue not string", LogLevel.FatalError);
            }
        }
    }
    public CleaningProjectNames()
    {
        ConstructorCore("Empty");
    }

    private void ConstructorCore(string corePathName)
    {
        Core = corePathName;
        ObjectsData = "ObjectsData.json";
        CleaningProjectInfo = "CleaningProjectInfo.json";

        Images = "Images";
        BlackAndWhiteImages = "BlackAndWhiteImages";
        SourceImages = "SourceImages";
        ResultImages = "ResultImages";

        Data = "Data";
    }
}


public class PathDirs : PathCoreClass
{
    public void SetFromExecutionPath(string inputExecutionPath, PathNames inputPathNames)
    {
        this.Core = GetCorePath(inputExecutionPath, inputPathNames.Core);

        this.Log = this.Core + @"\" + inputPathNames.Log;

        this.Data = this.Core + @"\" + inputPathNames.Data;
        this.CleaningProjects = this.Core + @"\" + inputPathNames.CleaningProjects;

        this.YoloData = this.Data + @"\" + inputPathNames.YoloData;
        this.Temp = this.Data + @"\" + inputPathNames.Temp;
        this.Variables = this.Data + @"\" + inputPathNames.Variables;
        this.Languages = this.Data + @"\" + inputPathNames.Languages;
        this.TextBoxFillingArguments = this.Data + @"\" + inputPathNames.TextBoxFillingArguments;
        this.Executables = this.Data + @"\" + inputPathNames.Executables;

        this.PROR = this.Executables + @"\" + inputPathNames.PROR;
        this.CGUI = this.Executables + @"\" + inputPathNames.CGUI;
        this.ProjectProcessingStatus = this.Temp + @"\" + inputPathNames.ProjectProcessingStatus;
        this.StartArguments = this.Temp + @"\" + inputPathNames.StartArguments;
        this.UserInterface = this.Executables + @"\" + inputPathNames.UserInterface;
        this.ObjectDetection = this.Executables + @"\" + inputPathNames.ObjectDetection;

        this.Settings = this.Core + @"\" + inputPathNames.Settings;
        this.CleaningProjectsGlobalInfo = this.CleaningProjects + @"\" + inputPathNames.CleaningProjectsGlobalInfo;
        this.LastRetrievedWebData = this.Variables + @"\" + inputPathNames.LastRetrievedWebData;
        this.Config = this.Variables + @"\" + inputPathNames.Config;
    }

    private static string GetCorePath(string inputCurrentPath, string inputCorePathName)
    {
        string corePathBuffer = "";
        string[] currentPathAsArray;
        bool corePathIsDetected = false;

        currentPathAsArray = inputCurrentPath.Split(Convert.ToChar(@"\"));

        for (int i = 0; i < currentPathAsArray.Length; i++)
        {
            corePathBuffer += currentPathAsArray[i] + @"\";

            if (currentPathAsArray[i] == inputCorePathName)
            {
                corePathIsDetected = true;
                break;
            }
        }

        corePathBuffer = corePathBuffer.Remove(corePathBuffer.Length - 1);

        if (!corePathIsDetected)
        {
            MessageBox.Show("Core path not found");
            return null;
        }
        else
        {
            return corePathBuffer;
        }
    }
}
public class CleaningProjectDirs : CleaningProjectPathCoreClass
{
    public void SetFromPath(string inputProcessingPath, CleaningProjectNames inputCleaningProjectNames)
    {
        this.Core = inputProcessingPath + @"\" + inputCleaningProjectNames.Core;

        this.CleaningProjectInfo = this.Core + @"\" + inputCleaningProjectNames.CleaningProjectInfo;

        this.Data = this.Core + @"\" + inputCleaningProjectNames.Data;
        this.Images = this.Core + @"\" + inputCleaningProjectNames.Images;

        this.ObjectsData = this.Data + @"\" + inputCleaningProjectNames.ObjectsData;

        this.BlackAndWhiteImages = this.Images + @"\" + inputCleaningProjectNames.BlackAndWhiteImages;
        this.SourceImages = this.Images + @"\" + inputCleaningProjectNames.SourceImages;
        this.ResultImages = this.Images + @"\" + inputCleaningProjectNames.ResultImages;
    }
}