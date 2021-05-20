using Melody.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Melody.SpectrumAnalyzer
{
    class SpectrumNormalizer
    {
        private int filterLimit;

        public SpectrumNormalizer(int limit)
        {
            filterLimit = limit;
        }

        public double[][] GetNormalizedSpectrum(Complex[][] spectrum)
        {
            var w = spectrum.Length;
            var h = spectrum[0].Length;

            var nSpec = new double[w][];

            for (var i = 0; i < w; i++)
            {
                var nCol = new double[h];
                for (var j = 0; j < h; j++)
                {
                    var val = spectrum[i][j].Magnitude;
                    nCol[j] = val;
                    // nCol[j] = MathUtils.LogBy(val * 1000 + 1, 10);
                }
                nSpec[i] = nCol;
            }

            var dSpec = new double[w][];
            // Косинусное преобразование
            for (var i = 0; i < w; i++)
            {
                var dCol = new double[h];
                for (var j = 0; j < h; j++)
                {
                    var val = 0d;

                    // Если меньше эпсилон, то обнуляем
                    if (j >= filterLimit)
                    {
                        for (var k = 0; k < h; k++)
                        {
                            val += nSpec[i][k] * Math.Cos(Math.PI * (0.5 + k) * j / h);
                        }
                    }
                    dCol[j] = val;
                }
                dSpec[i] = dCol;
            }

            var rSpec = new double[w][];
            // Обратное косинусное преобразование
            for (var i = 0; i < w; i++)
            {
                var rCol = new double[h];
                for (var j = 0; j < h; j++)
                {
                    var val = 0d;

                    for (var k = 0; k < h; k++)
                    {
                        val += dSpec[i][k] * Math.Cos(Math.PI * (0.5 + j) * k / h);
                    }
                    rCol[j] = val;
                }
                rSpec[i] = rCol;
            }

            return rSpec;
        }
    }
}
