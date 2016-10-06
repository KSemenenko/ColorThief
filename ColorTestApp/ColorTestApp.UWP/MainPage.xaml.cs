using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
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

        private async Task UWP(MediaFile file)
        {
            try
            {
                using(IRandomAccessStream stream = file.GetStream().AsRandomAccessStream())
                {
                    var decoder = await BitmapDecoder.CreateAsync(stream);
                    var ct = new ColorThief.ColorThief();
                    var xxxx = ct.GetColor(decoder);
                    var a = 5;
                }
            }
            catch(Exception ex)
            {
                var a = 5;
            }
        }
    }
}