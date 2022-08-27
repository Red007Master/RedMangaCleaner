using Newtonsoft.Json;
using RedsTools.Images;
using RedsTools.Types;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace RedsCleaningProject
{
    namespace Core
    {
        public class StartArguments
        {
            public FolderOptions FolderOptions { get; set; }

            public int CleaningProjectId { get; set; }
            public string CleaningProjectFolderName { get; set; }

            public bool OutputBlackAndWhiteImages { get; set; }
            public bool ConductObjectDetectionOnBlackAndWhiteVariants { get; set; }
            public bool ConductTextBoxFillingOnBlackAndWhiteVariants { get; set; }

            public string UserTag { get; set; }
            public string InputPath { get; set; }
        }

        //public class CleaningProject
        //{
        //    public CleaningProjectInfo CleaningProjectInfo { get; set; }

        //    public List<BasicImageData> BasicImageDatas { get; set; }
        //    public List<RedImageCore> RedImageCores { get; set; }
        //    public List<RedImageFull> RedImageFulls { get; set; }

        //    public FillParam DefFillParam { get; set; }
        //}

        public class CleaningProjectInfo : IComparable
        {
            public int Id { get; set; }
            public bool OutputBlackAndWhiteImages { get; set; }
            public bool ConductObjectDetectionOnBlackAndWhiteVariants { get; set; }
            public bool ConductTextBoxFillingOnBlackAndWhiteVariants { get; set; }
            public int ImageCount { get; set; }
            public string ImageSourcePath { get; set; }
            public string UserTag { get; set; }
            public DateTime CreateTime { get; set; }
            public bool IsPRORFinished { get; set; }

            public CleaningProjectInfo(StartArguments inputArguments, int id)
            {
                Id = id;
                OutputBlackAndWhiteImages = inputArguments.OutputBlackAndWhiteImages;
                ConductTextBoxFillingOnBlackAndWhiteVariants = inputArguments.ConductTextBoxFillingOnBlackAndWhiteVariants;
                ConductObjectDetectionOnBlackAndWhiteVariants = inputArguments.ConductObjectDetectionOnBlackAndWhiteVariants;
                ImageCount = Directory.GetFiles(inputArguments.InputPath).Length;
                ImageSourcePath = inputArguments.InputPath;
                UserTag = inputArguments.UserTag;
                CreateTime = DateTime.Now;
                IsPRORFinished = false;
            }

            public CleaningProjectInfo(StartArguments inputArguments)
            {
                Id = inputArguments.CleaningProjectId;
                OutputBlackAndWhiteImages = inputArguments.OutputBlackAndWhiteImages;
                ConductTextBoxFillingOnBlackAndWhiteVariants = inputArguments.ConductTextBoxFillingOnBlackAndWhiteVariants;
                ConductObjectDetectionOnBlackAndWhiteVariants = inputArguments.ConductObjectDetectionOnBlackAndWhiteVariants;
                ImageCount = Directory.GetFiles(inputArguments.InputPath).Length;
                ImageSourcePath = inputArguments.InputPath;
                UserTag = inputArguments.UserTag;
                CreateTime = DateTime.Now;
                IsPRORFinished = false;
            }

            // TODO crate newtons json contructor
            public CleaningProjectInfo() { }

            public void Save(string iPath)
            {
                string serialized = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(iPath, serialized);
            }

            public int CompareTo(object obj)
            {
                if (obj is CleaningProjectInfo)
                {
                    CleaningProjectInfo buffer = (CleaningProjectInfo)obj;

                    if (Id > buffer.Id)
                    {
                        return 1;
                    }
                    else if (Id < buffer.Id)
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
            CreateNewFolderByName = 1,
            AutoCreateById = 2,
        }
    }
}
