using System.Collections.Generic;
using System.Security.RightsManagement;

namespace PRNcompression.Services.Interfaces
{
    public class FieldInfo
    {
        public int[] ZoneEdges { get; set; }
        public byte[] Types { get; set; }
        public Dictionary<byte, List<int>> TypesDictionary { get; set; }
    }
    internal interface IPRNService
    {
        int PRNGeneration(byte type, int size);
        int GetNumberType(int num);
        FieldInfo FieldCharacterization(byte dimensionality);
    }
}
