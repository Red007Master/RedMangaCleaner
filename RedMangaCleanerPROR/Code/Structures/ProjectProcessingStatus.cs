using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace RedMangaCleanerPROR.Code.Structures
{
    public class ProjectProcessingStatus
    {
        public bool IsPRORFinished { get; set; } = false;
        public bool IsFinished { get; set; } = false;

        private string SavePath { get; set; }

        public Status Status { get; set; }

        public Operation Operation { get; set; }

        public int ImagesToProcess { get; set; }
        public int ProcessedImages { get; set; }

        #region Save/Load

        public void Save()
        {
            try
            {
                string serialized = JsonConvert.SerializeObject(this);
                File.WriteAllText(SavePath, serialized);
            }
            catch (Exception)
            { }
        }
        public void Load()
        {
            try
            {
                string serialized = File.ReadAllText(SavePath);
                ProjectProcessingStatus buffer = JsonConvert.DeserializeObject<ProjectProcessingStatus>(serialized);

                foreach (PropertyInfo propertyInfo in this.GetType().GetProperties())
                {
                    var property = typeof(ProjectProcessingStatus).GetProperty(propertyInfo.Name);
                    var value = property.GetValue(buffer);

                    property.SetValue(this, value);
                }
            }
            catch (System.Exception)
            { }
        }
        public void FinishPROR()
        {
            IsPRORFinished = true;
            ImagesToProcess = 0;
            ProcessedImages = 0;
            Save();
        }

        #region SetSave

        public void Set(Status status)
        {
            Status = status;
        }
        public void Set(Operation operation)
        {
            Operation = operation;
        }
        public void Set(Status status, Operation operation)
        {
            Status = status;
            Operation = operation;
        }
        public void Set(Operation operation, int imagesToProcess)
        {
            Operation = operation;
            ImagesToProcess = imagesToProcess;
        }
        public void Set(int processedImages)
        {
            ProcessedImages = processedImages;
        }
        public void Set(int imagesToProcess, int processedImages)
        {
            ImagesToProcess = imagesToProcess;
            ProcessedImages = processedImages;
        }
        public void Set(Status status, Operation operation, int imagesToProcess, int processedImages)
        {
            Status = status;
            Operation = operation;
            ImagesToProcess = imagesToProcess;
            ProcessedImages = processedImages;
        }
        public void Set(Operation operation, int imagesToProcess, int processedImages)
        {
            Operation = operation;
            ImagesToProcess = imagesToProcess;
            ProcessedImages = processedImages;
        }


        #endregion

        #endregion

        public ProjectProcessingStatus(string iPath)
        {
            SavePath = iPath;

            IsPRORFinished = false;
            IsFinished = false;
            Status = Status.IsBooting;
            Operation = Operation.IsCopyingImages;
            ImagesToProcess = 0;
            ProcessedImages = 0;

            Save();
        }
    }

    public enum Operation
    {
        IsCopyingImages = 0,
        IsConvertingToGrayscale = 1,
        IsDetectingObjects = 2,
        IsJsonSerializeing = 3,
        IsPrecompiling = 4,
    }

    public enum Status
    {
        IsBooting = 0,
        IsRunning = 1,
        IsFinished = 3,
    }
}
