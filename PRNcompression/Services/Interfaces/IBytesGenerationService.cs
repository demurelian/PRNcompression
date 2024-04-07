using System.Collections.Generic;

namespace PRNcompression.Services.Interfaces
{
    internal interface IBytesGenerationService
    {
        IEnumerable<byte> GenerateBytes(string byteNumStr);
    }
}
