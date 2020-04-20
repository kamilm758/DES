using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DES.KeyGenerator
{
    public class QueueKey
    {
        private bool[] _c = new bool[28];
        private bool[] _d = new bool[28];
        private List<bool[]> _keys = new List<bool[]>();
        public QueueKey(bool[] key)
        {
            PermuteChoice1(key);
            GenerateKeys();
        }

        private void PermuteChoice1(bool[] key)
        {
            Permute(key, Globals.PC1C, ref _c);
            Permute(key, Globals.PC1D, ref _d);
        }

        private void Permute(bool[] key, int[] permuteTable, ref bool[] output)
        {
            for (int i = 0; i < permuteTable.Length; i++)
            {
                int index = permuteTable[i];
                bool value = key[index];
                output[i] = value;
            }
        }

        private void LeftShift(int shift, ref bool[] bitArray)
        {
            bool[] bitsToTransfer = new bool[shift];
            for (int i = 0; i < shift; i++)
                bitsToTransfer[i] = bitArray[i];

            for (int i = shift; i < bitArray.Length; i++)
            {
                bitArray[i - shift] = bitArray[i];
            }

            int j = 0;
            for (int i = bitArray.Length - shift; i < bitArray.Length; i++)
            {
                bitArray[i] = bitsToTransfer[j];
                j++;
            }
        }

        private void GenerateKeys()
        {
            for (int i = 0; i < Globals.NumberOfLeftShiftsInIteration.Length; i++)
            {
                LeftShift(Globals.NumberOfLeftShiftsInIteration[i], ref _c);
                LeftShift(Globals.NumberOfLeftShiftsInIteration[i], ref _d);
                bool[] cAndDConcat = _c.Concat(_d).ToArray();
                bool[] currentKey = new bool[48];
                Permute(cAndDConcat, Globals.PC2, ref currentKey);
                _keys.Add(currentKey);
            }
        }

        public bool[] GetKey(int keyNumber) => _keys[keyNumber];
    }
}
