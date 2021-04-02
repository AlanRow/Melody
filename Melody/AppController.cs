using FileScaner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Melody
{
    class AppController
    {
        private ExtractedSound sound;
        private ISignal signal;
        private CalculatedSpectrum spectrum;

        public Structures.TransformParameters TransformParameters;
        public NoteData Note { get; private set; }

        public AppController()
        {
            TransformParameters = new Structures.TransformParameters(Structures.FilterType.Rectangle, 1024, 1024);
        }

        public double[][] SpectrumIntensities
        {
            get
            {
                if (spectrum == null)
                    throw new Exception("Spectrum cant be get, because its not initialized");

                var arr = new double[spectrum.Spectrum.Length][];

                for (var i = 0; i < arr.Length; i++)
                    arr[i] = spectrum.Spectrum[i].Select((c) => c.Magnitude).ToArray();

                return arr;
            }
        }

        public double GetWinDuration()
        {
            if (sound == null)
            {
                throw new Exception("Invalid get max freq of empty spectrum");
            }

            // win dur = win length / SF = win length / signal length * signal duration
            // SF = signal length / signal duration
            return signal.GetDurationInSeconds() * TransformParameters.WindowSize / signal.GetActualLength();
        }

        /// <exception cref="AudioFileReadingException">File reading error</exception>
        public void ReadFile(string path)
        {
            try
            {
                var file = new WAVFile(path);
                var soundArray = file.GetChannel(0).Select(x => (double)x).ToArray();
                var duration = file.GetDuration();

                sound = new ExtractedSound(soundArray, duration);
            } catch (FileScaner.Exceptions.FileReadingException ex)
            {
                throw new AudioFileReadingException(String.Format("File reading was failed: {0}", ex.Message));
            } catch (FileScaner.Exceptions.FileCompressedException ex)
            {
                throw new AudioFileReadingException("This application doesnt support compressed files data");
            } catch (FormatException)
            {
                throw new AudioFileReadingException("File has bad format");
            }
        }

        /// <exception cref="FileNotLoadedException">File not loaded</exception>
        public void TransformSignal()
        {
            if (sound == null)
                throw new FileNotLoadedException("You must load file before extract signal from it");

            var transformer = new SpectrumAnalyzer.Analyzer(TransformParameters);
            signal = new SpectrumAnalyzer.SimpleSignal(sound.Sound, sound.Duration);

            var specArr = transformer.GetSpectrum(signal);

            spectrum = new CalculatedSpectrum(specArr, sound.Duration * specArr[0].Length / sound.Sound.Length );
        }

        /// <exception cref="SpectrumNotCalculatedException">Spectrum calculation in process</exception>
        public void GetNote()
        {
            if (spectrum == null)
                throw new SpectrumNotCalculatedException("You must wait till transformation will be done");

            var detector = new NoteDetector.SimpleDetector();
            var notePair = detector.DetectNote(spectrum.Spectrum[0], spectrum.Duration);

            Note = new NoteData(notePair.Item2.ToString(), notePair.Item1);
        }
    }
}
