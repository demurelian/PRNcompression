using PRNcompression.Infrastructure.Commands;
using PRNcompression.Model;
using PRNcompression.ViewModels.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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

            if (lastByte < 16)
            {
                byte type = (byte)(item.Type << 4);
                Console.WriteLine($"Новый тип: {type}");
                byte sum = (byte)(type | lastByte);
                Console.WriteLine($"Сумма: {sum}");
                lastByte = sum;
            }
            var fileInfo = new FileInfo(SelectedFileViewModel.Path);
            string serviceInfoPath = fileInfo.FullName.Replace(fileInfo.Extension, "_serviceInfo" + fileInfo.Extension);
            WriteBytesToFile(serviceInfoPath, bytes, item.Type);

            string newFilePath = fileInfo.FullName.Replace(fileInfo.Extension, "_compressed" + fileInfo.Extension);
            WriteBitArrayToFile(newFilePath, correctInversion);

            Console.WriteLine($"Новый путь: {serviceInfoPath}");
            Console.WriteLine($"Новый путь: {newFilePath}");
            Console.WriteLine("Число:" + ByteArrayToInt(bytes));
        }
        static void WriteBytesToFile(string filePath, byte[] byteArray, byte extraByte)
        {
            // Создаем новый массив байтов с дополнительным байтом в конце
            byte[] newArray = new byte[byteArray.Length + 1];
            Array.Copy(byteArray, newArray, byteArray.Length);
            newArray[byteArray.Length] = extraByte;

            // Создаем FileStream для записи в файл
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                // Записываем новый массив байтов в файл
                fs.Write(newArray, 0, newArray.Length);
            }
        }
        static void WriteBitArrayToFile(string filePath, BitArray bits)
        {
            // Создаем массив байтов для хранения битов
            byte[] bytes = new byte[(bits.Length + 7) / 8];

            // Копируем биты из BitArray в массив байтов
            bits.CopyTo(bytes, 0);

            // Создаем FileStream для записи в файл
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                // Записываем массив байтов в файл
                fs.Write(bytes, 0, bytes.Length);
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
        static int ByteArrayToInt(byte[] bytes)
        {
            var list = new List<byte>();
            for (int i = 0; i < 4; i++)
            {
                if (i >= bytes.Length)
                    list.Add(0);
                else
                    list.Add(bytes[i]);
            }
            var arr = list.ToArray();

            return BitConverter.ToInt32(arr, 0);
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
