using Newtonsoft.Json;
using RedsCleaningProject.CleaningConfigs;
using System.IO;

namespace RedMangaCleanerPROR.Code.Structures
{
    public class Defaults
    {
        public PathDirs PathDirs { get; set; }

        public string DefaultFileName = "default.json";

        public string DefaultRectangleConfigPath { get; set; }
        public string DefaultTextConfigPath { get; set; }
        public string DefaultTextBoxFillingConfigPath { get; set; }

        public RectangleConfig RectangleConfig { get; set; }
        public TextConfig TextConfig { get; set; }
        public TextBoxFillingConfig TextBoxFillingConfig { get; set; }

        public Defaults(PathDirs pathDirs)
        {
            PathDirs = pathDirs;

            DefaultRectangleConfigPath = PathDirs.RectangleConfigs + @"\" + DefaultFileName;
            DefaultTextConfigPath = PathDirs.TextConfigs + @"\" + DefaultFileName;
            DefaultTextBoxFillingConfigPath = PathDirs.TextBoxFillingConfigs + @"\" + DefaultFileName;
        }

        public void LoadDefaults()
        {
            RectangleConfig = JsonConvert.DeserializeObject<RectangleConfig>(File.ReadAllText(DefaultRectangleConfigPath));
            TextConfig = JsonConvert.DeserializeObject<TextConfig>(File.ReadAllText(DefaultTextConfigPath));
            TextBoxFillingConfig = JsonConvert.DeserializeObject<TextBoxFillingConfig>(File.ReadAllText(DefaultTextBoxFillingConfigPath));
        }
    }
}
