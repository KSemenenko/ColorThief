using System;
using System.Runtime.InteropServices;
using Android.Graphics;
using ColorThieftShared;
using Java.Nio;

namespace ColorThief
{
    public class ColorThiefBitmap : IColorThiefBitmap
    {
        private const byte bytesPerPixel = 4;
        private readonly int height;
        private readonly int width;
        private Bitmap bitmap;
        private readonly string photoFile;
        private byte[] pixelData;

        public ColorThiefBitmap(string photo)
        {
            photoFile = photo;
            var options = new BitmapFactory.Options
            {
                InJustDecodeBounds = true
            };

            // Bitmap will be null because InJustDecodeBounds = true
            bitmap = BitmapFactory.DecodeFile(photoFile, options);
            width = options.OutWidth;
            height = options.OutHeight;
        }

        public byte[] ToPixelArray()
        {
            bitmap = BitmapFactory.DecodeFile(photoFile);

            var size = width*height*bytesPerPixel;
            pixelData = new byte[size];
            var byteBuffer = ByteBuffer.AllocateDirect(size);
            bitmap.CopyPixelsToBuffer(byteBuffer);
            Marshal.Copy(byteBuffer.GetDirectBufferAddress(), pixelData, 0, size);
            byteBuffer.Dispose();
            return pixelData;
        }

        public void TransformImage(Func<byte, byte, byte, double> pixelOperation)
        {
            byte r, g, b;

            try
            {
                // Pixel data order is RGBA
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

        public Bitmap ToImage()
        {
            var byteBuffer = ByteBuffer.AllocateDirect(width*height*bytesPerPixel);
            Marshal.Copy(pixelData, 0, byteBuffer.GetDirectBufferAddress(), width*height*bytesPerPixel);
            bitmap.CopyPixelsFromBuffer(byteBuffer);
            byteBuffer.Dispose();
            return bitmap;
        }

        public void Dispose()
        {
            if(bitmap != null)
            {
                bitmap.Recycle();
                bitmap.Dispose();
                bitmap = null;
            }
            pixelData = null;
        }
    }
}