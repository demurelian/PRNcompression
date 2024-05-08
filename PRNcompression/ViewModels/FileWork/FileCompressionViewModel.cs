using PRNcompression.Infrastructure.Commands;
using PRNcompression.Model;
using PRNcompression.ViewModels.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
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
        private FileViewModel _SelectedFileViewModel;
        public FileViewModel SelectedFileViewModel
        {
            get => _SelectedFileViewModel;
            set => Set(ref _SelectedFileViewModel, value);
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
            // Чтение содержимого файла в байтовый массив
            byte[] byteArray = File.ReadAllBytes(SelectedFileViewModel.Path);

            // Создание BitArray из байтового массива
            BitArray bitArray = new BitArray(byteArray);
            var length = byteArray.Length * 8;
            BitArray InversionInfo = new BitArray(length);

            var item = _prnDataWorker.FileCompression(bitArray, length, ref InversionInfo);

            byte[] bytes = IntToByteArray(item.Length);

            byte lastByte = 0;
            var correctInversion = InversionArrToCorrectLength(InversionInfo, item.Length);

            byte[] inversionInBytes = new byte[(correctInversion.Length + 7) / 8];

            // Копируем биты из BitArray в массив байтов
            Console.WriteLine("Биты после коррекции:");
            for (int j = 0; j < correctInversion.Length; j++)
            {
                if (correctInversion.Get(j))
                    Console.Write(1);
                else
                    Console.Write(0);
            }
            correctInversion.CopyTo(inversionInBytes, 0);

            Console.WriteLine();
            // Вывод массива байт на консоль для демонстрации
            Console.WriteLine("Массив байт:");
            foreach (byte b in bytes)
            {
                lastByte = b;
                Console.Write(b + " ");
            }

            var fileInfo = new FileInfo(SelectedFileViewModel.Path);
            string serviceInfoPath = fileInfo.FullName.Replace(fileInfo.Extension, "_serviceInfo" + fileInfo.Extension);
            WriteBytesToFile(serviceInfoPath, bytes, item.Type);

            if (lastByte < 16)
            {
                byte type = (byte)(item.Type << 4);
                Console.WriteLine($"Новый тип: {type}");
                byte sum = (byte)(type | lastByte);
                Console.WriteLine($"Сумма: {sum}");
                lastByte = sum;
                WriteBytesToFile(serviceInfoPath, bytes, lastByte, 1);
            }

            string newFilePath = fileInfo.FullName.Replace(fileInfo.Extension, "_compressed" + fileInfo.Extension);
            _prnDataWorker.WriteBitArrayToFile(newFilePath, correctInversion);

            Console.WriteLine($"Новый путь: {serviceInfoPath}");
            Console.WriteLine($"Новый путь: {newFilePath}");
            Console.WriteLine("Число:" + _prnDataWorker.ByteArrayToInt(bytes));

            MessageBox.Show($"Файл успешно преобразован в файлы {serviceInfoPath} и {newFilePath}", "Результат сжатия файла", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        static void WriteBytesToFile(string filePath, byte[] byteArray, byte extraByte, byte sew = 0 /*сшить длину и тип в последний байт*/)
        {
            // Создаем новый массив байтов с дополнительным байтом в конце
            byte[] newArray = new byte[byteArray.Length + 1 - sew];
            Array.Copy(byteArray, newArray, byteArray.Length);
            newArray[byteArray.Length - sew] = extraByte;

            // Создаем FileStream для записи в файл
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                // Записываем новый массив байтов в файл
                fs.Write(newArray, 0, newArray.Length);
            }
        }
        static BitArray InversionArrToCorrectLength(BitArray InversionInfo, int lengthOfPrn)
        {
            var newLength = InversionInfo.Length - lengthOfPrn;
            var result = new BitArray(newLength);
            for (int i = 0; i < newLength; i++)
            {
                result[i] = InversionInfo[i + lengthOfPrn];
            }
            return result;
        }
        static byte[] IntToByteArray(int value)
        {
            var list = new List<byte>();

            byte[] bytes = BitConverter.GetBytes(value);
            foreach (var b in bytes)
            {
                if (b != 0)
                    list.Add(b);
            }
            return list.ToArray();
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
