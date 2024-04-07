using PRNcompression.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

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
