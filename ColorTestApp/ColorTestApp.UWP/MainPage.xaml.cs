using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using ColorThiefDotNet;
using Plugin.Media.Abstractions;

namespace ColorTestApp.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

            // UWP(null);

            LoadApplication(new ColorTestApp.App());
        }


    }
}