using PRNcompression.Infrastructure.Commands;
using PRNcompression.ViewModels.Base;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;


namespace PRNcompression.ViewModels
{
    internal class FileCompareViewModel : ViewModel
    {
        private ObservableCollection<DirectoryViewModel> _Disks1;
        public ObservableCollection<DirectoryViewModel> Disks1
        {
            get => _Disks1;
            set => Set(ref _Disks1, value);
        }
        private ObservableCollection<DirectoryViewModel> _Disks2;
        public ObservableCollection<DirectoryViewModel> Disks2
        {
            get => _Disks2;
            set => Set(ref _Disks2, value);
        }
        private FileViewModel _SelectedServiceFileViewModel;
        public FileViewModel SelectedServiceFileViewModel
        {
            get => _SelectedServiceFileViewModel;
            set => Set(ref _SelectedServiceFileViewModel, value);
        }
        private FileViewModel _SelectedCompressedFileViewModel;
        public FileViewModel SelectedCompressedFileViewModel
        {
            get => _SelectedCompressedFileViewModel;
            set => Set(ref _SelectedCompressedFileViewModel, value);
        }
        public ICommand Update1Command { get; }
        private bool CanUpdate1CommandExecute(object p) => true;
        private void OnUpdate1CommandExecute(object p)
        {
            Disks1 = new ObservableCollection<DirectoryViewModel>();
            foreach (var drive in DriveInfo.GetDrives())
            {
                var dir = new DirectoryViewModel(drive.Name, false);
                Disks1.Add(dir);
            }
        }
        public ICommand Update2Command { get; }
        private bool CanUpdate2CommandExecute(object p) => true;
        private void OnUpdate2CommandExecute(object p)
        {
            Disks2 = new ObservableCollection<DirectoryViewModel>();
            foreach (var drive in DriveInfo.GetDrives())
            {
                var dir = new DirectoryViewModel(drive.Name, false);
                Disks2.Add(dir);
            }
        }
        public ICommand CompareCommand { get; }
        private bool CanCompareCommandExecute(object p) => true;
        private void OnCompareCommandExecute(object p)
        {
            CompareFiles(SelectedCompressedFileViewModel.Path, SelectedServiceFileViewModel.Path);
        }

        static void CompareFiles(string filePath1, string filePath2)
        {
            // Считываем содержимое обоих файлов в массивы байтов
            byte[] file1Bytes = File.ReadAllBytes(filePath1);
            byte[] file2Bytes = File.ReadAllBytes(filePath2);

            // Проверяем, одинаковы ли размеры файлов
            if (file1Bytes.Length != file2Bytes.Length)
            {
                MessageBox.Show("Файлы различаются по размеру", "Результат сравнения файлов", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Сравниваем содержимое файлов
            bool areEqual = true;
            for (int i = 0; i < file1Bytes.Length; i++)
            {
                if (file1Bytes[i] != file2Bytes[i])
                {
                    areEqual = false;
                    break;
                }
            }

            if (areEqual)
            {
                MessageBox.Show("Файлы идентичны", "Результат сравнения файлов", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Файлы различаются по содержимому", "Результат сравнения файлов", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public FileCompareViewModel()
        {
            Update1Command = new LambdaCommand(OnUpdate1CommandExecute, CanUpdate1CommandExecute);
            Update2Command = new LambdaCommand(OnUpdate2CommandExecute, CanUpdate2CommandExecute);
            CompareCommand = new LambdaCommand(OnCompareCommandExecute, CanCompareCommandExecute);

            Disks1 = new ObservableCollection<DirectoryViewModel>();
            foreach (var drive in DriveInfo.GetDrives())
            {
                var dir = new DirectoryViewModel(drive.Name, false);
                Disks1.Add(dir);
            }

            Disks2 = new ObservableCollection<DirectoryViewModel>();
            foreach (var drive in DriveInfo.GetDrives())
            {
                var dir = new DirectoryViewModel(drive.Name, false);
                Disks2.Add(dir);
            }
        }
    }
}
