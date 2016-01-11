using System;

namespace ColorThief
{
    /// <summary>
    ///     Defines a color in RGB space.
    /// </summary>
    public struct Color
    {
        /// <summary>
        ///     Get or Set the Alpha component value for sRGB.
        /// </summary>
        public byte A;

        /// <summary>
        ///     Get or Set the Red component value for sRGB.
        /// </summary>
        public byte R;

        /// <summary>
        ///     Get or Set the Green component value for sRGB.
        /// </summary>
        public byte G;

        /// <summary>
        ///     Get or Set the Blue component value for sRGB.
        /// </summary>
        public byte B;

        /// <summary>
        ///     Get HSL color.
        /// </summary>
        /// <returns></returns>
        public HslColor ToHsl()
        {
            const double toDouble = 1.0 / 255;
            double r = toDouble * R;
            double g = toDouble * G;
            double b = toDouble * B;
            double max = Math.Max(Math.Max(r, g), b);
            double min = Math.Min(Math.Min(r, g), b);
            double chroma = max - min;
            double h1;

            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (chroma == 0)
            {
                h1 = 0;
            }
            else if (max == r)
            {
                h1 = (g - b) / chroma % 6;
            }
            else if (max == g)
            {
                h1 = 2 + (b - r) / chroma;
            }
            else //if (max == b)
            {
                h1 = 4 + (r - g) / chroma;
            }

            double lightness = 0.5 * (max - min);
            double saturation = chroma == 0 ? 0 : chroma / (1 - Math.Abs(2 * lightness - 1));
            HslColor ret;
            ret.H = 60 * h1;
            ret.S = saturation;
            ret.L = lightness;
            ret.A = toDouble * A;
            return ret;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }
    }
}