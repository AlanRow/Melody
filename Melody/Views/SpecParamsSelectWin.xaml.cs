using Melody.Structures;
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
    /// Interaction logic for SpecParamsSelectWin.xaml
    /// </summary>
    public partial class SpecParamsSelectWin : Window
    {
        private TextBox startFreqInput;
        private TextBox octavesCountInput;

        private SpecViewParameters options;

        public SpecParamsSelectWin(SpecViewParameters viewOptions)
        {
            InitializeComponent();
            options = viewOptions;

            startFreqInput = (TextBox)FindName("StartFreqInput");
            startFreqInput.Text = options.StartFreq.ToString();

            octavesCountInput = (TextBox)FindName("OctavesCountInput");
            octavesCountInput.Text = options.OctavesCount.ToString();
        }
        public void AcceptParams(object sender, RoutedEventArgs e)
        {
            try
            {
                var startFreq = Double.Parse(startFreqInput.Text);
                if (startFreq <= 0)
                    throw new FormatException();
                options.StartFreq = startFreq;
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Начальная частота должна быть действительным положительным числом");
            }

            try
            {
                var octavesCount = Double.Parse(octavesCountInput.Text);
                if (octavesCount <= 0)
                    throw new FormatException();
                options.OctavesCount = octavesCount;
            }
            catch (FormatException ex)
            {
                MessageBox.Show("Число октав должно быть действительным положительным числом");
            }

            Close();
        }

        public void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
