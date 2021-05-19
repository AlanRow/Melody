using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Melody.Utils;

namespace Melody.SpectrumAnalyzer
{
    /**
     * Standard DFT with constant bounds on octave
     */
    class LogFTTransformer : ITransformer
    {
        // Min frequency (Hz)
        public double StartFreq;
        // Max frequency (Hz)
        public double EndFreq;
        // Count of spectrum calculations between two octaves
        public int BoundsPerOctave;

        // Window size in samples
        public readonly int WinSize;
        // Window step in samples
        public readonly int WinStep;
        // Filter mult function
        public readonly Filter Filter;

        public LogFTTransformer(double start, double end, int bounds, int size,int step, Filter filter)
        {
            StartFreq = start;
            EndFreq = end;
            BoundsPerOctave = bounds;

            WinSize = size;
            WinStep = step;
            Filter = filter;
        }

        // Octaves count between start frequency and end frequency
        public double OctavesCount
        {
            get
            {
                return MathUtils.Log2(EndFreq) - MathUtils.Log2(StartFreq);
            }
        }

        // All bounds count
        public int BoundsCount
        {
            get
            {
                return (int)Math.Floor(OctavesCount * BoundsPerOctave + 1);
            }
        }

        // Get spectrum
        public Spectrum Transform(double[] signal, double duration)
        {
            var specLength = MathUtils.GetWindowsCount(signal.Length, WinSize, WinStep);
            var spectrum = new Complex[specLength][];

            var frame = new double[WinSize];
            var frameDuration = MathUtils.GetFrameDuration(duration, signal.Length, WinSize);

            var freqsCount = BoundsCount;
            var freqs = new double[freqsCount];
            var actualFreqs = new double[freqsCount];
            var freqStep = Math.Pow(2, 1.0 / BoundsPerOctave);
            
            var f = StartFreq;

            for (var i = 0; i < freqs.Length; i++)
            {
                freqs[i] = f * frameDuration;
                actualFreqs[i] = f;
                f *= freqStep;
            }

            for (var time = 0; time + WinSize - 1 < signal.Length; time += WinStep)
            {
                var specAtTime = new Complex[freqsCount];

                // Calculate windowed signal frame
                Array.Copy(signal, time, frame, 0, WinSize);
                for (var i = 0; i < WinSize; i++)
                    frame[i] = Filter.GetFiltered(frame[i], WinSize, i);

                for (var freqIdx = 0; freqIdx < freqs.Length; freqIdx++)
                {
                    f = freqs[freqIdx];
                    var val = Complex.Zero;

                    for (var i = time; i < time + WinSize; i++)
                    {
                        val += Complex.FromPolarCoordinates(signal[i], -i * 2 * Math.PI * f  / WinSize);
                    }

                    specAtTime[freqIdx] = val;
                }

                spectrum[time / WinStep] = specAtTime;
            }

            return new Spectrum(spectrum, actualFreqs);
        }
    }
}
