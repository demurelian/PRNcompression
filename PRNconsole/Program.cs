using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

public class LZWCompression
{
    public static byte[] Compress(byte[] data)
    {
        Dictionary<string, int> dictionary = new Dictionary<string, int>();
        List<int> compressedData = new List<int>();

        for (int i = 0; i < 256; i++)
        {
            dictionary.Add(((char)i).ToString(), i);
        }

        string current = "";
        foreach (byte b in data)
        {
            string next = current + (char)b;
            if (dictionary.ContainsKey(next))
            {
                current = next;
            }
            else
            {
                compressedData.Add(dictionary[current]);
                dictionary.Add(next, dictionary.Count);
                current = ((char)b).ToString();
            }
        }

        compressedData.Add(dictionary[current]);

        List<byte> result = new List<byte>();
        foreach (int code in compressedData)
        {
            byte[] bytes = BitConverter.GetBytes(code);
            result.AddRange(bytes);
        }

        return result.ToArray();
    }
}

class Program
{
    static void Main(string[] args)
    {

        string inputFile = "D:\\TEST\\random555.bin";
        string compressedFile = "D:\\TEST\\random555_LZWcompressed.bin";

        // Read bytes from input file
        byte[] inputData = File.ReadAllBytes(inputFile);

        // Compress data using LZW
        byte[] compressedData = LZWCompression.Compress(inputData);

        // Write compressed data to file
        File.WriteAllBytes(compressedFile, compressedData);

        Console.WriteLine("Compression complete.");
    }
}