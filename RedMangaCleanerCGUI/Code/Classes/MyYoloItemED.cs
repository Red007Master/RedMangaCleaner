public class MyYoloItem
{
    public string Type { get; set; }
    public double Confidence { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public MyYoloItem()
    {
        Type = "";
        Confidence = 0;
        X = 0;
        Y = 0;
        Width = 0;
        Height = 0;
    }
}
