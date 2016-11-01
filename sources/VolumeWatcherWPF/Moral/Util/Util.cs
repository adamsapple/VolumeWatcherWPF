using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Moral.Util
{
    public static class Util
    {
        public static int GetNumInvocations(Delegate d)
        {
            if (d == null || d.GetInvocationList() == null)
            {
                return 0;
            }
            return d.GetInvocationList().Length;
        }
    }

    public static class DictionaryExtensions
    {
        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key)
        {
            if (source == null || source.Count == 0)
                return default(TValue);

            TValue result;
            if (source.TryGetValue(key, out result))
                return result;

            return default(TValue);
        }

        public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue defaultValue)
        {

            if (source == null || source.Count == 0)
                return defaultValue;

            TValue result;
            if (source.TryGetValue(key, out result))
                return result;

            return defaultValue;
        }
    }

    public static class WindowPreloader
    {
        public static void Preload(this Window win)
        {
            var dispatcher = Application.Current.Dispatcher;
            //dispatcher.Invoke(DispatcherPriority.Normal, (Action)delegate () {
            dispatcher.Invoke((Action)delegate ()
            {
                /*
                var ws = win.WindowStyle;
                var at = win.AllowsTransparency;
                //var bg = win.Background;
                var op = win.Opacity;

                win.WindowStyle = WindowStyle.None;
                win.AllowsTransparency = true;
                win.Opacity = 0;

                win.Show();
                win.Hide();

                win.Opacity = op;
                win.AllowsTransparency = at;
                win.WindowStyle = ws;
                */
                var point = new Point(win.Left, win.Top);
                var pointDummy = new Point(System.Windows.SystemParameters.PrimaryScreenWidth, System.Windows.SystemParameters.PrimaryScreenHeight);
                var pointDummy2 = new Point(System.Windows.SystemParameters.WorkArea.Width, System.Windows.SystemParameters.WorkArea.Height);

                win.Left = pointDummy.X;
                //win.Top = pointDummy.Y;
                win.Show();
                win.Hide();
            });
            dispatcher.Invoke((Action)delegate ()
            {
                var point = new Point(win.Left, win.Top);
                var pointDummy = new Point(System.Windows.SystemParameters.PrimaryScreenWidth, System.Windows.SystemParameters.PrimaryScreenHeight);
                var pointDummy2 = new Point(System.Windows.SystemParameters.WorkArea.Width, System.Windows.SystemParameters.WorkArea.Height);
                //win.Left = point.X != Double.NaN ? (pointDummy2.X - win.ActualWidth) / 2 : point.X;
                //win.Top = point.Y != Double.NaN ? (pointDummy2.Y - win.ActualHeight) / 2 : point.Y;
            });
        }
    }
}
