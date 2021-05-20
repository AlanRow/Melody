using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melody.Structures
{
    public enum FilterType
    {
        Rectangle,
        Triangle,
        Hann,
        Hamming,
    }

    public class TransformParameters
    {
        public FilterType Type;
        public int WindowSize;
        public int StepSize;
        // public double LPFLimit;
        public double StartFreq;
        public double EndFreq;
        public int BoundsPerOctave;

        // 0-100
        public double FilterPercentage;

        public TransformParameters(FilterType type, int window, int step, double start, double end, int bounds, double perc)
        {
            Type = type;
            WindowSize = window;
            StepSize = step;
            StartFreq = start;
            EndFreq = end;
            BoundsPerOctave = bounds;
            FilterPercentage = perc;
        }
    }
}
