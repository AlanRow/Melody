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
        public double SoundDS;
        private ExtractedSound sound;
        private ISignal signal;
        public CalculatedSpectrum Spectrum;

        public Structures.TransformParameters TransformParameters;
        public Structures.SpecViewParameters SpecParameters;
        public NoteData Note { get; private set; }

        public AppController()
        {
            TransformParameters = new Structures.TransformParameters(Structures.FilterType.Rectangle, 1024, 1024, 220.0, 880.0, 48);
            SpecParameters = new Structures.SpecViewParameters(100, 4, Views.IntensityCalcMethod.Linear, Views.IntensSumMethod.Max, Views.FreqScaleType.Linear);
        }

        public double[][] SpectrumIntensities
        {
            get
            {
                if (Spectrum == null)
                    throw new Exception("Spectrum cant be get, because its not initialized");

                var arr = new double[Spectrum.Spectrum.SpectrumMatrix.Length][];

                for (var i = 0; i < arr.Length; i++)
                    arr[i] = Spectrum.Spectrum.SpectrumMatrix[i].Select((c) => c.Magnitude).ToArray();

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

                SoundDS = file.Sampling;

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
            var spec = new CalculatedSpectrum(specArr, sound.Duration * TransformParameters.WindowSize / sound.Sound.Length);
            Spectrum = spec;
        }

        /// <exception cref="SpectrumNotCalculatedException">Spectrum calculation in process</exception>
        public NoteData[] GetNotes()
        {
            if (Spectrum == null)
                throw new SpectrumNotCalculatedException("You must wait till transformation will be done");
            var detector = new NoteDetector.SimpleDetector();
            var notes = new NoteData[Spectrum.Spectrum.SpectrumMatrix.Length];

            for (var i = 0; i < Spectrum.Spectrum.SpectrumMatrix.Length; i++)
            {
                var notePair = detector.DetectNote(Spectrum.Spectrum.SpectrumMatrix[i], Spectrum.Duration);
                notes[i] = new NoteData(notePair.Item2.ToString(), notePair.Item1);
            }

            return notes;

            // var notePair = detector.DetectNote(spectrum.Spectrum[0], spectrum.Duration);

            // Note = new NoteData(notePair.Item2.ToString(), notePair.Item1);
        }
    }
}
