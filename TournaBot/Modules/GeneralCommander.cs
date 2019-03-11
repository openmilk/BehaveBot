using Discord.Commands;
using Discord.WebSocket;

using BehaveBot.Classes;
using BehaveBot.Services;


namespace BehaveBot.Modules
{
    using static DiscordExtensions;


    public class GeneralCommander : ModuleBase<SocketCommandContext>
    {
        readonly SettingsHandlerService settings;
        private SocketGuildUser user;

        public GeneralCommander(SettingsHandlerService _settings)
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
