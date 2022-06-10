using Alturos.Yolo;

class MyYoloConfiguration
{
    public string ConfigFile { get; set; }
    public string WeightsFile { get; set; }
    public string NamesFile { get; set; }

    public YoloConfiguration GetYoloConfig()
    {
        return new YoloConfiguration(ConfigFile, WeightsFile, NamesFile);
    }
}
