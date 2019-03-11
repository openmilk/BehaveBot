using System;
using System.Collections.Generic;
using Discord.Rest;
using Newtonsoft.Json;
using System.Linq;
using BehaveBot.Classes;

namespace TournaBot.Classes
{
    using System.Threading.Tasks;
    using static DiscordExtensions;
    public class JsonExtensions
    {


        public class Scores
        {
            public List<Game> games { get; set; } = new List<Game>();
            public long id { get; set; } = 0;
            public string name { get; set; } = "";

            public List<int> ExtraPlacement { get; set; } = new List<int>();
            public List<int> ExtraKills { get; set; } = new List<int>();
        }

        public class Game
        {
            public ulong Scorer { get; set; } = 0;

            public int Kills { get; set; } = 0;

            public int Placement { get; set; } = 0;

            public string GameNumber { get; set; } = "";

            public string proof { get; set; } = "";

            public DateTime time { get; set; } = DateTime.Now;
        }


        public class information
        {
            public Type GameMode { get; set; } = Type.Null;
            public enum Type
            {
                Team,
                preset,
                Null
            }

            public string Name { get; set; } = "";
            public string Text { get; set; } = "";
        }




        public class ListAttendance
        {
            public string Users { get; set; } = "";
            public string GameCode { get; set; } = "";
        }

        public class gamecodes
        {
            public string games { get; set; } = "";
            public int amount { get; set; } = 1;
        }

        public class Attendance
        {
            public List<ListAttendance> lList { get; set; } = new List<ListAttendance>();
            public List<ulong> MessageID { get; set; } = new List<ulong>();
        }

        
    }


}
