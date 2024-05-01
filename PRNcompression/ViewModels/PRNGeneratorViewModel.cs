using PRNcompression.Infrastructure.Commands;
using PRNcompression.Model;
using PRNcompression.ViewModels.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
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
            TypesDiscriptions.Add(1, "0...0 + 1");
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
        }
    }
}
