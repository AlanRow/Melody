using Melody.Structures;
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
        public static Func<int, int, double> TriangleFilter = (size, idx) => 0.5 - (Math.Abs(idx - (double)size / 2)) / (size - 1);
        public static Func<int, int, double> HannFilter = (size, idx) => 0.5 * (1 - Math.Cos(Math.PI * 2 * idx / (size - 1)));
        public static Func<int, int, double> HammingFilter = (size, idx) => 0.53836 - 0.46164 * (Math.Cos(Math.PI * 2 * idx / (size - 1)));

        private static Dictionary<FilterType, Func<int, int, double>> filtersMap = new Dictionary<FilterType, Func<int, int, double>>()
        {
            { FilterType.Rectangle, RectangleFilter },
            { FilterType.Triangle, TriangleFilter },
            { FilterType.Hann, HannFilter },
            { FilterType.Hamming, HammingFilter },
        };

        public WelchTransformer(int size, int step, FilterType filterType) : this(size, step, filtersMap[filterType]) { }

        private WelchTransformer(int size, int step, Func<int, int, double> filter)
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

        public RawSpectrum Transform(double[] signal, double duration)
        {
            var len = (signal.Length - Size) / Step + 1;
            var frame = new double[Size];
            var frameDuration = duration * Size / signal.Length;

            var spectrum = new Complex[len][];
            var freqs = new double[Size];

            var factorArr = Enumerable.Range(0, Size).Select((n) => FilterFactor(Size, n)).ToArray();

            for (var i = 0; i < len; i++)
            {
                Array.Copy(signal, i * Step, frame, 0, Size);

                // Filtering
                for (var j = 0; j < Size; j++)
                    frame[j] *= factorArr[j];

                var specLine = transformer.Transform(frame, frameDuration);
                spectrum[i] = specLine.SpectrumMatrix[0];

                if (i == 0)
                    freqs = specLine.Freqs;
            }

            return new RawSpectrum(spectrum, freqs);
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
