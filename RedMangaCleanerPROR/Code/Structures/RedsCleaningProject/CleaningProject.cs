using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace RedsCleaningProject
{
    namespace Core
    {
        public class CleaningProjectPathCoreClass
        {
            public string Core { get; set; }

            public string Data { get; set; }
            public string ObjectDetectionData { get; set; }
            public string CleaningConfigs { get; set; }
            public string PathToIdMap { get; set; }
            public string CleaningProjectInfo { get; set; }
            public string Images { get; set; }
            public string BlackAndWhiteImages { get; set; }
            public string SourceImages { get; set; }
            public string ResultImages { get; set; }
        }
        public class CleaningProjectDirs : CleaningProjectPathCoreClass
        {
            public CleaningProjectDirs(string inputProcessingPath, CleaningProjectNames inputCleaningProjectNames)
            {
                this.Core = inputProcessingPath + @"\" + inputCleaningProjectNames.Core;

                this.CleaningProjectInfo = this.Core + @"\" + inputCleaningProjectNames.CleaningProjectInfo;

                this.Data = this.Core + @"\" + inputCleaningProjectNames.Data;
                this.Images = this.Core + @"\" + inputCleaningProjectNames.Images;

                this.ObjectDetectionData = this.Data + @"\" + inputCleaningProjectNames.ObjectDetectionData;
                this.CleaningConfigs = this.Data + @"\" + inputCleaningProjectNames.CleaningConfigs;
                this.PathToIdMap = this.Data + @"\" + inputCleaningProjectNames.PathToIdMap;

                this.BlackAndWhiteImages = this.Images + @"\" + inputCleaningProjectNames.BlackAndWhiteImages;
                this.SourceImages = this.Images + @"\" + inputCleaningProjectNames.SourceImages;
                this.ResultImages = this.Images + @"\" + inputCleaningProjectNames.ResultImages;
            }

            public CleaningProjectDirs() { }
        }
        public class CleaningProjectNames : CleaningProjectPathCoreClass
        {
            public CleaningProjectNames(string projectFolderName)
            {
                Core = projectFolderName;

                Data = "Data";                                         //1d
                ObjectDetectionData = "ObjectDetectionData.json";      //12f
                CleaningConfigs = "CleaningConfigs.json";              //12f
                PathToIdMap = "PathToIdMap.json";                      //12f
                CleaningProjectInfo = "CleaningProjectInfo.json";      //1f
                Images = "Images";                                     //1d
                BlackAndWhiteImages = "BlackAndWhiteImages";           //12d
                SourceImages = "SourceImages";                         //12d 
                ResultImages = "ResultImages";                         //12d
            }
        }


        public class CleaningProjectCreationArguments
        {
            public FolderOptions FolderOptions { get; set; }

            public int CleaningProjectId { get; set; }

            public bool OutputBlackAndWhiteImages { get; set; }
            public bool ConductObjectDetectionOnBlackAndWhiteVariants { get; set; }
            public bool ConductTextBoxFillingOnBlackAndWhiteVariants { get; set; }

            public string UserTag { get; set; }
            public string InputPath { get; set; }

            public CleaningProjectCreationArguments()
            {
                FolderOptions = FolderOptions.AutoCreateById;

                CleaningProjectId = -1;

                OutputBlackAndWhiteImages = true;
                ConductObjectDetectionOnBlackAndWhiteVariants = true;
                ConductTextBoxFillingOnBlackAndWhiteVariants = true;

                UserTag = "EmptyDefault";
                InputPath = "None";
            }
        }
        public class CleaningProjectInfo
        {
            public int Id { get; set; }

            public int ImageCount { get; set; }
            public string UserTag { get; set; }
            public DateTime CreateTime { get; set; }
            public bool IsPRORFinished { get; set; }

            public CleaningProjectInfo()
            {
                CreateTime = DateTime.Now;
                IsPRORFinished = false;
                UserTag = "Empty_Tag";
            }

            public CleaningProjectInfo(CleaningProjectCreationArguments cleaningProjectCreationArguments, int id) : this()
            {
                Id = id;
                ImageCount = Directory.GetFiles(cleaningProjectCreationArguments.InputPath).Length;
                UserTag = cleaningProjectCreationArguments.UserTag;
            }
        }
        public class CleaningProjectConfig
        {
            public bool OutputBlackAndWhiteImages { get; set; }
            public bool ConductObjectDetectionOnBlackAndWhiteVariants { get; set; }
            public bool ConductTextBoxFillingOnBlackAndWhiteVariants { get; set; }

            public CleaningProjectConfig(CleaningProjectCreationArguments cleaningProjectCreationArguments)
            {
                OutputBlackAndWhiteImages = cleaningProjectCreationArguments.OutputBlackAndWhiteImages;
                ConductTextBoxFillingOnBlackAndWhiteVariants = cleaningProjectCreationArguments.ConductTextBoxFillingOnBlackAndWhiteVariants;
                ConductObjectDetectionOnBlackAndWhiteVariants = cleaningProjectCreationArguments.ConductObjectDetectionOnBlackAndWhiteVariants;
            }

            public CleaningProjectConfig() { }
        }

        public class CleaningProject : IComparable
        {
            public CleaningProjectNames CleaningProjectNames { get; set; }
            public CleaningProjectDirs CleaningProjectDirs { get; set; }


            public CleaningProjectInfo CleaningProjectInfo { get; set; }
            public CleaningProjectConfig CleaningProjectConfig { get; set; }
            public CleaningProjectCreationArguments CleaningProjectCreationArguments { get; set; }

            public CleaningProject(CleaningProjectCreationArguments cleaningProjectCreationArguments, string projectsFolderPath)
            {
                CleaningProjectCreationArguments = cleaningProjectCreationArguments;

                CleaningProjectConfig = new CleaningProjectConfig(cleaningProjectCreationArguments);

                int idToUse = 0;
                if (cleaningProjectCreationArguments.FolderOptions == FolderOptions.CreateNewFolderById)
                {
                    idToUse = cleaningProjectCreationArguments.CleaningProjectId;
                }
                else if (cleaningProjectCreationArguments.FolderOptions == FolderOptions.AutoCreateById)
                {
                    idToUse = P.CleaningProjectsGlobalInfo.GetAndIncrementId();
                }

                CleaningProjectInfo = new CleaningProjectInfo(cleaningProjectCreationArguments, idToUse);
                CleaningProjectNames = new CleaningProjectNames($@"CleaningProject_ID-[{idToUse}]");
                CleaningProjectDirs = new CleaningProjectDirs(projectsFolderPath, CleaningProjectNames);
            }

            public CleaningProject()
            {

            }

            public void Save(string iPath)
            {
                string serialized = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(iPath, serialized);
            }
            public void UpdatePath(string projectsFolderPath)
            {
                CleaningProjectDirs = new CleaningProjectDirs(projectsFolderPath, CleaningProjectNames);
            }
            public int CompareTo(object obj)
            {
                if (obj is CleaningProject)
                {
                    CleaningProject buffer = (CleaningProject)obj;

                    if (CleaningProjectInfo.Id > buffer.CleaningProjectInfo.Id)
                    {
                        return 1;
                    }
                    else if (CleaningProjectInfo.Id < buffer.CleaningProjectInfo.Id)
                    {
                        return -1;
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    P.Logger.Log("Error in CompareTo/CleaningProjectInfo", LogLevel.FatalError);
                    return 0;
                }
            }
        }
        public class CleaningProjectsGlobalInfo
        {
            private string Path { get; set; }

            public int IdCounter { get; set; }
            public int CriticalErrorsCounter { get; set; }

            public void Load()
            {
                if (File.Exists(Path))
                {
                    string serialized = File.ReadAllText(Path);

                    CleaningProjectsGlobalInfo buffer = JsonConvert.DeserializeObject<CleaningProjectsGlobalInfo>(serialized);

                    foreach (PropertyInfo propertyInfo in this.GetType().GetProperties())
                    {
                        var property = typeof(CleaningProjectsGlobalInfo).GetProperty(propertyInfo.Name);
                        var value = property.GetValue(buffer);

                        property.SetValue(this, value);
                    }
                }
                else
                {
                    CleaningProjectsGlobalInfo buffer = new CleaningProjectsGlobalInfo();

                    Save();
                }
            }
            public void Save()
            {
                string serialized = JsonConvert.SerializeObject(this);
                File.WriteAllText(Path, serialized);
            }

            public int GetAndIncrementId()
            {
                Load();

                int id = IdCounter;
                IdCounter++;

                Save();

                return id;
            }

            public CleaningProjectsGlobalInfo()
            {
                IdCounter = 0;
                CriticalErrorsCounter = 0;
            }
            public CleaningProjectsGlobalInfo(string path)
            {
                Path = path;

                Load();
            }
        }

        public enum FolderOptions
        {
            CreateNewFolderById = 0,
            AutoCreateById = 1,
        }
    }
}
