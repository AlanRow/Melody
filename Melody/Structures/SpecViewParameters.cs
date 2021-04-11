using Melody.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melody.Structures
{
    public class SpecViewParameters
    {
        public double StartFreq;
        public double OctavesCount;
        public IntensityCalcMethod CalcMethod;
        public IntensSumMethod SumMethod;
        public FreqScaleType ScaleType;

        public SpecViewParameters(double startFreq, double octavesCount, IntensityCalcMethod calc, IntensSumMethod sum, FreqScaleType scale)
        {
            StartFreq = startFreq;
            OctavesCount = octavesCount;
            CalcMethod = calc;
            SumMethod = sum;
            ScaleType = scale;
        }
    }
}
