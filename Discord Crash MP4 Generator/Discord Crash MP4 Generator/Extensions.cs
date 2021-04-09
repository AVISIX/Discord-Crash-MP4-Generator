using System;
using System.Collections.Generic;
using System.IO;
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

        public static string ToHex(this byte[] input)
        {
            return string.Concat(input.Select(b => b.ToString("X2")));
        }

        /// <summary>
        /// Returns the Decimal Value by combining all bytes into 1
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int ToDecimal(this byte[] input)
        {
            string s = input.ToHex();

            return Convert.ToInt32(s, 16);
        }

        public static string GetString(this byte[] input)
        {
            string result = "";

            foreach (byte b in input)
                result += (char)b;

            return result;
        }
    }
}
