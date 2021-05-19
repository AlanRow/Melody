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
            var paramsWin = new TransformParamsSelectWin(app.TransformParameters, 44100);
            paramsWin.ShowDialog();
        }
        private void OpenViewSettingsClick(object sender, RoutedEventArgs e)
        {
            var paramsWin = new SpecParamsSelectWin(app.SpecParameters);
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
                    //try
                    //{
                        app.ReadFile(path);
                        app.TransformSignal();
                        /*MessageBox.Show(String.Format("Actual frequency is {0}HZ ({1})", (int)Math.Round(app.Note.Hz), app.Note.Name));*/
                    /*}
                    catch (AudioFileReadingException ex)
                    {
                        MessageBox.Show(ex.Message, "File reading error");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "Error");
                    }*/
                }
                else
                {
                    MessageBox.Show("Only WAV files is supported!", "Invalid file extension");
                }
            }
        }

        private void ShowNotesClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var notes = app.GetNotes();
                var noteDur = 0d;
                var durStep = app.Spectrum.Duration;
                var notesLine = "";
                for (var i = 0; i < notes.Length; i++)
                {
                    noteDur += durStep;
                    if (i == notes.Length - 1 || notes[i].Name != notes[i+1].Name)
                    {
                        var durLine = Math.Round(noteDur * 1000);
                        noteDur = 0;
                        notesLine += String.Format("{0} ({1} ms)", notes[i].Name, durLine);
                        if (i < notes.Length - 1)
                            notesLine += ", ";
                    }
                }
                MessageBox.Show(notesLine, "Notes");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void OpenSpecClick(object sender, RoutedEventArgs e)
        {
            // Spectrogram rendering
            var spectrogram = new SpectrogramWindow();

            /*try
            {
                var intensities = app.SpectrumIntensities;
                spectrogram.DrawSpectrogram(intensities, app.GetWinDuration(), app.SpecParameters);
                spectrogram.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Чтобы построить спектрограмму, загрузите звуковой файл.");
            }*/
            var intensities = app.SpectrumIntensities;
            spectrogram.DrawSpectrogram(intensities, app.Spectrum.Spectrum.Freqs, app.GetWinDuration(), app.Signal.GetDurationInSeconds(), app.SpecParameters);
            spectrogram.Show();
        }

    }
}
