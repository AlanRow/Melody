using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melody.Views
{
    class SpecInfoConverter
    {
        private double[] frequencies;
        private double duration;
        private int pixW;
        private int pixH;

        public SpecInfoConverter(double[] freqs, double dur, int width, int height)
        {
            frequencies = freqs;
            duration = dur;
            pixW = width;
            pixH = height;
        }

        public double GetFreqByCoord(int y)
        {
            var f = ((double)frequencies.Length) / pixH;
            var fIdx = (int)(y * f) - 1;

            // Restrictions
            if (fIdx < 0) fIdx = 0;
            if (fIdx >= frequencies.Length) fIdx = frequencies.Length - 1;
            
            return frequencies[fIdx];
        }

        public double GetTimeByCoord(int x)
        {
            var f = duration / pixW;
            return x * f;
        }
    }
}
