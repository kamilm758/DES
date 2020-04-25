using DES.KeyGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DES
{
    class Program
    {
        static void Main(string[] args)
        {
            //var test = DesTester.Encrypt("AlaMaKot");
            //System.Diagnostics.Debug.WriteLine(test);
            //System.Diagnostics.Debug.WriteLine(DesTester.Decrypt(test));

            //foreach(var s in Globals.ExampleKeyInt)
            //    System.Diagnostics.Debug.Write(s);

            //Step 0 - Read .bin file into blocks
            Console.Write("Input file name: ");
            string fileName = Console.ReadLine();
            bool[] input = FileManager.Read(fileName);
            List<bool[]> blocks = input
                    .Select((s, i) => new { Value = s, Index = i })
                    .GroupBy(x => x.Index / 64)
                    .Select(grp => grp.Select(x => x.Value).ToArray())
                    .ToList();
            Console.Write("Encrypyt (0) or Decrypt (1): ");
            int choice = Convert.ToInt32(Console.ReadLine());

            //Dodac jedynke a potem zera
            //if (choice == 0)
            //{
            //    EncryptionFix(blocks);
            //}
            //else if (choice == 1)
            //{
            //    //TODO: Usuwac zera z konca az do napotkania jedynki (jedynke tez usunac)
            //    int currentIndex = 63;
            //    for(; currentIndex >= 0; currentIndex--)
            //    {
            //        if (blocks[blocks.Count - 1][currentIndex] != false)
            //        {
            //            break;
            //        }
            //    }
            //    //Usunac część bloku
            //    if(currentIndex != 0)
            //    {
            //        int howManyBoolsToTake = currentIndex + 1;
            //        blocks[blocks.Count - 1] = blocks[blocks.Count - 1].Take(howManyBoolsToTake).ToArray();
            //    }
            //    //Usunac pelny blok
            //    else
            //    {
            //        blocks.RemoveAt(blocks.Count - 1);
            //    }
            //    //Nie wiem co z niepelnym blokiem bo bedzie problem z permutacja itp...
            //}
            //else
            //{
            //    throw new ArgumentException("Invalid choice");
            //}

            for (int i = 0; i < blocks.Count; i++)
            {
                System.Diagnostics.Debug.WriteLine("[{0}] Original block: ", i);
                foreach (var b in BitsHelper.ConvertToString(blocks[i]))
                    System.Diagnostics.Debug.Write(b);
                System.Diagnostics.Debug.WriteLine("");
            }

            //Step 1 - Initial Permutation
            for (int i = 0; i < blocks.Count; i++)
            {
                blocks[i] = DesMethods.Permute(Globals.IP, blocks[i]);
                System.Diagnostics.Debug.WriteLine("[{0}] Permuted block: ", i);
                foreach(var b in BitsHelper.ConvertToString(blocks[i]))
                    System.Diagnostics.Debug.Write(b);
                System.Diagnostics.Debug.WriteLine("");
            }
            //Step 2 - Divide each block into left and right side
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
            //DebugInfoBlock18(blocks, leftSide, rightSide);
            QueueKey key = new QueueKey(Globals.ExampleKey);
            //Loop
            //Foreach block
            for (int i = 0; i < blocks.Count; i++)
            {
                
                int j;
                //Repeat 15 times
                
                //Encryption -> Keys from 0 to 15
                if(choice == 0)
                {
                    for (j = 0; j < 15; j++)
                    {
                        //Copy right side to assign it to left side later
                        var rightSideCopy = rightSide[i];
                        //Step 3 - Function f(R, K):
                        //Step 3.1 - Extend right side
                        //Step 3.2 - Xor(extended, K) [nie wiem skad K]
                        //Step 3.3 - S-boxes
                        //Step 3.4 - Permutation
                        //Step 3.5 - Assign right side
                        //var right = DesMethods.DESFunctionRK(rightSide[i], key.GetKey(j));
                        //rightSide[i] = BitsHelper.XORTwoBitArrays(leftSide[i], right);

                        rightSide[i] = BitsHelper.XORTwoBitArrays(leftSide[i], DesMethods.DESFunctionRK(rightSide[i], key.GetKey(j)));

                        //Step 4 - Assign left side
                        leftSide[i] = rightSideCopy;
                    }
                }
                //Decryption -> Keys from 15 to 0
                else
                {
                    for(j = 15; j > 0; j--)
                    {
                        var rightSideCopy = rightSide[i];
                        rightSide[i] = BitsHelper.XORTwoBitArrays(leftSide[i], DesMethods.DESFunctionRK(rightSide[i], key.GetKey(j)));
                        leftSide[i] = rightSideCopy;
                    }
                }
                //16th is dirrerent (prawa z lewa sie nie zamieniaja)

                //1 (zamiana dwa razy)

                //var rightSideCopy2 = rightSide[i];
                //rightSide[i] = BitsHelper.XORTwoBitArrays(leftSide[i], DesMethods.DESFunctionRK(rightSide[i], key.GetKey(j)));
                //leftSide[i] = rightSideCopy2;

                //var cpy = rightSide[i];
                //rightSide[i] = leftSide[i];
                //leftSide[i] = cpy;

                //2 
                var leftSideCpy = leftSide[i];
                leftSide[i] = BitsHelper.XORTwoBitArrays(leftSide[i], DesMethods.DESFunctionRK(rightSide[i], key.GetKey(j)));
                rightSide[i] = leftSideCpy;

            }
            //Step (N - 2) - Make sure output is written to "blocks" structure
            for(int i = 0; i < blocks.Count; i++)
            {
                blocks[i] = leftSide[i].Concat(rightSide[i]).ToArray();
            }
            //Step (N - 1) - Inverse Initial Permutation
            for (int i = 0; i < blocks.Count; i++)
            {
                blocks[i] = DesMethods.Permute(Globals.IPT, blocks[i]);
            }
            //Step N - write to file
            byte[][] output = new byte[blocks.Count][];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = BitsHelper.ConvertToBytes(blocks[i]);
            }
            if(choice == 0)
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

        private static void EncryptionFix(List<bool[]> blocks)
        {
            //Dodaj jeden pelny blok
            if (blocks[blocks.Count - 1].Length == 64)
            {
                bool[] block = new bool[64];
                block[0] = true;
                blocks.Add(block);
            }
            //Wypelnij niepelny blok
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

        private static void DebugInfoBlock18(List<bool[]> blocks, List<bool[]> leftSide, List<bool[]> rightSide)
        {
            //Debug - print full block, left side, right side (looks like it's working)
            foreach (var b in blocks[18])
                System.Diagnostics.Debug.Write(b ? '1'.ToString() : '0'.ToString());
            System.Diagnostics.Debug.WriteLine("");

            foreach (var b in leftSide[18])
                System.Diagnostics.Debug.Write(b ? '1'.ToString() : '0'.ToString());
            System.Diagnostics.Debug.WriteLine("");

            foreach (var b in rightSide[18])
                System.Diagnostics.Debug.Write(b ? '1'.ToString() : '0'.ToString());
            System.Diagnostics.Debug.WriteLine("");
        }
    }
}
