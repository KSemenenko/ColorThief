using System;
using System.Collections.Generic;
using System.Linq;
using Android.Graphics;

namespace ColorThief
{
    public partial class ColorThief
    {
        /// <summary>
        ///     Use the median cut algorithm to cluster similar colors and return the base color from the largest cluster.
        /// </summary>
        /// <param name="sourceImage">The source image.</param>
        /// <param name="quality">
        ///     1 is the highest quality settings. 10 is the default. There is
        ///     a trade-off between quality and speed. The bigger the number,
        ///     the faster a color will be returned but the greater the
        ///     likelihood that it will not be the visually most dominant color.
        /// </param>
        /// <param name="ignoreWhite">if set to <c>true</c> [ignore white].</param>
        /// <returns></returns>
        public QuantizedColor GetColor(Bitmap sourceImage, int quality = DefaultQuality, bool ignoreWhite = DefaultIgnoreWhite)
        {
            var palette = GetPalette(sourceImage, DefaultColorCount, quality, ignoreWhite);
            var dominantColor = palette.FirstOrDefault();
            return dominantColor;
        }

        /// <summary>
        ///     Use the median cut algorithm to cluster similar colors.
        /// </summary>
        /// <param name="sourceImage">The source image.</param>
        /// <param name="colorCount">The color count.</param>
        /// <param name="quality">
        ///     1 is the highest quality settings. 10 is the default. There is
        ///     a trade-off between quality and speed. The bigger the number,
        ///     the faster a color will be returned but the greater the
        ///     likelihood that it will not be the visually most dominant color.
        /// </param>
        /// <param name="ignoreWhite">if set to <c>true</c> [ignore white].</param>
        /// <returns></returns>
        /// <code>true</code>
        public List<QuantizedColor> GetPalette(Bitmap sourceImage, int colorCount = DefaultColorCount, int quality = DefaultQuality, bool ignoreWhite = DefaultIgnoreWhite)
        {
            var pixelArray = GetPixelsFast(sourceImage, quality, ignoreWhite);
            var cmap = GetColorMap(pixelArray, colorCount);
            return cmap != null ? cmap.GeneratePalette() : new List<QuantizedColor>();
        }

        private byte[] GetIntFromPixel(Bitmap bmp)
        {
            var pixelList = new byte[bmp.Width * bmp.Height*4];
            int[] intArray = new int[bmp.Width * bmp.Height];

            bmp.GetPixels(intArray, 0, bmp.Width, 0, 0, bmp.Width, bmp.Height);
            var count = 0;

            foreach (var item in intArray)
            {
                var clr = BitConverter.GetBytes(item);

                pixelList[count] = clr[0];
                count++;

                pixelList[count] = clr[1];
                count++;

                pixelList[count] = clr[2];
                count++;

                pixelList[count] = clr[3];
                count++;
            }

            return pixelList;
        }

        private byte[][] GetPixelsFast(Bitmap sourceImage, int quality, bool ignoreWhite)
        {
            if(quality < 1)
            {
                quality = DefaultQuality;
            }

            var pixels = GetIntFromPixel(sourceImage);
            var pixelCount = sourceImage.Width*sourceImage.Height;

            return ConvertPixels(pixels, pixelCount, quality, ignoreWhite);
        }
    }
}