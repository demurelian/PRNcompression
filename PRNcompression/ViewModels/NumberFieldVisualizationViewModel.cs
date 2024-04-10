using PRNcompression.Infrastructure.Commands;
using PRNcompression.Services;
using PRNcompression.ViewModels.Base;
using System.Collections.ObjectModel;
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
                for (int i = 0; i < 16; i++)
                {
                    var row = new RowItem();
                    for (int j = 0; j < 16; j++)
                    {
                        var tempItem = new DataItem
                        {
                            Value = StartNumber + i * 16 + j,
                            Color = (i % 2 > 0) ? Brushes.LightCoral : Brushes.LightBlue
                        };
                        row.Columns.Add(tempItem);
                    }
                    Data.Add(row);
                }
            }
        }

        public NumberFieldVisualizationViewModel()
        {
            VisualizeFromNumberCommand = new LambdaCommand(OnVisualizeFromNumberCommandExecute, CanVisualizeFromNumberCommandExecute);
            
        }
    }
}
