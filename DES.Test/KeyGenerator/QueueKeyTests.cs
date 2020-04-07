using DES.KeyGenerator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace DES.Test.KeyGenerator
{
    //aby testy działały, należy zrobić publiczne wszystkie pola i metody
    public class QueueKeyTests
    {
        [Fact]
        public void CheckCorrectnessPermutationPC1()
        {
            QueueKey queueKey = new QueueKey(TestData.ExampleKey);
            queueKey.PermuteChoice1(TestData.ExampleKey);
            
            for(int i = 0; i < Globals.PC1C.Length; i++)
            {
                Assert.Equal(TestData.ExampleKey[Globals.PC1C[i]], queueKey._c[i]);
                Assert.Equal(TestData.ExampleKey[Globals.PC1D[i]], queueKey._d[i]);
            }
        }

        [Fact]
        public void CheckCorrectnessLeftShift()
        {
            bool[] exampleBitArray = new bool[11]
            {
                true, true, false, true, false, false, false, true, false, false, true
            };

            bool[] expectedResult = new bool[11]
            {
                false, false, true, false, false, true, true , true, false, true, false
            };

            QueueKey queueKey = new QueueKey(TestData.ExampleKey);
            queueKey.LeftShift(5, ref exampleBitArray);

            for(int i = 0; i < expectedResult.Length; i++)
            {
                Assert.Equal(expectedResult[i], exampleBitArray[i]);
            }
        }

        [Fact]
        public void CheckCorrectnessPermutationPC2()
        {
            QueueKey queueKey = new QueueKey(TestData.ExampleKey);
            bool[] result = new bool[48];
            queueKey.Permute(TestData.ExampleKey, Globals.PC2, ref result);

            for(int i=0; i < Globals.PC2.Length; i++)
            {
                Assert.Equal(TestData.ExampleKey[Globals.PC2[i]], result[i]);
            }
        }

        [Fact]
        public void CheckCorrectnessKeyOne()
        {
            QueueKey queueKey = new QueueKey(TestData.ExampleKey);
            queueKey.PermuteChoice1(TestData.ExampleKey);
            queueKey.LeftShift(1, ref queueKey._c);
            queueKey.LeftShift(1, ref queueKey._c);
            bool[] concatCD = queueKey._c.Concat(queueKey._d).ToArray();
            queueKey.GenerateKeys();
            bool[] result = queueKey.GetKey(0);

            for (int i=0; i < result.Length; i++)
            {
                Assert.Equal(concatCD[Globals.PC2[i]], result[i]);
            }
        }
    }
}
