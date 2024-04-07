using System.Collections.Generic;

namespace PRNcompression.Services.Interfaces
{
    interface IBytesGenerationService
    {
        IEnumerable<byte> GenerateBytes(string byteNumStr);
    }
}
