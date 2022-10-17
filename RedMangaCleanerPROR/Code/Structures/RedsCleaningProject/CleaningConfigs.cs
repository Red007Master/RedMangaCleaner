using RedMangaCleanerPROR.Code.Structures;
using RedsCleaningProject.RedImages;
using RedsTools.Types;
using System.Collections.Generic;
using System.Drawing;

namespace RedsCleaningProject
{
    namespace CleaningConfigs
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
            public Rectangle RectangleShift { get; set; }

            public bool AutoRectangleBorderThickness { get; set; }
            public int RectangleBorderThickness { get; set; }

            public bool AutoBorderColor { get; set; }
            public ColorConfig BorderColor { get; set; }

            public RectangleConfig() : base()
            {
                RectangleShift = new Rectangle();

                AutoRectangleBorderThickness = false;
                RectangleBorderThickness = 5;

                AutoBorderColor = false;
                BorderColor = new ColorConfig(false, Color.Red, Color.Red);
            }
            public RectangleConfig(RectangleConfig rectangleConfig) : this()
            {
                TypeConverter.ChildParentSetTo(rectangleConfig, this);
            }

            public static Rectangle ModifyRectangleAcordingToShiftRectangle(Rectangle rectangleTarget, Rectangle rectangleShift)
            {
                int x = rectangleTarget.X + rectangleShift.X;
                int y = rectangleTarget.Y + rectangleShift.Y;
                int width = rectangleTarget.Width + rectangleShift.Width;
                int height = rectangleTarget.Height + rectangleShift.Height;

                return new Rectangle(x, y, width, height);
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
            public TextConfig(TextConfig textConfig)
            {
                TypeConverter.ChildParentSetTo(textConfig, this);
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

            public TextBoxFillingConfig(TextBoxFillingConfig textBoxFillingConfig)
            {
                TypeConverter.ChildParentSetTo(textBoxFillingConfig, this);
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

        public class RedImageCleaningConfig
        {
            public string FileName { get; set; }

            public List<EditableObjectCleaningConfig> EditableObjectCleaningConfigs { get; set; }

            public RedImageCleaningConfig(BasicImageData basicImageData)
            {
                FileName = basicImageData.FileName;

                EditableObjectCleaningConfigs = new List<EditableObjectCleaningConfig>();
                for (int i = 0; i < basicImageData.DetectedObjects.Count; i++)
                    EditableObjectCleaningConfigs.Add(new EditableObjectCleaningConfig(P.Defaults));
            }

            public RedImageCleaningConfig() { }
        }

        public class EditableObjectCleaningConfig
        {
            public RectangleConfig RectangleConfig { get; set; }
            public TextConfig TextConfig { get; set; }
            public TextBoxFillingConfig TextBoxFillingConfig { get; set; }

            public EditableObjectCleaningConfig()
            {
                RectangleConfig = new RectangleConfig();
                TextConfig = new TextConfig();
                TextBoxFillingConfig = new TextBoxFillingConfig();
            }

            public EditableObjectCleaningConfig(Defaults defaults)
            {
                RectangleConfig = new RectangleConfig(defaults.RectangleConfig);
                TextConfig = new TextConfig(defaults.TextConfig);
                TextBoxFillingConfig = new TextBoxFillingConfig(defaults.TextBoxFillingConfig);
            }
        }
    }
}
