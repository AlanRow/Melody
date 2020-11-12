using FileScaner;
using Melody.NoteDetector;
using Melody.SpectrumAnalyzer;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Melody
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //private WAVFile file;
        private Spectrum spectrum;
        private SimpleSignal signal;
        //private double duration;
        private double hz;

        private void ReadFile(string path)
        {
            var file = new WAVFile(path);

            var length = 1;
            for (var i = 1; i <= file.SamplesCount; i*=2)
                length = i;

            var vals = file.GetChannel(0).Take(length).Select(i => (double)i).ToArray();
            var duration = file.GetDuration() * length / file.SamplesCount;
            signal = new SimpleSignal(vals, duration);
        }

        private void Transform()
        {
            if (signal == null)
            {
                throw new Exception("WAV file is not determined");
            }
            spectrum = new Analyzer().GetSpectrum(signal);
        }

        private void DetectHZ()
        {
            var spec = spectrum.Specs[0].Spectrum.Select(fp => fp.Coords).ToArray();
            hz = new SimpleDetector().DetectNote(spec, signal.GetDurationInSeconds());
        }

        private void AnalyzeFile(string path)
        {
            ReadFile(path);
            Transform();
            DetectHZ();
        }

        public MainWindow()
        {
            InitializeComponent();
        }
        private void OpenFileClick(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                var path = dialog.FileName;
                var name = path.Split('\\').Last();
                var ext = name.Split('.').Last().ToLower();

                if (ext == "wav")
                {
                    /*try
                    {*/
                        AnalyzeFile(path);
                        MessageBox.Show(String.Format("Actual frequency is {0}HZ", hz));
                    /*}
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "File handling error");
                    }*/
                }
                else
                {
                    MessageBox.Show("Only WAV files is supported!", "Invalid file extension");
                }

            }
        }

    }
}
