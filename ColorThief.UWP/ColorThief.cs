using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;

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
        public async Task<QuantizedColor> GetColor(BitmapDecoder sourceImage, int quality = DefaultQuality, bool ignoreWhite = DefaultIgnoreWhite)
        {
            var palette = await GetPalette(sourceImage, DefaultColorCount, quality, ignoreWhite);
            var dominantColor = palette.FirstOrDefault();
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
        public async Task<List<QuantizedColor>> GetPalette(BitmapDecoder sourceImage, int colorCount = DefaultColorCount, int quality = DefaultQuality, bool ignoreWhite = DefaultIgnoreWhite)
        {
            var cmap = await GetColorMap(sourceImage, colorCount, quality, ignoreWhite);
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
        private async Task<CMap> GetColorMap(BitmapDecoder sourceImage, int colorCount, int quality = DefaultQuality, bool ignoreWhite = DefaultIgnoreWhite)
        {
            var pixelArray = await GetPixelsFast(sourceImage, quality, ignoreWhite);

            // Send array to quantize function which clusters values using median
            // cut algorithm
            var cmap = Mmcq.Quantize(pixelArray, colorCount);
            return cmap;
        }

        private async Task<byte[]> GetIntFromPixel(BitmapDecoder decoder)
        {
            PixelDataProvider pixels = await decoder.GetPixelDataAsync(BitmapPixelFormat.Rgba16, BitmapAlphaMode.Premultiplied, new BitmapTransform(),
                ExifOrientationMode.IgnoreExifOrientation, ColorManagementMode.DoNotColorManage);

            return pixels.DetachPixelData();
        }

        private async Task<int[][]> GetPixelsFast(BitmapDecoder sourceImage, int quality, bool ignoreWhite)
        {
            var pixels = await GetIntFromPixel(sourceImage);
            var pixelCount = sourceImage.PixelWidth * sourceImage.PixelHeight;

            const int colorDepth = 4;

            var expectedDataLength = pixelCount * colorDepth;
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

            var numRegardedPixels = (quality <= 0) ? 0 : (pixelCount + quality - 1) / quality;

            var numUsedPixels = 0;
            var pixelArray = new int[numRegardedPixels][];

            for (var i = 0; i < pixelCount; i += quality)
            {
                var offset = i * 4;
                var b = (int)pixels[offset];
                var g = (int)pixels[offset + 1];
                var r = (int)pixels[offset + 2];
                var a = (int)pixels[offset + 3];

                // If pixel is mostly opaque and not white
                if (a >= 125 && !(ignoreWhite && r > 250 && g > 250 && b > 250))
                {
                    pixelArray[numUsedPixels] = new[] { r, g, b };
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