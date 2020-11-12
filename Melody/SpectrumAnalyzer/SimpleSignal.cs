using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melody.SpectrumAnalyzer
{
    class SimpleSignal : ISignal
    {
        private double[] signal;
        private double duration;

        public SimpleSignal(double[] signalValues, double durationInSeconds)
        {
            signal = signalValues;
            duration = durationInSeconds;
        }

        public override double GetDurationInSeconds()
        {
            return duration;
        }

        public override int GetLength()
        {
            return signal.Length;
        }

        public override double GetValueAt(int time)
        {
            return signal[time];
        }
    }
}
