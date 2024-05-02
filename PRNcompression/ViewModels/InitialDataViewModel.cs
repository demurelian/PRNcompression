using PRNcompression.Infrastructure.Commands;
using PRNcompression.Model;
using PRNcompression.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace PRNcompression.ViewModels
{
    internal class InitialDataViewModel : ViewModel
    {
        private PRNDataWorker _prnDataWorker;

        private ObservableCollection<DirectoryViewModel> _Disks;
        public ObservableCollection<DirectoryViewModel> Disks
        {
            get => _Disks;
            set => Set(ref _Disks, value);
        }

        private IEnumerable<byte> _InitialBytes;
        public IEnumerable<byte> InitialBytes
        {
            get => _InitialBytes;
            private set => Set(ref _InitialBytes, value);
        }

        private string _ByteNumberStr;
        public string ByteNumberStr
        {
            get => _ByteNumberStr;
            set => Set(ref _ByteNumberStr, value);
        }

        public ICommand FileSelectedCommand { get; }
        private bool CanFileSelectedCommandExecute(object p) => true;
        private void OnFileSelectedCommandExecute(object p)
        {
            string fileName = "random_data.bin"; // Имя файла для чтения

            try
            {
                // Проверяем существует ли файл
                if (File.Exists(fileName))
                {
                    // Получаем размер файла
                    FileInfo fileInfo = new FileInfo(fileName);
                    long fileSize = fileInfo.Length;

                    // Создаем массив байт нужного размера
                    byte[] bytes = new byte[fileSize];

                    // Открываем файл для чтения
                    using (FileStream fs = new FileStream(fileName, FileMode.Open))
                    {
                        // Считываем данные из файла в массив байт
                        fs.Read(bytes, 0, (int)fileSize);
                    }

                    Console.WriteLine($"Данные из файла {fileName} успешно считаны в массив байт");
                }
                else
                {
                    Console.WriteLine($"Файл {fileName} не существует");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при чтении файла: {ex.Message}");
            }
        }

        public ICommand GenerateDataCommand { get; }
        private bool CanGenerateDataCommandExecute(object p) => true;
        private void OnGenerateDataCommandExecute(object p)
        {
            try
            {
                var size = int.Parse(ByteNumberStr);
                string path = PathString + "\\" + FileName + ".bin";
                if (GenerateOption1 == true)
                {
                    byte[] bytes = RandomBytes(size);
                    BytesToFile(bytes, path);
                }
                else
                {
                    byte[] bytes = _prnDataWorker.PRNByteArrGenerator(SelectedType.Key, size);
                    BytesToFile(bytes, path);
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            
        }
        private byte[] RandomBytes(int size)
        {
            // Создаем массив байт
            byte[] bytes = new byte[size];

            // Создаем генератор случайных чисел
            Random rand = new Random();

            // Заполняем массив случайными байтами
            rand.NextBytes(bytes);

            if (bytes[size - 1] < 128)
                bytes[size - 1] += 128;
            return bytes;
        }

        private void BytesToFile(byte[] arr, string path)
        {
            try
            {
                // Открываем файл для записи
                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    // Записываем массив байт в файл
                    fs.Write(arr, 0, arr.Length);
                }

                Console.WriteLine($"Массив байт успешно записан в файл {path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при записи файла: {ex.Message}");
            }
        }
        private bool _GenerateOption1;
        public bool GenerateOption1
        {
            get => _GenerateOption1;
            set => Set(ref _GenerateOption1, value);
        }
        private bool _GenerateOption2;
        public bool GenerateOption2
        {
            get => _GenerateOption2;
            set => Set(ref _GenerateOption2, value);
        }
        private Dictionary<byte, string> _TypesDiscriptions;
        public Dictionary<byte, string> TypesDiscriptions
        {
            get => _TypesDiscriptions;
            set => Set(ref _TypesDiscriptions, value);
        }
        private KeyValuePair<byte, string> _SelectedType;
        public KeyValuePair<byte, string> SelectedType
        {
            get => _SelectedType;
            set => Set(ref _SelectedType, value);
        }
        private string _PathString;
        public string PathString
        {
            get => _PathString;
            set => Set(ref _PathString, value);
        }
        private string _FileName;
        public string FileName
        {
            get => _FileName;
            set => Set(ref _FileName, value);
        }

        public InitialDataViewModel()
        {
            _prnDataWorker = new PRNDataWorker();

            TypesDiscriptions = new Dictionary<byte, string>();
            TypesDiscriptions.Add(2, "00 + 0101... + 10 (00 + 0101... + 11 для нечет)");
            TypesDiscriptions.Add(3, "00 + 1010... + 11 (00 + 1010... + 10 для нечет)");
            TypesDiscriptions.Add(4, "01 + 0...0");
            TypesDiscriptions.Add(5, "0101...");
            TypesDiscriptions.Add(6, "01 + 1010...");
            TypesDiscriptions.Add(7, "01 + 1...1");
            TypesDiscriptions.Add(8, "1 + 0...0");
            TypesDiscriptions.Add(9, "10 + 0101...");
            TypesDiscriptions.Add(10, "1010...");
            TypesDiscriptions.Add(11, "10 + 1...1");
            TypesDiscriptions.Add(12, "11 + 0101... + 00 (11 + 0101... + 01 для нечет)");
            TypesDiscriptions.Add(13, "11 + 1010... + 01 (11 + 1010... + 00 для нечет)");
            TypesDiscriptions.Add(14, "1...1 + 0");
            TypesDiscriptions.Add(15, "1...1");

            GenerateOption1 = true;
            GenerateDataCommand = new LambdaCommand(OnGenerateDataCommandExecute, CanGenerateDataCommandExecute);
            FileSelectedCommand = new LambdaCommand(OnFileSelectedCommandExecute, CanFileSelectedCommandExecute);
            Disks = new ObservableCollection<DirectoryViewModel>();

            // Simulating some data (replace with your logic)
            foreach (var drive in DriveInfo.GetDrives())
            {
                var dir = new DirectoryViewModel(drive.Name, true);
                Disks.Add(dir);
            }
        }
    }
}
