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

        public SpecViewParameters(double startFreq, double octavesCount)
        {
            StartFreq = startFreq;
            OctavesCount = octavesCount;
        }
    }
}
