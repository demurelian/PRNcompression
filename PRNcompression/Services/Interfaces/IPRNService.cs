using System.Collections.Generic;
using System.Security.RightsManagement;

namespace PRNcompression.Services.Interfaces
{
    internal interface IPRNService
    {
        int PRNGeneration(byte type, int size);
        int GetNumberType(int num);
        byte[] FieldCharacterization(byte dimensionality);
    }
}
