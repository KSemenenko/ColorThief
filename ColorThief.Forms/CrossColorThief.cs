using System;

namespace ColorThiefDotNet.Forms
{
    public partial class CrossColorThief
    {
        private static readonly Lazy<IColorThief> Implementation =
            new Lazy<IColorThief>(CreatePluginColorThief, System.Threading.LazyThreadSafetyMode.PublicationOnly);

        /// <summary>
        /// Current settings to use
        /// </summary>
        public static IColorThief Current
        {
            get
            {
                var ret = Implementation.Value;
                if (ret == null)
                {
                    throw NotImplementedInReferenceAssembly();
                }
                return ret;
            }
        }

        private static IColorThief CreatePluginColorThief()
        {
            throw NotImplementedInReferenceAssembly();
        }

        static Exception NotImplementedInReferenceAssembly()
        {
            return
                new NotImplementedException(
                    "This functionality is not implemented in the portable version of this assembly.  " +
                    "You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
        }
    }
}