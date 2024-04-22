using PRNcompression.Infrastructure.Commands;
using PRNcompression.Services;
using PRNcompression.Services.Interfaces;
using PRNcompression.ViewModels.Base;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows.Input;
using System.Text;

namespace PRNcompression.ViewModels
{
    public class StringThree
    {
        public string Property { get; set; }
        public string ValueString { get; set; }
        public string BinaryString { get; set; }
    }
    //public class NewCompressedInfo
    //{
    //    public StringPair InitialNumber { get; set; }
    //    public StringPair InitialLength { get; set; }
    //    public StringPair Type { get; set; }
    //    public StringPair TypeLength { get; set; }
    //    public StringPair Inversion { get; set; }
    //}
    internal class CompressionViewModel : ViewModel
    {
        private IPRNService _prnService;
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
        public ICommand CompressionStartCommand { get; }
        private bool CanCompressionStartCommandExecute(object p) => true;
        private void OnCompressionStartCommandExecute(object p)
        {
            var number = ulong.Parse(NumStr);
            var length = _prnService.GetNumberLength(number);
            var list = new List<bool>();
            var item = _prnService.Compression(number, length, ref list);

            CompressionInfo = new ObservableCollection<StringThree>();

            var item2 = new StringThree
            {
                Property = "Число",
                ValueString = number.ToString(),
                BinaryString = Convert.ToString((long)number, 2).PadLeft(length, '0')
            };
            CompressionInfo.Add(item2);

            var item3 = new StringThree
            {
                Property = "Длина",
                ValueString = length.ToString(),
                BinaryString = ""
            };
            CompressionInfo.Add(item3);

            var item4 = new StringThree
            {
                Property = "Тип ПРЧ",
                ValueString = item.Type.ToString(),
                BinaryString = Convert.ToString(item.Type, 2).PadLeft(4, '0')
            };
            CompressionInfo.Add(item4);

            var lengthOfTypeLength = _prnService.GetNumberLength((ulong)item.Length);
            var item5 = new StringThree
            {
                Property = "Длина ПРЧ",
                ValueString = item.Length.ToString(),
                BinaryString = Convert.ToString(item.Length, 2).PadLeft(lengthOfTypeLength, '0')
            };
            CompressionInfo.Add(item5);

            var item6 = new StringThree
            {
                Property = "Инверсии",
                ValueString = item.InversionInfo.Count.ToString(),
                BinaryString = ConvertBoolListToBinaryString(item.InversionInfo)
            };
            CompressionInfo.Add(item6);
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
        private string _DecompressionResultStr;
        public string DecompressionResultStr
        {
            get => _DecompressionResultStr;
            set => Set(ref _DecompressionResultStr, value);
        }
        private DecompressedInfo _DecompressedInfo;
        public DecompressedInfo DecompressedInfo
        {
            get => _DecompressedInfo;
            set => Set(ref _DecompressedInfo, value);
        }
        public ICommand DecompressionStartCommand { get; }
        private bool CanDecompressionStartCommandExecute(object p) => true;
        private void OnDecompressionStartCommandExecute(object p)
        {
            var item = _prnService.Decompression(BitArrayFromBinaryString(ServiceInfoStr), BitArrayFromBinaryString(DataStr));
            DecompressionResultStr = item.Type.ToString() + " " + item.Length.ToString() + " " + item.ResultNumber.ToString();
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
            _prnService = new PRNService();

            CompressionStartCommand = new LambdaCommand(OnCompressionStartCommandExecute, CanCompressionStartCommandExecute);
            DecompressionStartCommand = new LambdaCommand(OnDecompressionStartCommandExecute, CanDecompressionStartCommandExecute);

        }
    }
}
