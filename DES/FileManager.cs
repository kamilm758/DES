using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DES
{
    public static class FileManager
    {
        public static bool[] Read(string fileName)
        {
            byte[] input = File.ReadAllBytes(fileName);
            List<bool> bits = new List<bool>();
            foreach (var i in input)
            {
                var tempBits = BitsHelper.ConvertToBits(i).ToArray();
                foreach (var bit in tempBits)
                {
                    bits.Add(bit ? true : false);
                }
            }
            return bits.ToArray();
        }

        public static void Write(string filename, byte[][] bytes)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(filename, FileMode.Create)))
            {
                for(int i=0;i<bytes.Length;i++)
                    writer.Write(bytes[i]);
            }
        }
    }
}
