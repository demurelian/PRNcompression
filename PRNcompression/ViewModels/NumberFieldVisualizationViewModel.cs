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
    public class RowItem
    {
        public ObservableCollection<DataItem> Columns { get; set; }

        public RowItem()
        {
            Columns = new ObservableCollection<DataItem>();
        }
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

        private ObservableCollection<RowItem> _Data;
        public ObservableCollection<RowItem> Data
        {
            get => _Data;
            set => Set(ref _Data, value);
        }

        public ICommand VisualizeFromNumberCommand { get; }
        private bool CanVisualizeFromNumberCommandExecute(object p) => true;
        private void OnVisualizeFromNumberCommandExecute(object p)
        {
            StartNumber = ValidationHelper.ValidateNumberString(NumStr);
            if (StartNumber >= 0)
            {
                Data = new ObservableCollection<RowItem>();

                MyDataGrid = new DataGrid();
                MyDataGrid.IsReadOnly = true;
                MyDataGrid.AutoGenerateColumns = false;
                MyDataGrid.ItemsSource = Data;

                // Очищаем существующие столбцы в MyDataGrid
                MyDataGrid.Columns.Clear();

                for (int i = 0; i < 16; i++)
                {
                    // Создаем новый шаблон столбца
                    DataGridTemplateColumn column = new DataGridTemplateColumn();
                    column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
                    

                    // Создаем ячейку шаблона
                    DataTemplate cellTemplate = new DataTemplate();
                    
                    var row = new RowItem();

                    // Создаем TextBlock для отображения значения из DataItem
                    FrameworkElementFactory textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
                    textBlockFactory.SetBinding(TextBlock.TextProperty, new Binding($"Columns[{i}].Value"));
                    textBlockFactory.SetBinding(TextBlock.BackgroundProperty, new Binding($"Columns[{i}].Color"));

                    for (int j = 0; j < 16; j++)
                    {
                        var tempItem = new DataItem
                        {
                            Value = StartNumber + i * 16 + j,
                            Color = (i % 2 > 0) ? Brushes.LightCoral : Brushes.LightBlue
                        };
                        row.Columns.Add(tempItem);

                        // Добавляем TextBlock в ячейку шаблона
                        cellTemplate.VisualTree = textBlockFactory;
                    }

                    // Устанавливаем шаблон для ячейки столбца
                    column.CellTemplate = cellTemplate;

                    Data.Add(row);
                    MyDataGrid.Columns.Add(column);
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
            VisualizeFromNumberCommand = new LambdaCommand(OnVisualizeFromNumberCommandExecute, CanVisualizeFromNumberCommandExecute);


            

            //// Создаем шаблон ячейки
            //DataTemplate cellTemplate = new DataTemplate();

            //// Создаем элемент управления TextBlock
            //FrameworkElementFactory textBlockFactory = new FrameworkElementFactory(typeof(TextBlock));
            //textBlockFactory.SetBinding(TextBlock.TextProperty, new Binding("Columns[0].Value"));
            //textBlockFactory.SetBinding(TextBlock.BackgroundProperty, new Binding("Columns[0].Color"));

            //// Добавляем созданный TextBlock в шаблон ячейки
            //cellTemplate.VisualTree = textBlockFactory;

            //// Устанавливаем шаблон ячейки для колонки
            //templateColumn.CellTemplate = cellTemplate;

            //// Добавляем колонку в DataGrid
            //MyDataGrid.Columns.Add(templateColumn);
        }
    }
}
