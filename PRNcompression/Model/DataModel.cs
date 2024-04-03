using System;

namespace PRNcompression.Model
{
    internal class DataModel
    {
        public byte[] initialData { get; set; }
        public byte[] resultData { get; set; }
        public byte[] GenerateBytes(int size)
        {
            byte[] byteArray = new byte[size];
            new Random().NextBytes(byteArray);
            return byteArray;
        }

        public DataModel() { }
    }
}
