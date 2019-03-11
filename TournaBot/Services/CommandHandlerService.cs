using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BehaveBot.Services
{
    using static MessageManager;

    public class CommandHandlerService
    {
        public DiscordSocketClient discord;
        private CommandService commands;
        private IServiceProvider provider;
        private SettingsHandlerService settings;
        private Dictionary<ulong, Dictionary<ulong, channelMesages>> messageHandler;

        public List<string> DiscordInvites = new List<string> { "/discord.gg/", };

        public async Task InitializeAsync()
        {
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), provider);
        }

        public CommandHandlerService(IServiceProvider _provider, DiscordSocketClient _discord, CommandService _commands, SettingsHandlerService _settings, MessageManager _messageManager)
        {
            discord = _discord;
            commands = _commands;
            provider = _provider;
            settings = _settings;
            messageHandler = _messageManager.MessageHandler;

            discord.MessageReceived += HandleCommandAsync;

            discord.SetGameAsync("");
        }

        private async Task HandleCommandAsync(SocketMessage socketMessage)
        {
            if (socketMessage.Author.IsBot)
                return;

            var DefaultSettings = settings.discord.defaultDiscordSettings;//get defualt discord settings

            var msg = socketMessage as SocketUserMessage;
            if (msg == null)         
                return;
            
            var context = new SocketCommandContext(discord, msg);
            int argPos = 0;
            var botPrefix = discord.CurrentUser as SocketSelfUser;

            bool canUseBotPrefix = DefaultSettings.allowBotTagPrefix;
            //bool canUseBotPrefix = settings.discord.allowBotTagPrefix;
            if (msg.Channel is IDMChannel)//Dms of the user
            {

            }
            else if (msg.Channel is SocketGuildChannel chnl)//guild channel
            {
                var guildsettings = new DiscordSettings.DiscordSetting();

                if (!settings.discord.customDiscordSettings.TryGetValue(chnl.Guild.Id, out guildsettings))//check for guild settings
                {//if failed to find settings add creat them
                    guildsettings = new DiscordSettings.DiscordSetting { DiscordID = chnl.Guild.Id, notAllowedMessageTypes = new List<bool?> { false,false,false } };//  need to make it so list does not need to be declared like that

                    settings.discord.customDiscordSettings.Add(chnl.Guild.Id, guildsettings);
                }

                var bypass = false; //check for a bypass, admins get bypass by default. Roles can be set to bypass as well
                if (msg.Author is SocketGuildUser usr)
                {
                    //bypass = usr.GuildPermissions.Administrator;

                    if (!bypass)
                        bypass = usr.Roles.Any(x => guildsettings.BypassRoles.Any(y => y == x.Id));
                }

                

                if (bypass || MessageIsSafe(msg, chnl.Guild, guildsettings))// checks if the message follows all protocals set
                {
                    var prefix = guildsettings.customPrefix;
                    if (prefix == "")
                        prefix = DefaultSettings.defaultPrefix;

                    if (msg.HasStringPrefix(prefix, ref argPos) || (canUseBotPrefix && msg.HasMentionPrefix(botPrefix, ref argPos)))
                    {
                        await commands.ExecuteAsync(context, argPos, provider);
                    }
                }
                else
                {
                    await msg.DeleteAsync();
                }
            }
        }

        private bool MessageIsSafe(SocketUserMessage uMsg, SocketGuild guild, DiscordSettings.DiscordSetting guildsettings)
        {//uMsg stands for user message
            var safe = true;

            var checkBadSafeWords = guildsettings.notAllowedMessageTypes[0];
            var checkLinksWords = guildsettings.notAllowedMessageTypes[1];
            var checkDiscordWords = guildsettings.notAllowedMessageTypes[2];

            var msg = uMsg.Content;
            if (guildsettings.overrideChannelsCats.Any(x => x.Key.Any(y => y == uMsg.Channel.Id))) //check for overide perms channel
            {
                var perms = guildsettings.overrideChannelsCats.First(x => x.Key.Any(y => y == uMsg.Channel.Id)).Value;
                if (perms.allowedMessageTypes[0] != null)//null means use defualt
                {
                    checkBadSafeWords = perms.allowedMessageTypes[0];
                }

                if (perms.allowedMessageTypes[1] != null)//null means use defauly
                {
                    checkLinksWords = perms.allowedMessageTypes[1];
                }

                if (perms.allowedMessageTypes[2] != null)//null means use default
                {
                    checkDiscordWords = perms.allowedMessageTypes[2];
                }
            }

            //using nullable bools, this means bools have 3 diffrerent stats true, false and null (null in this case means use defualt)
            if (checkBadSafeWords == true)//also means i have to add == true
            {
                if (settings.discord.defaultSafeWords.Any(x => msg.Contains(x)))
                {
                    foreach (var a in settings.discord.defaultSafeWords.Where(x => msg.Contains(x)))
                    {
                        msg = msg.Replace(a, "");
                    }
                }

                if (settings.discord.defaultBadWords.Any(x => msg.Contains(x)))
                {
                    safe = false;
                    addtext(guild, uMsg.Channel as SocketTextChannel, uMsg.Author as SocketGuildUser, guildsettings.customReplies.NoSwearingMessage, messageHandler);
                }
            }

            if (checkDiscordWords == true)
                if (settings.discord.DiscordInviteLinks.Any(x => msg.Contains(x)))
                {
                    safe = false;
                    addtext(guild, uMsg.Channel as SocketTextChannel, uMsg.Author as SocketGuildUser, guildsettings.customReplies.NoDiscordLinksMessage, messageHandler);
                    foreach (var a in settings.discord.DiscordInviteLinks.Where(x => msg.Contains(x)))
                    {
                        msg = msg.Replace(a, "");
                    }
                }

            if (checkLinksWords == true)
                if (msg.Contains("https://"))
                {
                    safe = false;
                    addtext(guild, uMsg.Channel as SocketTextChannel, uMsg.Author as SocketGuildUser, guildsettings.customReplies.NoLinksMessage, messageHandler);
                }


           

            return safe;
        }
    }
}