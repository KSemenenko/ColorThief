using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using CoreGraphics;
using UIKit;

namespace ColorThiefDotNet
{
    public partial class ColorThief
    {
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
            var palette = GetPalette(sourceImage, 1, quality, ignoreWhite);
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
            var pixelArray = GetPixelsFast(sourceImage, quality, ignoreWhite);
            var cmap = GetColorMap(pixelArray, colorCount);
            return cmap != null ? cmap.GeneratePalette() : new List<QuantizedColor>();
        }

        private byte[] GetIntFromPixel(UIImage bmp)
        {
            var width = (int)bmp.Size.Width;
            var height = (int)bmp.Size.Height;
            using(var colourSpace = CGColorSpace.CreateDeviceRGB())
            {
                var rawData = Marshal.AllocHGlobal(width * height * 4);
                using(var context = new CGBitmapContext(rawData, width, height, 8, 4 * width, colourSpace, CGImageAlphaInfo.PremultipliedLast))
                {
                    context.DrawImage(new CGRect(0, 0, width, height), bmp.CGImage);
                    var pixelData = new byte[width * height * 4];
                    Marshal.Copy(rawData, pixelData, 0, pixelData.Length);
                    Marshal.FreeHGlobal(rawData);

                    return pixelData;
                }
            }
        }

        private byte[][] GetPixelsFast(UIImage sourceImage, int quality, bool ignoreWhite)
        {
            if(quality < 1)
            {
                quality = DefaultQuality;
            }
            var pixels = GetIntFromPixel(sourceImage);
            var pixelCount = (int)sourceImage.Size.Width * (int)sourceImage.Size.Height;

            return ConvertPixels(pixels, pixelCount, quality, ignoreWhite);
        }
    }
}