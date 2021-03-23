using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melody.Structures
{
    public enum FilterType
    {
        Rectangle,
        Triangle,
        Hann,
        Hamming,
    }

    public class TransformParameters
    {
        public FilterType Type;
        public int WindowSize;
        public int StepSize;
    }
}
