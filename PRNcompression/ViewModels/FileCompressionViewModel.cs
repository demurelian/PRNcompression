using PRNcompression.Infrastructure.Commands;
using PRNcompression.Model;
using PRNcompression.ViewModels.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PRNcompression.ViewModels
{
    internal class FileCompressionViewModel : ViewModel
    {
        private PRNDataWorker _prnDataWorker;

        private ObservableCollection<DirectoryViewModel> _Disks;
        public ObservableCollection<DirectoryViewModel> Disks
        {
            get => _Disks;
            set => Set(ref _Disks, value);
        }
        private string _PathString;
        public string PathString
        {
            get => _PathString;
            set => Set(ref _PathString, value);
        }
        public ICommand UpdateCommand { get; }
        private bool CanUpdateCommandExecute(object p) => true;
        private void OnUpdateCommandExecute(object p)
        {
            Disks = new ObservableCollection<DirectoryViewModel>();
            foreach (var drive in DriveInfo.GetDrives())
            {
                var dir = new DirectoryViewModel(drive.Name, false);
                Disks.Add(dir);
            }
        }
        public ICommand CompressFileCommand { get; }
        private bool CanCompressFileCommandExecute(object p) => true;
        private void OnCompressFileCommandExecute(object p)
        {
            try
            {
                // Чтение содержимого файла в байтовый массив
                byte[] byteArray = File.ReadAllBytes(PathString);

                // Создание BitArray из байтового массива
                BitArray bitArray = new BitArray(byteArray);

                // Пример использования BitArray
                Console.WriteLine("Первые 8 бит в файле:");
                for (int i = 0; i < 8; i++)
                {
                    Console.Write(bitArray[i] ? "1" : "0");
                }
                Console.WriteLine("Успешно");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public FileCompressionViewModel()
        {
            _prnDataWorker = new PRNDataWorker();

            CompressFileCommand = new LambdaCommand(OnCompressFileCommandExecute, CanCompressFileCommandExecute);
            UpdateCommand = new LambdaCommand(OnUpdateCommandExecute, CanUpdateCommandExecute);

            Disks = new ObservableCollection<DirectoryViewModel>();
            foreach (var drive in DriveInfo.GetDrives())
            {
                var dir = new DirectoryViewModel(drive.Name, false);
                Disks.Add(dir);
            }
        }
    }
}
