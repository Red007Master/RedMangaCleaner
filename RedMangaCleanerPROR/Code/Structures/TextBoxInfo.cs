namespace RedMangaCleanerPROR.Code.Structures
{
    public class TextBoxInfo
    {
        public DetectedObject DetectedObject { get; set; }

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

        public TextBoxInfo(DetectedObject detectedObject)
        {
            SetFromMyYoloItem(detectedObject);
        }

        public void SetFromMyYoloItem(DetectedObject detectedObject)
        {
            DetectedObject = detectedObject;
            X = DetectedObject.Rectangle.X;
            Y = DetectedObject.Rectangle.Y;
            Height = DetectedObject.Rectangle.Height;
            Width = DetectedObject.Rectangle.Width;
        }
    }
}
