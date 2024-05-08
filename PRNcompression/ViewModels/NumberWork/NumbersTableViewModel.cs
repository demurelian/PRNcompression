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
            for (byte i = 1; i <= 12; i++)
            {
                var item = new ZoneInfo
                {
                    ZoneName = _zoneNames[i],
                    Quantity = (i >= 7) ? TypeValues[i + 1 + 1].Number - TypeValues[i + 1].Number - 1 : TypeValues[i + 1].Number - TypeValues[i].Number - 1
                };
                Zones.Add(item);
            }

            GeneralTable = new ObservableCollection<TableValue>();
            var maxNum = (ulong)Math.Pow(2, numberLength);

            //for (ulong i = 0; i < maxNum; i++)
            //{
            //    var type = _prnDataWorker.GetNumberType(i, numberLength);
                
            //    if (type != -1)
            //    {
            //        var item = new TableValue
            //        {
            //            TypeString = "ПРЧ " + type.ToString(),
            //            Number = i,
            //            BinaryString = Convert.ToString((long)i, 2).PadLeft(numberLength, '0')
            //        };
            //        GeneralTable.Add(item);
            //    } else
            //    {
            //        ulong num = 5;
            //        string str = "";
            //        if (i > TypeValues[8].Number)
            //        {
            //            if (i < TypeValues[9].Number)
            //            {
            //                num = (ulong)Math.Abs((long)(i - TypeValues[8].Number - Zones[6].Quantity)) + 1;
            //                str = Zones[6].ZoneName + " " + num.ToString();
            //            } else
            //            {
            //                if (i < TypeValues[10].Number)
            //                {
            //                    num = (ulong)Math.Abs((long)(i - TypeValues[9].Number - Zones[7].Quantity)) + 1;
            //                    str = Zones[7].ZoneName + " " + num.ToString();
            //                }
            //                else
            //                {
            //                    if (i < TypeValues[11].Number)
            //                    {
            //                        num = (ulong)Math.Abs((long)(i - TypeValues[10].Number - Zones[8].Quantity)) + 1;
            //                        str = Zones[8].ZoneName + " " + num.ToString();
            //                    }
            //                    else
            //                    {
            //                        if (i < TypeValues[12].Number)
            //                        {
            //                            num = (ulong)Math.Abs((long)(i - TypeValues[11].Number - Zones[9].Quantity)) + 1;
            //                            str = Zones[9].ZoneName + " " + num.ToString();
            //                        }
            //                        else
            //                        {

            //                            if (i < TypeValues[13].Number)
            //                            {
            //                                num = (ulong)Math.Abs((long)(i - TypeValues[12].Number - Zones[10].Quantity)) + 1;
            //                                str = Zones[10].ZoneName + " " + num.ToString();
            //                            }
            //                            else
            //                            {
            //                                num = (ulong)Math.Abs((long)(i - TypeValues[13].Number - Zones[11].Quantity)) + 1;
            //                                str = Zones[11].ZoneName + " " + num.ToString();
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        else
            //        {
            //            if (i > TypeValues[6].Number)
            //            {
            //                num = i - TypeValues[6].Number;
            //                str = Zones[5].ZoneName + " " + num.ToString();
            //            } else
            //            {
            //                if (i > TypeValues[5].Number)
            //                {
            //                    num = i - TypeValues[5].Number;
            //                    str = Zones[4].ZoneName + " " + num.ToString();
            //                }
            //                else
            //                {
            //                    if (i > TypeValues[4].Number)
            //                    {
            //                        num = i - TypeValues[4].Number;
            //                        str = Zones[3].ZoneName + " " + num.ToString();
            //                    }
            //                    else
            //                    {
            //                        if (i > TypeValues[3].Number)
            //                        {
            //                            num = i - TypeValues[3].Number;
            //                            str = Zones[2].ZoneName + " " + num.ToString();
            //                        }
            //                        else
            //                        {
            //                            if (i > TypeValues[2].Number)
            //                            {
            //                                num = i - TypeValues[2].Number;
            //                                str = Zones[1].ZoneName + " " + num.ToString();
            //                            }
            //                            else
            //                            {
            //                                num = i - TypeValues[1].Number;
            //                                str = Zones[0].ZoneName + " " + num.ToString();
            //                            }
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        var item = new TableValue
            //        {
            //            TypeString = str,
            //            Number = i,
            //            BinaryString = Convert.ToString((long)i, 2).PadLeft(numberLength, '0')
            //        };
            //        GeneralTable.Add(item);
            //    }
            //}

        }

        public NumbersTableViewModel()
        {
            _zoneNames = new Dictionary<int, string>
            {
                { 1, "A"}, 
                { 2, "B"},
                { 3, "C"},
                { 4, "D"},
                { 5, "E"},
                { 6, "F"},
                { 7, "FF"},
                { 8, "EE"},
                { 9, "DD"},
                {10, "CC"},
                {11, "BB"},
                {12, "AA" }
            };
            DimensionalityOptions = new ObservableCollection<int> { 3,4,5,6,7,8 };
            _prnDataWorker = new PRNDataWorker();
            CreateTableCommand = new LambdaCommand(OnCreateTableCommandExecute, CanCreateTableCommandExecute);
        }
    }
}
