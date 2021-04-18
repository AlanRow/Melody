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
        private int SAMPLING = 44100;

        private double startFreq = 440.0;
        private int octavesCount = 1;
        public Complex[][] Transform(double[] signal)
        {
            var freqsCount = FREQS_IN_OCTAVE * octavesCount;
            var freqStep = Math.Pow(2, 1.0 / FREQS_IN_OCTAVE);
            var minSize = (int)(SAMPLING / ((freqStep - 1) * startFreq * Math.Pow(freqStep, freqsCount - 1)));
            var specLength = signal.Length / minSize;
            var spectrum = new Complex[specLength][];

            for (var i = 0; i < spectrum.Length; i++)
            {
                spectrum[i] = new Complex[freqsCount];
            }

            var f = startFreq;

            for (var freqIdx = 0; freqIdx < freqsCount; freqIdx++)
            {
                var size = (int)(SAMPLING / (f * freqStep - f));
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
                    for (var idx = start; idx < end; idx++)
                        val += Complex.FromPolarCoordinates(1, -2 * Math.PI * idx * freqIdx / size);
                    spectrum[i][freqIdx] = val;
                }
                f *= freqStep;
            }

            return spectrum;


            /*for (var c = 0; c + minSize < )
                var f = startFreq;*/

            /*for (var i = 0; i < spectrum[0].Length; i++)
            {
                var size = (int)(SAMPLING / (f * freqStep - f));
                var specVal = Complex.Zero;

                for (var j = 0; j < size; j++)
                {
                    specVal += Complex.FromPolarCoordinates(1, -2 * Math.PI * j * i / size);
                }
                spectrum[0][i] = specVal;
                f *= freqStep;
            }

            return spectrum;*/
        }
    }
}
