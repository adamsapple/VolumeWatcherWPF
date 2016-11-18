using System;
using System.Collections.Generic;
using System.Text;

namespace Audio.DMO
{
    [Flags]
    enum EDmoSetTypeFlags
    {
        None,
        DMO_SET_TYPEF_TEST_ONLY = 0x00000001,
        DMO_SET_TYPEF_CLEAR     = 0x00000002
    }
}
