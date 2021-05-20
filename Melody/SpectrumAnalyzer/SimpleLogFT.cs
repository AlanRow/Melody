using Melody.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Melody.SpectrumAnalyzer
{
    class SimpleLogFT : ITransformer
    {
        // Min frequency (Hz)
        public double StartFreq;
        // Max frequency (Hz)
        public double EndFreq;
        // Count of spectrum calculations between two octaves
        public int BoundsPerOctave;


        // Octaves count between start frequency and end frequency
        public double OctavesCount
        {
            get
            {
                return MathUtils.Log2(EndFreq) - MathUtils.Log2(StartFreq);
            }
        }

        // All bounds count
        public int BoundsCount
        {
            get
            {
                return (int)Math.Floor(OctavesCount * BoundsPerOctave + 1);
            }
        }

        public SimpleLogFT(double start, double end, int bounds)
        {
            StartFreq = start;
            EndFreq = end;
            BoundsPerOctave = bounds;
        }

        public RawSpectrum Transform(double[] signal, double duration)
        {
            var freqsCount = BoundsCount;
            var freqs = new double[freqsCount];
            var freqStep = Math.Pow(2, 1.0 / BoundsPerOctave);
            var f = StartFreq * duration;

            var spec = new Complex[freqsCount];

            for (var i = 0; i < freqs.Length; i++)
            {
                freqs[i] = f;
                f *= freqStep;
            }

            for (var freqIdx = 0; freqIdx < freqs.Length; freqIdx++)
            {
                f = freqs[freqIdx];
                var val = Complex.Zero;

                for (var i = 0; i < signal.Length; i++)
                {
                    val += Complex.FromPolarCoordinates(signal[i], -i * 2 * Math.PI * f / signal.Length);
                }

                spec[freqIdx] = val;
            }

            return new RawSpectrum(new Complex[1][] { spec }, freqs);
        }
    }
}
