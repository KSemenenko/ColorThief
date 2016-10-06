using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BenchmarkDotNet.Running;

namespace SpeedTest
{
    class Program
    {
        static void Main(string[] args)
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
