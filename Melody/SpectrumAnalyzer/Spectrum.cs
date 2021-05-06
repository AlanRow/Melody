using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Melody.SpectrumAnalyzer
{
    public class Spectrum
    {
        public readonly Complex[][] SpectrumMatrix;
        public readonly double[] Freqs;

        public Spectrum(Complex[][] spec, double[] frequencies)
        {
            // Validation
            if (spec.Length == 0)
                throw new Exception("Spectrum must contain at least 1 column");

            if (spec[0].Length != frequencies.Length)
                throw new Exception("There are must be frequency-row mapping");

            SpectrumMatrix = spec;
            Freqs = frequencies;
        }
    }
}
