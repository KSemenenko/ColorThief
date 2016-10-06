using System;

namespace SpeedTest
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var x = new TestClass();
            x.Image1();
            x.Image2();

            x.Image1();
            x.Image2();

            x.Image1();
            x.Image2();

            x.Image1();
            x.Image2();

            x.Image1();
            x.Image2();

            x.Image1();
            x.Image2();

            x.Image1();
            x.Image2();

            //var summary = BenchmarkRunner.Run<TestClass>();
            Console.ReadLine();
        }
    }
}