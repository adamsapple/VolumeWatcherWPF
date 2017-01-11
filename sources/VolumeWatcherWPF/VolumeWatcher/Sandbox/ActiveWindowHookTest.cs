using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Moral.Etc;

namespace VolumeWatcher.Sandbox
{
    public class ActiveWindowHookTest
    {

        ActiveWindowHook Hooker;

        public ActiveWindowHookTest()
        {
            Hooker = new ActiveWindowHook();




            Hooker.Callbacks += OnHook;
        }

        void OnHook(object sender, ActiveWindowChangedEventArgs e)
        {
            Console.WriteLine("Get new Event.");
        }
    }
}
