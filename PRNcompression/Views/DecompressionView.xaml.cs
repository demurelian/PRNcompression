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
