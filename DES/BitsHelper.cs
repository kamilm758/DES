using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DES
{
    public static class BitsHelper
    {
        public static IEnumerable<bool> ConvertToBits(byte b)
        {
            for (int i = 0; i < 8; i++)
            {
                yield return (b & 0x80) != 0;
                b *= 2;
            }
        }

        public static string ConvertToString(bool[] bits)
        {
            string s = "";
            foreach (var b in bits)
                s += (b ? '1' : '0');
            return s;
        }

        public static byte[] ConvertToBytes(bool[] bits)
        {
            byte[] bytes = new byte[(bits.Length) / 8];
            string bitsAsString = ConvertToString(bits);
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = Convert.ToByte(bitsAsString.Substring(8 * i, 8), 2);
            }
            return bytes;
        }
    }
}
