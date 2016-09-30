namespace ColorThieftShared
{
    public interface IColorThiefBitmap
    {
        int Width { get; }
        int Height { get; }
        byte[] ToPixelArray();
    }
}