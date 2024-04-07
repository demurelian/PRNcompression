using PRNcompression.ViewModels;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PRNcompression.Views
{
    public partial class InitialDataView : UserControl
    {
        public InitialDataView() => InitializeComponent();

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                var selectedItem = (FileViewModel)e.NewValue;
                FilePathBlock.Text = selectedItem.Path;
            }
            catch (InvalidCastException castEx)
            {
                FilePathBlock.Text = null;
            }
        }
    }
}
