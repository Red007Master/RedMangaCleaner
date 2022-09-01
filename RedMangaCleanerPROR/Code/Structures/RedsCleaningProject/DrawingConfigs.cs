using System.Drawing;

namespace RedsCleaningProject
{
    namespace DrawingConfigs
    {
        public class CoreConfig
        {
            public bool Draw { get; set; }
            public bool IsDrawed { get; set; }



            public CoreConfig()
            {
                Draw = true;
                IsDrawed = false;
            }
        }

        public class RectangleConfig : CoreConfig
        {
            public Rectangle Rectangle { get; set; }

            public bool AutoRectangleBorderThickness { get; set; }
            public int RectangleBorderThickness { get; set; }

            public bool AutoBorderColor { get; set; }
            public ColorConfig BorderColor { get; set; }

            public RectangleConfig() : base()
            {
                AutoRectangleBorderThickness = false;
                RectangleBorderThickness = 5;

                AutoBorderColor = false;
                BorderColor = new ColorConfig(false, Color.Red, Color.Red);
            }
            public RectangleConfig(Rectangle rectangle) : this()
            {
                Rectangle = rectangle;
            }
        }
        public class TextConfig : CoreConfig
        {
            public bool DrawConfidence { get; set; }
            public FontConfig ConfidenceFontSettings { get; set; }

            public bool DrawClassName { get; set; }
            public FontConfig ClassNameFontSettings { get; set; }

            public ColorConfig BackgroundColor { get; set; }

            public TextConfig()
            {
                DrawConfidence = true;
                ConfidenceFontSettings = new FontConfig();

                DrawClassName = true;
                ClassNameFontSettings = new FontConfig();

                BackgroundColor = new ColorConfig(false, Color.DarkRed, Color.DarkRed);
            }
        }
        public class TextBoxFillingConfig : CoreConfig
        {
            public byte PixelGrayScaleLimit { get; set; }

            public bool AutomaticCroshairLineLenght { get; set; }
            public int CroshairLineLenght { get; set; }

            public ColorConfig FillingColor { get; set; }

            public TextBoxFillingConfig() : base()
            {
                PixelGrayScaleLimit = 245;

                AutomaticCroshairLineLenght = false;
                CroshairLineLenght = 10;

                FillingColor = new ColorConfig(true, Color.Green, Color.Green);
            }
        }


        public class FontConfig
        {
            public bool Draw { get; set; }

            public Font Font { get { return new Font(FontName, FontSize, GraphicsUnit.Pixel); } }

            public string FontName { get; set; }

            public bool AutoFontSize { get; set; }
            public int FontSize { get; set; }

            public ColorConfig FontColor { get; set; }

            public FontConfig()
            {
                Draw = true;

                FontName = "Consolas";

                FontSize = 30;

                FontColor = new ColorConfig(false, Color.White, Color.White);
            }
        }
        public class ColorConfig
        {
            public bool UseAutoColor { get; set; }

            public Color DisplayColor { get; set; }
            public Color FinalColor { get; set; }

            public ColorConfig(bool useAutoColor, Color displayColor, Color finalColor)
            {
                UseAutoColor = useAutoColor;

                DisplayColor = displayColor;
                FinalColor = finalColor;
            }
        }
    }
}
