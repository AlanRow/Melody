using Melody.SpectrumAnalyzer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Melody.NoteDetector
{
    // Represents type of metric, by what distance to note executing 

    class SimpleDetector
    {
        /*
         * Returns most-powerfull frequency in signal
         * Spectrum must be calculated by FFT
         **/
        public Tuple<double, Note> DetectNote(Complex[] spectrum, double duration)
        {
            var hz = CalcHz(spectrum, duration);
            var conv = new NoteConverter(new NoteMeasurer(DetectionMetric.Scalar));
            var note = conv.Convert((float)hz);

            return new Tuple<double, Note>(hz, note);
        }

        private double CalcHz(Complex[] spectrum, double duration)
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
