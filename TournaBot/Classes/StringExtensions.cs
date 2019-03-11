using System;
using System.Linq;

namespace BehaveBot.Classes
{
    public static class StringExtensions
    {

        public static string CenterString(this string value)
        {
            return String.Format("{0," + ((Console.WindowWidth / 2) + ((value).Length / 2)) + "}", value);
        }

        public static string RemoveSpaces(this string value)
        {
            while (value.First() == ' ' && value != "")
                value = value.Substring(1);

            while (value.Last() == ' ' && value != "")
                value = value.Substring(0, value.Length - 1);

            return value;
        }
    }
}
