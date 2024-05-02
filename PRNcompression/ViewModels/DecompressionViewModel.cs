using PRNcompression.Infrastructure.Commands;
using PRNcompression.Model;
using PRNcompression.ViewModels.Base;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows.Input;

namespace PRNcompression.ViewModels
{
    internal class DecompressionViewModel : ViewModel
    {
        private PRNDataWorker _prnDataWorker;

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

        public ICommand DecompressFileCommand { get; }
        private bool CanDecompressFileCommandExecute(object p) => true;
        private void OnDecompressFileCommandExecute(object p)
        {
            var serviceBytes = ReadBytesFromFile(SelectedServiceFileViewModel.Path);
            byte type;
            byte[] newArr;
            if (serviceBytes[serviceBytes.Length - 1] > 16)
            {
                type = (byte)(serviceBytes[serviceBytes.Length - 1] >> 4);
                byte mask = 15;
                byte temp = (byte)(serviceBytes[serviceBytes.Length - 1] & mask);
                serviceBytes[serviceBytes.Length - 1] = temp;
                newArr = serviceBytes;
            }
            else
            {
                type = serviceBytes[serviceBytes.Length - 1];
                newArr = RemoveLastByte(serviceBytes);
            }

            var length = _prnDataWorker.ByteArrayToInt(newArr);
            var prnBits = _prnDataWorker.PRNGeneration(type, length);

            var bytes = ReadBytesFromFile(SelectedCompressedFileViewModel.Path);
            var inversions = new BitArray(bytes);
            int lastIndex = FindLastSignificantBitIndex(inversions);
            var correctInversions = RemoveTrailingZeros(inversions, lastIndex);

            for (int i = 0; i < correctInversions.Length; i++)
            {
                var temp = new BitArray(prnBits.Length + 1);
                for (int j = 0; j < prnBits.Length; j++)
                    temp[j] = prnBits[j];
                if (correctInversions[i])
                    temp.Not();
                prnBits = temp;
            }
            Console.WriteLine("Результат распаковки:");
            for (int i = 0; i < prnBits.Length; i++)
            {
                if (prnBits.Get(i))
                    Console.Write(1);
                else
                    Console.Write(0);
            }

            var fileInfo = new FileInfo(SelectedCompressedFileViewModel.Path);
            string newFilePath = fileInfo.FullName.Replace("_compressed.bin","_unpacked.bin");
            _prnDataWorker.WriteBitArrayToFile(newFilePath, prnBits);
        }
        //11110110100100011111100111010111111101000000010010100001010011110010000011001011
        //11110110100100011111100111010111111101000000010010100001010011110010000011001011
        static int FindLastSignificantBitIndex(BitArray bits)
        {
            for (int i = bits.Length - 1; i >= 0; i--)
            {
                if (bits[i])
                {
                    return i;
                }
            }
            return -1;
        }
        static BitArray RemoveTrailingZeros(BitArray bits, int lastIndex)
        {
            BitArray newBits = new BitArray(lastIndex + 1);
            for (int i = 0; i <= lastIndex; i++)
            {
                newBits[i] = bits[i];
            }
            return newBits;
        }
        static byte[] RemoveLastByte(byte[] byteArray)
        {
            byte[] newArray = new byte[byteArray.Length - 1];
            Array.Copy(byteArray, newArray, newArray.Length);
            return newArray;
        }

        static byte[] ReadBytesFromFile(string filePath)
        {
            // Создаем FileStream для чтения из файла
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                // Создаем буфер для чтения данных из файла
                byte[] buffer = new byte[fs.Length];

                // Считываем данные из файла в буфер
                fs.Read(buffer, 0, buffer.Length);

                return buffer;
            }
        }
        public DecompressionViewModel()
        {
            _prnDataWorker = new PRNDataWorker();

            Update1Command = new LambdaCommand(OnUpdate1CommandExecute, CanUpdate1CommandExecute);
            Update2Command = new LambdaCommand(OnUpdate2CommandExecute, CanUpdate2CommandExecute);
            DecompressFileCommand = new LambdaCommand(OnDecompressFileCommandExecute, CanDecompressFileCommandExecute);

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
