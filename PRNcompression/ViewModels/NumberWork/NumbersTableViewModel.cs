using PRNcompression.Infrastructure.Commands;
using PRNcompression.Model;
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
        public string ZoneName { get; set; }
        public ulong Quantity { get; set; }
    }

    internal class NumbersTableViewModel : ViewModel
    {
        private PRNDataWorker _prnDataWorker;
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
        private Dictionary<int, string> _zoneNames;
        public ICommand CreateTableCommand { get; }
        private bool CanCreateTableCommandExecute(object p) => true;
        private void OnCreateTableCommandExecute(object p)
        {
            var numberLength = int.Parse(NumStr);
            TypeValues = new ObservableCollection<TypeValue>();
            for (byte i = 0; i <= 15; ++i)
            {
                var x = _prnDataWorker.BitArrayToLong(_prnDataWorker.PRNGeneration(i, numberLength));

                var item = new TypeValue
                {
                    Type = i,
                    Number = x,
                    Binary = Convert.ToString((long)x, 2).PadLeft(numberLength, '0')
                };
                TypeValues.Add(item);
            }

            Zones = new ObservableCollection<ZoneInfo>();
            for (byte i = 0; i <= 7; i++)
            {
                int firstPrn = i * 2;
                int secondPrn = i * 2 + 1;
                var item = new ZoneInfo
                {
                    ZoneName = i.ToString(),
                    Quantity = TypeValues[secondPrn].Number - TypeValues[firstPrn].Number + 1
                };
                Zones.Add(item);
            }

            GeneralTable = new ObservableCollection<TableValue>();
            var maxNum = (ulong)Math.Pow(2, numberLength);

            for (ulong i = 0; i < maxNum; i++)
            {
                var type = _prnDataWorker.GetNumberType(i, numberLength);

                if (type != -1)
                {
                    var item = new TableValue
                    {
                        TypeString = "ПРЧ " + type.ToString(),
                        Number = i,
                        BinaryString = Convert.ToString((long)i, 2).PadLeft(numberLength, '0')
                    };
                    GeneralTable.Add(item);
                }
                else
                {
                    ulong num = 5;
                    string str = "";
                    if (i >= TypeValues[0].Number && i <= TypeValues[1].Number)
                    {
                        num = i - TypeValues[0].Number;
                        str = _zoneNames[0] + " " + num.ToString();
                    }
                    if (i >= TypeValues[2].Number && i <= TypeValues[3].Number)
                    {
                        num = i - TypeValues[2].Number;
                        str = _zoneNames[1] + " " + num.ToString();
                    }
                    if (i >= TypeValues[4].Number && i <= TypeValues[5].Number)
                    {
                        num = i - TypeValues[4].Number;
                        str = _zoneNames[2] + " " + num.ToString();
                    }
                    if (i >= TypeValues[6].Number && i <= TypeValues[7].Number)
                    {
                        num = i - TypeValues[6].Number;
                        str = _zoneNames[3] + " " + num.ToString();
                    }
                    if (i >= TypeValues[8].Number && i <= TypeValues[9].Number)
                    {
                        num = (ulong)Math.Abs((long)(TypeValues[9].Number - i));
                        str = _zoneNames[4] + " " + num.ToString();
                    }
                    if (i >= TypeValues[10].Number && i <= TypeValues[11].Number)
                    {
                        num = (ulong)Math.Abs((long)(TypeValues[11].Number - i));
                        str = _zoneNames[5] + " " + num.ToString();
                    }
                    if (i >= TypeValues[12].Number && i <= TypeValues[13].Number)
                    {
                        num = (ulong)Math.Abs((long)(TypeValues[13].Number - i));
                        str = _zoneNames[6] + " " + num.ToString();
                    }
                    if (i >= TypeValues[14].Number && i <= TypeValues[15].Number)
                    {
                        num = (ulong)Math.Abs((long)(TypeValues[15].Number - i));
                        str = _zoneNames[7] + " " + num.ToString();
                    }
                    var item = new TableValue
                    {
                        TypeString = str,
                        Number = i,
                        BinaryString = Convert.ToString((long)i, 2).PadLeft(numberLength, '0')
                    };
                    GeneralTable.Add(item);
                }
            }

        }

        public NumbersTableViewModel()
        {
            _zoneNames = new Dictionary<int, string>
            {
                { 0, "A"}, 
                { 1, "B"},
                { 2, "C"},
                { 3, "D"},
                { 4, "DD"},
                { 5, "CC"},
                { 6, "BB"},
                { 7, "AA" }
            };
            DimensionalityOptions = new ObservableCollection<int> { 3,4,5,6,7,8 };
            _prnDataWorker = new PRNDataWorker();
            CreateTableCommand = new LambdaCommand(OnCreateTableCommandExecute, CanCreateTableCommandExecute);
        }
    }
}
