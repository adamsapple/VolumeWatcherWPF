using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Audio.DMO.Interfaces
{
    internal static class ComIds
    {
        internal const string IID_UNKNOWN         = "00000000-0000-0000-C000-000000000046";

        // DMO
        internal const string IID_MEDIA_OBJECT    = "d8ad0f58-5494-4102-97c5-ec798e59bcf4";
        internal const string IID_MEDIA_BUFFER    = "59eff8b9-938c-4a26-82f2-95cb84cdc837";
        internal const string IID_ENUM_DMO        = "2c3cd98a-2bfa-4a53-9c27-5249ba64ba0f";
        internal const string IID_WM_RESAMPLER_PROPS = "E7E9984F-F09F-4da4-903F-6E2E0EFE56B5";

        // RESAMPLER
        internal const string CLSID_RESAMPLER_MFT = "f447b69e-1884-4a7e-8055-346f74d6edb3";
    }
}
