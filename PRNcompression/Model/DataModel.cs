﻿using System;

namespace PRNcompression.Model
{
    internal class DataModel
    {
        public byte[] initialData { get; set; }
        public byte[] resultData { get; set; }
        public byte[] GenerateBytes(int size)
        {
            initialData = new byte[size];
            new Random().NextBytes(initialData);
            return initialData;
        }

        public DataModel() { }
    }
}
