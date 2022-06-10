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

    public FillPoint(MyYoloItem myYoloItem)
    {
        Point = new Point(myYoloItem.Width / 2, myYoloItem.Height / 2);
        FillParam = FillParam.GetDefault();
    }
}
