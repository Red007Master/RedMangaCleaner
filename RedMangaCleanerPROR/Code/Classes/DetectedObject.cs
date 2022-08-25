using System;
using System.Collections.Generic;
using System.Drawing;
using Yolov5Net.Scorer;

public class DetectedObject
{
    public int Id { get; set; }
    public string Type { get; set; }
    public double Confidence { get; set; }
    public Rectangle Rectangle { get; set; }

    public DetectedObject()
    {
        Id = 0;
        Type = "";
        Confidence = 0;
        Rectangle = new Rectangle();
    }

    public DetectedObject(YoloPrediction yoloPrediction)
    {
        Id = yoloPrediction.Label.Id;
        Type = yoloPrediction.Label.Name;
        Confidence = yoloPrediction.Score;

        int x = Convert.ToInt32(yoloPrediction.Rectangle.X);
        int y = Convert.ToInt32(yoloPrediction.Rectangle.Y);
        int width = Convert.ToInt32(yoloPrediction.Rectangle.Width);
        int height = Convert.ToInt32(yoloPrediction.Rectangle.Height);
        Rectangle = new Rectangle(x, y, width, height);
    }

    public static List<DetectedObject> ConvertYPLToDetectedObjectList(List<YoloPrediction> yoloPredictions)
    {
        List<DetectedObject> detectedObjectsList = new List<DetectedObject>();

        for (int i = 0; i < yoloPredictions.Count; i++)
            detectedObjectsList.Add(new DetectedObject(yoloPredictions[i]));

        return detectedObjectsList;
    }
}
