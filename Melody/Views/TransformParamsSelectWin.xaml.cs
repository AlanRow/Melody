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
    class ParamItem
    {
        public readonly string Label;

        public ParamItem(string label)
        {
            Label = label;
        }

        public override string ToString()
        {
            return Label;
        }

        public override bool Equals(object obj)
        {
            if (obj is ParamItem)
            {
                return (obj as ParamItem).Label == Label;
            }
            return false;
        }
    }

    class FilterTypeItem : ParamItem
    {
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
        private static int MIN_SIZE_DEG = 6;
        private static int MAX_SIZE_DEG = 15;

        private Structures.TransformParameters trParams;

        private ComboBox filterTypeBox;
        private ComboBox winSizeBox;
        private ComboBox stepSizeBox;
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
        private SizeItem[] GetSizeItems()
        {
            var sizes = new SizeItem[MAX_SIZE_DEG - MIN_SIZE_DEG + 1];
            var startSize = (int)Math.Pow(2, MIN_SIZE_DEG);

            for (var i = 0; i < MAX_SIZE_DEG - MIN_SIZE_DEG + 1; i++)
            {
                sizes[i] = new SizeItem(startSize.ToString(), startSize);
                startSize *= 2;
            }

            return sizes;
        }

        public TransformParamsSelectWin(Structures.TransformParameters initParams)
        {
            InitializeComponent();

            trParams = initParams;
            
            var filterTypeItems = GetFilterTypeItems();

            filterTypeBox = (ComboBox)FindName("FilterTypeSelect");
            filterTypeBox.ItemsSource = filterTypeItems;

            for (var i = 0; i < filterTypeItems.Length; i++)
                if (filterTypeItems[i].Type == trParams.Type)
                    filterTypeBox.SelectedIndex = i;

            var sizeItems = GetSizeItems();

            winSizeBox = (ComboBox)FindName("WinSizeSelect");
            winSizeBox.ItemsSource = sizeItems;

            for (var i = 0; i < sizeItems.Length; i++)
                if (sizeItems[i].Size == trParams.WindowSize)
                    winSizeBox.SelectedIndex = i;

            stepSizeBox = (ComboBox)FindName("StepSizeSelect");
            stepSizeBox.ItemsSource = sizeItems;

            for (var i = 0; i < sizeItems.Length; i++)
                if (sizeItems[i].Size == trParams.StepSize)
                    stepSizeBox.SelectedIndex = i;
        }

        public void AcceptParams(object sender, RoutedEventArgs e)
        {
            trParams.Type = ((FilterTypeItem)filterTypeBox.SelectedItem).Type;
            trParams.WindowSize = ((SizeItem)winSizeBox.SelectedItem).Size;
            trParams.StepSize = ((SizeItem)stepSizeBox.SelectedItem).Size;
            Close();
        }

        public void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
