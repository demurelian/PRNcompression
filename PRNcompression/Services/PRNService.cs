using PRNcompression.Services.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Packaging;
using System.IO.Ports;
using System.Windows.Documents;
using System.Windows.Media;

namespace PRNcompression.Services
{
    internal class PRNService : IPRNService
    {
        /// <summary>Проверка числа на псевдорегулярную структуру и возврат типа числа</summary>
        /// <returns>тип ПРЧ 1-15, или 0 если число обычное</returns>
        public int GetNumberType(int num)
        {
            var bits = (num == 1) ? 1 : (int)Math.Floor(Math.Log(num, 2))+1;
            for(int i = 0; i < 15; i++)
            {
                var x = PRNGeneration((byte)i, bits);
                if (num == x)
                    return i;
            }
            return 0;
        }

        /// <summary> Генерирует псевдорегулярное число </summary>
        /// <returns> ПРЧ или -1 в случае ошибки </returns>
        public int PRNGeneration(byte type, int size)
        {
            var bits = new BitArray(size);
            int[] arr = new int[1];
            arr[0] = -1;
            switch (type)
            {
                case 1:
                    for (int i = 0; i < size; i++)
                        bits.Set(i, Convert.ToBoolean(i % 2));
                    bits.CopyTo(arr, 0);
                    break;
                case 2:
                    for (int i = 0; i < size; i++)
                        bits.Set(i, Convert.ToBoolean((i+1) % 2));
                    bits.CopyTo(arr, 0);
                    break;
                case 3:
                    for (int i = 0; i < size-1; i++)
                        bits.Set(i, true);
                    bits.Set(size - 1, false);
                    bits.CopyTo(arr, 0);
                    break;
                case 4:
                    for (int i = 0; i < size - 1; i++)
                        bits.Set(i, false);
                    bits.Set(size - 1, true);
                    bits.CopyTo(arr, 0);
                    break;
                case 5:
                    if (size % 2 == 0)
                    {
                        for (int i = 0; i < size/2; i++)
                            bits.Set(i, true);
                        for (int i = size/2; i < size; i++)
                            bits.Set(i, false);
                        bits.CopyTo(arr, 0);
                    }
                    break;
                case 6:
                    if (size % 2 == 0)
                    {
                        for (int i = 0; i < size / 2; i++)
                            bits.Set(i, false);
                        for (int i = size / 2; i < size; i++)
                            bits.Set(i, true);
                        bits.CopyTo(arr, 0);
                    }
                    break;
                case 7:
                    for (int i = 0; i < size; i++)
                        bits.Set(i, true);
                    bits.CopyTo(arr, 0);
                    break;
                case 8:
                    arr[0] = 1;
                    break;
                case 9:
                    bits.Set(0, false);
                    for (int i = 1; i < size; i++)
                        bits.Set(i, true);
                    bits.CopyTo(arr, 0);
                    break;
                case 10:
                    bits.Set(0, true);
                    for (int i = 1; i < size - 1; i++)
                        bits.Set(i, false);
                    bits.Set(size - 1, true);
                    bits.CopyTo(arr, 0);
                    break;
                case 11:
                    bits.Set(0, true);
                    for (int i = 1; i < size - 2; i++)
                        bits.Set(i, false);
                    if(size > 1)
                        bits.Set(size - 2, true);
                    bits.Set(size - 1, true);
                    bits.CopyTo(arr, 0);
                    break;
                case 12:
                    for (int i = 0; i < size; i++)
                        bits.Set(i, true);
                    if (size > 1)
                        bits.Set(size - 2, false);
                    bits.CopyTo(arr, 0);
                    break;
                case 13:
                    bits.Set(0, true);
                    for (int i = 1; i < size - 2; i++)
                        bits.Set(i, false);
                    if (size > 1)
                        bits.Set(size - 2, true);
                    bits.Set(size - 1, false);
                    bits.CopyTo(arr, 0);
                    break;
                case 14:
                    for (int i = 0; i < size; i++)
                        bits.Set(i, true);
                    if (size > 1)
                        bits.Set(1, false);
                    bits.CopyTo(arr, 0);
                    break;
                case 15:
                    arr[0] = 0;
                    break;
            }
            return arr[0];
        }

        /// <summary>Индекс в массиве = число</summary>
        /// <returns>Тип ПРЧ по данному числу</returns>
        public byte[] PRNFieldGeneration(byte dimensionality)
        {
            var types = new byte[(int)Math.Pow(2, dimensionality)];
            var typesDictionary = new Dictionary<byte, IList<int>>();
            IndependentGeneration(dimensionality, ref types, ref typesDictionary);
            EvenGeneration(dimensionality, ref types, ref typesDictionary);
            OddGeneration(dimensionality, ref types, ref typesDictionary);
            return types;
        }
        public void IndependentGeneration(byte dimensionality, ref byte[] types, ref Dictionary<byte, IList<int>> typesDictionary)
        {
            //10 type
            types[0] = 10;
            typesDictionary.Add(10, new List<int> { 0 });
            //8 type
            types[1] = 8;
            typesDictionary.Add(8, new List<int> { 1 });
            //4 type: степени двойки
            var size = Math.Pow(2, dimensionality);
            var list4 = new List<int>();
            for (int i = 1; i < size; i *= 2)
            {
                if (types[i] == 0)
                    types[i] = 4;
                list4.Add(i);
            }
            typesDictionary.Add(4, list4);
            //9 type
            var list9 = new List<int>();
            for(int i = 1; i < dimensionality; i++)
            {
                var bits = new BitArray(dimensionality);
                for (int j = i; j > 0; j--)
                {
                    bits.Set(j, true);
                }
                var x = new int[1];
                bits.CopyTo(x, 0);
                if (types[x[0]] == 0)
                    types[x[0]] = 9;
                list9.Add(x[0]);
            }
            typesDictionary.Add(9, list9);
        }
        public void EvenGeneration(byte dimensionality, ref byte[] types, ref Dictionary<byte, IList<int>> typesDictionary)
        {
            //1 type
            var list1 = new List<int>();
            for(int i = 1; i < dimensionality; i += 2)
            {
                var bits = new BitArray(dimensionality);
                for (int j = i; j > 0; j -= 2)
                {
                    bits.Set(j, true);
                }
                var x = new int[1];
                bits.CopyTo(x, 0);
                if (types[x[0]] == 0)
                    types[x[0]] = 1;
                list1.Add(x[0]);
            }
            typesDictionary.Add(1, list1);
            //5 type
            var list5 = new List<int>();
            for(int i = 1; i < dimensionality; i += 2)
            {
                var bits = new BitArray(dimensionality);
                for (int j = i/2; j >=0; j--)
                {
                    bits.Set(j, true);
                }
                var x = new int[1];
                bits.CopyTo(x, 0);
                if (types[x[0]] == 0)
                    types[x[0]] = 5;
                list5.Add(x[0]);
            }
            typesDictionary.Add(5, list5);
            //6 type
            var list6 = new List<int>();
            for(int i = 1;i < dimensionality; i += 2)
            {
                var bits = new BitArray(dimensionality);
                for(int j = i; j > i/2; j--)
                {
                    bits.Set(j, true);
                }
                var x = new int[1];
                bits.CopyTo(x, 0);
                if (types[x[0]] == 0)
                    types[x[0]] = 6;
                list6.Add(x[0]);
            }
            typesDictionary.Add(6, list6);
        }
        
        public void OddGeneration(byte dimensionality, ref byte[] types, ref Dictionary<byte, IList<int>> typesDictionary)
        {
            //2 type
            var list2 = new List<int>();
            for (int i = 0; i < dimensionality; i += 2)
            {
                var bits = new BitArray(dimensionality);
                for (int j = i; j >= 0; j -= 2)
                {
                    bits.Set(j, true);
                }
                var x = new int[1];
                bits.CopyTo(x, 0);
                if (types[x[0]] == 0)
                    types[x[0]] = 2;
                list2.Add(x[0]);
            }
            typesDictionary.Add(2, list2);
            //3 type
            var list3 = new List<int>();
            for (int i = 1; i < dimensionality; i++)
            {
                var bits = new BitArray(dimensionality);
                bits.Set(i, false);
                for (int j = i-1; j >= 0; j--)
                {
                    bits.Set(j, true);
                }
                var x = new int[1];
                bits.CopyTo(x, 0);
                if (types[x[0]] == 0)
                    types[x[0]] = 3;
                list3.Add(x[0]);
            }
            typesDictionary.Add(3, list3);
            //7 type
            var list7 = new List<int>();
            for (int i = 0; i < dimensionality; i++)
            {
                var bits = new BitArray(dimensionality);
                for (int j = i; j >= 0; j--)
                {
                    bits.Set(j, true);
                }
                var x = new int[1];
                bits.CopyTo(x, 0);
                if (types[x[0]] == 0)
                    types[x[0]] = 7;
                list7.Add(x[0]);
            }
            typesDictionary.Add(7, list7);
        }
    }
}
