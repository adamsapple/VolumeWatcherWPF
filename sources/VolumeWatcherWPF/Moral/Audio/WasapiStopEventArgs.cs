﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moral.Audio
{
    /// <summary>
    /// Stopped Event Args
    /// </summary>
    public class WasapiStopEventArgs : EventArgs
    {
        private readonly Exception exception;

        /// <summary>
        /// Initializes a new instance of StoppedEventArgs
        /// </summary>
        /// <param name="exception">An exception to report (null if no exception)</param>
        public WasapiStopEventArgs(Exception exception = null)
        {
            this.exception = exception;
        }

        /// <summary>
        /// An exception. Will be null if the playback or record operation stopped
        /// </summary>
        public Exception Exception { get { return exception; } }
    }
}
