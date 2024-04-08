using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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

namespace PRNWpfTest
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Получение списка дисков
            string[] drives = Directory.GetLogicalDrives();
            var rootItems = new ObservableCollection<FileSystemItem>();

            foreach (string drive in drives)
            {
                var driveItem = new FileSystemItem
                {
                    Name = drive,
                    Type = FileSystemItemType.Drive,
                    Icon = "Images/drive.png" // Путь к изображению для иконки диска
                };
                rootItems.Add(driveItem);
            }

            // Установка корневых элементов в TreeView
            fileTreeView.ItemsSource = rootItems;
        }
    }

    public class FileSystemItem
    {
        public string Name { get; set; }
        public ObservableCollection<FileSystemItem> Children { get; set; }
        public string Icon { get; set; }
        public FileSystemItemType Type { get; set; }

        public FileSystemItem()
        {
            Children = new ObservableCollection<FileSystemItem>();
        }
    }

    public enum FileSystemItemType
    {
        Drive,
        Folder,
        File
    }
}
