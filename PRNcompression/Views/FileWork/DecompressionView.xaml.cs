using PRNcompression.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PRNcompression.Views.FileWork
{
    /// <summary>
    /// Логика взаимодействия для DecompressionView.xaml
    /// </summary>
    public partial class DecompressionView : UserControl
    {
        public DecompressionView()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                DecompressionViewModel viewModel = DataContext as DecompressionViewModel;

                if (e.NewValue is FileViewModel)
                {
                    viewModel.SelectedServiceFileViewModel = (FileViewModel)e.NewValue;
                }
            }
            catch (InvalidCastException ex)
            {

            }
        }

        private void TreeView_SelectedItemChanged_1(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                DecompressionViewModel viewModel = DataContext as DecompressionViewModel;

                if (e.NewValue is FileViewModel)
                {
                    viewModel.SelectedCompressedFileViewModel = (FileViewModel)e.NewValue;
                }
            }
            catch (InvalidCastException ex)
            {

            }
        }
    }
}
