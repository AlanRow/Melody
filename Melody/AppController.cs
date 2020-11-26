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
        private CalculatedSpectrum spectrum;
        public NoteData Note { get; private set; }

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

            var transformer = new SpectrumAnalyzer.Analyzer();
            var signal = new SpectrumAnalyzer.SimpleSignal(sound.Sound, sound.Duration);

            var specArr = transformer.GetSpectrum(signal);

            spectrum = new CalculatedSpectrum(specArr, sound.Duration);
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
