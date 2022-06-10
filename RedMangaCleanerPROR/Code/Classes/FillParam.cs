using System.Drawing;

public class FillParam
{
    public bool IsFillParamIsCustomSet { get; set; }

    public Color FillTargetColorRGB { get; set; }
    public byte MaxDeviationFromR { get; set; }
    public byte MaxDeviationFromG { get; set; }
    public byte MaxDeviationFromB { get; set; }

    public byte FillTargetColorBaW { get; set; }
    public byte MaxDeviationFromBaW { get; set; }

    public static FillParam GetDefault()
    {
        FillParam result = new FillParam();

        result.FillTargetColorBaW = P.Settings.SettingsList.DefaultFillTargetColorBaW;
        result.MaxDeviationFromBaW = P.Settings.SettingsList.DefaultMaxDeviationFromBaW;

        result.FillTargetColorRGB = Color.White;
        result.MaxDeviationFromR = 10;
        result.MaxDeviationFromG = 10;
        result.MaxDeviationFromB = 10; //TODO add to setings

        return result;
    }
}
