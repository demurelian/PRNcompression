using PRNcompression.ViewModels;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using System.Windows.Media;

namespace PRNcompression.Model
{
    public class FileCompressedInfo
    {
        public byte Type { get; set; }
        public int Length { get; set; }
        public BitArray InversionInfo { get; set; }
    }
    public class NewCompressedInfo
    {
        public byte Type { get; set; }
        public int Length { get; set; }
        public List<bool> Data { get; set; }
    }
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
        public void WriteBitArrayToFile(string filePath, BitArray bits)
        {
            // Создаем массив байтов для хранения битов
            byte[] bytes = new byte[(bits.Length + 7) / 8];

            // Копируем биты из BitArray в массив байтов
            bits.CopyTo(bytes, 0);

            // Создаем FileStream для записи в файл
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            {
                // Записываем массив байтов в файл
                fs.Write(bytes, 0, bytes.Length);
            }
        }

        public byte[] PRNByteArrGenerator(byte type, int size)
        {
            byte[] data = new byte[size];
            switch(type)
            {
                case 1:
                    data[size - 1] = 31;
                    for (int i = 0; i <= size - 2; i++)
                        data[i] = 255;
                    break;
                case 2:
                    data[size - 1] = 32;
                    for (int i = 0; i <= size - 2; i++)
                        data[i] = 0;
                    break;
                case 3:
                    data[size - 1] = 63;
                    for (int i = 0; i <= size - 2; i++)
                        data[i] = 255;
                    break;
                case 4:
                    data[size - 1] = 64;
                    for (int i = 0; i <= size - 2; i++)
                        data[i] = 0;
                    break;
                case 5:
                    data[size - 1] = 95;
                    for (int i = 0; i <= size - 2; i++)
                        data[i] = 255;
                    break;
                case 6:
                    data[size - 1] = 96;
                    for (int i = 0; i <= size - 2; i++)
                        data[i] = 0;
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
                    data[size - 1] = 159;
                    for (int i = 0; i <= size - 2; i++)
                        data[i] = 255;
                    break;
                case 10:
                    data[size - 1] = 160;
                    for (int i = 0; i <= size - 2; i++)
                        data[i] = 0;
                    break;
                case 11:
                    data[size - 1] = 191;
                    for (int i = 0; i <= size - 2; i++)
                        data[i] = 255;
                    break;
                case 12:
                    data[size - 1] = 192;
                    for (int i = 0; i <= size - 2; i++)
                        data[i] = 0;
                    break;
                case 13:
                    data[size - 1] = 223;
                    for (int i = 0; i <= size - 2; i++)
                        data[i] = 255;
                    break;
                case 14:
                    data[size - 1] = 224;
                    for (int i = 0; i <= size - 2; i++)
                        data[i] = 0;
                    break;
                case 15:
                    for (int i = 0; i <= size - 1; i++)
                        data[i] = 255;
                    break;
            }
            return data;
        }
        private bool BitArrCompare(BitArray arr1, BitArray arr2)
        {
            if (arr1.Length != arr2.Length) return false;
            for (int i = 0; i < arr1.Length; i++)
            {
                if (arr1.Get(i) != arr2.Get(i)) return false;
            }
            return true;
        }
        public int ByteArrayToInt(byte[] bytes)
        {
            var list = new List<byte>();
            for (int i = 0; i < 4; i++)
            {
                if (i >= bytes.Length)
                    list.Add(0);
                else
                    list.Add(bytes[i]);
            }
            var arr = list.ToArray();

            return BitConverter.ToInt32(arr, 0);
        }
        public FileCompressedInfo FileCompression(BitArray bits, int length, ref BitArray InversionInfo)
        {
            var result = new FileCompressedInfo();
            Console.WriteLine("Биты:");
            for (int j = 0; j < bits.Length; j++)
            {
                if (bits.Get(j))
                    Console.Write(1);
                else
                    Console.Write(0);
            }
            Console.WriteLine();
            for (byte i = 0; i <= 15; i++)
            {
                var prn = PRNGeneration(i, length);

                if (BitArrCompare(bits, prn))
                {
                    result.Type = i;
                    result.Length = length;
                    result.InversionInfo = InversionInfo;
                    return result;
                }
            }
            int newLength = length - 1;
            if (bits.Get(length - 1) == true)
            {
                InversionInfo.Set(length - 1, true);
                bits.Not();
            }
            else
            {
                InversionInfo.Set(length - 1, false);
            }
            for (int j = 0; j < InversionInfo.Length; j++)
            {
                if (InversionInfo.Get(j))
                    Console.Write(1);
                else
                    Console.Write(0);
            }
            Console.WriteLine();
            var newBits = new BitArray(newLength);
            for (int i = 0; i < newLength; i++)
            {
                newBits.Set(i, bits.Get(i));
            }
            return FileCompression(newBits, newLength, ref InversionInfo);
        }

        public ulong GetIndexInZone(ulong number, int length)
        {
            var prns = new Dictionary<byte, ulong>();
            for (byte i = 0; i <= 15; i++)
            {
                prns.Add(i, BitArrayToLong(PRNGeneration(i, length)));
            }
            int zone = -1;
            for (byte i = 0; i <= 7; i++)
            {
                byte firstPrn = (byte)(i * 2);
                byte secondPrn = (byte)(i * 2 + 1);
                if (number <= prns[secondPrn] && number >= prns[firstPrn])
                {
                    zone = i;
                    return number - prns[firstPrn];
                }
            }
            return ulong.MaxValue;
        }

        public ulong GetZoneSize(int length) => (ulong)Math.Pow(2, length - 3);

        //public NewCompressedInfo NewCompression(ulong number, int numberLength, ref List<bool> data, ref List<ulong> numbers)
        //{
        //    var result = new NewCompressedInfo();
        //    var prns = new Dictionary<byte, ulong>();
        //    for (byte i = 0; i <= 15; i++)
        //    {
        //        prns.Add(i, BitArrayToLong(PRNGeneration(i, numberLength)));
        //    }

        //    foreach (var prn in prns)
        //    {
        //        if (prn.Value == number)
        //        {
        //            result.Type = prn.Key;
        //            result.Length = numberLength;
        //            result.Data = data;
        //            return result;
        //        }
        //    }
        //    ulong newNumber;
        //    int newLength = numberLength - 1;
        //    if (number > prns[7])
        //    {
        //        data.Add(true);
        //        ulong mask = prns[15];
        //        newNumber = number ^ mask;
        //    }
        //    else
        //    {
        //        data.Add(false);
        //        newNumber = number;
        //    }
        //    Console.WriteLine("Before stopping 1");
        //    var zoneIndex = GetIndexInZone(newNumber, numberLength);
        //    Console.WriteLine("Before stopping 2");
        //    var zoneSize = GetZoneSize(numberLength);
        //    bool up = false;
        //    Console.WriteLine("Before stopping 3");
        //    if (zoneIndex % 8 == 2 || zoneIndex % 8 == 3 || zoneIndex % 8 == 6 || zoneIndex % 8 == 7)
        //        up = true;
        //    if (up)
        //        newNumber++;
        //    else
        //        newNumber--;
        //    //data.Add(up);
        //    numbers.Add(newNumber);
        //    Console.WriteLine("Something happening");
        //    return NewCompression(newNumber, newLength, ref data, ref numbers);
        //}
        //public DecompressedInfo NewDecompression(BitArray serviceInfo, BitArray data, ref List<ulong> numbers)
        //{
        //    var item = new DecompressedInfo();
        //    byte type;
        //    int length;
        //    SplitBitArray(serviceInfo, out type, out length);
        //    item.Type = type;
        //    item.Length = length;

        //    var number = BitArrayToLong(PRNGeneration(type, length));
        //    numbers.Add(number);

        //    for (int i = data.Length - 1; i >= 0; i--)
        //    {
        //        length++;
        //        if (data[i])
        //        {
        //            var mask = BitArrayToLong(PRNGeneration(15, length));
        //            number = number ^ mask;
        //            numbers.Add(number);
        //        }
        //    }

        //    item.ResultNumber = number;
        //    return item;
        //}


        public CompressedInfo Compression(ulong number, int numberLength, ref List<bool> inversionList, ref List<ulong> numbers)
        {
            var result = new CompressedInfo();
            var prns = new Dictionary<byte, ulong>();
            for (byte i = 0; i <= 15; i++)
            {
                prns.Add(i, BitArrayToLong(PRNGeneration(i, numberLength)));
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
                newNumber = ++number ^ mask;
            }
            else
            {
                inversionList.Add(false);
                newNumber = --number;
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

            var number = BitArrayToLong(PRNGeneration(type, length));
            numbers.Add(number);

            if (data != null)
                for (int i = data.Length - 1; i >= 0; i--)
                {
                    length++;
                    if (data[i])
                    {
                        var mask = BitArrayToLong(PRNGeneration(15, length));
                        number = number ^ mask;
                        number--;
                        numbers.Add(number);
                    } else
                    {
                        number++;
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
                var x = BitArrayToLong(PRNGeneration(i, dimensionality));
                if (num == x)
                    return i;
            }
            return -1;
        }

        public int GetNumberLength(ulong num) => (num == 1 || num == 0) ? 1 : (int)Math.Floor(Math.Log(num, 2)) + 1;

        public ulong BitArrayToLong(BitArray bits)
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
        public BitArray PRNGeneration(byte type, int size)
        {
            var bits = new BitArray(size);
            switch (type)
            {
                case 0:
                    for (int i = 0; i < size; i++)
                        bits.Set(i, false);
                    break;
                case 1:
                    bits.Set(size - 1, false);
                    bits.Set(size - 2, false);
                    bits.Set(size - 3, false);
                    for (int i = size - 4; i >= 0; i--)
                        bits.Set(i, true);
                    break;
                case 2:
                    bits.Set(size - 1, false);
                    bits.Set(size - 2, false);
                    bits.Set(size - 3, true);
                    for (int i = size - 4; i >= 0; i--)
                        bits.Set(i, false);
                    break;
                case 3:
                    bits.Set(size - 1, false);
                    bits.Set(size - 2, false);
                    bits.Set(size - 3, true);
                    for (int i = size - 4; i >= 0; i--)
                        bits.Set(i, true);
                    break;
                case 4:
                    bits.Set(size - 1, false);
                    bits.Set(size - 2, true);
                    bits.Set(size - 3, false);
                    for (int i = size - 4; i >= 0; i--)
                        bits.Set(i, false);
                    break;
                case 5:
                    bits.Set(size - 1, false);
                    bits.Set(size - 2, true);
                    bits.Set(size - 3, false);
                    for (int i = size - 4; i >= 0; i--)
                        bits.Set(i, true);
                    break;
                case 6:
                    bits.Set(size - 1, false);
                    bits.Set(size - 2, true);
                    bits.Set(size - 3, true);
                    for (int i = size - 4; i >= 0; i--)
                        bits.Set(i, false);
                    break;
                case 7:
                    bits.Set(size - 1, false);
                    for (int i = size - 2; i >= 0; i--)
                        bits.Set(i, true);
                    break;
                case 8:
                    bits.Set(size - 1, true);
                    for (int i = size - 2; i >= 0; i--)
                        bits.Set(i, false);
                    break;
                case 9:
                    bits.Set(size - 1, true);
                    bits.Set(size - 2, false);
                    bits.Set(size - 3, false);
                    for (int i = size - 4; i >= 0; i--)
                        bits.Set(i, true);
                    break;
                case 10:
                    bits.Set(size - 1, true);
                    bits.Set(size - 2, false);
                    bits.Set(size - 3, true);
                    for (int i = size - 4; i >= 0; i--)
                        bits.Set(i, false);
                    break;
                case 11:
                    bits.Set(size - 1, true);
                    bits.Set(size - 2, false);
                    bits.Set(size - 3, true);
                    for (int i = size - 4; i >= 0; i--)
                        bits.Set(i, true);
                    break;
                case 12:
                    bits.Set(size - 1, true);
                    bits.Set(size - 2, true);
                    bits.Set(size - 3, false);
                    for (int i = size - 4; i >= 0; i--)
                        bits.Set(i, false);
                    break;
                case 13:
                    bits.Set(size - 1, true);
                    bits.Set(size - 2, true);
                    bits.Set(size - 3, false);
                    for (int i = size - 4; i >= 0; i--)
                        bits.Set(i, true);
                    break;
                case 14:
                    bits.Set(size - 1, true);
                    bits.Set(size - 2, true);
                    bits.Set(size - 3, true);
                    for (int i = size - 4; i >= 0; i--)
                        bits.Set(i, false);
                    break;
                case 15:
                    for (int i = 0; i < size; i++)
                        bits.Set(i, true);
                    break; 
                    //case 0:
                    //    for (int i = 0; i < size; i++)
                    //        bits.Set(i, false);
                    //    break;
                    //case 1:
                    //    bits.Set(0, true);
                    //    for (int i = 1; i < size; i++)
                    //        bits.Set(i, false);
                    //    break;
                    //case 2:
                    //    if (size % 2 == 0)
                    //    {
                    //        bits.Set(0, false);
                    //        bits.Set(1, true);
                    //        var flag10 = false;
                    //        for (int i = size - 3; i >= 2; i--)
                    //        {
                    //            bits.Set(i, flag10);
                    //            flag10 = !flag10;
                    //        }
                    //        bits.Set(size - 2, false);
                    //        bits.Set(size - 1, false);
                    //    }
                    //    else
                    //    {
                    //        bits.Set(0, true);
                    //        bits.Set(1, true);
                    //        var flag10 = false;
                    //        for (int i = size - 3; i >= 2; i--)
                    //        {
                    //            bits.Set(i, flag10);
                    //            flag10 = !flag10;
                    //        }
                    //        bits.Set(size - 2, false);
                    //        bits.Set(size - 1, false);
                    //    }
                    //    break;
                    //case 3:
                    //    if (size % 2 == 0)
                    //    {
                    //        bits.Set(0, true);
                    //        bits.Set(1, true);
                    //        var flag2 = true;
                    //        for (int i = size - 3; i >= 2; i--)
                    //        {
                    //            bits.Set(i, flag2);
                    //            flag2 = !flag2;
                    //        }
                    //        bits.Set(size - 2, false);
                    //        bits.Set(size - 1, false);
                    //    }
                    //    else
                    //    {
                    //        bits.Set(0, false);
                    //        bits.Set(1, true);
                    //        var flag2 = true;
                    //        for (int i = size - 3; i >= 2; i--)
                    //        {
                    //            bits.Set(i, flag2);
                    //            flag2 = !flag2;
                    //        }
                    //        bits.Set(size - 2, false);
                    //        bits.Set(size - 1, false);
                    //    }
                    //    break;
                    //case 4:
                    //    bits.Set(size - 2, true);
                    //    break;
                    //case 5:
                    //    var flag3 = false;
                    //    for (int i = size - 1; i >= 0; i--)
                    //    {
                    //        bits.Set(i, flag3);
                    //        flag3 = !flag3;
                    //    }
                    //    break;
                    //case 6:
                    //    bits.Set(size - 2, true);
                    //    var flag12 = true;
                    //    for (int i = size - 3; i >= 0; i--)
                    //    {
                    //        bits.Set(i, flag12);
                    //        flag12 = !flag12;
                    //    }
                    //    break;
                    //case 7:
                    //    for (int i = 0; i < size - 1; i++)
                    //        bits.Set(i, true);
                    //    bits.Set(size - 1, false);
                    //    break;
                    //case 8:
                    //    for (int i = 0; i < size - 1; i++)
                    //        bits.Set(i, false);
                    //    bits.Set(size - 1, true);
                    //    break;
                    //case 9:
                    //    bits.Set(size - 1, true);
                    //    var flag13 = true;
                    //    for (int i = size - 4; i >= 0; i--)
                    //    {
                    //        bits.Set(i, flag13);
                    //        flag13 = !flag13;
                    //    }
                    //    break;
                    ////supplementary types
                    //case 10:
                    //    var flag1 = true;
                    //    for (int i = size - 1; i >= 0; i--)
                    //    {
                    //        bits.Set(i, flag1);
                    //        flag1 = !flag1;
                    //    }
                    //    break;
                    //case 11:
                    //    for (int i = 0; i < size; i++)
                    //        bits.Set(i, true);
                    //    bits.Set(size - 2, false);
                    //    break;
                    //case 12:
                    //    if (size % 2 == 0)
                    //    {
                    //        bits.Set(0, false);
                    //        bits.Set(1, false);
                    //        var flag7 = false;
                    //        for (int i = size - 3; i >= 2; i--)
                    //        {
                    //            bits.Set(i, flag7);
                    //            flag7 = !flag7;
                    //        }
                    //        bits.Set(size - 2, true);
                    //        bits.Set(size - 1, true);
                    //    }
                    //    else
                    //    {
                    //        bits.Set(0, true);
                    //        bits.Set(1, false);
                    //        var flag7 = false;
                    //        for (int i = size - 3; i >= 2; i--)
                    //        {
                    //            bits.Set(i, flag7);
                    //            flag7 = !flag7;
                    //        }
                    //        bits.Set(size - 2, true);
                    //        bits.Set(size - 1, true);
                    //    }
                    //    break;
                    //case 13:
                    //    if (size % 2 == 0)
                    //    {
                    //        bits.Set(0, true);
                    //        bits.Set(1, false);
                    //        var flag15 = true;
                    //        for (int i = size - 3; i >= 2; i--)
                    //        {
                    //            bits.Set(i, flag15);
                    //            flag15 = !flag15;
                    //        }
                    //        bits.Set(size - 2, true);
                    //        bits.Set(size - 1, true);
                    //    }
                    //    else
                    //    {
                    //        bits.Set(0, false);
                    //        bits.Set(1, false);
                    //        var flag15 = true;
                    //        for (int i = size - 3; i >= 2; i--)
                    //        {
                    //            bits.Set(i, flag15);
                    //            flag15 = !flag15;
                    //        }
                    //        bits.Set(size - 2, true);
                    //        bits.Set(size - 1, true);
                    //    }
                    //    break;
                    //case 14:
                    //    bits.Set(0, false);
                    //    for (int i = 1; i < size; i++)
                    //        bits.Set(i, true);
                    //    break;
                    //case 15:
                    //    for (int i = 0; i < size; i++)
                    //        bits.Set(i, true);
                    //    break;
            }
            return bits;
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
                    var x = BitArrayToLong(PRNGeneration(j, i));
                    if (types[x] == 0)
                        types[x] = (byte)(dimensionality - i + 1);
                }
            }

            return types;
        }
    }
}
