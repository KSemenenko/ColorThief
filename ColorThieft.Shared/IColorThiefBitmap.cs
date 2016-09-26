using System;
using System.Collections.Generic;
using System.Text;

namespace ColorThieftShared
{
    public interface IColorThiefBitmap<T>
    {
        byte[] ToPixelArray();

        void TransformImage(Func<byte, byte, byte, double> pixelOperation);

        T ToImage();
    }
}