using PRNcompression.ViewModels;
using System;
using System.Diagnostics.Eventing.Reader;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Media.Media3D;

namespace PRNcompression
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                var selectedItem = (FileViewModel)e.NewValue;
                FilePathText.Text = selectedItem.Path;
            }
            catch (InvalidCastException castEx)
            {
                FilePathText.Text = null;
            }
        }
    }
}
