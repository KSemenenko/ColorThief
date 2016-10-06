using System;

namespace ColorThief
{
    public partial class ColorThief
    {
        private const int DefaultColorCount = 5;
        private const int DefaultQuality = 10;
        private const bool DefaultIgnoreWhite = true;

        /// <summary>
        ///     Use the median cut algorithm to cluster similar colors.
        /// </summary>
        /// <param name="pixelArray">Pixel array.</param>
        /// <param name="colorCount">The color count.</param>
        /// <returns></returns>
        private CMap GetColorMap(int[][] pixelArray, int colorCount)
        {
            // Send array to quantize function which clusters values using median
            // cut algorithm
            var cmap = Mmcq.Quantize(pixelArray, colorCount);
            return cmap;
        }

        private int[][] ConvertPixels(int[] pixels, int pixelCount, int quality, bool ignoreWhite)
        {
            const int colorDepth = 4;

            var expectedDataLength = pixelCount*colorDepth;
            if(expectedDataLength != pixels.Length)
            {
                throw new ArgumentException("(expectedDataLength = "
                                            + expectedDataLength + ") != (pixels.length = "
                                            + pixels.Length + ")");
            }

            // Store the RGB values in an array format suitable for quantize
            // function

            // numRegardedPixels must be rounded up to avoid an
            // ArrayIndexOutOfBoundsException if all pixels are good.

            var numRegardedPixels = (pixelCount + quality - 1)/quality;

            var numUsedPixels = 0;
            var pixelArray = new int[numRegardedPixels][];

            for(var i = 0; i < pixelCount; i += quality)
            {
                var offset = i*4;
                var b = pixels[offset];
                var g = pixels[offset + 1];
                var r = pixels[offset + 2];
                var a = pixels[offset + 3];

                // If pixel is mostly opaque and not white
                if(a >= 125 && !(ignoreWhite && r > 250 && g > 250 && b > 250))
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