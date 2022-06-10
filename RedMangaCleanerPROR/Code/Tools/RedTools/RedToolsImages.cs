using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace RedsTools
{
    namespace Images
    {
        public static class Images
        {
            [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool DeleteObject([In] IntPtr hObject);

            //public static byte[,] BaWImageToByteArray(string inputPath)
            //{
            //    Bitmap buffer = new Bitmap(inputPath);
            //    byte[,] result = new byte[buffer.Width, buffer.Height];

            //    for (int x = 0; x < buffer.Width; x++)
            //    {
            //        for (int y = 0; y < buffer.Height; y++)
            //        {
            //            result[x, y] = buffer.GetPixel(x, y).G;
            //        }
            //    }

            //    return result;
            //}
            //public static Color[,] ColorImageToColorArray(string inputPath)
            //{
            //    Bitmap imageAsBitMap = new Bitmap(inputPath);
            //    Color[,] imageAsColorArray = new Color[imageAsBitMap.Width, imageAsBitMap.Height];

            //    for (int i = 0; i < imageAsBitMap.Width; i++)
            //    {
            //        for (int j = 0; j < imageAsBitMap.Height; j++)
            //        {
            //            Color buffer = imageAsBitMap.GetPixel(i, j);
            //            imageAsColorArray[i, j] = buffer;
            //        }
            //    }

            //    return imageAsColorArray;
            //}
            public static class BlackAndWhite
            {
                public static byte[,] ByteArrayFromBitmap(Bitmap inputBitmap)
                {
                    byte[,] result = new byte[inputBitmap.Width, inputBitmap.Height];

                    for (int x = 0; x < inputBitmap.Width; x++)
                    {
                        for (int y = 0; y < inputBitmap.Height; y++)
                        {
                            result[x, y] = inputBitmap.GetPixel(x, y).G;
                        }
                    }

                    return result;
                }

                public static DirectBitmap ByteArrayToDirectBitmap(byte[,] inputByteArray)
                {
                    int width = inputByteArray.GetLength(0);
                    int height = inputByteArray.GetLength(1);

                    DirectBitmap result = new DirectBitmap(inputByteArray.GetLength(0), inputByteArray.GetLength(1));

                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            result.SetPixel(x, y, Color.FromArgb(inputByteArray[x, y], inputByteArray[x, y], inputByteArray[x, y]));
                        }
                    }

                    return result;
                }
            }
            public static class RGB
            {
                public static Color[,] BitmapToColorArray(Bitmap inputBitmap)
                {
                    Color[,] result = new Color[inputBitmap.Width, inputBitmap.Height];

                    for (int x = 0; x < inputBitmap.Width; x++)
                    {
                        for (int y = 0; y < inputBitmap.Height; y++)
                        {
                            result[x, y] = inputBitmap.GetPixel(x, y);
                        }
                    }

                    return result;
                }
                public static DirectBitmap ColorArrayToDirectBitmap(Color[,] inputImageAsColorArray)
                {
                    int width = inputImageAsColorArray.GetLength(0);
                    int height = inputImageAsColorArray.GetLength(1);

                    DirectBitmap result = new DirectBitmap(width, height);

                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            result.SetPixel(x, y, inputImageAsColorArray[x, y]);
                        }
                    }

                    return result;
                }
            }

            public static DirectBitmap DirectBitmapFromPath(string inputPath) //dev tool
            {
                try
                {
                    Bitmap bitmap = new Bitmap(inputPath);

                    int width = bitmap.Width;
                    int height = bitmap.Height;

                    DirectBitmap result = new DirectBitmap(width, height);

                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            result.SetPixel(x, y, bitmap.GetPixel(x, y));
                        }
                    }

                    return result;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            public static System.Windows.Media.ImageSource ImageSourceFromBitmap(System.Drawing.Bitmap bmp)
            {
                var handle = bmp.GetHbitmap();
                try
                {
                    return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
                }
                finally { DeleteObject(handle); }
            }
        }
    }
}

