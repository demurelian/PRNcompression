using PRNcompression.Infrastructure.Commands;
using PRNcompression.Services;
using PRNcompression.Services.Interfaces;
using PRNcompression.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace PRNcompression.ViewModels 
{
    internal class SymmetricalInversionViewModel : ViewModel
    {
        private IPRNService _prnService;
        private string _NumStr;
        public string NumStr
        {
            get => _NumStr;
            set => Set(ref _NumStr, value);
        }
        private Dictionary<byte, int> _PRNDictionary;
        public Dictionary<byte, int> PRNDictionary
        {
            get => _PRNDictionary;
            set => Set(ref _PRNDictionary, value);
        }
        public ICommand CreateTableCommand { get; }
        private bool CanCreateTableCommandExecute(object p) => true;
        private void OnCreateTableCommandExecute(object p)
        {
            var number = ValidationHelper.ValidateNumberString(NumStr);
            var numberLength = (int)Math.Floor(Math.Log(number, 2)) + 1;
            PRNDictionary = new Dictionary<byte, int>();
            PRNDictionary.Add(10, _prnService.PRNGeneration(10, numberLength));
            PRNDictionary.Add(8, _prnService.PRNGeneration(8, numberLength));
            PRNDictionary.Add(5, _prnService.PRNGeneration(5, numberLength));
            PRNDictionary.Add(2, _prnService.PRNGeneration(2, numberLength));
            PRNDictionary.Add(3, _prnService.PRNGeneration(3, numberLength));
            PRNDictionary.Add(4, _prnService.PRNGeneration(4, numberLength));
            PRNDictionary.Add(1, _prnService.PRNGeneration(1, numberLength));
            PRNDictionary.Add(6, _prnService.PRNGeneration(6, numberLength));
            PRNDictionary.Add(9, _prnService.PRNGeneration(9, numberLength));
            PRNDictionary.Add(7, _prnService.PRNGeneration(7, numberLength));
            var list = new List<bool>();
            var CompressionInfo = _prnService.Compression(number, numberLength, ref list);
        }
        public SymmetricalInversionViewModel()
        {
            _prnService = new PRNService();
            CreateTableCommand = new LambdaCommand(OnCreateTableCommandExecute, CanCreateTableCommandExecute);
        }
    }
}
