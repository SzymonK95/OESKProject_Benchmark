using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmark.Helpers
{
    public static class DataManager
    {
        public const string STOP = @"#@#";

        public static string ConvertColorValueToString(byte colorValue)
        {
            return Convert.ToString(colorValue, 2).PadLeft(8, '0');
        }

        public static string GetPartOfMessageByPosition(string message, int messagePosition, int sizeOfPart)
        {
            try
            {
                return message.Substring(messagePosition, sizeOfPart);
            }
            catch (Exception ex)
            {
                return message.Substring(messagePosition, message.Length - messagePosition);
            }
        }
    }
}
