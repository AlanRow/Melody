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
    class ScaleTypeItem : ParamItem
    {
        public readonly FreqScaleType Type;
        public ScaleTypeItem(string label, FreqScaleType type) : base(label)
        {
            Type = type;
        }
        public override bool Equals(object obj)
        {
            if (obj is ScaleTypeItem)
            {
                return (obj as ScaleTypeItem).Type == Type;
            }
            return false;
        }
    }
    class SumMethodItem : ParamItem
    {
        public readonly IntensSumMethod Type;
        public SumMethodItem(string label, IntensSumMethod type) : base(label)
        {
            Type = type;
        }
        public override bool Equals(object obj)
        {
            if (obj is SumMethodItem)
            {
                return (obj as SumMethodItem).Type == Type;
            }
            return false;
        }
    }
    class CalcMethodItem : ParamItem
    {
        public readonly IntensityCalcMethod Type;
        public CalcMethodItem(string label, IntensityCalcMethod type) : base(label)
        {
            Type = type;
        }
        public override bool Equals(object obj)
        {
            if (obj is CalcMethodItem)
            {
                return (obj as CalcMethodItem).Type == Type;
            }
            return false;
        }
    }
    /// <summary>
    /// Interaction logic for SpecParamsSelectWin.xaml
    /// </summary>
    public partial class SpecParamsSelectWin : Window
    {
        private TextBox startFreqInput;
        private TextBox octavesCountInput;
        private ComboBox scaleTypeSelect;
        private ComboBox calcMethodSelect;
        private ComboBox sumMethodSelect;
        private ScaleTypeItem[] GetScaleTypeItems()
        {
            return new ScaleTypeItem[]
            {
                new ScaleTypeItem("Линейная", FreqScaleType.Linear),
                new ScaleTypeItem("Логарифмическая", FreqScaleType.Log),
            };
        }
        private CalcMethodItem[] GetCalcMethodItems()
        {
            return new CalcMethodItem[]
            {
                new CalcMethodItem("Линейное", IntensityCalcMethod.Linear),
                new CalcMethodItem("Логарифмическое", IntensityCalcMethod.Log),
                new CalcMethodItem("Квадратичное", IntensityCalcMethod.Pow),
            };
        }
        private SumMethodItem[] GetSumMethodItems()
        {
            return new SumMethodItem[]
            {
                new SumMethodItem("Без суммирования", IntensSumMethod.No),
                new SumMethodItem("Максимум", IntensSumMethod.Max),
                new SumMethodItem("Среднее", IntensSumMethod.Average),
                new SumMethodItem("Среднее квадратов", IntensSumMethod.SquareAverage),
            };
        }


        private SpecViewParameters options;

        public SpecParamsSelectWin(SpecViewParameters viewOptions)
        {
            InitializeComponent();
            options = viewOptions;

            startFreqInput = (TextBox)FindName("StartFreqInput");
            startFreqInput.Text = options.StartFreq.ToString();

            octavesCountInput = (TextBox)FindName("OctavesCountInput");
            octavesCountInput.Text = options.OctavesCount.ToString();

            scaleTypeSelect = (ComboBox)FindName("ScaleTypeSelect");
            var scaleTypes = GetScaleTypeItems();
            scaleTypeSelect.ItemsSource = scaleTypes;
            for (var i = 0; i < scaleTypes.Length; i++)
                if (scaleTypes[i].Type == options.ScaleType)
                    scaleTypeSelect.SelectedIndex = i;

            calcMethodSelect = (ComboBox)FindName("CalcMethodSelect");
            var calcMethods = GetCalcMethodItems();
            calcMethodSelect.ItemsSource = calcMethods;
            for (var i = 0; i < calcMethods.Length; i++)
                if (calcMethods[i].Type == options.CalcMethod)
                    calcMethodSelect.SelectedIndex = i;

            sumMethodSelect = (ComboBox)FindName("SumMethodSelect");
            var sumMethods = GetSumMethodItems();
            sumMethodSelect.ItemsSource = sumMethods;
            for (var i = 0; i < sumMethods.Length; i++)
                if (sumMethods[i].Type == options.SumMethod)
                    sumMethodSelect.SelectedIndex = i;

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

            options.ScaleType = ((ScaleTypeItem)scaleTypeSelect.SelectedItem).Type;
            options.CalcMethod = ((CalcMethodItem)calcMethodSelect.SelectedItem).Type;
            options.SumMethod = ((SumMethodItem)sumMethodSelect.SelectedItem).Type;

            Close();
        }

        public void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
