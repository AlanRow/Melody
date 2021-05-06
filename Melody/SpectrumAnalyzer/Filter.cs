using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melody.SpectrumAnalyzer
{
    public class Filter
    {
        public static Filter Rectangle = new Filter((size, idx) => 1d);
        public static Filter Triangle = new Filter((size, idx) => 0.5 - (Math.Abs(idx - (double)size / 2)) / (size - 1));
        public static Filter Hann = new Filter((size, idx) => 0.5 * (1 - Math.Cos(Math.PI * 2 * idx / (size - 1))));
        public static Filter Hamming = new Filter((size, idx) => 0.53836 - 0.46164 * Math.Cos(Math.PI * 2 * idx / (size - 1)));

        private Func<int, int, double> filterFunction;

        private Filter(Func<int, int, double> func)
        {
            filterFunction = func;
        }

        public double GetFiltered(double value, int size, int idx)
        {
            return value * filterFunction(size, idx);
        }
    }
}
