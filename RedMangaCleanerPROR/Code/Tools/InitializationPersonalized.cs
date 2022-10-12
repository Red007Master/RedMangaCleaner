using RedsCleaningProject.Core;

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
        P.CleaningProject = new CleaningProject(P.StartArguments, P.PathDirs.CleaningProjects);

        Dir.CreateAllDirsInObject<CleaningProjectDirs>(P.CleaningProject.CleaningProjectDirs);
        P.CleaningProject.Save(P.CleaningProject.CleaningProjectDirs.CleaningProjectInfo);
    }
}