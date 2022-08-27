using RedsCleaningProject.Settings;
using RedsCleaningProject.MaskWorking;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedsCleaningProject
{
    namespace EditableObjects
    {
        public class RedsMask
        {
            public Point ShiftRelativelyToBitmap { get; set; }

            public int Width { get; set; }
            public int Height { get; set; }

            public byte[,] Mask { get; set; }

            private int PaletteMaxIndex { get; set; }
            public Color[] Palette { get; set; }

            public byte GetOrSetAndGetColorId(Color iColor)
            {
                int result = 0;

                byte iR = iColor.R;
                byte iG = iColor.G;
                byte iB = iColor.B;

                bool isAlredyExist = false;

                for (int i = 0; i < PaletteMaxIndex + 1; i++)
                {
                    byte pR = Palette[i].R;
                    byte pG = Palette[i].G;
                    byte pB = Palette[i].B;

                    if (iR == pR && iG == pG && iB == pB)
                    {
                        isAlredyExist = true;
                        result = i;
                        break;
                    }
                }

                if (!isAlredyExist)
                {
                    PaletteMaxIndex++;
                    Palette[PaletteMaxIndex] = Color.FromArgb(iR, iG, iB);
                    result = PaletteMaxIndex;
                }

                return Convert.ToByte(result);
            }

            public RedsMask(int width, int height, Point shiftRelativelyToBitmap = new Point())
            {
                Width = width;
                Height = height;

                Mask = new byte[Width, Height];

                PaletteMaxIndex = 0;
                Palette = new Color[255];
                Palette[0] = Color.Empty;

                ShiftRelativelyToBitmap = shiftRelativelyToBitmap;
            }
            public RedsMask(Rectangle rectangle) : this(rectangle.Width, rectangle.Height, new Point(rectangle.X, rectangle.Y)) { }
            public RedsMask(byte[,] mask) : this(mask.GetLength(0), mask.GetLength(1)) { Mask = mask; }
        }

        public class EditableObject
        {
            public DirectBitmap ParentDirectBitmap { get; set; }
            public Color[,] ParentColorArray { get; set; }

            public DetectedObject DetectedObject { get; set; }

            public RectangleSettings RectangleSettings { get; set; }
            public RedsMask RectangleMask { get; set; }

            public TextSettings TextSettings { get; set; }
            public RedsMask TextMask { get; set; }

            public void CalculateRectangleMask()
            {
                RectangleMask = MaskWork.DrawRectangleOnMask(DetectedObject, RectangleSettings);
            }
            public void CalculateRectangleMask(RectangleSettings rectangleSettings)
            {
                RectangleMask = MaskWork.DrawRectangleOnMask(DetectedObject, rectangleSettings);
            }

            public void CalculateTextMask()
            {
                TextMask = MaskWork.DrawTextOnMask(DetectedObject, TextSettings);
            }
            public void CalculateTextMask(TextSettings textSettings)
            {
                TextMask = MaskWork.DrawTextOnMask(DetectedObject, textSettings);
            }

            public void ApplyOverlayPixelsTo(DirectBitmap targetDirectBitmap)
            {
                MaskWork.FillByMask(targetDirectBitmap, RectangleMask);
                MaskWork.FillByMask(targetDirectBitmap, TextMask);
            }
            public void UnApplyOverlayPixelsTo(DirectBitmap targetDirectBitmap)
            {
                MaskWork.UnFillByMask(targetDirectBitmap, RectangleMask, ParentColorArray);
                MaskWork.UnFillByMask(targetDirectBitmap, TextMask, ParentColorArray);
            }

            public EditableObject(DetectedObject detectedObject)
            {
                DetectedObject = detectedObject;
                RectangleSettings = new RectangleSettings(detectedObject.Rectangle);
                TextSettings = new TextSettings();
            }
            public EditableObject(DetectedObject detectedObject, DirectBitmap parentDirectBitmap, Color[,] parentColorArray) : this(detectedObject)
            {
                ParentDirectBitmap = parentDirectBitmap;
                ParentColorArray = parentColorArray;
            }
        }
        public class TextBox : EditableObject
        {
            public RedsMask FillingMask { get; set; }

            public TextBoxFillingSettings FillingSettings { get; set; } = new TextBoxFillingSettings();

            public void CalculateTextBoxFillingMask()
            {
                FillingMask = MaskWork.DrawTextBoxFillingOnMask(this, ParentColorArray, FillingSettings);
            }
            public void CalculateTextBoxFillingMask(TextBoxFillingSettings textBoxFillingSettings)
            {
                FillingMask = MaskWork.DrawTextBoxFillingOnMask(this, ParentColorArray, textBoxFillingSettings);
            }

            public void ApplyFiledTextBoxPixelsTo(DirectBitmap targetDirectBitmap)
            {
                MaskWork.FillByMask(targetDirectBitmap, FillingMask);
            }
            public void UnApplyFiledTextBoxPixelsTo(DirectBitmap targetDirectBitmap)
            {
                MaskWork.UnFillByMask(targetDirectBitmap, FillingMask, ParentColorArray);
            }

            public TextBox(DetectedObject detectedObject) : base(detectedObject) { }
            public TextBox(DetectedObject detectedObject, DirectBitmap parentDirectBitmap, Color[,] parentColorArray) : base(detectedObject, parentDirectBitmap, parentColorArray) { }
            public TextBox(DetectedObject detectedObject, DirectBitmap parentDirectBitmap, Color[,] parentColorArray, TextBoxFillingSettings textBoxFillingSettings)
            : base(detectedObject, parentDirectBitmap, parentColorArray)
            {
                FillingSettings = textBoxFillingSettings;
            }
        }
    }
}
