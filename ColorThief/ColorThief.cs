using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using ColorThief.MMCO;

namespace ColorThief
{
    public class ColorThief
    {
        private const int DefaultColorCount = 5;
        private const int DefaultQuality = 10;
        private const bool DefaultIgnoreWhite = true;

        /// <summary>
        ///     Use the median cut algorithm to cluster similar colors and return the base color from the largest cluster.
        /// </summary>
        /// <param name="sourceImage">The source image.</param>
        /// <param name="quality">
        ///     0 is the highest quality settings. 10 is the default. There is
        ///     a trade-off between quality and speed. The bigger the number,
        ///     the faster a color will be returned but the greater the
        ///     likelihood that it will not be the visually most dominant color.
        /// </param>
        /// <param name="ignoreWhite">if set to <c>true</c> [ignore white].</param>
        /// <returns></returns>
        public QuantizedColor GetColor(Bitmap sourceImage, int quality = DefaultQuality, bool ignoreWhite = DefaultIgnoreWhite)
        {
            List<QuantizedColor> palette = GetPalette(sourceImage, DefaultColorCount, quality, ignoreWhite);
            QuantizedColor dominantColor = palette.FirstOrDefault();
            return dominantColor;
        }

        /// <summary>
        ///     Use the median cut algorithm to cluster similar colors.
        /// </summary>
        /// <param name="sourceImage">The source image.</param>
        /// <param name="colorCount">The color count.</param>
        /// <param name="quality">
        ///     0 is the highest quality settings. 10 is the default. There is
        ///     a trade-off between quality and speed. The bigger the number,
        ///     the faster a color will be returned but the greater the
        ///     likelihood that it will not be the visually most dominant color.
        /// </param>
        /// <param name="ignoreWhite">if set to <c>true</c> [ignore white].</param>
        /// <returns></returns>
        /// <code>true</code>
        public List<QuantizedColor> GetPalette(Bitmap sourceImage, int colorCount = DefaultColorCount, int quality = DefaultQuality, bool ignoreWhite = DefaultIgnoreWhite)
        {
            CMap cmap = GetColorMap(sourceImage, colorCount, quality, ignoreWhite);
            return cmap != null ? cmap.GeneratePalette() : new List<QuantizedColor>();
        }

        /// <summary>
        ///     Use the median cut algorithm to cluster similar colors.
        /// </summary>
        /// <param name="sourceImage">The source image.</param>
        /// <param name="colorCount">The color count.</param>
        /// <param name="quality">
        ///     0 is the highest quality settings. 10 is the default. There is
        ///     a trade-off between quality and speed. The bigger the number,
        ///     the faster a color will be returned but the greater the
        ///     likelihood that it will not be the visually most dominant color.
        /// </param>
        /// <param name="ignoreWhite">if set to <c>true</c> [ignore white].</param>
        /// <returns></returns>
        private CMap GetColorMap(Bitmap sourceImage, int colorCount, int quality = DefaultQuality, bool ignoreWhite = DefaultIgnoreWhite)
        {
            int[][] pixelArray = GetPixelsFast(sourceImage, quality, ignoreWhite);

            // Send array to quantize function which clusters values using median
            // cut algorithm
            CMap cmap = MMCQ.Quantize(pixelArray, colorCount);
            return cmap;
        }

        private IEnumerable<int> GetIntFromPixel(Bitmap bmp)
        {
            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    System.Drawing.Color clr = bmp.GetPixel(x, y);
                    yield return clr.B;
                    yield return clr.G;
                    yield return clr.R;
                    yield return clr.A;
                }
            }
        }

        private int[][] GetPixelsFast(Bitmap sourceImage, int quality, bool ignoreWhite)
        {
            IEnumerable<int> imageData = GetIntFromPixel(sourceImage);
            int[] pixels = imageData.ToArray();
            int pixelCount = sourceImage.Width * sourceImage.Height;

            const int colorDepth = 4;

            int expectedDataLength = pixelCount * colorDepth;
            if (expectedDataLength != pixels.Length)
            {
                throw new ArgumentException("(expectedDataLength = "
                                            + expectedDataLength + ") != (pixels.length = "
                                            + pixels.Length + ")");
            }

            // Store the RGB values in an array format suitable for quantize
            // function

            // numRegardedPixels must be rounded up to avoid an
            // ArrayIndexOutOfBoundsException if all pixels are good.
            int numRegardedPixels = (pixelCount + quality - 1) / quality;

            int numUsedPixels = 0;
            var pixelArray = new int[numRegardedPixels][];

            for (int i = 0; i < pixelCount; i += quality)
            {
                int offset = i * 4;
                int b = pixels[offset];
                int g = pixels[offset + 1];
                int r = pixels[offset + 2];
                int a = pixels[offset + 3];

                // If pixel is mostly opaque and not white
                if (a >= 125 && !(ignoreWhite && r > 250 && g > 250 && b > 250))
                {
                    pixelArray[numUsedPixels] = new[] {r, g, b};
                    numUsedPixels++;
                }
            }

            // Remove unused pixels from the array
            var copy = new int[numUsedPixels][];
            Array.Copy(pixelArray, copy, numUsedPixels);
            return copy;
        }
    }
}