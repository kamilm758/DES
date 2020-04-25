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
            if (input.Length != permutationTable.Length)
                throw new ArgumentException("Permutation array length did not match argument array length");
            bool[] result = new bool[input.Length];
            for(int i = 0; i < permutationTable.Length; i++)
            {
                int currentIndex = permutationTable[i];
                result[i] = input[currentIndex];
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

        public static bool[] DESFunctionRK(bool[] rBitArray, bool[] key)
        {
            bool[] extendedRBitArray = Extend(Globals.E,rBitArray);
            bool[] xorResult = BitsHelper.XORTwoBitArrays(extendedRBitArray, key);
            bool[] result=new bool[0];
            for(int i=0; i < xorResult.Length; i+=6)
            {
                int whichSbox = i / 6;
                bool[] currentBits = xorResult.Skip(i).Take(6).ToArray();
                int boundaryBitsDecimalValue = BitsHelper.ConvertBinaryToDecimalValue(new bool[] { currentBits[0], currentBits[5] });
                int internalValues = BitsHelper.ConvertBinaryToDecimalValue(currentBits.Skip(1).Take(4).ToArray());
                result = (result.Concat(BitsHelper.ConvertDecimalToFourBits(Globals.sBoxArrays[whichSbox][boundaryBitsDecimalValue, internalValues]))).ToArray();
            }
            return Permute(Globals.permuteArrayForFunctionRK,result);
        }
    }
}
