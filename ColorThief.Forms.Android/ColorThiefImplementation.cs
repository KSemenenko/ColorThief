using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Android.Graphics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace ColorThiefDotNet.Forms
{
    public class ColorThiefImplementation : IColorThief
    {
        private readonly ColorThief ct = new ColorThief();

        public async Task<QuantizedColor> GetColor(ImageSource sourceImage, int quality = ColorThief.DefaultQuality, bool ignoreWhite = ColorThief.DefaultIgnoreWhite)
        {
            return ct.GetColor(await GetImageFromImageSource(sourceImage), quality, ignoreWhite);
        }

        public async Task<List<QuantizedColor>> GetPalette(ImageSource sourceImage, int colorCount = ColorThief.DefaultColorCount, int quality = ColorThief.DefaultQuality,
            bool ignoreWhite = ColorThief.DefaultIgnoreWhite)
        {
            return ct.GetPalette(await GetImageFromImageSource(sourceImage), colorCount, quality, ignoreWhite);
        }

        private async Task<Bitmap> GetImageFromImageSource(ImageSource imageSource)
        {
            IImageSourceHandler handler;

            if(imageSource is FileImageSource)
            {
                handler = new FileImageSourceHandler();
            }
            else if(imageSource is StreamImageSource)
            {
                handler = new StreamImagesourceHandler();
            }
            else if(imageSource is UriImageSource)
            {
                handler = new ImageLoaderSourceHandler();
            }
            else
            {
                throw new NotImplementedException();
            }

            var originalBitmap = await handler.LoadImageAsync(imageSource, Xamarin.Forms.Forms.Context);

            return originalBitmap;
        }
    }
}