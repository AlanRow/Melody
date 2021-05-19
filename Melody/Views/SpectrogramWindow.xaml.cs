using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Melody.Views
{
    /// <summary>
    /// Interaction logic for SpectrogramWindow.xaml
    /// </summary>
    public partial class SpectrogramWindow : Window
    {
        private static int WIDTH = 1200;
        private static int HEIGHT = 800;

        private Canvas cont;
        private Image img;
        private Popup freqPopup;
        private SimpleRenderer renderer;
        private SpecInfoConverter converter;

        public SpectrogramWindow()
        {
            InitializeComponent();
            img = (Image)FindName("SpectrogramImage");
            cont = (Canvas)FindName("SpectrogramContainer");
            freqPopup = (Popup)FindName("FreqPopup");
        }

        public void DrawSpectrogram(double[][] spectrum, double[] freqs, double dur, Structures.SpecViewParameters options)
        {
            renderer = new SimpleRenderer(spectrum, options);
            renderer.DrawSpectrogram(img, WIDTH, HEIGHT);

            converter = new SpecInfoConverter(freqs, dur, WIDTH, HEIGHT);
        }

        public void HandleMouseMove(object sender, MouseEventArgs e)
        {
            var p = e.GetPosition(img);
            freqPopup.HorizontalOffset = p.X;
            freqPopup.VerticalOffset = p.Y;

            var time = converter.GetTimeByCoord((int)p.X);
            var freq = converter.GetFreqByCoord((int)p.Y);

            var textBlock = (TextBlock)FindName("FreqTextBox");
            textBlock.Text = String.Format("{0} Гц ({1}мс)", Math.Round(freq), Math.Round(time * 1000));

            freqPopup.IsOpen = true;
        }
    }
}
