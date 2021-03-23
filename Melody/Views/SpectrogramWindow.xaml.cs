using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
        public SpectrogramWindow()
        {
            InitializeComponent();
        }

        public void DrawSpectrogram(double[][] spectrum)
        {
            var renderer = new SimpleRenderer(spectrum);
            var img = (Image)FindName("SpectrogramImage");
            var cont = (Canvas)FindName("SpectrogramContainer");
            renderer.DrawSpectrogram(img, 1000, 1000);
        }
    }
}
