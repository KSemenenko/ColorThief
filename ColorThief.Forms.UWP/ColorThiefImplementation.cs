using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

namespace ColorThiefDotNet.Forms
{
    public class ColorThiefImplementation : IColorThief
    {
        private readonly ColorThief ct = new ColorThief();

        public async Task<QuantizedColor> GetColor(ImageSource sourceImage, int quality = ColorThief.DefaultQuality, bool ignoreWhite = ColorThief.DefaultIgnoreWhite)
        {
            return await ct.GetColor(await GetImageFromImageSource(sourceImage), quality, ignoreWhite);
        }

        public async Task<List<QuantizedColor>> GetPalette(ImageSource sourceImage, int colorCount = ColorThief.DefaultColorCount, int quality = ColorThief.DefaultQuality,
            bool ignoreWhite = ColorThief.DefaultIgnoreWhite)
        {
            return await ct.GetPalette(await GetImageFromImageSource(sourceImage), colorCount, quality, ignoreWhite);
        }

        private async Task<BitmapDecoder> GetImageFromImageSource(ImageSource imageSource)
        {
            IImageSourceHandler handler;

            if(imageSource is FileImageSource)
            {
                handler = new FileImageSourceHandler();
            }
            else if(imageSource is StreamImageSource)
            {
                handler = new StreamImageSourceHandler();
                var stream = await ((IStreamImageSource)imageSource).GetStreamAsync();
                if(stream != null)
                {
                    return await BitmapDecoder.CreateAsync(WindowsRuntimeStreamExtensions.AsRandomAccessStream(stream));
                }
                
            }
            else if(imageSource is UriImageSource)
            {
                handler = new UriImageSourceHandler();
            }
            else
            {
                throw new NotImplementedException();
            }

            var bitmapImage = await handler.LoadImageAsync(imageSource) as BitmapImage;

            if (bitmapImage?.UriSource != null)
            {
                using (IRandomAccessStreamWithContentType streamWithContent = await RandomAccessStreamReference.CreateFromUri(bitmapImage.UriSource).OpenReadAsync())
                {
                    return await BitmapDecoder.CreateAsync(streamWithContent);
                }

            }

            throw new NotImplementedException();

        }
    }
}