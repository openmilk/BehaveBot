using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace LlamaBot.Classes
{
    public static class StringExtensions
    {
        public static string FormatTeamName(this string teamName, SocketUser captain, string prefix)
        {
            var index = teamName.Replace("!", "").IndexOf(captain.Mention.Replace("!", ""));
            if (index == 0)
            {
                return "";
            }

            teamName = teamName.Remove(index - 1, teamName.Length - index + 1);
            if (!teamName.StartsWith(prefix))
            {
                teamName = prefix + " " + teamName;
            }
            return teamName.ToUpper();
        }

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
