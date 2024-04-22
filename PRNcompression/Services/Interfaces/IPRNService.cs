using System.Collections;
using System.Collections.Generic;
using System.Security.RightsManagement;

namespace PRNcompression.Services.Interfaces
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
    internal interface IPRNService
    {
        ulong PRNGeneration(byte type, int size);
        int GetNumberType(ulong num, int dimensionality);
        int GetNumberLength(ulong num);
        byte[] FieldCharacterization(byte dimensionality);
        CompressedInfo Compression(ulong number, int numberLength, ref List<bool> list);
        DecompressedInfo Decompression(BitArray serviceInfo, BitArray data);
    }
}
