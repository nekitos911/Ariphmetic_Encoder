using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ariphmetic
{
    static class Util
    {
        public static readonly int Bits = 30;
        public static readonly int HighestBit = 1 << (Bits - 1);
        public static readonly int Half = 1 << (Bits - 2);
        public static readonly int Mask = (1 << Bits) - 1;
        public static readonly int End = 256;

        public static void ShowPercents(int count, ref double index)
        {
            if (((int)((index - 1) / count * 100) < (int)(index / count * 100)) && (int)(index / count * 100) <= 100)
            {
                Console.Clear();
                Console.WriteLine("{0} %", (int)(index / count * 100));
            }

            index++;
        }
    }
}
