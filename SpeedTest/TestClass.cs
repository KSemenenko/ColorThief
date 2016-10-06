using System.Drawing;
using BenchmarkDotNet.Attributes;

namespace SpeedTest
{
    public class TestClass
    {
        [Benchmark]
        public void Image1()
        {
            var colorThief = new ColorThief.ColorThief();
            var bitmap = (Bitmap)Image.FromFile("test1.jpg");
            var result = colorThief.GetColor(bitmap);
        }

        [Benchmark]
        public void Image2()
        {
            var colorThief = new ColorThief.ColorThief();
            var bitmap = (Bitmap)Image.FromFile("test2.jpg");
            var result = colorThief.GetColor(bitmap);
        }
    }
}