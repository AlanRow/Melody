using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Melody.SpectrumAnalyzer
{
    public class Spectrogram
    {
        public readonly double[][] SpectrumMatrix;
        public readonly double[] Freqs;
        public readonly double Duration;

        public Spectrogram(double[][] spec, double[] frequencies, double dur)
        {
            // Validation
            if (spec.Length == 0)
                throw new Exception("Spectrum must contain at least 1 column");

            if (spec[0].Length != frequencies.Length)
                throw new Exception("There are must be frequency-row mapping");

            SpectrumMatrix = spec;
            Freqs = frequencies;
            Duration = dur;
        }
    }
}
