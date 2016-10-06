using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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

        private int[][] GetPixelsFast(Bitmap sourceImage, int quality, bool ignoreWhite)
        {
            if(quality < 1)
            {
                quality = DefaultQuality;
            }

            var imageData = GetIntFromPixel(sourceImage);
            var pixels = imageData.ToArray();
            var pixelCount = sourceImage.Width*sourceImage.Height;

            return ConvertPixels(pixels, pixelCount, quality, ignoreWhite);
        }

        private IEnumerable<int> GetIntFromPixel(Bitmap bmp)
        {
            for(var x = 0; x < bmp.Width; x++)
            {
                for(var y = 0; y < bmp.Height; y++)
                {
                    var clr = bmp.GetPixel(x, y);
                    yield return clr.B;
                    yield return clr.G;
                    yield return clr.R;
                    yield return clr.A;
                }
            }
        }
    }
}