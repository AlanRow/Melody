using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Melody.SpectrumAnalyzer
{
    class ConstantQTransformer : ITransformer
    {
        private int FREQS_IN_OCTAVE = 96;

        private double startFreq = 440.0;
        private int octavesCount = 2;

        public RawSpectrum Transform(double[] signal, double duration)
        {
            var sampling = (int)(signal.Length / duration);
            var freqsCount = FREQS_IN_OCTAVE * octavesCount;
            var freqStep = Math.Pow(2, 1.0 / FREQS_IN_OCTAVE);
            var minSize = (int)(sampling / ((freqStep - 1) * startFreq * Math.Pow(freqStep, freqsCount - 1)));
            var specLength = signal.Length / minSize;

            var spectrum = new Complex[specLength][];

            for (var i = 0; i < spectrum.Length; i++)
            {
                spectrum[i] = new Complex[freqsCount];
            }

            var freqs = new double[freqsCount];
            var f = startFreq;

            for (var freqIdx = 0; freqIdx < freqsCount; freqIdx++)
            {
                var size = (int)(sampling / (f * freqStep - f));
                freqs[freqIdx] = f;
                for (var i = 0; i < specLength; i++)
                {
                    var center = i * minSize + minSize / 2;
                    var start = center - size / 2;
                    var end = start + size;

                    if (start < 0)
                    {
                        start = 0;
                        end = size;
                    }
                    
                    if (end > signal.Length)
                    {
                        end = signal.Length;
                        start = end - size;
                    }

                    var val = Complex.Zero;
                    if (freqIdx > 0)
                    {
                        for (var idx = start; idx < end; idx++)
                            val += Complex.FromPolarCoordinates(signal[idx], -2 * Math.PI * idx * (freqIdx + 1) / size);
                    }
                    spectrum[i][freqIdx] = val;
                }
                f *= freqStep;
            }

            return new RawSpectrum(spectrum, freqs);
        }
    }
}
