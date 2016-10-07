using System.IO;

namespace ColorThiefDotNettShared
{
    public interface IColorThiefBitmap
    {
        int Width { get; }
        int Height { get; }
        int[] ToPixelArray();
    }
}