using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Melody.NoteDetector
{
    enum DetectionMetric
    {
        Scalar, // Distance between note is measuring as simple frequencies difference
        Log, // Distance is measuring as difference between ferquencies logarithm
    }

    enum Note
    {
        C,
        D,
        E,
        F,
        G,
        A,
        B,
        C_dur,
        D_dur,
        F_dur,
        G_dur,
        A_dur,
    }

    class NoteMeasurer
    {
        public DetectionMetric Metric { get; private set; }

        public NoteMeasurer(DetectionMetric metric)
        {
            Metric = metric;
        }

        public float GetDistance(float f1, float f2)
        {
            var dist = 0f;
            switch (Metric)
            {
                case DetectionMetric.Scalar:
                    dist = Math.Abs(f1 - f2);
                    break;
                case DetectionMetric.Log:
                    dist = (float) Math.Abs(Math.Log(f1) - Math.Log(f2));
                    break;
                default:
                    throw new ArgumentException("Unknown metric type - " + Metric.ToString());
            }

            return dist;
        }
    }

    class NoteConverter
    {
        private float step = 1.05946309f;
        private float A = 440;
        private float A2 = 880;
        private Note[] notes = new Note[] {
            Note.A, Note.A_dur, Note.B, Note.C, Note.C_dur, Note.D,
            Note.D_dur, Note.E, Note.F, Note.F_dur,Note.G, Note.G_dur
        };
        
        private NoteMeasurer measurer;

        public NoteConverter(NoteMeasurer distanceMeasurer)
        {
            measurer = distanceMeasurer;
        }

        public Note Convert(float freq)
        {
            // Fix octave
            while (freq < A)
                freq *= 2;
            while (freq > A2)
                freq /= 2;

            var bot = A;
            var top = A * step;
            var botIdx = 0;

            while (freq > top)
            {
                bot *= step;
                top *= step;
                botIdx++;
            }

            var botDist = measurer.GetDistance(bot, freq);
            var topDist = measurer.GetDistance(top, freq);

            if (botDist < topDist)
                return notes[botIdx];
            else if (botIdx < 11)
                return notes[botIdx + 1];
            else
                return notes[0];
        }
    }
}
