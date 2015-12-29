#region License

// Copyright (c) 2015 Harold Martinez-Molina <hanthonym@outlook.com>
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.

//http://blog.zumicts.com/how-to-get-a-color-palette-from-an-image-in-uwp-cwin10/

#endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;


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
        public static MMCQ.QuantizedColor GetColor(
            Bitmap sourceImage,
            int quality = DefaultQuality,
            bool ignoreWhite = DefaultIgnoreWhite)
        {
            var palette = GetPalette(sourceImage, DefaultColorCount, quality, ignoreWhite);
            var dominantColor = palette?[0];
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
        public static List<MMCQ.QuantizedColor> GetPalette(
            Bitmap sourceImage,
            int colorCount = DefaultColorCount,
            int quality = DefaultQuality,
            bool ignoreWhite = DefaultIgnoreWhite)
        {
            var cmap = GetColorMap(sourceImage, colorCount, quality, ignoreWhite);
            return cmap?.GeneratePalette();
        }

        /// <summary>
        ///     Use the median cut algorithm to cluster similar colors.
        /// </summary>
        /// <param name="sourceImage">The source image.</param>
        /// <param name="colorCount">The color count.</param>
        /// <returns></returns>
        public static MMCQ.CMap GetColorMap(Bitmap sourceImage, int colorCount)
        {
            return GetColorMap(
                sourceImage,
                colorCount,
                DefaultQuality,
                DefaultIgnoreWhite);
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
        public static MMCQ.CMap GetColorMap(
            Bitmap sourceImage,
            int colorCount,
            int quality,
            bool ignoreWhite)
        {
            var pixelArray = GetPixelsFast(sourceImage, quality, ignoreWhite);

            // Send array to quantize function which clusters values using median
            // cut algorithm
            var cmap = MMCQ.Quantize(pixelArray, colorCount);
            return cmap;
        }

        public static IEnumerable<int> GetIntFromPixel(Bitmap bmp)
        {
            for (var x = 0; x < bmp.Width; x++)
            {
                for (var y = 0; y < bmp.Height; y++)
                {
                    var clr = bmp.GetPixel(x, y);
                    yield return clr.B;
                    yield return clr.G;
                    yield return clr.R;
                    yield return clr.A;
                }
            }


        }

        private static int[][] GetPixelsFast(
            Bitmap sourceImage,
            int quality,
            bool ignoreWhite)
        {
            var imageData = GetIntFromPixel(sourceImage);
            var pixels = imageData.ToArray();
            var pixelCount = sourceImage.Width*sourceImage.Height;

            var colorDepth = 4;

            var expectedDataLength = pixelCount*colorDepth;
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
            var numRegardedPixels = (pixelCount + quality - 1)/quality;

            var numUsedPixels = 0;
            var pixelArray = new int[numRegardedPixels][];


            for (var i = 0; i < pixelCount; i += quality)
            {
                var offset = i*4;
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