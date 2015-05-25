using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenerateJSONParamFiles
{
    public static class Progress
    {
        static string[] symbols = new string[4] { "-", "\\", "|", "/" };
        static int current = 0;
        static decimal i = 0;
        static decimal total = 0;

        public static void Increment()
        {
            Console.Write("{0}\r", GetSymbol());
        }

        private static string GetSymbol()
        {
            string result = String.Format("{0}", symbols[current % symbols.Length]);
            current++;
            return result;
        }

        public static decimal Total
        {
            get
            {
                i++;
                return total;
            }
            set
            {
                i = 0;
                total = value;
            }
        }

        public static string Percent()
        {
            return String.Format("{0} {1}%\r", GetSymbol(), Math.Round(i / Total * 100, 2));
        }

        public static void PrintPercent()
        {
            Console.Write(Percent());
        }
    }
}
