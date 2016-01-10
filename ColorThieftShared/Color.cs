using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ColorThief
{
    /// <summary>
    ///     Defines a color in RGB space.
    /// </summary>
    public struct Color
    {
        /// <summary>
        /// Get or Set the Alpha component value for sRGB.
        /// </summary>
        public byte A;

        /// <summary>
        /// Get or Set the Red component value for sRGB.
        /// </summary>
        public byte R;

        /// <summary>
        /// Get or Set the Green component value for sRGB.
        /// </summary>
        public byte G;

        /// <summary>
        /// Get or Set the Blue component value for sRGB.
        /// </summary>
        public byte B;

    }
}
