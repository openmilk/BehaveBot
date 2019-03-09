using LlamaBot.Classes;
using LlamaBot.Services;
using System;
using System.Collections.Generic;
using System.Text;
using TournaBot.Classes;

namespace TournaBot.Modules
{
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;
    using Newtonsoft.Json;
    using System.Linq;
    using System.Threading.Tasks;
    using static Cache;
    using static DiscordExtensions;
    using static JsonExtensions;
    using static LeagueExtensions;
    using static SUCaptainCommander;

    class preset : ModuleBase<SocketCommandContext>
    {

        readonly SettingsHandlerService settings;
        private SocketGuildUser user;
        private Dictionary<ulong, Dictionary<ulong, Cache.GameList>> gameList;
        private List<GamePreset> gamePresent;

        public preset(SettingsHandlerService _settings, Cache _cache)
        {
            settings = _settings;
            gameList = _cache.gameList;
            gamePresent = _cache.gamePresent;
        }

        protected override void BeforeExecute(CommandInfo command)
        {
            if (Context.Guild != null)
            {
                user = Context.User as SocketGuildUser;
            }
        }

        [Command("create preset")]
        [RequireContext(ContextType.Guild)]
        private async Task createPresent([Remainder]string RemainingText)
        {
            if (user.Roles.Any(x => x.Name == Cache.ModRole || user.GuildPermissions.Administrator))
            {
                if (!gamePresent.Any(x => x.Name.ToLower() == RemainingText.ToLower()))
                {
                    var GamePresent = new Cache.GamePreset
                    {
                        Creator = Context.User.Id,
                        Description = "Join us for competitive scrims and practice your team's tactics using tournament settings!\n\n 📅 **Tuesday, May 15** \n⏰ **7PM AEST / 9PM NZST**\n\n",
                        Name = RemainingText,
                    };
                    gamePresent.Add(GamePresent);
                    var _GamePreset = gamePresent.First(x => x.Name == GamePresent.Name);
                    await CreatePresetEmbed(_GamePreset, Context);
                }
                else
                {
                    await BuildEmbed("⛔ Allready a preset with that name", Context.Channel as SocketTextChannel);
                }
            }
        }

    }
}
