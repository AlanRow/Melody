using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Melody
{
    class CalculatedSpectrum
    {
        public readonly Complex[][] Spectrum;
        public readonly double Duration;
        private Complex[][] specArr;

        public CalculatedSpectrum(Complex[][] spec, double dur)
        {
            Spectrum = spec;
            Duration = dur;
        }
    }

}
