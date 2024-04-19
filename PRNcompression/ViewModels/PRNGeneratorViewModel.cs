using PRNcompression.Infrastructure.Commands;
using PRNcompression.Services;
using PRNcompression.Services.Interfaces;
using PRNcompression.ViewModels.Base;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;

namespace PRNcompression.ViewModels
{
    internal class PRNGeneratorViewModel :  ViewModel
    {
        private IPRNService _PRNGenerationService;
        public ICommand GeneratePRNCommand { get; }
        private bool CanGeneratePRNCommandExecute(object p) => true;
        private void OnGeneratePRNCommandExecute(object p)
        {
            long result = -1;
            var size = ValidationHelper.ValidateNumberString(BitNumStr);
            if (size > 0 && size <= 64)
                result = _PRNGenerationService.PRNGeneration(SelectedType.Key, (int)size);
            ResultStr = result.ToString();
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
            _PRNGenerationService = new PRNService();

            GeneratePRNCommand = new LambdaCommand(OnGeneratePRNCommandExecute, CanGeneratePRNCommandExecute);

            TypesDiscriptions = new Dictionary<byte, string>();
            TypesDiscriptions.Add(1, "10101010...");
            TypesDiscriptions.Add(2, "01010101...");
            TypesDiscriptions.Add(3, "01111111...");
            TypesDiscriptions.Add(4, "10000000...");
            TypesDiscriptions.Add(5, "...00001111...");
            TypesDiscriptions.Add(6, "...11110000...");
            TypesDiscriptions.Add(7, "11111111...");
            TypesDiscriptions.Add(8, "...00000001");
            TypesDiscriptions.Add(9, "...11111110");
            TypesDiscriptions.Add(10, "10 000...01 (10 + тип 4)");
            TypesDiscriptions.Add(11, "11 000...01 (11 + тип 4)");
            TypesDiscriptions.Add(12, "10 111...11 (10 + тип 7)");
            TypesDiscriptions.Add(13, "01 000...01 (01 + тип 4)");
            TypesDiscriptions.Add(14, "11...111 01 (тип 7 + 01)");
            TypesDiscriptions.Add(15, "000...000");
        }
    }
}
