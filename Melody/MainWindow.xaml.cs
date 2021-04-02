using FileScaner;
using Melody.NoteDetector;
using Melody.SpectrumAnalyzer;
using Melody.Views;
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
        private AppController app;

        public MainWindow()
        {
            InitializeComponent();
            app = new AppController();
        }
        private void OpenSettingsClick(object sender, RoutedEventArgs e)
        {
            var paramsWin = new TransformParamsSelectWin(app.TransformParameters);
            paramsWin.ShowDialog();
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
                    // Parameters settings
                    try
                    {
                        app.ReadFile(path);
                        app.TransformSignal();
                        app.GetNote();
                        MessageBox.Show(String.Format("Actual frequency is {0}HZ ({1})", (int)Math.Round(app.Note.Hz), app.Note.Name));
                    }
                    catch (AudioFileReadingException ex)
                    {
                        MessageBox.Show(ex.Message, "File reading error");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error");
                    }
                    // Spectrogram rendering
                    var spectrogram = new SpectrogramWindow();
                    var intensities = app.SpectrumIntensities;
                    spectrogram.DrawSpectrogram(intensities, app.GetWinDuration());
                    spectrogram.Show();
                }
                else
                {
                    MessageBox.Show("Only WAV files is supported!", "Invalid file extension");
                }

            }
        }

    }
}
