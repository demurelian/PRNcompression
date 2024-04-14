using PRNcompression.Infrastructure.Commands;
using PRNcompression.Services;
using PRNcompression.Services.Interfaces;
using PRNcompression.ViewModels.Base;
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
        private IPRNService _PRNService;
        private string _NumStr;
        public string NumStr
        {
            get => _NumStr;
            set => Set(ref _NumStr, value);
        }

        private int _StartNumber;
        public int StartNumber
        {
            get => _StartNumber;
            set => Set(ref _StartNumber, value);
        }

        private ObservableCollection<RowInfo> _Data;
        public ObservableCollection<RowInfo> Data
        {
            get => _Data;
            set => Set(ref _Data, value);
        }

        private ObservableCollection<int> _NumQuantities;
        public ObservableCollection <int> NumQuantities
        {
            get => _NumQuantities;
            set => Set(ref _NumQuantities, value);
        }

        private int _SelectedQuantity;
        public int SelectedQuantity
        {
            get => _SelectedQuantity;
            set => Set(ref _SelectedQuantity, value);
        }

        public ICommand VisualizeFromNumberCommand { get; }
        private bool CanVisualizeFromNumberCommandExecute(object p) => true;
        private void OnVisualizeFromNumberCommandExecute(object p)
        {
            StartNumber = ValidationHelper.ValidateNumberString(NumStr);
            if (SelectedQuantity > 0)
            {
                if (StartNumber >= 0)
                {
                    for (int i = 0; i < 16; i++)
                        TypesInfo[i].Quantity = 0;

                    Data = new ObservableCollection<RowInfo>();
                    for (int i = 0; i < 16; i++)
                    {
                        var row = new RowInfo();
                        row.Items = new ObservableCollection<DataItem>();
                        for (int j = 0; j < SelectedQuantity / 16; j++)
                        {
                            var x = StartNumber + (SelectedQuantity / 16) * i + j;
                            var prnType = _PRNService.GetNumberType(x);
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
                    MyDataGrid.IsReadOnly = true;
                    MyDataGrid.AutoGenerateColumns = false;
                    MyDataGrid.ItemsSource = Data;
                    //Создаем столбцы с шаблонами
                    for (int i = 0; i < SelectedQuantity / 16; i++)
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
            NumQuantities = new ObservableCollection<int> { 64, 128, 256, 512, 1024, 2048, 4092, 8192 };

            _PRNService = new PRNService();

            VisualizeFromNumberCommand = new LambdaCommand(OnVisualizeFromNumberCommandExecute, CanVisualizeFromNumberCommandExecute);

            colorDictionary = new Dictionary<int, SolidColorBrush>
            {
                { 0, Brushes.Transparent },
                { 1, Brushes.Blue },
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

            TypesInfo = new ObservableCollection<TypeInfo>();
            for (int i = 0; i < 16; i++)
            {
                var typeInfo = new TypeInfo();
                typeInfo.TypeNum = i;
                typeInfo.Color = colorDictionary[i];
                typeInfo.Quantity = 0;
                TypesInfo.Add(typeInfo);
            }
        }
    }
}
