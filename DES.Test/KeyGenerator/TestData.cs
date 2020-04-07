using System;
using System.Collections.Generic;
using System.Text;

namespace DES.Test.KeyGenerator
{
    public static class TestData
    {
        public static bool[] ExampleKey = new bool[]
        {
            true, false, false, true , true, false, true, false,
            false, true, false, true, true, true, true, false,
            false, true, false, false, true, true, false, false,
            false, true, true, true, true, true, true, true,
            false, true, false, true, true, true, true, false,
            false, true, false, false, true, true, false, true,
            false, true, true, true, true, true, true, true,
            false, true, false, false, false, false, false, false,
        };
    }
}
