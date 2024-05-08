using PRNcompression.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PRNcompression.Views.FileWork
{
    /// <summary>
    /// Логика взаимодействия для FileCompressionView.xaml
    /// </summary>
    public partial class FileCompressionView : UserControl
    {
        public FileCompressionView()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                FileCompressionViewModel viewModel = DataContext as FileCompressionViewModel;

                if (e.NewValue is FileViewModel)
                {
                    viewModel.SelectedFileViewModel = (FileViewModel)e.NewValue;
                }
            }
            catch (InvalidCastException ex)
            {

            }
        }
    }
}
