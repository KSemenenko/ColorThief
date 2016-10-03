using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Plugin.Media.Abstractions;

namespace ColorTestApp.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

           // UWP(null);

            LoadApplication(new ColorTestApp.App());

            

        }
        private async Task UWP(MediaFile file)
        {
            try
            {
    

                using (IRandomAccessStream stream = file.GetStream().AsRandomAccessStream())
                {
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                    ColorThief.ColorThief ct = new ColorThief.ColorThief();
                    var xxxx = ct.GetColor(decoder);
                    int a = 5;
                }
            }
            catch (Exception ex)
            {
                int a = 5;
            }

        }

    }
}
