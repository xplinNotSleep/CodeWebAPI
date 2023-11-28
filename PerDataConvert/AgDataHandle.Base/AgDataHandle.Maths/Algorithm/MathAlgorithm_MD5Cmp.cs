using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace AgDataHandle.Maths
{
    public partial class MathAlgorithm_MD5Cmp
    {
        public static byte[] Compute(string str)
        {
            var byts = Encoding.UTF8.GetBytes(str);
            var md5 = new MD5CryptoServiceProvider();
            var bts = md5.ComputeHash(byts);
            return bts;
        }
    }
}
