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
    internal interface IPRNService
    {
        int PRNGeneration(byte type, int size);
        int GetNumberType(int num);
        byte[] FieldCharacterization(byte dimensionality);
        CompressedInfo Compression(int number, int numberLength, ref List<bool> list);
    }
}
