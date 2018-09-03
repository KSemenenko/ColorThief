using System.Drawing;
using System.IO;
using BenchmarkDotNet.Attributes;
using ColorThiefDotNet;

namespace SpeedTest.Netstandard
{
    public class TestClass
    {
        [Benchmark]
        public void Image1()
        {
            var colorThief = new ColorThief();

            var test = Directory.GetCurrentDirectory();
            var bitmap = (Bitmap)Image.FromFile("test1.jpg");
            var result = colorThief.GetColor(bitmap);
        }

        [Benchmark]
        public void Image2()
        {
            var colorThief = new ColorThief();
            var bitmap = (Bitmap)Image.FromFile("test2.jpg");
            var result = colorThief.GetColor(bitmap);
        }
    }
}