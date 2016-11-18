using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Audio.DMO.Interfaces
{
    [Guid(ComIds.IID_ENUM_DMO), 
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    interface IEnumDmo
    {
        // int Next(int itemsToFetch, CLSID[] clsids, string[] names, out int itemsFetched);
        // lets do one at a time to keep it simple - don't call with itemsToFetch > 1        
        int Next(int itemsToFetch, out Guid clsid, out IntPtr name, out int itemsFetched);
                
        int Skip(int itemsToSkip);
        
        int Reset();
        
        int Clone(out IEnumDmo enumPointer);
    }
}
