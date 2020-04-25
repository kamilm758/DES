using DES.KeyGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DES
{
    enum Mode
    {
        Encryption,
        Decryption
    }
    class Program
    {
        static void Main(string[] args)
        {
            //Step 0 - Read .bin file into blocks
            Console.Write("Input file name: ");
            string fileName = Console.ReadLine();
            bool[] input = FileManager.Read(fileName);
            List<bool[]> blocks = input
                    .Select((s, i) => new { Value = s, Index = i })
                    .GroupBy(x => x.Index / 64)
                    .Select(grp => grp.Select(x => x.Value).ToArray())
                    .ToList();
            Console.Write("Encrypt (0) or Decrypt (1): ");
            int choice = Convert.ToInt32(Console.ReadLine());

            //Step 1 - Add padding if encryption
            if (choice == (int)Mode.Encryption)
            {
                AddPadding(blocks);
            }

            //Step 2 - Initial Permutation
            for (int i = 0; i < blocks.Count; i++)
            {
                blocks[i] = DesMethods.Permute(Globals.IP, blocks[i]);
            }
            //Step 3 - Divide each block into left and right side
            List<bool[]> leftSide = new List<bool[]>();
            List<bool[]> rightSide = new List<bool[]>();
            for (int i = 0; i < blocks.Count; i++)
            {
                leftSide.Add(blocks[i].Where((x, index) => index < 32).ToArray());
            }
            for (int i = 0; i < blocks.Count; i++)
            {
                rightSide.Add(blocks[i].Where((x, index) => index >= 32 && index < 64).ToArray());
            }
            QueueKey key = new QueueKey(Globals.ExampleKey);
            //Loop
            //Foreach block
            for (int i = 0; i < blocks.Count; i++)
            {
                int j;
                //Repeat 15 times
                //Encryption -> Keys from 0 to 15
                if(choice == (int)Mode.Encryption)
                {
                    for (j = 0; j < 15; j++)
                    {
                        //Copy right side to assign it to left side later
                        var rightSideCopy = (bool[])rightSide[i].Clone();
                        //Step 4 - Function f(R, K):
                        //Step 4.1 - Extend right side
                        //Step 4.2 - Xor(extended, K) [nie wiem skad K]
                        //Step 4.3 - S-boxes
                        //Step 4.4 - Permutation
                        //Step 4.5 - Assign right side

                        rightSide[i] = BitsHelper.XORTwoBitArrays(leftSide[i], DesMethods.DESFunctionRK(rightSide[i], key.GetKey(j)));

                        //Step 5 - Assign left side
                        leftSide[i] = (bool[])rightSideCopy.Clone();
                    }
                }
                //Decryption -> Keys from 15 to 0
                else
                {
                    for(j = 15; j > 0; j--)
                    {
                        var rightSideCopy = (bool[])rightSide[i].Clone();
                        rightSide[i] = BitsHelper.XORTwoBitArrays(leftSide[i], DesMethods.DESFunctionRK(rightSide[i], key.GetKey(j)));
                        leftSide[i] = (bool[])rightSideCopy.Clone();
                    }
                }
                //16th iteration is dirrerent (right and left side does not swap)

                //swap twice (equals to no swap)
                var rightSideCopy2 = (bool[])rightSide[i].Clone();
                rightSide[i] = BitsHelper.XORTwoBitArrays(leftSide[i], DesMethods.DESFunctionRK(rightSide[i], key.GetKey(j)));
                leftSide[i] = (bool[])rightSideCopy2.Clone();

                var cpy = rightSide[i];
                rightSide[i] = leftSide[i];
                leftSide[i] = cpy;

            }
            //Step 6 - Concat left and right sides to "blocks" structure
            for(int i = 0; i < blocks.Count; i++)
            {
                blocks[i] = leftSide[i].Concat(rightSide[i]).ToArray();
            }
            //Step 7 - Inverse Initial Permutation on "blocks"
            for (int i = 0; i < blocks.Count; i++)
            {
                blocks[i] = DesMethods.Permute(Globals.IPT, blocks[i]);
            }
            //Step 8 - Remove padding if decryption
            if (choice == (int)Mode.Decryption)
            {
                RemovePadding(blocks);
            }
            //Step 9 - write to file
            byte[][] output = new byte[blocks.Count][];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = BitsHelper.ConvertToBytes(blocks[i]);
            }
            if(choice == (int)Mode.Encryption)
            {
                FileManager.Write("Encrypted-" + fileName, output);
            }
            else
            {
                FileManager.Write("Decrypted-" + fileName, output);
            }
            Console.WriteLine("Success");
            Console.ReadKey();
        }

        private static void RemovePadding(List<bool[]> blocks)
        {
            int currentIndex = 63;
            for (; currentIndex >= 0; currentIndex--)
            {
                if (blocks[blocks.Count - 1][currentIndex] != false)
                {
                    break;
                }
            }
            //Case 1: Remove portion of the block
            if (currentIndex != 0)
            {
                int howManyBoolsToTake = currentIndex + 1;
                blocks[blocks.Count - 1] = blocks[blocks.Count - 1].Take(howManyBoolsToTake).ToArray();
            }
            //Case 2: Remove whole block
            else
            {
                blocks.RemoveAt(blocks.Count - 1);
            }
        }

        private static void AddPadding(List<bool[]> blocks)
        {
            //Case 1: Add full block
            if (blocks[blocks.Count - 1].Length == 64)
            {
                bool[] block = new bool[64];
                block[0] = true;
                blocks.Add(block);
            }
            //Case 2: Fill the last block
            else
            {
                bool[] block = new bool[64];
                int blockSize = blocks[blocks.Count - 1].Length;
                int i;
                for (i = 0; i < blockSize; i++)
                {
                    block[i] = blocks[blocks.Count - 1][i];
                }
                i++;
                block[i] = true;
                blocks.RemoveAt(blocks.Count - 1);
                blocks.Add(block);
            }
        }
    }
}
