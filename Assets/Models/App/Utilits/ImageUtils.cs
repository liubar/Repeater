using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace App
{
    // https://dukesoftware00.blogspot.com/2012/09/c-read-image-as-double-array-for-image.html
    public static class ImageUtils
    {
        // *** to Bitmap ***
        public static Bitmap ToBitmap(this double[,] doubleArray, int width, int height, PixelFormat pixelFormat)
        {
            var bmp = new Bitmap(width, height, pixelFormat);
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var bmpData = bmp.LockBits(rect, ImageLockMode.WriteOnly, bmp.PixelFormat);

            var ptr = bmpData.Scan0;

            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            var values = doubleArray.To1DimensionalArray();
            Marshal.Copy(values, 0, ptr, bytes);
            bmp.UnlockBits(bmpData);
            return bmp;
        }

        // *** to 2 dimensional double array ***
        public static double[,] To2DimDoubleArray(this Bitmap bmp)
        {
            var pixelFormat = bmp.PixelFormat;

            // should switch based on pixel format!
            if (pixelFormat.HasFlag(PixelFormat.Format24bppRgb))
            {
                return bmp.To2DimDoubleArray(3);
            }
            else if (pixelFormat.HasFlag(PixelFormat.Format32bppArgb))
            {
                return bmp.To2DimDoubleArray(4);
            }
            else
            {
                throw new NotImplementedException("Unsupported type of image:" + pixelFormat);
            }
        }

        private static double[,] To2DimDoubleArray(this Bitmap bmp, int block)
        {
            var rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            var bmpData = bmp.LockBits(rect, ImageLockMode.ReadOnly, bmp.PixelFormat);

            // Get the address of the first line.
            var ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            var rgbValues = new byte[bytes];

            // Copy the RGB values into the array.
            Marshal.Copy(ptr, rgbValues, 0, bytes);

            // Unlock the bits.
            bmp.UnlockBits(bmpData);

            return rgbValues.To2DimensionalArray(block);
        }

        private static double[,] To2DimensionalArray(this byte[] source, int block)
        {
            int len = source.Length / block;
            var ret = new double[block, len];
            int tail = block - 1;
            for (int i = 0, offset = 0; i < len; i++, offset = i * block)
            {
                for (int j = 0; j < block; j++)
                {
                    ret[tail - j, i] = source[offset + j];
                }
            }
            return ret;
        }

        private static byte[] To1DimensionalArray(this double[,] source)
        {
            int block = source.GetLength(0);
            var ret = new byte[source.Length * block];
            int len = source.Length / block;
            int tail = block - 1;
            for (int i = 0, offset = 0; i < len; i++, offset = i * block)
            {
                for (int j = 0; j < block; j++)
                {
                    ret[offset + j] = ToByte(source[tail - j, i]);
                }
            }
            return ret;
        }

        // does anyone know smarter (or faster) way to convert 0~255 double value to byte?
        private static byte ToByte(double raw)
        {
            if (raw > 255 || raw < 0)
            {
                throw new InvalidDataException("value should be in range [0, 255]");
            }
            return BitConverter.GetBytes((int)Math.Floor(raw))[0];
        }
    }
}
