using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moral.Util
{
    class Ease
    {
        private List<long> list = new List<long>();

        public void addFrame( long ms)
        {
            list.Add(ms);
        }

        public long getFrame(int id)
        {
            return list[id];
        }

        public void editFrame(int id, long ms)
        {
            list[id] = ms;
        }

        public double getFrameRate(long ms)
        {
            double result = 0;
            for (int i = 0, len = list.Count; i < len; ++i)
            {
                long n = list[i];
                if(ms < n)
                {
                    result += 1.0 * ms / n;
                }
                else
                {
                    result += 1.0;
                }
                ms -= n;

                if( ms <= 0)
                {
                    break;
                }
            }
            return result;
        }
    }
}
