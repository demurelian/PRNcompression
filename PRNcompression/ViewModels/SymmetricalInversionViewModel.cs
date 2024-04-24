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
        public ulong Number { get; set; }
        public string Binary { get; set; }
    }
    class TableValue
    {
        public string TypeString { get; set; }
        public ulong Number { get; set; }
        public string BinaryString { get; set; }
    }
    class ZoneInfo
    {
        public byte ZoneNumber { get; set; }
        public ulong Quantity { get; set; }
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
        private ObservableCollection<TypeValue> _TypeValues;
        public ObservableCollection<TypeValue> TypeValues
        {
            get => _TypeValues;
            set => Set(ref _TypeValues, value);
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

        private ObservableCollection<ZoneInfo> _Zones;
        public ObservableCollection<ZoneInfo> Zones
        {
            get => _Zones;
            set => Set(ref _Zones, value);
        }

        private int _Dimensionality;
        public int Dimensionality
        {
            get => _Dimensionality;
            set => Set(ref _Dimensionality, value);
        }
        public ICommand CreateTableCommand { get; }
        private bool CanCreateTableCommandExecute(object p) => true;
        private void OnCreateTableCommandExecute(object p)
        {
            var numberLength = int.Parse(NumStr);
            TypeValues = new ObservableCollection<TypeValue>();
            for (byte i = 0; i <= 15; ++i)
            {
                var x = _prnService.PRNGeneration(i, numberLength);

                var item = new TypeValue
                {
                    Type = i,
                    Number = x,
                    Binary = Convert.ToString((long)x, 2).PadLeft(numberLength, '0')
                };
                TypeValues.Add(item);
            }

            GeneralTable = new ObservableCollection<TableValue>();
            var maxNum = (ulong)Math.Pow(2, numberLength);

            for (ulong i = 0; i < maxNum; i++)
            {
                var type = _prnService.GetNumberType(i, numberLength);

                var item = new TableValue
                {
                    TypeString = (type == -1) ? "" : type.ToString(),
                    Number = i,
                    BinaryString = Convert.ToString((long)i, 2).PadLeft(numberLength, '0')
                };

                GeneralTable.Add(item);
            }

            Zones = new ObservableCollection<ZoneInfo>();
            for (byte i = 1; i <= 12; i++)
            {
                var item = new ZoneInfo
                {
                    ZoneNumber = i,
                    Quantity = (i >= 7) ? TypeValues[i + 1 + 1].Number - TypeValues[i + 1].Number - 1 : TypeValues[i + 1].Number - TypeValues[i].Number - 1
                };
                Zones.Add(item);
            }

            //var list = new List<bool>();
            //var CompressionInfo = _prnService.Compression(number, numberLength, ref list);
        }
        public SymmetricalInversionViewModel()
        {
            DimensionalityOptions = new ObservableCollection<int> { 3,4,5,6,7,8 };
            _prnService = new PRNService();
            CreateTableCommand = new LambdaCommand(OnCreateTableCommandExecute, CanCreateTableCommandExecute);
        }
    }
}
