using Alturos.Yolo.Model;
using System.Collections.Generic;

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
    public MyYoloItem(YoloItem inputYI)
    {
        Type = inputYI.Type;
        Confidence = inputYI.Confidence;
        X = inputYI.X;
        Y = inputYI.Y;
        Width = inputYI.Width;
        Height = inputYI.Height;
    }

    public static List<MyYoloItem> ConvertYIListToMyYiList(List<YoloItem> inputYoloItemList)
    {
        List<MyYoloItem> result = new List<MyYoloItem>();

        for (int i = 0; i < inputYoloItemList.Count; i++)
        {
            result.Add(new MyYoloItem(inputYoloItemList[i]));
        }

        return result;
    }
}
