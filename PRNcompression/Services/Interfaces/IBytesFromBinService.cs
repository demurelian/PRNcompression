using System.Collections.Generic;

namespace PRNcompression.Services.Interfaces
{
    interface IBytesFromBinService
    {
        IEnumerable<byte> GetBytesFromBin(string filePath);
    }
}
