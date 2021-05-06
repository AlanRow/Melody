using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Melody.SpectrumAnalyzer
{
    class BartlettTransformer : ITransformer
    {
        private FFTTransformer transformer;
        public readonly int Size;

        public BartlettTransformer(int size)
        {
            transformer = new FFTTransformer();

            if (IsPowerOfTwo(size))
                Size = size;
            else
                throw new ArgumentException(String.Format("Frame size for FFT must be power of 2, but it is {0}", size));
        }

        public Spectrum Transform(double[] signal, double duration)
        {
            var len = signal.Length / Size;
            var frame = new double[Size];
            var spectrum = new Complex[len][];
            var freqs = new double[Size];
            var frameDur = duration * Size / signal.Length;
            
            for (var i = 0; i < len; i++)
            {
                Array.Copy(signal, i * Size, frame, 0, Size);
                var specLine = transformer.Transform(frame, frameDur);
                spectrum[i] = specLine.SpectrumMatrix[0];

                if (i == 0)
                    freqs = specLine.Freqs;
                
            }

            return new Spectrum(spectrum, freqs);
        }

        private bool IsPowerOfTwo(int number)
        {
            while (number > 1)
            {
                if (number % 2 > 0)
                    return false;
                number /= 2;
            }
            return true;
        }
    }
}
