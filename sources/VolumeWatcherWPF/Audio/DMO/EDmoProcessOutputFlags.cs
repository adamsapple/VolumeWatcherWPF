using System;
using System.Collections.Generic;
using System.Text;

namespace Audio.DMO
{
    /// <summary>
    /// DMO Process Output Flags
    /// </summary>
    [Flags]
    public enum EDmoProcessOutputFlags
    {
        /// <summary>
        /// None
        /// </summary>
        None,
        /// <summary>
        /// DMO_PROCESS_OUTPUT_DISCARD_WHEN_NO_BUFFER
        /// </summary>
        DiscardWhenNoBuffer = 0x00000001
    }
}
