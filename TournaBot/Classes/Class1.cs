using System;
using System.Collections.Generic;
using Discord.Rest;
using Newtonsoft.Json;

namespace TournaBot.Classes
{
    class JsonExtensions
    {
        game _games = new game
        {
            Kills = "",
            Placement = "",
            GameNumber = "",
            proof = ""
        };


        public class Season
        {
            public IList<game> games;
        }

        public class SeasonUnload
        {
            public IList<game> games;
        }

        public class Attendance
        {
            public List<ListAttendance> lList;
        }

        public class game
        {
            public string Kills { get; set; } = "";

            public string Placement { get; set; } = "";

            public string GameNumber { get; set; } = "";

            public string proof { get; set; } = "";
        }

        public class ListAttendance
        {
            public string Users { get; set; } = "";
            public string GameCode { get; set; } = "";
            public string Team { get; set; } = "";

        }

        public class Message
        {
            public string Lmessage { get; set; } = "";
        }

        public class Text
        {
            public List<ListMessage> MList;
        }

        public class ListMessage
        {
            public string MessageID = "";
        }
    }
}
