using System.Drawing;

public class FillPoint
{
    public Point Point { get; set; }
    public FillParam FillParam { get; set; }
    private byte[,] FillStatusBeforeApply { get; set; }

    public byte[,] GetFillStatusBeforeApply()
    {
        return FillStatusBeforeApply;
    }
    public void SetFillStatusBeforeApply(byte[,] inputArray)
    {
        FillStatusBeforeApply = inputArray;
    }

    public FillPoint(DetectedObject detectedObject)
    {
        Point = new Point(detectedObject.Rectangle.Width / 2, detectedObject.Rectangle.Height / 2);
        FillParam = FillParam.GetDefault();
    }
}
