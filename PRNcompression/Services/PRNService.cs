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
        public CompressedInfo Compression(int number, int numberLength, ref List<bool> inversionList)
        {
            var result = new CompressedInfo();
            //10 8 5 2 3 4 1 6 9 7
            Dictionary<byte, int> prns = new Dictionary<byte, int>();
            if (numberLength % 2 == 0) // четное
            {
                prns.Add(10, PRNGeneration(10, numberLength));
                prns.Add(8, PRNGeneration(8, numberLength));
                prns.Add(5, PRNGeneration(5, numberLength));
                var x = prns[5];
                prns.Add(2, PRNGeneration(2, numberLength));
                prns.Add(3, PRNGeneration(3, numberLength));
                prns.Add(4, PRNGeneration(4, numberLength));
                prns.Add(1, PRNGeneration(1, numberLength));
                prns.Add(6, PRNGeneration(6, numberLength));
                prns.Add(9, PRNGeneration(9, numberLength));
                prns.Add(7, PRNGeneration(7, numberLength));
            }
            else // нечетное
            {
                prns.Add(10, PRNGeneration(10, numberLength));
                prns.Add(8, PRNGeneration(8, numberLength));
                prns.Add(2, PRNGeneration(2, numberLength));
                prns.Add(3, PRNGeneration(3, numberLength));
                prns.Add(4, PRNGeneration(4, numberLength));
                prns.Add(1, PRNGeneration(1, numberLength));
                prns.Add(9, PRNGeneration(9, numberLength));
                prns.Add(7, PRNGeneration(7, numberLength));
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
            int newNumber;
            int newLength = numberLength - 1;
            if (number > prns[4])
            {
                inversionList.Add(true);
                int mask = prns[7];
                newNumber = number ^ mask;
            } else
            {
                inversionList.Add(false);
                newNumber = number;
            }
            return Compression(newNumber, newLength, ref inversionList);
        }
        int countSetBits(int n)
        {
            int count = 0;
            while (n > 0)
            {
                count++;
                n = n >> 1; // Сдвиг вправо на один бит
            }
            return count;
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

        /// <summary> Генерирует псевдорегулярное число </summary>
        /// <returns> ПРЧ или -1 в случае ошибки </returns>
        public int PRNGeneration(byte type, int size)
        {
            var bits = new BitArray(size);
            int[] arr = new int[1];
            arr[0] = -1;
            switch (type)
            {
                case 0:
                    arr[0] = 0;
                    break;
                case 1:
                    arr[0] = 1;
                    break;
                case 2:
                    for (int i = 0; i < size / 2; i++)
                        bits.Set(i, true);
                    for (int i = size / 2; i < size; i++)
                        bits.Set(i, false);
                    bits.CopyTo(arr, 0);
                    break;
                case 3:
                    var flag2 = false;
                    for (int i = size - 1; i >= 0; i--)
                    {
                        bits.Set(i, flag2);
                        flag2 = !flag2;
                    }
                    bits.CopyTo(arr, 0);
                    break;
                case 4:
                    for (int i = 0; i < size - 1; i++)
                        bits.Set(i, true);
                    bits.Set(size - 1, false);
                    bits.CopyTo(arr, 0);
                    break;
                case 5:
                    for (int i = 0; i < size - 1; i++)
                        bits.Set(i, false);
                    bits.Set(size - 1, true);
                    bits.CopyTo(arr, 0);
                    break;
                case 6:
                    var flag1 = true;
                    for (int i = size - 1; i >= 0; i--)
                    {
                        bits.Set(i, flag1);
                        flag1 = !flag1;
                    }
                    bits.CopyTo(arr, 0);
                    break;
                case 7:
                    for (int i = 0; i < size / 2; i++)
                        bits.Set(i, false);
                    for (int i = size / 2; i < size; i++)
                        bits.Set(i, true);
                    bits.CopyTo(arr, 0);
                    break;
                case 8:
                    bits.Set(0, false);
                    for (int i = 1; i < size; i++)
                        bits.Set(i, true);
                    bits.CopyTo(arr, 0);
                    break;
                case 9:
                    for (int i = 0; i < size; i++)
                        bits.Set(i, true);
                    bits.CopyTo(arr, 0);
                    break;
                //supplementary types
                case 10:
                    bits.Set(size/2 - 1, true);
                    bits.CopyTo(arr, 0);
                    break;
                case 11:
                    bits.Set(size - 2, true);
                    bits.CopyTo(arr, 0);
                    break;
                case 12:
                    bits.Set(size - 2, true);
                    var flag12 = true;
                    for (int i = size - 3; i >= 0; i--)
                    {
                        bits.Set(i, flag12);
                        flag12 = !flag12;
                    }
                    bits.CopyTo(arr, 0);
                    break;
                case 13:
                    bits.Set(size - 1, true);
                    var flag13 = true;
                    for (int i = size - 4; i >= 0; i--)
                    {
                        bits.Set(i, flag13);
                        flag13 = !flag13;
                    }
                    bits.CopyTo(arr, 0);
                    break;
                case 14:
                    for (int i = 0; i < size; i++)
                        bits.Set(i, true);
                    bits.Set(size - 2, false);
                    bits.CopyTo(arr, 0);
                    break;
                case 15:
                    for (int i = 0; i < size; i++)
                        bits.Set(i, true);
                    bits.Set(size/2 - 1, false);
                    bits.CopyTo(arr, 0);
                    break;
                    //case 13:
                    //    bits.Set(size - 1, true);
                    //    bits.Set(size - 2, false);
                    //    for (int i = 0; i < size - 2; i++)
                    //        bits.Set(i, true);
                    //    bits.CopyTo(arr, 0);
                    //    break;
                    //case 14:
                    //    bits.Set(0, true);
                    //    bits.Set(size - 2, true);
                    //    for (int i = 1; i < size - 2; i++)
                    //        bits.Set(i, false);
                    //    bits.CopyTo(arr, 0);
                    //    break;
                    //case 15:
                    //    bits.Set(0, true);
                    //    bits.Set(1, false);
                    //    for (int i = 2; i < size; i++)
                    //        bits.Set(i, true);
                    //    bits.CopyTo(arr, 0);
                    //    break;
            }
            return arr[0];
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
