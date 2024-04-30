using PRNcompression.Infrastructure.Commands;
using PRNcompression.Model;
using PRNcompression.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;

namespace PRNcompression.ViewModels
{
    public class RowInfo
    {
        public ObservableCollection<DataItem> Items { get; set; }
    }

    public class DataItem
    {
        public int Value { get; set; }
        public Brush Color { get; set; }
    }

    public class TypeInfo : INotifyPropertyChanged
    {
        public int TypeNum { get; set; }
        public SolidColorBrush Color { get; set; }
        private int _Quantity;

        public event PropertyChangedEventHandler PropertyChanged;

        public int Quantity 
        {
            get => _Quantity;
            set
            {
                if (_Quantity != value)
                {
                    _Quantity = value;
                    OnPropertyChanged(nameof(Quantity));
                }
            }
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    internal class NumberFieldVisualizationViewModel : ViewModel
    {
        private PRNDataWorker _prnDataWorker;

        private ObservableCollection<RowInfo> _Data;
        public ObservableCollection<RowInfo> Data
        {
            get => _Data;
            set => Set(ref _Data, value);
        }

        private ObservableCollection<int> _DimensionalityOptions;
        public ObservableCollection <int> DimensionalityOptions
        {
            get => _DimensionalityOptions;
            set => Set(ref _DimensionalityOptions, value);
        }

        private int _Dimensionality;
        public int Dimensionality
        {
            get => _Dimensionality;
            set => Set(ref _Dimensionality, value);
        }

        public ICommand VisualizeFromNumberCommand { get; }
        private bool CanVisualizeFromNumberCommandExecute(object p) => true;
        private void OnVisualizeFromNumberCommandExecute(object p)
        {
            var fieldTypes = _prnDataWorker.FieldCharacterization((byte)Dimensionality);

            //var columnQuantity = (Dimensionality % 2 == 0) ? Math.Pow(2, Dimensionality / 2) : 2 * Math.Pow(2, Dimensionality / 2);
            //var rowQuantity = Math.Pow(2, Dimensionality / 2);
            var rowQuantity = 16;
            var columnQuantity = Math.Pow(2, Dimensionality) / 16;

            TypesInfo = new ObservableCollection<TypeInfo>();
            for (int i = 0; i <= Dimensionality - 3; i++)
            {
                var typeInfo = new TypeInfo();
                typeInfo.TypeNum = i;
                typeInfo.Color = colorDictionary[i];
                typeInfo.Quantity = 0;
                TypesInfo.Add(typeInfo);
            }

            Data = new ObservableCollection<RowInfo>();
            var currNum = 0;
            for (int i = 0; i < columnQuantity; i++)
            {
                var row = new RowInfo();
                row.Items = new ObservableCollection<DataItem>();
                for (int j = 0; j <  rowQuantity; j++)
                {
                    var x = currNum;
                    currNum++;
                    var prnType = fieldTypes[x];
                    //
                    TypesInfo[prnType].Quantity++;
                    var item = new DataItem
                    {
                        Value = x,
                        Color = TypesInfo[prnType].Color,
                    };
                    row.Items.Add(item);
                }
                Data.Add(row);
            }

            MyDataGrid = new DataGrid();
            MyDataGrid.EnableColumnVirtualization = true;
            MyDataGrid.EnableRowVirtualization = true;
            MyDataGrid.IsReadOnly = true;
            MyDataGrid.AutoGenerateColumns = false;
            MyDataGrid.ItemsSource = Data;
            //Создаем столбцы с шаблонами
            for (int i = 0; i < rowQuantity; i++)
            {
                var templateColumn = new DataGridTemplateColumn();

                // Создаем шаблон содержимого ячейки
                var cellTemplate = new DataTemplate();
                var textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
                textBlockFactory.SetBinding(TextBlock.TextProperty, new Binding(string.Format("Items[{0}].Value", i)));

                // Устанавливаем цвет фона из объекта DataItem
                var colorBinding = new Binding(string.Format("Items[{0}].Color", i));
                textBlockFactory.SetBinding(TextBlock.BackgroundProperty, colorBinding);

                cellTemplate.VisualTree = textBlockFactory;
                templateColumn.CellTemplate = cellTemplate;

                MyDataGrid.Columns.Add(templateColumn);
            }
        }

        private DataGrid _MyDataGrid;
        public DataGrid MyDataGrid
        {
            get => _MyDataGrid;
            set => Set(ref _MyDataGrid, value);
        }
        private ObservableCollection<TypeInfo> _TypesInfo;
        public ObservableCollection<TypeInfo> TypesInfo
        {
            get => _TypesInfo;
            set => Set(ref _TypesInfo, value);
        }
        private Dictionary<int, SolidColorBrush> colorDictionary;

        public NumberFieldVisualizationViewModel()
        {
            DimensionalityOptions = new ObservableCollection<int> { 4,5,6,7,8, 9, 10, 11, 12, 13, 14, 15, 16};

            _prnDataWorker = new PRNDataWorker();

            VisualizeFromNumberCommand = new LambdaCommand(OnVisualizeFromNumberCommandExecute, CanVisualizeFromNumberCommandExecute);

            colorDictionary = new Dictionary<int, SolidColorBrush>
            {
                { 0, Brushes.Transparent },
                { 1, Brushes.Gray },
                { 2, Brushes.DodgerBlue },
                { 3, Brushes.DeepSkyBlue },
                { 4, Brushes.LightBlue },
                { 5, Brushes.LightCyan },
                { 6, Brushes.Indigo },
                { 7, Brushes.BlueViolet },
                { 8, Brushes.DeepPink },
                { 9, Brushes.Pink },
                {10, Brushes.Silver },
                {11, Brushes.Aquamarine },
                {12, Brushes.MediumAquamarine },
                {13, Brushes.Lime },
                {14, Brushes.YellowGreen },
                {15, Brushes.DarkGreen }
            };

        }
    }
}
