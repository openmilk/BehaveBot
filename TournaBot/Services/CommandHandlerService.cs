using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json;

using LlamaBot.Classes;
using Discord.Rest;
using LlamaBot.Services;

namespace LlamaBot.Services
{
    
    
    public class CommandHandlerService
    {
        public DiscordSocketClient discord;
        private CommandService commands;
        private IServiceProvider provider;
        private SettingsHandlerService settings;

        public List<string> DiscordInvites =  new List<string> { "/discord.gg/",  };

        public async Task InitializeAsync()
        {
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), provider);
        }

        public CommandHandlerService(IServiceProvider _provider, DiscordSocketClient _discord, CommandService _commands, SettingsHandlerService _settings)
        {
            discord = _discord;
            commands = _commands;
            provider = _provider;
            settings = _settings;

            discord.MessageReceived += HandleCommandAsync;

            discord.SetGameAsync("");
        }

        private async Task HandleCommandAsync(SocketMessage socketMessage)
        {
            var message = socketMessage as SocketUserMessage;
            if (message == null)
            {
                return;
            }
            var context = new SocketCommandContext(discord, message);
            int argPos = 0;
            var botPrefix = discord.CurrentUser as SocketSelfUser;
            bool canUseBotPrefix = settings.discord.allowBotTagPrefix;
            //bool canUseBotPrefix = settings.discord.allowBotTagPrefix;
            if (message.Channel is IDMChannel)//Dms of the user
            {

            }
            else
            {
                foreach (var prefix in settings.discord.prefixes)
                {
                    if (message.HasCharPrefix(prefix[0], ref argPos) || (canUseBotPrefix && message.HasMentionPrefix(botPrefix, ref argPos)))
                    {
                        await commands.ExecuteAsync(context, argPos, provider);
                        break;
                    }
                }
            }
        }
    }
}