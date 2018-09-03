using System;
using BenchmarkDotNet.Running;

namespace SpeedTest.Netstandard
{
    internal class Program
    {
        public static void Main(string[] args)
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

            var summary = BenchmarkRunner.Run<TestClass>();
            Console.ReadLine();
        }
    }
}