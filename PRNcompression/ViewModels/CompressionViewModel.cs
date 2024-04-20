using PRNcompression.Infrastructure.Commands;
using PRNcompression.Services;
using PRNcompression.Services.Interfaces;
using PRNcompression.ViewModels.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Documents;
using System.Windows.Input;

namespace PRNcompression.ViewModels
{
    public class NewCompressedInfo : CompressedInfo
    {
        public long InitialNumber { get; set; }
        public int InitialLength { get; set; }
        public int ListSize { get; set; }
    }
    internal class CompressionViewModel : ViewModel
    {
        private IPRNService _prnService;
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
        private ObservableCollection<NewCompressedInfo> _CompressionInfo;
        public ObservableCollection<NewCompressedInfo> CompressionInfo
        {
            get => _CompressionInfo;
            set => Set(ref _CompressionInfo, value);
        }
        public ICommand CompressionStartCommand { get; }
        private bool CanCompressionStartCommandExecute(object p) => true;
        private void OnCompressionStartCommandExecute(object p)
        {
            var number = ValidationHelper.ValidateNumberString(NumStr);
            var Numbers = 10000;
            var EfficientNumbers = 0;
            for (long i = number; i < number + Numbers; i++)
            {
                var length = _prnService.GetNumberLength(i);
                var list = new List<bool>();

                var item = _prnService.Compression(i, length, ref list);
                var item2 = new NewCompressedInfo
                {
                    InitialLength = length,
                    Type = item.Type,
                    Length = item.Length,
                    InversionInfo = item.InversionInfo,
                    InitialNumber = i,
                    ListSize = list.Count
                };
                var lengthOfLength = _prnService.GetNumberLength(item2.Length);
                if(item2.ListSize < length - 4 - lengthOfLength)
                {
                    CompressionInfo.Add(item2);
                    EfficientNumbers++;
                }
            }
            ResultStr = EfficientNumbers.ToString() + '/' + Numbers.ToString();
        }
        public CompressionViewModel()
        {
            _prnService = new PRNService();
            CompressionStartCommand = new LambdaCommand(OnCompressionStartCommandExecute, CanCompressionStartCommandExecute);
            CompressionInfo = new ObservableCollection<NewCompressedInfo>();
        }
    }
}
