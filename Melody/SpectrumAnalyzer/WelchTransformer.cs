using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Melody.SpectrumAnalyzer
{
    class WelchTransformer : ITransformer
    {
        private FFTTransformer transformer;
        public readonly int Size;
        public readonly int Step;
        public readonly Func<int, int, double> FilterFactor;

        // Standard filters
        public static Func<int, int, double> RectangleFilter = (size, idx) => 1d;
        public static Func<int, int, double> TriangleFilter = (size, idx) => 0.5 - (Math.Abs(idx - (double)size/2))/(size - 1);
        public static Func<int, int, double> HannFilter = (size, idx) => 0.5 * (1 - Math.Cos(Math.PI * 2 * idx / (size - 1)));
        public static Func<int, int, double> HammingFilter = (size, idx) => 0.53836 - 0.46164 * (Math.Cos(Math.PI * 2 * idx / (size - 1)));

        public WelchTransformer(int size, int step, Func<int, int, double> filter)
        {
            transformer = new FFTTransformer();

            if (IsPowerOfTwo(size))
                Size = size;
            else
                throw new ArgumentException("Transform window size must be power of 2");

            if (step > 0 && step <= Size)
                Step = step;
            else if (step <= 0)
                throw new ArgumentException("Transform window step must be positive");
            else
                throw new ArgumentException("Transform window step must be lower than window size");

            FilterFactor = filter;
        }

        public Complex[][] Transform(double[] signal)
        {
            var len = (signal.Length - Size) / Step + 1;
            var frame = new double[Size];
            var spectrum = new Complex[len][];

            var factorArr = Enumerable.Range(0, Size).Select((n) => FilterFactor(Size, n)).ToArray();

            for (var i = 0; i < len; i++)
            {
                Array.Copy(signal, i * Step, frame, 0, Size);

                // Filtering
                for (var j = 0; j < Size; j++)
                    frame[j] *= factorArr[j];

                var specLine = transformer.Transform(frame)[0];
                spectrum[i] = specLine;
            }

            return spectrum;
            /*var spectrum = new Complex[Size];
            var frame = new double[Size];
            var n = 0;

            for (var start = 0; start + Size < signal.Length; start += Step)
            {
                Array.Copy(signal, start, frame, 0, Size);

                // Filtering
                for (var i = 0; i < spectrum.Length; i++)
                {
                    if (i == 2048)
                    {
                        var a = "";
                    }
                    var f = FilterFactor(Size, i);
                    frame[i] *= f;
                }

                var spec = transformer.Transform(frame);
                for (var i = 0; i < spec.Length; i++)
                    spectrum[i] += spec[i];
                n++;
            }

            for (var i = 0; i < spectrum.Length; i++)
                spectrum[i] /= n;

            return spectrum;*/
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
