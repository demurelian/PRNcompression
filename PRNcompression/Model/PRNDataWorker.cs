using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRNcompression.Model
{
    public class CompressedInfo
    {
        public byte Type { get; set; }
        public int Length { get; set; }
        public List<bool> InversionInfo { get; set; }
    }
    public class DecompressedInfo
    {
        public byte Type { get; set; }
        public int Length { get; set; }
        public ulong ResultNumber { get; set; }
    }
    
    internal class PRNDataWorker
    {
        public byte[] PRNByteArrGenerator(byte type, int size)
        {
            byte[] data = new byte[size];
            switch(type)
            {
                case 2:
                    data[0] = 86; data[size - 1] = 21;
                    for (int i = 1; i <= size - 2; i++)
                        data[i] = 85;
                    break;
                case 3:
                    data[0] = 171; data[size - 1] = 42;
                    for (int i = 1; i <= size - 2; i++)
                        data[i] = 170;
                    break;
                case 4:
                    data[size - 1] = 64;
                    for (int i = 0; i <= size - 2; i++)
                        data[i] = 0;
                    break;
                case 5:
                    for (int i = 0; i <= size - 1; i++)
                        data[i] = 85;
                    break;
                case 6:
                    data[size - 1] = 106;
                    for (int i = 0; i <= size - 2; i++)
                        data[i] = 170;
                    break;
                case 7:
                    data[size - 1] = 127;
                    for (int i = 0; i <= size - 2; i++)
                        data[i] = 255;
                    break;
                case 8:
                    data[size - 1] = 128;
                    for (int i = 0; i <= size - 2; i++)
                        data[i] = 0;
                    break;
                case 9:
                    data[size - 1] = 149;
                    for (int i = 0; i <= size - 2; i++)
                        data[i] = 85;
                    break;
                case 10:
                    for (int i = 0; i <= size - 1; i++)
                        data[i] = 170;
                    break;
                case 11:
                    data[size - 1] = 191;
                    for (int i = 0; i <= size - 2; i++)
                        data[i] = 255;
                    break;
                case 12:
                    data[0] = 84; data[size - 1] = 213;
                    for (int i = 1; i <= size - 2; i++)
                        data[i] = 85;
                    break;
                case 13:
                    data[0] = 169; data[size - 1] = 234;
                    for (int i = 1; i <= size - 2; i++)
                        data[i] = 170;
                    break;
                case 14:
                    data[0] = 254;
                    for (int i = 1; i <= size - 1; i++)
                        data[i] = 255;
                    break;
                case 15:
                    for (int i = 0; i <= size - 1; i++)
                        data[i] = 255;
                    break;
            }
            return data;
        }
        public CompressedInfo Compression(ulong number, int numberLength, ref List<bool> inversionList, ref List<ulong> numbers)
        {
            var result = new CompressedInfo();
            var prns = new Dictionary<byte, ulong>();
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
            ulong newNumber;
            int newLength = numberLength - 1;
            if (number > prns[7])
            {
                inversionList.Add(true);
                ulong mask = prns[15];
                newNumber = number ^ mask;
            }
            else
            {
                inversionList.Add(false);
                newNumber = number;
            }
            numbers.Add(newNumber);
            return Compression(newNumber, newLength, ref inversionList, ref numbers);
        }

        public DecompressedInfo Decompression(BitArray serviceInfo, BitArray data, ref List<ulong> numbers)
        {
            var item = new DecompressedInfo();
            byte type;
            int length;
            SplitBitArray(serviceInfo, out type, out length);
            item.Type = type;
            item.Length = length;

            var number = PRNGeneration(type, length);

            for (int i = data.Length - 1; i >= 0; i--)
            {
                length++;
                if (data[i])
                {
                    var mask = PRNGeneration(15, length);
                    number = number ^ mask;
                    numbers.Add(number);
                }
            }

            item.ResultNumber = number;
            return item;
        }


        static void SplitBitArray(BitArray bits, out byte firstFourBits, out int remainingBits)
        {
            firstFourBits = 0;
            remainingBits = 0;

            // Получаем первые 4 бита как byte
            for (int i = 0; i < 4; i++)
            {
                if (bits[i])
                    firstFourBits |= (byte)(1 << (3 - i));
            }

            // Получаем остальные биты как int
            for (int i = 4; i < bits.Length; i++)
            {
                if (bits[i])
                    remainingBits |= (1 << (bits.Length - 1 - i));
            }
        }

        /// <summary>Проверка числа на псевдорегулярную структуру и возврат типа числа</summary>
        /// <returns>тип ПРЧ 1-15, или 0 если число обычное</returns>
        public int GetNumberType(ulong num, int dimensionality)
        {
            for (byte i = 0; i <= 15; i++)
            {
                var x = PRNGeneration(i, dimensionality);
                if (num == x)
                    return i;
            }
            return -1;
        }

        public int GetNumberLength(ulong num) => (num == 1 || num == 0) ? 1 : (int)Math.Floor(Math.Log(num, 2)) + 1;

        static ulong BitArrayToLong(BitArray bits)
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
            return BitConverter.ToUInt64(bytes, 0);
        }

        /// <summary> Генерирует псевдорегулярное число </summary>
        /// <returns> ПРЧ или -1 в случае ошибки </returns>
        public ulong PRNGeneration(byte type, int size)
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

            for (byte i = dimensionality; i >= 4; i--)
            {
                for (byte j = 0; j <= 15; j++)
                {
                    var x = PRNGeneration(j, i);
                    if (types[x] == 0)
                        types[x] = (byte)(dimensionality - i + 1);
                }
            }

            return types;
        }
    }
}
