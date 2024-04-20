using PRNcompression.Services.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Packaging;
using System.IO.Ports;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace PRNcompression.Services
{
    
    internal class PRNService : IPRNService
    {
        public CompressedInfo Compression(long number, int numberLength, ref List<bool> inversionList)
        {
            var result = new CompressedInfo();
            //10 8 5 2 3 4 1 6 9 7
            Dictionary<byte, long> prns = new Dictionary<byte, long>();
            for (byte i = 0; i <= 15; i++)
            {
                prns.Add(i, PRNGeneration(i, numberLength));
            }

            foreach (var prn in prns)
            {
                if (prn.Value == number)
                {
                    result.Type = prn.Key;
                    result.Length = numberLength;
                    result.InversionInfo = inversionList;
                    return result;
                }
            }
            long newNumber;
            int newLength = numberLength - 1;
            if (number > prns[4])
            {
                inversionList.Add(true);
                long mask = prns[9];
                newNumber = number ^ mask;
            } else
            {
                inversionList.Add(false);
                newNumber = number;
            }
            return Compression(newNumber, newLength, ref inversionList);
        }

        /// <summary>Проверка числа на псевдорегулярную структуру и возврат типа числа</summary>
        /// <returns>тип ПРЧ 1-15, или 0 если число обычное</returns>
        public int GetNumberType(int num, int dimensionality)
        {
            for (byte i = 0; i <= 15; i++)
            {
                var x = PRNGeneration(i, dimensionality);
                if (num == x)
                    return i;
            }
            return -1;
        }

        public int GetNumberLength(int num) => (num == 1 || num == 0) ? 1 : (int)Math.Floor(Math.Log(num, 2)) + 1;

        static long BitArrayToLong(BitArray bits)
        {
            if (bits.Count > 64)
                throw new ArgumentException("BitArray должен содержать не более 64 бит");

            byte[] bytes = new byte[8];
            bits.CopyTo(bytes, 0);
            // Если количество битов меньше 64, дополняем массив нулями
            if (bits.Count < 64)
            {
                // Определяем количество нулевых битов для дополнения
                int zeroBits = 64 - bits.Count;

                // Устанавливаем нулевые биты
                for (int i = bits.Count; i < 64; i++)
                {
                    bytes[i / 8] &= (byte)~(1 << (i % 8));
                }
            }
            return BitConverter.ToInt64(bytes, 0);
        }

        /// <summary> Генерирует псевдорегулярное число </summary>
        /// <returns> ПРЧ или -1 в случае ошибки </returns>
        public long PRNGeneration(byte type, int size)
        {
            var bits = new BitArray(size);
            //long[] arr = new long[1];
            //arr[0] = -1;
            switch (type)
            {
                case 0:
                    return 0;
                case 1:
                    return 1;
                case 2:
                    if (size % 2 == 0)
                    {
                        bits.Set(0, false);
                        bits.Set(1, true);
                        var flag10 = false;
                        for (int i = size - 3; i >= 2; i--)
                        {
                            bits.Set(i, flag10);
                            flag10 = !flag10;
                        }
                        bits.Set(size - 2, false);
                        bits.Set(size - 1, false);
                    }
                    else
                    {
                        bits.Set(0, true);
                        bits.Set(1, true);
                        var flag10 = false;
                        for (int i = size - 3; i >= 2; i--)
                        {
                            bits.Set(i, flag10);
                            flag10 = !flag10;
                        }
                        bits.Set(size - 2, false);
                        bits.Set(size - 1, false);
                    }
                    break;
                case 3:
                    if (size % 2 == 0)
                    {
                        bits.Set(0, true);
                        bits.Set(1, true);
                        var flag2 = true;
                        for (int i = size - 3; i >= 2; i--)
                        {
                            bits.Set(i, flag2);
                            flag2 = !flag2;
                        }
                        bits.Set(size - 2, false);
                        bits.Set(size - 1, false);
                    }
                    else
                    {
                        bits.Set(0, false);
                        bits.Set(1, true);
                        var flag2 = true;
                        for (int i = size - 3; i >= 2; i--)
                        {
                            bits.Set(i, flag2);
                            flag2 = !flag2;
                        }
                        bits.Set(size - 2, false);
                        bits.Set(size - 1, false);
                    }
                    break;
                case 4:
                    bits.Set(size - 2, true);
                    break;
                case 5:
                    var flag3 = false;
                    for (int i = size - 1; i >= 0; i--)
                    {
                        bits.Set(i, flag3);
                        flag3 = !flag3;
                    }
                    break;
                case 6:
                    bits.Set(size - 2, true);
                    var flag12 = true;
                    for (int i = size - 3; i >= 0; i--)
                    {
                        bits.Set(i, flag12);
                        flag12 = !flag12;
                    }
                    break;
                case 7:
                    for (int i = 0; i < size - 1; i++)
                        bits.Set(i, true);
                    bits.Set(size - 1, false);
                    break;
                case 8:
                    for (int i = 0; i < size - 1; i++)
                        bits.Set(i, false);
                    bits.Set(size - 1, true);
                    break;
                case 9:
                    bits.Set(size - 1, true);
                    var flag13 = true;
                    for (int i = size - 4; i >= 0; i--)
                    {
                        bits.Set(i, flag13);
                        flag13 = !flag13;
                    }
                    break;
                //supplementary types
                case 10:
                    var flag1 = true;
                    for (int i = size - 1; i >= 0; i--)
                    {
                        bits.Set(i, flag1);
                        flag1 = !flag1;
                    }
                    break;
                case 11:
                    for (int i = 0; i < size; i++)
                        bits.Set(i, true);
                    bits.Set(size - 2, false);
                    break;
                case 12:
                    if (size % 2 == 0)
                    {
                        bits.Set(0, false);
                        bits.Set(1, false);
                        var flag7 = false;
                        for (int i = size - 3; i >= 2; i--)
                        {
                            bits.Set(i, flag7);
                            flag7 = !flag7;
                        }
                        bits.Set(size - 2, true);
                        bits.Set(size - 1, true);
                    }
                    else
                    {
                        bits.Set(0, true);
                        bits.Set(1, false);
                        var flag7 = false;
                        for (int i = size - 3; i >= 2; i--)
                        {
                            bits.Set(i, flag7);
                            flag7 = !flag7;
                        }
                        bits.Set(size - 2, true);
                        bits.Set(size - 1, true);
                    }
                    break;
                case 13:
                    if (size % 2 == 0)
                    {
                        bits.Set(0, true);
                        bits.Set(1, false);
                        var flag15 = true;
                        for (int i = size - 3; i >= 2; i--)
                        {
                            bits.Set(i, flag15);
                            flag15 = !flag15;
                        }
                        bits.Set(size - 2, true);
                        bits.Set(size - 1, true);
                    }
                    else
                    {
                        bits.Set(0, false);
                        bits.Set(1, false);
                        var flag15 = true;
                        for (int i = size - 3; i >= 2; i--)
                        {
                            bits.Set(i, flag15);
                            flag15 = !flag15;
                        }
                        bits.Set(size - 2, true);
                        bits.Set(size - 1, true);
                    }
                    break;
                case 14:
                    bits.Set(0, false);
                    for (int i = 1; i < size; i++)
                        bits.Set(i, true);
                    break;
                case 15:
                    for (int i = 0; i < size; i++)
                        bits.Set(i, true);
                    break;
            }
            return BitArrayToLong(bits);
        }

        /// <summary>Индекс в массиве = число</summary>
        /// <returns>Тип ПРЧ по данному числу</returns>
        public byte[] FieldCharacterization(byte dimensionality)
        {
            var size = (int)Math.Pow(2, dimensionality);
            var types = new byte[size];

            IndependentGeneration(dimensionality, ref types);
            EvenGeneration(dimensionality, ref types);
            OddGeneration(dimensionality, ref types);
            SupplementaryGeneration(dimensionality, ref types);

            return types;
        }

        public void IndependentGeneration(byte dimensionality, ref byte[] types)
        {
            //15 type
            types[0] = 15;

            //8 type
            types[1] = 8;

            //4 type: степени двойки
            var size = Math.Pow(2, dimensionality);
            for (int i = 1; i < size; i *= 2)
            {
                types[i] = 4;
            }

            //9 type
            for(int i = 1; i < dimensionality; i++)
            {
                var bits = new BitArray(dimensionality);
                for (int j = i; j > 0; j--)
                {
                    bits.Set(j, true);
                }
                var x = new int[1];
                bits.CopyTo(x, 0);
                types[x[0]] = 9;
            }
        }
        public void EvenGeneration(byte dimensionality, ref byte[] types)
        {
            //1 type
            for(int i = 1; i < dimensionality; i += 2)
            {
                var bits = new BitArray(dimensionality);
                for (int j = i; j > 0; j -= 2)
                {
                    bits.Set(j, true);
                }
                var x = new int[1];
                bits.CopyTo(x, 0);
                types[x[0]] = 1;
            }

            //5 type
            for(int i = 1; i < dimensionality; i += 2)
            {
                var bits = new BitArray(dimensionality);
                for (int j = i/2; j >=0; j--)
                {
                    bits.Set(j, true);
                }
                var x = new int[1];
                bits.CopyTo(x, 0);
                types[x[0]] = 5;
            }

            //6 type
            for(int i = 1;i < dimensionality; i += 2)
            {
                var bits = new BitArray(dimensionality);
                for(int j = i; j > i/2; j--)
                {
                    bits.Set(j, true);
                }
                var x = new int[1];
                bits.CopyTo(x, 0);
                types[x[0]] = 6;
            }
        }
        
        public void OddGeneration(byte dimensionality, ref byte[] types)
        {
            //2 type
            for (int i = 0; i < dimensionality; i += 2)
            {
                var bits = new BitArray(dimensionality);
                for (int j = i; j >= 0; j -= 2)
                {
                    bits.Set(j, true);
                }
                var x = new int[1];
                bits.CopyTo(x, 0);
                types[x[0]] = 2;
            }

            //3 type
            for (int i = 5; i < dimensionality; i++)
            {
                var bits = new BitArray(dimensionality);
                bits.Set(i, false);
                for (int j = i-1; j >= 0; j--)
                {
                    bits.Set(j, true);
                }
                var x = new int[1];
                bits.CopyTo(x, 0);
                types[x[0]] = 3;
            }

            //7 type
            for (int i = dimensionality - 1; i < dimensionality; i++)
            {
                var bits = new BitArray(dimensionality);
                for (int j = i; j >= 0; j--)
                {
                    bits.Set(j, true);
                }
                var x = new int[1];
                bits.CopyTo(x, 0);
                types[x[0]] = 7;
            }
        }

        public void SupplementaryGeneration(byte dimensionality, ref byte[] types)
        {
            //10 type
            for (int i = 3; i < dimensionality; i += 2)
            {
                var bits = new BitArray(dimensionality);
                bits.Set(i, true);
                bits.Set(0, true);
                for (int j = i-1; j > 0; j--)
                {
                    bits.Set(j, false);
                }
                var x = new int[1];
                bits.CopyTo(x, 0);
                types[x[0]] = 10;
            }

            //13 type
            var list13 = new List<int>();
            for (int i = 4; i < dimensionality; i += 2)
            {
                var bits = new BitArray(dimensionality);
                bits.Set(i, true);
                bits.Set(0, true);
                for (int j = i - 1; j > 0; j--)
                {
                    bits.Set(j, false);
                }
                var x = new int[1];
                bits.CopyTo(x, 0);
                types[x[0]] = 13;
            }

            //11 type
            for(int i = 3; i < dimensionality; i++)
            {
                var bits = new BitArray(dimensionality);
                bits.Set(i, true);
                bits.Set(i-1, true);
                bits.Set(0, true);
                for(int j = i - 2; j > 0; j--)
                {
                    bits.Set(j, false);
                }
                var x = new int[1];
                bits.CopyTo(x, 0);
                types[x[0]] = 11;
            }

            //12 type
            for (int i = 3; i < dimensionality; i++)
            {
                var bits = new BitArray(dimensionality);
                bits.Set(i, true);
                bits.Set(i - 1, false);
                for (int j = i - 2; j >= 0; j--)
                {
                    bits.Set(j, true);
                }
                var x = new int[1];
                bits.CopyTo(x, 0);

                types[x[0]] = 12;
            }

            //14 type
            for (int i = 4; i < dimensionality; i++)
            {
                var bits = new BitArray(dimensionality);
                bits.Set(0, true);
                bits.Set(1, false);
                for (int j = i; j > 1; j--)
                {
                    bits.Set(j, true);
                }
                var x = new int[1];
                bits.CopyTo(x, 0);

                types[x[0]] = 14;
            }
        }
    }
}
