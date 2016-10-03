using System.IO;

namespace ColorThieftShared
{
    public interface IColorThiefBitmap
    {
        int Width { get; }
        int Height { get; }
        int[] ToPixelArray();
    }
}