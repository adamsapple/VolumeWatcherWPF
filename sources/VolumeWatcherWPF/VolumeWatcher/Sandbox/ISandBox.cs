using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolumeWatcher.Sandbox
{
    public interface ISandBox : IDisposable
    {
        void Start();
        void Stop();
    }
}
