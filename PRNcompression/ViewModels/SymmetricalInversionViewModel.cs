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
    class TypeValue
    {
        public byte Type {  get; set; }
        public int Number { get; set; }
        public string Binary { get; set; }
    }
    internal class SymmetricalInversionViewModel : ViewModel
    {
        private IPRNService _prnService;
        private string _NumStr;
        public string NumStr
        {
            get => _NumStr;
            set => Set(ref _NumStr, value);
        }
        private ObservableCollection<TypeValue> _CoreTypeValues;
        public ObservableCollection<TypeValue> CoreTypeValues
        {
            get => _CoreTypeValues;
            set => Set(ref _CoreTypeValues, value);
        }
        private ObservableCollection<TypeValue> _SupTypeValues;
        public ObservableCollection<TypeValue> SupTypeValues
        {
            get => _SupTypeValues;
            set => Set(ref _SupTypeValues, value);
        }
        public ICommand CreateTableCommand { get; }
        private bool CanCreateTableCommandExecute(object p) => true;
        private void OnCreateTableCommandExecute(object p)
        {
            var number = ValidationHelper.ValidateNumberString(NumStr);
            var numberLength = (int)Math.Floor(Math.Log(number, 2)) + 1;
            CoreTypeValues = new ObservableCollection<TypeValue>();
            for (byte i = 1; i <= 10; ++i)
            {
                if (numberLength % 2 == 1)
                    if (i == 5 || i == 6)
                        continue;
                var x = _prnService.PRNGeneration(i, numberLength);

                var item = new TypeValue
                {
                    Type = i,
                    Number = x,
                    Binary = Convert.ToString(x, 2).PadLeft(numberLength, '0')
                };
                CoreTypeValues.Add(item);
            }

            SupTypeValues = new ObservableCollection<TypeValue>();
            for (byte i = 11; i <= 15; i++)
            {
                var x = _prnService.PRNGeneration(i, numberLength);

                var item = new TypeValue
                {
                    Type = i,
                    Number = x,
                    Binary = Convert.ToString(x, 2).PadLeft(numberLength, '0')
                };
                SupTypeValues.Add(item);
            }

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
