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
                    //var selectedItem = (FileViewModel)e.NewValue;
                    //viewModel.PathString = selectedItem.Path;
                }
            }
            catch (InvalidCastException ex)
            {
                PathTextBox.Text = null;
            }
        }
    }
}
