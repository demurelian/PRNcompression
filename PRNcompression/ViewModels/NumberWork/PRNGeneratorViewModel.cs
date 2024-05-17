using PRNcompression.Infrastructure.Commands;
using PRNcompression.Model;
using PRNcompression.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Input;

namespace PRNcompression.ViewModels
{
    internal class PRNGeneratorViewModel :  ViewModel
    {
        private PRNDataWorker _prnDataWorker;
        public ICommand GeneratePRNCommand { get; }
        private bool CanGeneratePRNCommandExecute(object p) => true;
        private void OnGeneratePRNCommandExecute(object p)
        {
            try
            {
                var size = ulong.Parse(BitNumStr);
                if (size > 0 && size > 64)
                    throw new InvalidDataException();
                var result = _prnDataWorker.BitArrayToLong(_prnDataWorker.PRNGeneration(SelectedType.Key, (int)size));
                ResultStr = result.ToString();
                ResultBinaryStr = Convert.ToString((long)result, 2).PadLeft((int)size, '0');
            }
            catch (Exception e)
            {

            }
        }

        private string _BitNumStr;
        public string BitNumStr
        {
            get => _BitNumStr;
            set => Set(ref _BitNumStr, value);
        }

        private KeyValuePair<byte, string> _SelectedType;
        public KeyValuePair<byte, string> SelectedType
        {
            get => _SelectedType;
            set => Set(ref _SelectedType, value);
        }

        private string _ResultStr;
        public string ResultStr
        {
            get => _ResultStr;
            set => Set(ref _ResultStr, value);
        }

        private string _ResultBinaryStr;
        public string ResultBinaryStr
        {
            get => _ResultBinaryStr;
            set => Set(ref _ResultBinaryStr, value);
        }

        private Dictionary<byte, string> _TypesDiscriptions;
        public Dictionary<byte, string> TypesDiscriptions
        {
            get => _TypesDiscriptions;
            set => Set(ref _TypesDiscriptions, value);
        }

        public PRNGeneratorViewModel()
        {
            _prnDataWorker = new PRNDataWorker();

            GeneratePRNCommand = new LambdaCommand(OnGeneratePRNCommandExecute, CanGeneratePRNCommandExecute);

            TypesDiscriptions = new Dictionary<byte, string>();
            TypesDiscriptions.Add(0, "0...0");
            TypesDiscriptions.Add(1, "0001 + 1...1");
            TypesDiscriptions.Add(2, "0010 + 0...0");
            TypesDiscriptions.Add(3, "0011 + 1...1");
            TypesDiscriptions.Add(4, "0100 + 0...0");
            TypesDiscriptions.Add(5, "0010 + 1...1");
            TypesDiscriptions.Add(6, "0110 + 0...0");
            TypesDiscriptions.Add(7, "0111 + 1...1");
            TypesDiscriptions.Add(8, "1000 + 0...0");
            TypesDiscriptions.Add(9, "1001 + 1...1");
            TypesDiscriptions.Add(10, "1010 + 0...0");
            TypesDiscriptions.Add(11, "1011 + 1...1");
            TypesDiscriptions.Add(12, "1100 + 0...0");
            TypesDiscriptions.Add(13, "1101 + 1...1");
            TypesDiscriptions.Add(14, "1110 + 0...0");
            TypesDiscriptions.Add(15, "1...1");
        }
    }
}
