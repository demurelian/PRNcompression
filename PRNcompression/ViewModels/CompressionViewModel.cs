using PRNcompression.Infrastructure.Commands;
using PRNcompression.ViewModels.Base;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows.Input;
using System.Text;
using System.Net;
using PRNcompression.Model;

namespace PRNcompression.ViewModels
{
    struct StringThree
    {
        public string Discription { get; set; }
        public string ValueString { get; set; }
        public string BinaryString { get; set; }
    }

    internal class CompressionViewModel : ViewModel
    {
        private PRNDataWorker _prnDataWorker;
        #region Compression
        private string _NumStr;
        public string NumStr
        {
            get => _NumStr;
            set => Set(ref  _NumStr, value);
        }
        private string _ResultStr;
        public string ResultStr
        {
            get => _ResultStr;
            set => Set(ref _ResultStr, value);
        }
        private ObservableCollection<StringThree> _CompressionInfo;
        public ObservableCollection<StringThree> CompressionInfo
        {
            get => _CompressionInfo;
            set => Set(ref _CompressionInfo, value);
        }
        private ObservableCollection<StringThree> _NumberWayInfo;
        public ObservableCollection<StringThree> NumberWayInfo
        {
            get => _NumberWayInfo;
            set => Set(ref _NumberWayInfo, value);
        }
        public ICommand CompressionStartCommand { get; }
        private bool CanCompressionStartCommandExecute(object p) => true;
        private void OnCompressionStartCommandExecute(object p)
        {
            var number = ulong.Parse(NumStr);
            var length = _prnDataWorker.GetNumberLength(number);
            var boolList = new List<bool>();
            var ulongList = new List<ulong>();
            var item = _prnDataWorker.Compression(number, length, ref boolList, ref ulongList);

            if (ulongList.Count > 0)
            NumberWayInfo = new ObservableCollection<StringThree>();
            int i = 0;
            foreach (var currentNumber in ulongList)
            {
                var currItem = new StringThree
                {
                    Discription = $"{i+1}",
                    ValueString = currentNumber.ToString(),
                    BinaryString = Convert.ToString((long)currentNumber, 2).PadLeft(length - i, '0')
                };
                i++;
                NumberWayInfo.Add(currItem);
            }

            CompressionInfo = new ObservableCollection<StringThree>();

            var item2 = new StringThree
            {
                Discription = "Число",
                ValueString = number.ToString(),
                BinaryString = Convert.ToString((long)number, 2).PadLeft(length, '0')
            };
            CompressionInfo.Add(item2);

            var item3 = new StringThree
            {
                Discription = "Длина",
                ValueString = length.ToString(),
                BinaryString = "-"
            };
            CompressionInfo.Add(item3);

            var item4 = new StringThree
            {
                Discription = "Тип ПРЧ",
                ValueString = item.Type.ToString(),
                BinaryString = Convert.ToString(item.Type, 2).PadLeft(4, '0')
            };
            CompressionInfo.Add(item4);

            var lengthOfTypeLength = _prnDataWorker.GetNumberLength((ulong)item.Length);
            var item5 = new StringThree
            {
                Discription = "Длина ПРЧ",
                ValueString = item.Length.ToString(),
                BinaryString = Convert.ToString(item.Length, 2).PadLeft(lengthOfTypeLength, '0')
            };
            CompressionInfo.Add(item5);

            var item6 = new StringThree
            {
                Discription = "Инверсии",
                ValueString = item.InversionInfo.Count.ToString(),
                BinaryString = ConvertBoolListToBinaryString(item.InversionInfo)
            };
            CompressionInfo.Add(item6);

            var compressionFactor = (double)length / (4 + lengthOfTypeLength + item.InversionInfo.Count);
            var item7 = new StringThree
            {
                Discription = "Фактор сжатия",
                ValueString = compressionFactor.ToString(),
                BinaryString = "-"
            };
            CompressionInfo.Add(item7);

            var ration = (4 + lengthOfTypeLength + item.InversionInfo.Count) / (double)length;
            var item8= new StringThree
            {
                Discription = "Коэффициент сжатия",
                ValueString = ration.ToString(),
                BinaryString = "-"
            };
            CompressionInfo.Add(item8);

        }

        public static string ConvertBoolListToBinaryString(List<bool> boolList)
        {
            StringBuilder binaryString = new StringBuilder();

            foreach (bool bit in boolList)
            {
                binaryString.Append(bit ? '1' : '0');
            }

            return binaryString.ToString();
        }
        #endregion
        #region Decompression
        private string _ServiceInfoStr;
        public string ServiceInfoStr
        {
            get => _ServiceInfoStr;
            set => Set(ref _ServiceInfoStr, value);
        }
        private string _DataStr;
        public string DataStr
        {
            get => _DataStr;
            set => Set(ref _DataStr, value);
        }

        private ObservableCollection<StringThree> _DecompressionInfo;
        public ObservableCollection<StringThree> DecompressionInfo
        {
            get => _DecompressionInfo;
            set => Set(ref _DecompressionInfo, value);
        }
        private ObservableCollection<StringThree> _DecompressionNumbers;
        public ObservableCollection<StringThree> DecompressionNumbers
        {
            get => _DecompressionNumbers;
            set => Set(ref _DecompressionNumbers, value);
        }
        public ICommand DecompressionStartCommand { get; }
        private bool CanDecompressionStartCommandExecute(object p) => true;
        private void OnDecompressionStartCommandExecute(object p)
        {
            var numbers = new List<ulong>();
            var item = _prnDataWorker.Decompression(BitArrayFromBinaryString(ServiceInfoStr), BitArrayFromBinaryString(DataStr), ref numbers);

            DecompressionNumbers = new ObservableCollection<StringThree>();
            int i = 1;
            foreach(var num in numbers)
            {
                var newItem = new StringThree
                {
                    ValueString = num.ToString(),
                    BinaryString = Convert.ToString((long)num, 2),
                    Discription = $"{i}"
                };
                i++;
                DecompressionNumbers.Add(newItem);
            }

            DecompressionInfo = new ObservableCollection<StringThree>();
            var item2 = new StringThree
            {
                Discription = "Результат",
                ValueString = item.ResultNumber.ToString(),
                BinaryString = Convert.ToString((long)item.ResultNumber, 2)
            };
            DecompressionInfo.Add(item2);
        }

        public static BitArray BitArrayFromBinaryString(string binaryString)
        {
            if (binaryString == null)
                throw new ArgumentNullException(nameof(binaryString));

            var result = new BitArray(binaryString.Length);

            for (int i = 0; i < binaryString.Length; i++)
            {
                if (binaryString[i] == '1')
                    result.Set(i, true);
                else if (binaryString[i] == '0')
                    result.Set(i, false);
                else
                    throw new ArgumentException("Строка содержит недопустимые символы. Допустимы только '0' и '1'.");
            }

            return result;
        }
        #endregion
        public CompressionViewModel()
        {
            _prnDataWorker = new PRNDataWorker();

            CompressionStartCommand = new LambdaCommand(OnCompressionStartCommandExecute, CanCompressionStartCommandExecute);
            DecompressionStartCommand = new LambdaCommand(OnDecompressionStartCommandExecute, CanDecompressionStartCommandExecute);

        }
    }
}
