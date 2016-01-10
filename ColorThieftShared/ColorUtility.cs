using System;

namespace ColorThief
{
    public static class ColorUtility
    {
        public static HslColor ToHsl(this Color rgba)
        {
            const double toDouble = 1.0/255;
            var r = toDouble*rgba.R;
            var g = toDouble*rgba.G;
            var b = toDouble*rgba.B;
            var max = Math.Max(Math.Max(r, g), b);
            var min = Math.Min(Math.Min(r, g), b);
            var chroma = max - min;
            double h1;

            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (chroma == 0)
            {
                h1 = 0;
            }
            else if (max == r)
            {
                h1 = (g - b)/chroma%6;
            }
            else if (max == g)
            {
                h1 = 2 + (b - r)/chroma;
            }
            else //if (max == b)
            {
                h1 = 4 + (r - g)/chroma;
            }

            var lightness = 0.5*(max - min);
            var saturation = chroma == 0 ? 0 : chroma/(1 - Math.Abs(2*lightness - 1));
            HslColor ret;
            ret.H = 60*h1;
            ret.S = saturation;
            ret.L = lightness;
            ret.A = toDouble*rgba.A;
            return ret;
            // ReSharper restore CompareOfFloatsByEqualityOperator
        }

        public static int CalculateYiqLuma(Color color)
        {
            return
                (int)
                    Math.Round((299*color.R + 587*color.G +
                                114*color.B)/1000f);
        }

        public static Color FromRgb(int red, int green, int blue)
        {
            var color = new Color
            {
                A = 255,
                R = (byte)red,
                G = (byte)green,
                B = (byte)blue
            };

            return color;
        }
    }
}