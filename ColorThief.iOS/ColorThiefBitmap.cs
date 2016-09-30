using System;
using System.Runtime.InteropServices;
using ColorThieftShared;
using CoreGraphics;
using UIKit;

namespace ColorThief
{
    public class ColorThiefBitmap : IColorThiefBitmap
    {
        private const byte bitsPerComponent = 8;
        private const byte bytesPerPixel = 4;
        public int Height { get; set; }
        public int Width { get; set; }

        private UIImage image;
        private readonly UIImageOrientation orientation;
        private byte[] pixelData;
        private IntPtr rawData;

        public ColorThiefBitmap(UIImage uiImage)
        {
            image = uiImage;
            orientation = image.Orientation;
            Width = (int)image.CGImage.Width;
            Height = (int)image.CGImage.Height;
        }

        public byte[] ToPixelArray()
        {
            using(var colourSpace = CGColorSpace.CreateDeviceRGB())
            {
                rawData = Marshal.AllocHGlobal(Width * Height * 4);
                using(var context = new CGBitmapContext(rawData, Width, Height, bitsPerComponent, bytesPerPixel* Width, colourSpace, CGImageAlphaInfo.PremultipliedLast))
                {
                    context.DrawImage(new CGRect(0, 0, Width, Height), image.CGImage);
                    pixelData = new byte[Width * Height * bytesPerPixel];
                    Marshal.Copy(rawData, pixelData, 0, pixelData.Length);
                    Marshal.FreeHGlobal(rawData);

                    return pixelData;
                }
            }
        }

        public void TransformImage(Func<byte, byte, byte, double> pixelOperation)
        {
            byte r, g, b;

            // Pixel data order is RGBA
            try
            {
                for(var i = 0; i < pixelData.Length; i += bytesPerPixel)
                {
                    r = pixelData[i];
                    g = pixelData[i + 1];
                    b = pixelData[i + 2];

                    // Leave alpha value intact
                    pixelData[i] = pixelData[i + 1] = pixelData[i + 2] = (byte)pixelOperation(r, g, b);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public UIImage ToImage()
        {
            using(var dataProvider = new CGDataProvider(pixelData, 0, pixelData.Length))
            {
                using(var colourSpace = CGColorSpace.CreateDeviceRGB())
                {
                    using(var cgImage = new CGImage(Width, Height, bitsPerComponent, bitsPerComponent*bytesPerPixel, bytesPerPixel*Width, colourSpace,
                        CGBitmapFlags.ByteOrderDefault, dataProvider, null, false, CGColorRenderingIntent.Default))
                    {
                        image.Dispose();
                        image = null;
                        pixelData = null;
                        return UIImage.FromImage(cgImage, 0, orientation);
                    }
                }
            }
        }

    }
}