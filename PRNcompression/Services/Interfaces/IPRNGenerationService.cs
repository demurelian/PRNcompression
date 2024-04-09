using System.Security.RightsManagement;

namespace PRNcompression.Services.Interfaces
{
    internal interface IPRNGenerationService
    {
        int PRNGeneration(byte type, int size);
    }
}
