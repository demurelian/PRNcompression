using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace PRNcompression.Model
{
    internal class DataModel
    {
        public static byte[] GenerateBytes(int size)
        {
            var byte_arr = new byte[size];
            new Random().NextBytes(byte_arr);
            return byte_arr;
        }
    }
}
