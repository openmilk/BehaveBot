using Discord.Commands;
using Discord.WebSocket;
using Discord.Rest;
using Discord;

using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.Serialization;
using System;

using LlamaBot.Classes;
using LlamaBot.Services;
using TournaBot.Classes;
using RestSharp;
using Newtonsoft.Json;


namespace LlamaBot.Modules
{
    using static DiscordExtensions;
    using static JsonExtensions;
    using static LeagueExtensions;


    public class SUCommander : ModuleBase<SocketCommandContext>
    {
        readonly SettingsHandlerService settings;
        private SocketGuildUser user;

        public SUCommander(SettingsHandlerService _settings)
        {
            settings = _settings;
        }

        protected override void BeforeExecute(CommandInfo command)
        {
            if (Context.Guild != null)
            {
                user = Context.User as SocketGuildUser;
            }
        }


        
    }
}
