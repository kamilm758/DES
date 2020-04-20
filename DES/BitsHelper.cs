using System;
using System.Collections;
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

        public static bool[] XORTwoBitArrays(bool[] firstBitArray, bool[] secondBitArray)
        {
            if (firstBitArray.Length != secondBitArray.Length)
                throw new Exception("Arrays have different length");
            bool[] result = new bool[firstBitArray.Length];
            for (int i = 0; i < result.Length; i++)
                result[i] = firstBitArray[i] == secondBitArray[i] ? false : true;
            return result;
        }

        public static bool[] ConvertDecimalToFourBits(int decimalValue)
        {
            if (decimalValue > 15)
                throw new Exception("Paremeter must be less than 15");
            BitArray bitArray = new BitArray(new int[] { decimalValue });
            List<bool> result = new List<bool>();
            for(int i = 0; i < bitArray.Length; i++)
            {
                result.Add(bitArray[i]);
            }
            return result.Take(4).Reverse().ToArray();
        }

        public static int ConvertBinaryToDecimalValue(bool[] bitArray)
        {
            string binaryValue = string.Join("", bitArray.Select(s => s ? "1" : "0"));
            return Convert.ToInt32(binaryValue, 2);
        }
    }
}
