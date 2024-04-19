﻿using System;

namespace PRNcompression.Services
{
    internal static class ValidationHelper
    {
        /// <summary>byte number string validation</summary>
        /// <returns>byte array size or -1 if validation failed</returns>
        public static long ValidateNumberString(string byteNumberStr)
        {
            try
            {
                var x = long.Parse(byteNumberStr);
                if (x < 0) 
                    return -1;
                return x;
            }
            catch (Exception e)
            {
                return -1;
            }
        }
    }
}
