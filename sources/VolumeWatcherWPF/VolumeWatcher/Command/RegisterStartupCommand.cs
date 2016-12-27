using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using Moral.Util;


namespace VolumeWatcher.Command
{
    /// <summary>
    /// スタートアップに実行ファイルのショートカットを設置/削除する為のCommand
    /// </summary>
    class RegisterStartupCommand : SimpleCommandBase
    {
        /// <summary>
        /// ショートカット名
        /// </summary>
        public String StartupName { get; set; } = string.Empty;

        /// <summary>
        /// ショートカットの設置/削除
        /// </summary>
        /// <param name="parameter">設置or削除</param>
        override public void Execute(object parameter)
        {
            var IsExecute = parameter as bool? ?? false;

            if (IsExecute)
            {
                WindowsUtil.RegiserStartUp_CurrentUserRun(StartupName);
            }
            else
            {
                WindowsUtil.UnregiserStartUp_CurrentUserRun(StartupName);
            }
        }
    }
}