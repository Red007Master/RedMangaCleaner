using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedsCleaningProject
{
    namespace Settings
    {
        public class RectangleSettings
        {
            public bool Draw { get; set; }
            public Rectangle Rectangle { get; set; }

            public bool AutoRectangleBorderThickness { get; set; }
            public int RectangleBorderThickness { get; set; }

            public bool AutoBorderColor { get; set; }
            public Color BorderColor { get; set; }

            public RectangleSettings()
            {
                Draw = true;

                AutoRectangleBorderThickness = false;
                RectangleBorderThickness = 5;

                AutoBorderColor = false;
                BorderColor = Color.Red;
            }
            public RectangleSettings(Rectangle rectangle) : this()
            {
                Rectangle = rectangle;
            }
        }

        public class TextBoxFillingSettings
        {
            public bool FillAsBlackAndWhite { get; set; }

            public byte PixelGrayScaleLimit { get; set; }

            public bool AutomaticCroshairLineLenght { get; set; }
            public int CroshairLineLenght { get; set; }

            public Color FillingDisplayColor { get; set; }
            public Color FillingFinalColor { get; set; }


            public TextBoxFillingSettings()
            {
                FillAsBlackAndWhite = true;

                PixelGrayScaleLimit = 245;

                AutomaticCroshairLineLenght = false;
                CroshairLineLenght = 10;

                FillingDisplayColor = Color.DarkGreen;
                FillingFinalColor = Color.White;
            }
        }

        public class TextSettings
        {
            public bool DrawConfidence { get; set; }
            public FontSettings ConfidenceFontSettings { get; set; }

            public bool DrawClassName { get; set; }
            public FontSettings ClassNameFontSettings { get; set; }

            public bool DrawBackground { get; set; }
            public Color BackgroundColor { get; set; }

            public TextSettings()
            {
                DrawConfidence = true;
                ConfidenceFontSettings = new FontSettings();

                DrawClassName = true;
                ClassNameFontSettings = new FontSettings();

                DrawBackground = true;
                BackgroundColor = Color.DarkRed;
            }
        }
        public class FontSettings
        {
            public Font Font { get { return new Font(FontName, FontSize, GraphicsUnit.Pixel); } set { } }

            public bool Draw { get; set; }

            public string FontName { get; set; }

            public bool AutoFontSize { get; set; }
            public int FontSize { get; set; }

            public bool AutoFontColor { get; set; }
            public Color FontColor { get; set; }

            public FontSettings()
            {
                Draw = true;

                FontName = "Consolas";

                AutoFontColor = true;
                FontSize = 30;

                AutoFontColor = false;
                FontColor = Color.White;
            }
        }
    }
}
