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

        public Complex[] Transform(double[] signal)
        {
            var spectrum = new Complex[Size];
            var frame = new double[Size];
            var n = 0;

            for (var start = 0; start + Size < signal.Length; start += Size)
            {
                Array.Copy(signal, start, frame, 0, Size);
                var spec = transformer.Transform(frame);
                for (var i = 0; i < spec.Length; i++)
                    spectrum[i] += spec[i];
                n++;
            }

            for (var i = 0; i < spectrum.Length; i++)
                spectrum[i] /= n;

            return spectrum;
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
