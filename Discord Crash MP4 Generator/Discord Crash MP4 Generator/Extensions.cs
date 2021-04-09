using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_Crash_MP4_Generator
{
    static class Extensions
    {
        public static string Repeat(this string input, int amount)
        {
            string result = "";

            for (int i = 0; i < amount - 1; i++)
                result += input;

            return result;
        }
    }
}
