using Melody.SpectrumAnalyzer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Melody.NoteDetector
{
    class SimpleDetector
    {
        /*
         * Returns most-powerfull frequency in signal
         * Spectrum must be calculated by FFT
         **/
        public double DetectNote(Complex[] spectrum, double duration)
        {
            var max = 0d;
            var maxFreq = 0d;
            var maxIdx = -1;
            for (var i = 1; i < spectrum.Length / 2; i++)
            {
                var magn = spectrum[i].Magnitude;
                if (magn >= max)
                {
                    max = magn;
                    maxIdx = i;
                }
            }

            var freq = ((double)maxIdx) / duration;
            return freq;
        }
    }
}
