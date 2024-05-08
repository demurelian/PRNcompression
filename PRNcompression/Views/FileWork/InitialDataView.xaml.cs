using PRNcompression.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PRNcompression.Views.FileWork
{
    public partial class InitialDataView : UserControl
    {
        public InitialDataView() => InitializeComponent();

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                InitialDataViewModel viewModel = DataContext as InitialDataViewModel;

                if (e.NewValue is DirectoryViewModel)
                {
                    var selectedItem = (DirectoryViewModel)e.NewValue;
                    viewModel.PathString = selectedItem.Path;
                }
            }
            catch (InvalidCastException ex)
            {
                PathTextBox.Text = null;
            }
        }
    }
}
