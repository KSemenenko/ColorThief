using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Runtime.InteropServices;
using ColorThieftShared;
using CoreGraphics;
using UIKit;

namespace ColorThief
{
    public class ColorThief
    {
        private const int DefaultColorCount = 5;
        private const int DefaultQuality = 10;
        private const bool DefaultIgnoreWhite = true;

       // private IColorThiefBitmap bitmapConverter = new ColorThiefBitmap();

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
        public QuantizedColor GetColor(UIImage sourceImage, int quality = DefaultQuality, bool ignoreWhite = DefaultIgnoreWhite)
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
        public List<QuantizedColor> GetPalette(UIImage sourceImage, int colorCount = DefaultColorCount, int quality = DefaultQuality, bool ignoreWhite = DefaultIgnoreWhite)
        {
            var cmap = GetColorMap(sourceImage, colorCount, quality, ignoreWhite);
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
        private CMap GetColorMap(UIImage sourceImage, int colorCount, int quality = DefaultQuality, bool ignoreWhite = DefaultIgnoreWhite)
        {
            var pixelArray = GetPixelsFast(sourceImage, quality, ignoreWhite);

            // Send array to quantize function which clusters values using median
            // cut algorithm
            var cmap = Mmcq.Quantize(pixelArray, colorCount);
            return cmap;
        }

        private IEnumerable<int> GetIntFromPixel(UIImage bmp)
        {
            for (var x = 0; x < bmp.Size.Width; x++)
            {
                for (var y = 0; y < bmp.Size.Height; y++)
                {


                    var rawData = new byte[4];
                    var handle = GCHandle.Alloc(rawData);
                    try
                    {
                        using (var colorSpace = CGColorSpace.CreateDeviceRGB())
                        {
                            using (var context = new CGBitmapContext(rawData, 1, 1, 8, 4, colorSpace, CGImageAlphaInfo.PremultipliedLast))
                            {
                                context.DrawImage(new RectangleF(-x, y - Convert.ToInt32(bmp.Size.Height), Convert.ToInt32(bmp.Size.Width), 
                                    Convert.ToInt32(bmp.Size.Height)), bmp.CGImage);

                                //float red = (rawData[0]) / 255.0f;
                                //float green = (rawData[1]) / 255.0f;
                                //float blue = (rawData[2]) / 255.0f;
                                //float alpha = (rawData[3]) / 255.0f;

                                yield return (int)rawData[2]; //B;
                                yield return (int)rawData[1]; //G;
                                yield return (int)rawData[0]; //R;
                                yield return (int)rawData[3]; //A;

                            }
                        }
                    }
                    finally
                    {
                        handle.Free();
                    }

                }
            }
        }



        private int[][] GetPixelsFast(UIImage sourceImage, int quality, bool ignoreWhite)
        {
            if (quality < 1)
            {
                quality = DefaultQuality;
            }

            var imageData = GetIntFromPixel(sourceImage);
            var pixels = imageData.ToArray();
            var pixelCount = (int)sourceImage.Size.Width * (int)sourceImage.Size.Height;

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

            var numRegardedPixels = (pixelCount + quality - 1) / quality;

            var numUsedPixels = 0;
            var pixelArray = new int[numRegardedPixels][];

            for (var i = 0; i < pixelCount; i += quality)
            {
                var offset = i * 4;
                var b = pixels[offset];
                var g = pixels[offset + 1];
                var r = pixels[offset + 2];
                var a = pixels[offset + 3];

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