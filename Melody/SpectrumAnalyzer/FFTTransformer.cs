using System;
using System.Numerics;

using System.Linq;

namespace Melody.SpectrumAnalyzer
{
	/// <summary>
	/// Description of FFTTransformer.
	/// </summary>
	public class FFTTransformer : ITransformer
	{
		public Spectrum Transform(double[] signal, double duration)
        {
            var size = 1;
            while (size <= signal.Length)
                size *= 2;
            size /= 2;

            if (size != signal.Length)
                duration *= ((double)size) / signal.Length;

            var spec = FFTByTime(signal, size, 0);

            for (var i = 0; i < spec.Length; i++)
                spec[i] /= spec.Length;

            spec[0] /= 2;
            spec[size - 1] /= 2;

            var specMatrix = new Complex[][] { spec };
            
            var freqs = new double[spec.Length];
            for (var i = 1; i < freqs.Length; i++)
            {
                freqs[i] = ((double)i) / duration; 
            }

            return new Spectrum(specMatrix, freqs);
        }

        // выделить в отдельный класс
        private Complex[] FFTByTime(double[] signal, int size, int start)
        {
            var layer = new Complex[size];
            // если size равен 1, то возвращаем массив из одного элемента
            if (size == 1)
            {
                layer[0] = signal[start];
            }
            // иначе вызываем функцию рекурсивно и формируем из ответов новый слой
            else
            {
                var half = size >> 1;
                var left = FFTByTime(signal, half, start);
                var right = FFTByTime(signal, half, start + signal.Length / size);

                for (var i = 0; i < half; i++)
                {
                    var w = Complex.FromPolarCoordinates(1, -i * 2 * Math.PI / size);
                    layer[i] = left[i] + w * right[i];
                    layer[half + i] = left[i] - w * right[i];
                }
            }

            return layer;
        }
	}
}
