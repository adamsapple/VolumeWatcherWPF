using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VolumeWatcher.Enumrate
{
    /// <summary>
    /// Volumeウィンドウがどのボリュームを表示するかどうかの定数
    /// </summary>
    public enum EVolumeViewMode
    {
        /// <summary>変更無し</summary>
        NotChange,
        /// <summary>RenderVolume表示</summary>
        Render,
        /// <summary>Capturevolume表示</summary>
        Capture
    }
}
