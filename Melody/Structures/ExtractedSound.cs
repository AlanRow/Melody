using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melody
{
    class ExtractedSound
    {
        public readonly double[] Sound;
        public readonly double Duration;

        public ExtractedSound(double[] sound, double duration)
        {
            Sound = sound;
            Duration = duration;
        }
    }
}
