using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace UI
{
    static class RadioGroupBox
    {
        /// <summary>
        /// ラジオボタンを抽出
        /// http://takabosoft.com/20110830045233.html
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        internal static IEnumerable<RadioButton> GetRadioButtons(this Control parent)
        {
            return parent.Controls.Cast<Control>().Select(x => x as RadioButton).Where(x => x != null);
        }

        /// <summary>
        /// Tagを元にラジオボタンにチェックを入れる
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="tag"></param>
        internal static void CheckRadioButtonByTag(this Control parent, object tag)
        {
            foreach (var radio in parent.GetRadioButtons())
            {
                if (radio.Tag.Equals(tag))
                {
                    radio.Checked = true;
                    break;
                }
            }
        }

        /// <summary>
        /// チェックが入ったラジオボタンのタグを取得する
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        internal static object GetCheckedRadioButtonTag(this Control parent)
        {
            foreach (var radio in parent.GetRadioButtons())
            {
                if (radio.Checked)
                {
                    return radio.Tag;
                }
            }
            return parent.GetRadioButtons().First().Tag;
        }

        /// <summary>
        /// ラジオボタンのチェックイベントを設定
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="action"></param>
        //internal static void SetRadioButtonCheckedEvent(this Control parent, Action<RadioButton> action)
        internal static void SetRadioButtonCheckedEvent(this Control parent, Action<RadioButton, EventArgs> action)
        {
            foreach (var radio in parent.GetRadioButtons())
            {
                radio.CheckedChanged += (sender, _) =>
                {
                    RadioButton r = (RadioButton)sender;
                    if (r.Checked)
                    {
                        action(r,_);
                    }
                };
            }
        }
    }
}
