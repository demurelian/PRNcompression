using PRNcompression.Infrastructure.Commands;
using PRNcompression.Services;
using PRNcompression.Services.Interfaces;
using PRNcompression.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace PRNcompression.ViewModels 
{
    class TypeValue
    {
        public byte Type {  get; set; }
        public int Number { get; set; }
        public string Binary { get; set; }
    }
    class TableValue
    {
        public string TypeString { get; set; }
        public int Number { get; set; }
        public string BinaryString { get; set; }
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

        private ObservableCollection<TableValue> _GeneralTable;
        public ObservableCollection<TableValue> GeneralTable
        {
            get => _GeneralTable;
            set => Set(ref _GeneralTable, value);
        }
        private ObservableCollection<int> _DimensionalityOptions;
        public ObservableCollection<int> DimensionalityOptions
        {
            get => _DimensionalityOptions;
            set => Set(ref _DimensionalityOptions, value);
        }

        private int _Dimensionality;
        public int Dimensionality
        {
            get => _Dimensionality;
            set => Set(ref _Dimensionality, value);
        }
        public ICommand CreateGeneralTableCommand { get; }
        private bool CanCreateGeneralTableCommandExecute(object p) => true;
        private void OnCreateGeneralTableCommandExecute(object p)
        {
            GeneralTable = new ObservableCollection<TableValue>();
            int maxNum = (int)Math.Pow(2, Dimensionality);

            for (int i = 0; i < maxNum; i++)
            {
                var type = _prnService.GetNumberType(i, Dimensionality);

                var item = new TableValue
                {
                    TypeString = (type == -1) ? "" : type.ToString(),
                    Number = i,
                    BinaryString = Convert.ToString(i, 2).PadLeft(Dimensionality, '0')
                };

                GeneralTable.Add(item);
            }
        }
        public ICommand CreateTableCommand { get; }
        private bool CanCreateTableCommandExecute(object p) => true;
        private void OnCreateTableCommandExecute(object p)
        {
            var number = ValidationHelper.ValidateNumberString(NumStr);
            var numberLength = (int)Math.Floor(Math.Log(number, 2)) + 1;
            CoreTypeValues = new ObservableCollection<TypeValue>();
            for (byte i = 0; i <= 9; ++i)
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
            for (byte i = 10; i <= 15; i++)
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
            DimensionalityOptions = new ObservableCollection<int> { 3,4,5,6,7,8 };
            _prnService = new PRNService();
            CreateTableCommand = new LambdaCommand(OnCreateTableCommandExecute, CanCreateTableCommandExecute);
            CreateGeneralTableCommand = new LambdaCommand(OnCreateGeneralTableCommandExecute, CanCreateGeneralTableCommandExecute);
        }
    }
}
