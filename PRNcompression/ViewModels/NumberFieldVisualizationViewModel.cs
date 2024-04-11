using PRNcompression.Infrastructure.Commands;
using PRNcompression.Services;
using PRNcompression.ViewModels.Base;
using System.Collections.ObjectModel;
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

    internal class NumberFieldVisualizationViewModel : ViewModel
    {
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
                    Data = new ObservableCollection<RowInfo>();
                    for (int i = 0; i < 16; i++)
                    {
                        var row = new RowInfo();
                        row.Items = new ObservableCollection<DataItem>();
                        for (int j = 0; j < SelectedQuantity / 16; j++)
                        {
                            var item = new DataItem
                            {
                                Value = StartNumber + (SelectedQuantity / 16) * i + j,
                                Color = (i % 2 > 0) ? Brushes.LightCoral : Brushes.LightBlue
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

        public NumberFieldVisualizationViewModel()
        {
            NumQuantities = new ObservableCollection<int> { 256, 512, 1024 };

            VisualizeFromNumberCommand = new LambdaCommand(OnVisualizeFromNumberCommandExecute, CanVisualizeFromNumberCommandExecute);
        }
    }
}
