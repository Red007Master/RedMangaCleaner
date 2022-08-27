using RedsCleaningProject.Core;
using System;

public static class InitializationPersonalized //PROR
{
    public static string Title = "RdmMangaCleaner(PROR)";
    public static string LogFile = "LogPROR.txt";

    public static void MainInit()
    {
        CleaningProjectDirsInit();
    }

    private static void CleaningProjectDirsInit()
    {
        if (P.StartArguments.FolderOptions == FolderOptions.CreateNewFolderById)
        {
            CreateNewFolderById();
        }
        else if (P.StartArguments.FolderOptions == FolderOptions.CreateNewFolderByName)
        {
            CreateNewFolderByName();
        }
        else if (P.StartArguments.FolderOptions == FolderOptions.AutoCreateById)
        {
            AutoCreateById();
        }
    }

    private static void CreateNewFolderById()
    {
        P.CleaningProjectInfo = new CleaningProjectInfo(P.StartArguments, P.StartArguments.CleaningProjectId);

        P.CleaningProjectNames = new CleaningProjectNames(FolderOptions.CreateNewFolderById, P.CleaningProjectInfo.Id);

        P.CleaningProjectDirs.SetFromPath(P.PathDirs.CleaningProjects, P.CleaningProjectNames);

        Dir.CreateAllDirsInObject<CleaningProjectDirs>(P.CleaningProjectDirs);

        P.CleaningProjectInfo.Save(P.CleaningProjectDirs.CleaningProjectInfo);
    }
    private static void CreateNewFolderByName()
    {
        throw new NotImplementedException();
    }
    private static void AutoCreateById()
    {
        P.CleaningProjectInfo = new CleaningProjectInfo(P.StartArguments, P.CleaningProjectsGlobalInfo.GetAndIncrementId());

        P.CleaningProjectNames = new CleaningProjectNames(FolderOptions.CreateNewFolderById, P.CleaningProjectInfo.Id);

        P.CleaningProjectDirs.SetFromPath(P.PathDirs.CleaningProjects, P.CleaningProjectNames);

        Dir.CreateAllDirsInObject<CleaningProjectDirs>(P.CleaningProjectDirs);

        P.CleaningProjectInfo.Save(P.CleaningProjectDirs.CleaningProjectInfo);
    }
}