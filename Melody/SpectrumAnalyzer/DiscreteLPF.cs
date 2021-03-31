using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Melody.SpectrumAnalyzer
{
    public static class DiscreteLPF
    {
        public static void Filter(Complex[][] spectrum, double winDuration, double borderFreq)
        {
            var freqMult = 1 / winDuration;
            var freqsCount = spectrum[0].Length / 2;
            for (var i = 0; i < freqsCount && i * freqMult < borderFreq; i++)
            {
                for (var j = 0; j < spectrum.Length; j++)
                {
                    spectrum[j][i] = Complex.Zero;
                    spectrum[j][freqsCount * 2 - i - 1] = Complex.Zero;
                }
            }
        }
    }
}
