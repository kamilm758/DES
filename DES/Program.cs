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
            //Step 0 - Read bin file into blocks
            Console.Write("Input file name: ");
            string fileName = Console.ReadLine();
            bool[] input = FileManager.Read(fileName);
            bool[][] blocks = input
                    .Select((s, i) => new { Value = s, Index = i })
                    .GroupBy(x => x.Index / 64)
                    .Select(grp => grp.Select(x => x.Value).ToArray())
                    .ToArray();
            //Step 1 - Initial Permutation
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i] = DesMethods.Permute(Globals.IP, blocks[i]);
            }
            //Step 2 - Divide each block into left and right side
            bool[][] leftSide = new bool[blocks.Length][];
            bool[][] rightSide = new bool[blocks.Length][];
            for (int i = 0; i < blocks.Length; i++)
            {
                leftSide[i] = blocks[i].Where((x, index) => index < 32).ToArray();
            }
            for (int i = 0; i < blocks.Length; i++)
            {
                rightSide[i] = blocks[i].Where((x, index) => index >= 32 && index < 64).ToArray();
            }
            DebugInfoBlock18(blocks, leftSide, rightSide);

            //Loop
            //Foreach block
            for(int i=0;i<blocks.Length;i++)
            {
                //Repeat 15 times
                for(int j=0;j<15;j++)
                {
                    //Step 3 - Function f(R, K):
                    //Step 3.1 - Extend right side
                    bool[] extendedRightSide = DesMethods.Extend(Globals.E, rightSide[i]);
                    //Step 3.2 - Xor(extended, K) [nie wiem skad K]
                    //Step 3.3 - S-boxes
                    //Step 3.4 - Permutation
                    //Step 4 - Assign left side, assign right side
                }
                //16th is dirrerent
            }
            //Step (N - 2) - Make sure output is written to "blocks" structure
            //Step (N - 1) - Inverse Initial Permutation
            for (int i = 0; i < blocks.Length; i++)
            {
                blocks[i] = DesMethods.Permute(Globals.IPT, blocks[i]);
            }
            //Step N - write to file
            byte[][] output = new byte[blocks.Length][];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = BitsHelper.ConvertToBytes(blocks[i]);
            }
            FileManager.Write("Results-" + fileName, output);
            Console.ReadKey();
        }

        private static void DebugInfoBlock18(bool[][] blocks, bool[][] leftSide, bool[][] rightSide)
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
