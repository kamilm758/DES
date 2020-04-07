using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DES
{
    public static class DesMethods
    {
        public static bool[] Permute(int[] permutationTable, bool[] input)
        {
            if (input.Length < 64)
                System.Diagnostics.Debug.WriteLine("Niepelny blok, dlugosc {0}", input.Length);
            bool[] result = new bool[64];
            for (int i = 0; i < input.Length; i++)
            {
                int indexOfCurrentBit = permutationTable[i];
                result[indexOfCurrentBit] = input[i];
            }
            return result;
        }

        public static bool[] Extend(int[] extendTable, bool[] input)
        {
            bool[] output = new bool[extendTable.Length];
            for (int i = 0; i < output.Length; i++)
            {
                int currentIndex = extendTable[i];
                output[i] = input[currentIndex];
            }
            return output;
        }
    }
}
