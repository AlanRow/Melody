using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melody.Utils
{
    public static class MathUtils
    {
        public static double Log2(double num)
        {
            return Math.Log(num)/Math.Log(2);
        }

        public static int GetWindowsCount(int fullLength, int windowSize, int stepSize)
        {
            return (fullLength - windowSize) / stepSize + 1;
        }

        public static double GetFrameDuration(double fullDuration, int fullSize, int frameSize)
        {
            return fullDuration * frameSize / fullSize;
        }
    }
}
