public class TextBoxInfo
{
    public MyYoloItem MyYoloItem { get; set; }

    private byte[,] FilledPixelsAsByteArray { get; set; }

    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }

    public byte[,] GetFilledPixelsAsByteArray()
    {
        return FilledPixelsAsByteArray;
    }
    public void SetFilledPixelsAsByteArray(byte[,] inputArray)
    {
        FilledPixelsAsByteArray = inputArray;
    }

    public TextBoxInfo(MyYoloItem myYoloItem)
    {
        SetFromMyYoloItem(myYoloItem);
    }

    public void SetFromMyYoloItem(MyYoloItem myYoloItem)
    {
        MyYoloItem = myYoloItem;
        X = myYoloItem.X;
        Y = myYoloItem.Y;
        Height = myYoloItem.Height;
        Width = myYoloItem.Width;
    }
}