using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ColorThiefDotNet.Forms
{
    public class ColorThiefImplementation : IColorThief
    {
        public QuantizedColor GetColor(ImageSource sourceImage, int quality, bool ignoreWhite)
        {
            throw new NotImplementedException();
        }

        public List<QuantizedColor> GetPalette(ImageSource sourceImage, int colorCount, int quality, bool ignoreWhite)
        {
            throw new NotImplementedException();
        }
    }
}