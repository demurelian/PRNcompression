using PRNcompression.ViewModels.Base;
using System.Collections.ObjectModel;
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
        public ObservableCollection<RowItem> Data { get; set; }
        public NumberFieldVisualizationViewModel()
        {
            Data = new ObservableCollection<RowItem>();
            for (int i = 0; i < 16; i++)
            {
                var row = new RowItem();
                for (int j = 0; j < 16; j++)
                {
                    var tempItem = new DataItem
                    {
                        Value = i * 16 + j,
                        Color = (i % 2 > 0) ? Brushes.LightCoral : Brushes.LightBlue
                    };
                    row.Columns.Add(tempItem);
                }
                Data.Add(row);
            }
        }
    }
}
