using PRNcompression.Services.Interfaces;
using System;
using System.Collections;

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
    }
}
