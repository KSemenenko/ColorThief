using System;
using System.Runtime.InteropServices;
using ColorThieftShared;
using CoreGraphics;
using UIKit;

namespace ColorThief.iOS
{
    public class ColorThiefBitmap : IColorThiefBitmap<UIImage>
    {
        private const byte bitsPerComponent = 8;
        private const byte bytesPerPixel = 4;
        private readonly int height;
        private readonly int width;
        private UIImage image;
        private readonly UIImageOrientation orientation;
        private byte[] pixelData;
        private IntPtr rawData;

        public ColorThiefBitmap(UIImage uiImage)
        {
            image = uiImage;
            orientation = image.Orientation;
            width = (int)image.CGImage.Width;
            height = (int)image.CGImage.Height;
        }

        public byte[] ToPixelArray()
        {
            using(var colourSpace = CGColorSpace.CreateDeviceRGB())
            {
                rawData = Marshal.AllocHGlobal(width*height*4);
                using(var context = new CGBitmapContext(rawData, width, height, bitsPerComponent, bytesPerPixel*width, colourSpace, CGImageAlphaInfo.PremultipliedLast))
                {
                    context.DrawImage(new CGRect(0, 0, width, height), image.CGImage);
                    pixelData = new byte[width*height*bytesPerPixel];
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
                    using(var cgImage = new CGImage(width, height, bitsPerComponent, bitsPerComponent*bytesPerPixel, bytesPerPixel*width, colourSpace,
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