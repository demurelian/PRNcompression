using PRNcompression.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PRNcompression.Services
{
    internal class BytesGenerationService : IBytesGenerationService
    {
        public IEnumerable<byte> GenerateBytes(string byteNumStr)
        {
            var generate_size = ValidationHelper.ValidateNumberString(byteNumStr);
            if(generate_size > 0)
            {
                var byte_arr = new byte[generate_size];
                new Random().NextBytes(byte_arr);
                return byte_arr;
            } else
            {
                return Enumerable.Empty<byte>();
            }
        }
    }
}
