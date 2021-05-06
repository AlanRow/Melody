using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Melody.SpectrumAnalyzer
{
    interface ITransformer
    {
        Spectrum Transform(double[] signal, double duration);
    }
}
