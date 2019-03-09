using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using Newtonsoft.Json;

using Discord;
using Discord.Commands;
using Discord.WebSocket;
using TournaBot.Classes;

namespace LlamaBot.Classes
{
    using static JsonExtensions;
    using static DiscordExtensions;
    using System.IO;
    using Discord.Rest;
    using LlamaBot.Services;
    using Discord.Net;
    using System.Net;

    public static class LeagueExtensions
    {


        public static async Task CreateTeamVoiceChannel(SocketCommandContext Context, string teamName, string leagueCat)
        {
            var guild = Context.Guild;
            var everyoneRole = guild.Roles.First(x => x.Name == "@everyone");
            var moderatorRole = guild.Roles.First(x => x.Name == "Moderator");
            var timeoutRole = guild.Roles.First(x => x.Name == "Timeout");

            var HelperRole = guild.Roles.First(x => x.Name == "Helper");
            var leagueCategory = guild.CategoryChannels.First(x => x.Name == leagueCat);

            var defaultPermissions = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Deny, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit);
            var timeoutPermissions = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Deny, PermValue.Deny, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit);
            var teamPermissions = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit);
            var moderatorPermissions = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Inherit);
            //var leaguePermissions = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit);

            var channel = await guild.CreateVoiceChannelAsync(teamName, null, properties =>
            {
                properties.CategoryId = leagueCategory.Id;
            });

            await channel.AddPermissionOverwriteAsync(everyoneRole, defaultPermissions);
            await channel.AddPermissionOverwriteAsync(timeoutRole, timeoutPermissions);
            await channel.AddPermissionOverwriteAsync(moderatorRole, moderatorPermissions);
            //await channel.AddPermissionOverwriteAsync(leagueRole, leaguePermissions);
            await channel.AddPermissionOverwriteAsync(HelperRole, moderatorPermissions);

            foreach (var user in Context.Message.MentionedUsers)
            {
                await channel.AddPermissionOverwriteAsync(user, teamPermissions);
                //await user.SendMessageAsync("Team **" + channel.Name +"** is now in the scrims");
            }
            //await channel.AddPermissionOverwriteAsync(Context.User, teamPermissions);
            //await Context.User.SendMessageAsync(" Your team **" + channel.Name + "** is now in the scrims");
        }

        public static async Task CreateTeamVoiceChannel(SocketCommandContext Context, string teamName, string leagueCat, List<SocketGuildUser> users)
        {
            var guild = Context.Guild;
            var everyoneRole = guild.Roles.First(x => x.Name == "@everyone");
            var moderatorRole = guild.Roles.First(x => x.Name == "Moderator");
            var timeoutRole = guild.Roles.First(x => x.Name == "Timeout");

            var HelperRole = guild.Roles.First(x => x.Name == "Helper");
            var leagueCategory = guild.CategoryChannels.First(x => x.Name == leagueCat);

            var defaultPermissions = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Deny, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit);
            var timeoutPermissions = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Deny, PermValue.Deny, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit);
            var teamPermissions = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit);
            var moderatorPermissions = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Inherit);
            //var leaguePermissions = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit);

            var channel = await guild.CreateVoiceChannelAsync(teamName, null, properties =>
            {
                properties.CategoryId = leagueCategory.Id;
            });

            await channel.AddPermissionOverwriteAsync(everyoneRole, defaultPermissions);
            await channel.AddPermissionOverwriteAsync(timeoutRole, timeoutPermissions);
            await channel.AddPermissionOverwriteAsync(moderatorRole, moderatorPermissions);
            //await channel.AddPermissionOverwriteAsync(leagueRole, leaguePermissions);
            await channel.AddPermissionOverwriteAsync(HelperRole, moderatorPermissions);

            foreach (var user in users)
            {
                await channel.AddPermissionOverwriteAsync(user, teamPermissions);
                //await user.SendMessageAsync("Team **" + channel.Name +"** is now in the scrims");
            }
            //await channel.AddPermissionOverwriteAsync(Context.User, teamPermissions);
            //await Context.User.SendMessageAsync(" Your team **" + channel.Name + "** is now in the scrims");
        }

        public static async Task CreateTeamVoiceChannel(SocketCommandContext Context, SocketRole teamRole, string leagueCat)
        {

            var guild = Context.Guild;
            var everyoneRole = guild.Roles.First(x => x.Name == "@everyone");
            var moderatorRole = guild.Roles.First(x => x.Name == "Moderator");
            var timeoutRole = guild.Roles.First(x => x.Name == "Timeout");
            var leagueRole = guild.Roles.First(x => x.Name == "League");
            var HelperRole = guild.Roles.First(x => x.Name == "Helper");
            var leagueCategory = guild.CategoryChannels.First(x => x.Name == leagueCat);

            var defaultPermissions = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Deny, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit);
            var timeoutPermissions = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Deny, PermValue.Deny, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit);
            var teamPermissions = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit);
            var moderatorPermissions = new OverwritePermissions(PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Inherit, PermValue.Inherit, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Allow, PermValue.Inherit);

            var channel = await guild.CreateVoiceChannelAsync(teamRole.Name, null, properties =>
            {
                properties.CategoryId = leagueCategory.Id;
            });

            await channel.AddPermissionOverwriteAsync(everyoneRole, defaultPermissions);
            await channel.AddPermissionOverwriteAsync(timeoutRole, timeoutPermissions);
            await channel.AddPermissionOverwriteAsync(teamRole, teamPermissions);
            await channel.AddPermissionOverwriteAsync(moderatorRole, moderatorPermissions);
            //await channel.AddPermissionOverwriteAsync(leagueRole, leaguePermissions);
            await channel.AddPermissionOverwriteAsync(HelperRole, moderatorPermissions);
        }





        public static bool CanCreateTeam(this SocketCommandContext Context, string teamName, int min, int max)
        {
            return true;
        }

        public static async Task<IRole> CreateTeam(this SocketCommandContext Context, string teamName)
        {
            var teamRoleColour = new Color(31, 139, 76);
            var teamRolePermission = new GuildPermissions(true, false, false, false, false, false, false, false, true, true, false, false, false, true, false, false, false, false, false, false, false, false, false, false, false, false, false);
            var teamRole = await Context.Guild.CreateRoleAsync(teamName, teamRolePermission, teamRoleColour);
            return teamRole;
        }



        public static async Task UpdateScores(string directory, int kills, int place, string gameNumber, string url, long id, ulong userid)
        {
            await CheckIfFolerExistAndCreate(directory);
            //await CheckIfFolerExistAndCreate(directory + "/images/");

            List<Game> _game = new List<Game>();
            Scores Season = new Scores { games = _game };
            Console.WriteLine("Team Ids are");
            Console.WriteLine(id);
            Console.WriteLine(userid);
            string jsonData = JsonConvert.SerializeObject(Season, Formatting.Indented);

            await CheckifJsonExistAndCreate(directory, "/score.json", jsonData);

            Console.WriteLine("QWE");

            var loadedString = System.IO.File.ReadAllText(directory + "/score.json");
            var setting = JsonConvert.DeserializeObject<Scores>(loadedString);
            //int fCount = Directory.GetFiles(directory + "/images/", "*", SearchOption.AllDirectories).Length;
            //var imageType = "";
            //if (url != "/")
            //imageType = UrlImageType(url);

            Game game3 = new Game
            {
                Kills = kills,
                Placement = place,
                GameNumber = gameNumber.ToUpper(),
                proof = url,
                Scorer = userid
            };


            var count = 0;
            var numb = -1;
            foreach (Game a in setting.games)
            {
                if (a.GameNumber.ToUpper() == gameNumber.ToUpper() && a.time.DayOfYear == DateTime.Today.DayOfYear)
                {
                    numb = count;
                }
                count++;
            }

            if (numb != -1)
            {
                setting.games.RemoveAt(numb);
            }


            setting.id = id;

            Console.WriteLine(id);
            Console.WriteLine(setting.id);

            setting.games.Add(game3);
            jsonData = JsonConvert.SerializeObject(setting, Formatting.Indented);
            System.IO.File.WriteAllText(directory + "/score.json", jsonData);

        }

        public static async Task UpdateScores(string directory, int kills, int place, string gameNumber, string url, ulong id, ulong userid)
        {
            await CheckIfFolerExistAndCreate(directory);
            //await CheckIfFolerExistAndCreate(directory + "/images/");

            List<Game> _game = new List<Game>();
            Scores Season = new Scores { games = _game };
            Console.WriteLine("Team Ids are");
            Console.WriteLine(id);
            Console.WriteLine(userid);
            string jsonData = JsonConvert.SerializeObject(Season, Formatting.Indented);

            await CheckifJsonExistAndCreate(directory, "/score.json", jsonData);

            Console.WriteLine("QWE");

            var loadedString = System.IO.File.ReadAllText(directory + "/score.json");
            var setting = JsonConvert.DeserializeObject<Scores>(loadedString);
            //int fCount = Directory.GetFiles(directory + "/images/", "*", SearchOption.AllDirectories).Length;
            //var imageType = "";
            //if (url != "/")
            //imageType = UrlImageType(url);

            Game game3 = new Game
            {
                Kills = kills,
                Placement = place,
                GameNumber = gameNumber.ToUpper(),
                proof = url,
                Scorer = userid
            };


            var count = 0;
            var numb = -1;
            foreach (Game a in setting.games)
            {
                if (a.GameNumber.ToUpper() == gameNumber.ToUpper() && a.time.DayOfYear == DateTime.Today.DayOfYear)
                {
                    numb = count;
                }
                count++;
            }

            if (numb != -1)
            {
                setting.games.RemoveAt(numb);
            }

            setting.id = (long)id;
            Console.WriteLine(id);
            Console.WriteLine(setting.id);

            setting.games.Add(game3);
            jsonData = JsonConvert.SerializeObject(setting, Formatting.Indented);
            System.IO.File.WriteAllText(directory + "/score.json", jsonData);

        }

        public static async Task AddUserToAttendance(string score, SocketGuild guild, SocketChannel channel, string id)
        {
            List<ListAttendance> _list = new List<ListAttendance>();
            Attendance _attendance = new Attendance { lList = _list };



            var file = "attendance.json";
            var directory = Environment.CurrentDirectory + "/Guilds/" + guild.Id.ToString() + "/attendance/" + channel.Id.ToString();
            string _jsonData = JsonConvert.SerializeObject(_attendance, Formatting.Indented);

            await CheckIfFolerExistAndCreate(directory);
            await CheckifJsonExistAndCreate(directory + "/", file, _jsonData);


            var loadedString = System.IO.File.ReadAllText(directory + "/" + file);
            var setting = JsonConvert.DeserializeObject<Attendance>(loadedString);
            ListAttendance list = new ListAttendance
            {
                Users = id,
                GameCode = score
            };

            var count = 0;
            var numb = -1;
            foreach (ListAttendance a in setting.lList)
            {
                if (a.Users == id)
                {
                    numb = count;
                }
                count++;
            }

            if (numb != -1)
            {
                setting.lList.RemoveAt(numb);
            }
            setting.lList.Add(list);


            string jsonData = JsonConvert.SerializeObject(setting, Formatting.Indented);
            System.IO.File.WriteAllText(directory + "/" + file, jsonData);
            Console.WriteLine("phase1done");
        }

        

        

        

    }

}
