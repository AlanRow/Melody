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
    class FilterTypeItem : ParamItem
    {
        private static double MIN_WIN = 0.005;
        private static double MIN_STEP = 0.005;

        public readonly Structures.FilterType Type;
        public FilterTypeItem(string label, Structures.FilterType type) : base(label)
        {
            Type = type;
        }
        public override bool Equals(object obj)
        {
            if (obj is FilterTypeItem)
            {
                return (obj as FilterTypeItem).Type == Type;
            }
            return false;
        }
    }
    class SizeItem : ParamItem
    {
        public readonly int Size;
        public SizeItem(string label, int size) : base(label)
        {
            Size = size;
        }
    }

    /// <summary>
    /// Interaction logic for TransformParamsSelectWin.xaml
    /// </summary>
    public partial class TransformParamsSelectWin : Window
    {
        private Structures.TransformParameters trParams;
        private int sampling;

        private ComboBox filterTypeBox;
        private TextBox winSizeInput;
        private TextBox winStepInput;
        private TextBox startFreqInput;
        private TextBox endFreqInput;
        private TextBox boundsInput;
        private FilterTypeItem[] GetFilterTypeItems()
        {
            return new FilterTypeItem[]
            {
                new FilterTypeItem("Прямоугольное окно", Structures.FilterType.Rectangle),
                new FilterTypeItem("Треугольное окно", Structures.FilterType.Triangle),
                new FilterTypeItem("Окно Ханна", Structures.FilterType.Hann),
                new FilterTypeItem("Окно Хэмминга", Structures.FilterType.Hamming),
            };
        }

        public TransformParamsSelectWin(Structures.TransformParameters initParams, int sampleRate)
        {
            InitializeComponent();

            trParams = initParams;
            sampling = sampleRate;
            
            var filterTypeItems = GetFilterTypeItems();

            filterTypeBox = (ComboBox)FindName("FilterTypeSelect");
            filterTypeBox.ItemsSource = filterTypeItems;

            for (var i = 0; i < filterTypeItems.Length; i++)
                if (filterTypeItems[i].Type == trParams.Type)
                    filterTypeBox.SelectedIndex = i;

            winSizeInput = (TextBox)FindName("WinSizeInput");
            winSizeInput.Text = GetDoubleLine(GetDurationInMs(trParams.WindowSize));

            winStepInput = (TextBox)FindName("WinStepInput");
            winStepInput.Text = GetDoubleLine(GetDurationInMs(trParams.StepSize));

            startFreqInput = (TextBox)FindName("StartFreqInput");
            startFreqInput.Text = trParams.StartFreq.ToString();

            endFreqInput = (TextBox)FindName("EndFreqInput");
            endFreqInput.Text = trParams.EndFreq.ToString();

            boundsInput = (TextBox)FindName("BoundsInput");
            boundsInput.Text = trParams.BoundsPerOctave.ToString();
        }

        public void AcceptParams(object sender, RoutedEventArgs e)
        {
            trParams.Type = ((FilterTypeItem)filterTypeBox.SelectedItem).Type;


            // TODO: Добавить валидацию полей
            try
            {
                var winSizeMs = Double.Parse(winSizeInput.Text);
                var winSize = GetSamples(winSizeMs);
                trParams.WindowSize = winSize;

                var winStepMs = Double.Parse(winStepInput.Text);
                var winStep = GetSamples(winStepMs);
                trParams.StepSize = winStep;

                var startFreq = Double.Parse(startFreqInput.Text);
                trParams.StartFreq = startFreq;

                var endFreq = Double.Parse(endFreqInput.Text);
                trParams.EndFreq = endFreq;

                var bounds = Int32.Parse(boundsInput.Text);
                trParams.BoundsPerOctave = bounds;

                Close();
            }
            catch (FormatException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private int GetSamples(double durationInMs)
        {
            return (int)Math.Round(durationInMs * sampling / 1000);
        }

        private double GetDurationInMs(int samplesLen)
        {
            return ((double)samplesLen) * 1000d / sampling;
        }

        private string GetDoubleLine(double d)
        {
            return Math.Round(d, 2).ToString();
        }
    }
}
