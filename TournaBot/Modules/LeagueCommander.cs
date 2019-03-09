using Discord.Commands;
using Discord.WebSocket;
using Discord.Rest;
using Discord;

using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

using LlamaBot.Classes;
using LlamaBot.Services;
using TournaBot.Classes;
using Newtonsoft.Json;



using System.IO;
using System.Threading;


namespace LlamaBot.Modules
{
    using static DiscordExtensions;
    using static LeagueExtensions;
    using static StringExtensions;
    using static JsonExtensions;


    public class LeagueCommander : ModuleBase<SocketCommandContext>
    {
        private readonly LeagueHandlerService league;
        private readonly SettingsHandlerService settings;
        private SocketGuildUser user;

        public LeagueCommander(LeagueHandlerService _league, SettingsHandlerService _settings)
        {
            league = _league;
            settings = _settings;
        }

        protected override void BeforeExecute(CommandInfo command)
        {
            if (Context.Guild != null)
            {
                user = Context.User as SocketGuildUser;
            }
        }       

       

        



        [Command("DayOfWeek")]
        [RequireContext(ContextType.Guild)]
        private async Task DayOfWeek()
        {
            await Context.Channel.SendMessageAsync(DateTime.Today.DayOfWeek.ToString());
        }

    }
}
