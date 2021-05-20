using Melody.SpectrumAnalyzer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Melody
{
    public class CalculatedSpectrum
    {
        public readonly Spectrogram Spectrum;
        public readonly double Duration;

        public CalculatedSpectrum(Spectrogram spec, double dur)
        {
            Spectrum = spec;
            Duration = dur;
        }
    }

}
