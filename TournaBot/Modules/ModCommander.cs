using Discord.Commands;
using Discord.WebSocket;

using BehaveBot.Classes;
using BehaveBot.Services;

using System.Linq;
using System.Collections.Generic;


namespace BehaveBot.Modules
{
    using System;
    using System.Threading.Tasks;
    using static BehaveBot.Services.DiscordSettings.CustomDiscordSetting;
    using static BehaveBot.Services.MessageManager;
    using static DiscordExtensions;
    using static StringExtensions;


    public class ModCommander : ModuleBase<SocketCommandContext>
    {
        private SettingsHandlerService settings;
        private Dictionary<ulong, Dictionary<ulong, channelMesages>> messageHandler;
        private SocketGuildUser user;
        private List<ulong> devs;
        private DiscordSettings.CustomDiscordSetting discordSettings = null;

        public ModCommander(SettingsHandlerService _settings, MessageManager _messageManager)
        {
            settings = _settings;
            messageHandler = _messageManager.MessageHandler;

            devs = _settings.discord.defaultDiscordSettings.botDevs;


        }

        protected override void BeforeExecute(CommandInfo command)
        {
            if (Context.Guild != null)
            {
                user = Context.User as SocketGuildUser;
            }

            if (Context.Guild != null)
            {
                if (settings.discord.customDiscordSettings.Any(x => x.Key == Context.Guild.Id))
                    discordSettings = settings.discord.customDiscordSettings.First(x => x.Key == Context.Guild.Id).Value;
            }
        }

        [Command("set")]
        [Alias("s")]
        private async Task Set([Remainder]string RemainingText)
        {
            var isDev = devs.Any(x => x == user.Id);

            if (RemainingText.ToLower().Trim().StartsWith("reply") && IsAdmin(user, isDev))
            {
                RemainingText = RemoveString(RemainingText, "reply");
                if (RemainingText.ToLower().Trim().StartsWith("links"))
                {
                    RemainingText = RemoveString(RemainingText, "links");
                    discordSettings.customReplies.NoLinksMessage = setString(RemainingText, "Custom No Links Reply");
                }

                else if (RemainingText.ToLower().Trim().StartsWith("discord"))
                {
                    RemainingText = RemoveString(RemainingText, "discord");
                    discordSettings.customReplies.NoDiscordLinksMessage = setString(RemainingText, "Custom No Discord Links Reply");
                }

                else if (RemainingText.ToLower().Trim().StartsWith("swearing"))
                {
                    RemainingText = RemoveString(RemainingText, "swearing");
                    discordSettings.customReplies.NoSwearingMessage = setString(RemainingText, "Custom No Swearing Reply");
                }

                else
                    addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "You need to add either Swearing, discord or links", messageHandler);
            }
            else if (RemainingText.ToLower().Trim().StartsWith("prefix") && IsAdmin(user, isDev))
            {
                RemainingText = RemoveString(RemainingText, "prefix");

                discordSettings.customPrefix = RemainingText;
                addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "Set Custom Prefix to " + RemainingText, messageHandler);
            }
            else if (RemainingText.ToLower().Trim().StartsWith("perms") && IsAdmin(user, isDev))
            {
                RemainingText = RemoveString(RemainingText, "perms");

                //custom perm settings area
                if (discordSettings.overrideChannelsCats.Any(x => RemainingText.ToLower().Trim().StartsWith(x.Value.overideCatName.ToLower())))
                {
                    var overide = discordSettings.overrideChannelsCats.First(x => RemainingText.ToLower().Trim().StartsWith(x.Value.overideCatName.ToLower()));
                    RemainingText = RemoveString(RemainingText, overide.Value.overideCatName);

                    overide.Value.allowedMessageTypes = SetAllowedMessagesTypes(discordSettings.notAllowedMessageTypes, RemainingText, overide.Value.overideCatName + " allowed messages");
                }

                else if (RemainingText.ToLower().Trim().StartsWith("links"))
                {
                    RemainingText = RemoveString(RemainingText, "links");
                    discordSettings.notAllowedMessageTypes.NoLinks = GetBoolFromString(discordSettings.notAllowedMessageTypes.NoLinks, RemainingText, "Default Allowed Links").Item1;
                }

                else if (RemainingText.ToLower().Trim().StartsWith("discord"))
                {
                    RemainingText = RemoveString(RemainingText, "discord");
                    discordSettings.notAllowedMessageTypes.NoDiscordLinksMessage = GetBoolFromString(discordSettings.notAllowedMessageTypes.NoDiscordLinksMessage, RemainingText, "Default Allowed discord links").Item1;
                }

                else if (RemainingText.ToLower().Trim().StartsWith("swearing"))
                {
                    RemainingText = RemoveString(RemainingText, "swearing");
                    discordSettings.notAllowedMessageTypes.NoSwearing = GetBoolFromString(discordSettings.notAllowedMessageTypes.NoSwearing, RemainingText, "Default Allowed swearing").Item1;

                }
                else
                {
                    discordSettings.notAllowedMessageTypes = SetAllowedMessagesTypes(discordSettings.notAllowedMessageTypes, RemainingText, "Default allowed messages");
                }
            }

            else if (IsAdmin(user, isDev))
                if (discordSettings.overrideChannelsCats.Any(x => RemainingText.ToLower().Trim().StartsWith(x.Value.overideCatName.ToLower())))
                {
                    var overide = discordSettings.overrideChannelsCats.First(x => RemainingText.ToLower().Trim().StartsWith(x.Value.overideCatName.ToLower()));
                    //RemainingText = RemoveString(RemainingText, overide.Value.overideCatName);


                    discordSettings.overrideChannelsCats = UpdateDicKey(discordSettings.overrideChannelsCats, Context.Channel.Id, RemainingText, "Overide channels");
                    //need to set up bypass roles
                }
            else
                    addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "Plzs make sure you are typing correctly", messageHandler);

        }

        [Command("remove")]
        [Alias("r")]
        //[Alias("r")] see if i can combine reset and remove
        private async Task Remove([Remainder]string RemainingText)
        {
            var isDev = devs.Any(x => x == user.Id);

            if (RemainingText.ToLower().Trim().StartsWith("overide") && IsAdmin(user, isDev))
            {
                RemainingText = RemoveString(RemainingText, "overide");
                discordSettings.overrideChannelsCats = RemoveOverideChannel(discordSettings.overrideChannelsCats, RemainingText);
            }
            else if (RemainingText.ToLower().Trim().StartsWith("bypass") && IsAdmin(user, isDev))
            {
                RemainingText = RemoveString(RemainingText, "bypass");

                if (Context.Message.MentionedUsers.Count != 0)
                    foreach (var a in Context.Message.MentionedUsers)
                    {
                        discordSettings.BypassRoles = RemoveUlongFromList(discordSettings.BypassRoles, a.Id, "Bypass roles");
                    }
                else
                    addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "You need to mention users", messageHandler);

            }

            else if (RemainingText.ToLower().Trim().StartsWith("dev") && isDev)
            {
                RemainingText = RemoveString(RemainingText, "dev");

                if (Context.Message.MentionedUsers.Count != 0)
                {
                    foreach (var a in Context.Message.MentionedUsers)
                        devs = RemoveUlongFromList(devs, a.Id, "devs");
                }
                else
                    addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "No Users were Tagged", messageHandler);
            }

            else if (RemainingText.ToLower().Trim().StartsWith("badword") && isDev)
            {
                RemainingText = RemoveString(RemainingText, "badword");
                settings.discord.defaultBadWords = RemoveStringFromList(settings.discord.defaultBadWords, RemainingText, "bad words");
            }

            else if (RemainingText.ToLower().Trim().StartsWith("goodword") && isDev)
            {
                RemainingText = RemoveString(RemainingText, "goodword");

                settings.discord.defaultSafeWords = RemoveStringFromList(settings.discord.defaultSafeWords, RemainingText, "good words");
            }

            else if (RemainingText.ToLower().Trim().StartsWith("links") && isDev)
            {
                RemainingText = RemoveString(RemainingText, "links");

                settings.discord.defaultSafeWords = RemoveStringFromList(settings.discord.defaultSafeWords, RemainingText, "bad links");
            }


            else if (IsAdmin(user, isDev))
                if (discordSettings.overrideChannelsCats.Any(x => RemainingText.ToLower().Trim().StartsWith(x.Value.overideCatName.ToLower())))
                {
                    var overide = discordSettings.overrideChannelsCats.First(x => RemainingText.ToLower().Trim().StartsWith(x.Value.overideCatName.ToLower()));
                    discordSettings.overrideChannelsCats.Remove(overide.Key);

                    addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "Removed override type", messageHandler);
                }
        }

        [Command("add")]
        [Alias("a")]
        private async Task Add([Remainder]string RemainingText)
        {
            var isDev = devs.Any(x => x == user.Id);

            if (RemainingText.ToLower().Trim().StartsWith("dev") && isDev)
            {
                RemainingText = RemoveString(RemainingText, "dev");

                if (Context.Message.MentionedUsers.Count != 0)
                {
                    foreach (var a in Context.Message.MentionedUsers)
                        devs = AddUlongFromList(devs, a.Id, "devs");
                }
                else
                    addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "No Users were Tagged", messageHandler);
            }

            else if (RemainingText.ToLower().Trim().StartsWith("badword") && isDev)
            {
                RemainingText = RemoveString(RemainingText, "badword");
                settings.discord.defaultBadWords = AddStringFromList(settings.discord.defaultBadWords, RemainingText, "bad words");
            }

            else if (RemainingText.ToLower().Trim().StartsWith("goodword") && isDev)
            {
                RemainingText = RemoveString(RemainingText, "goodword");
                settings.discord.defaultSafeWords = AddStringFromList(settings.discord.defaultSafeWords, RemainingText, "good words");
            }

            else if (RemainingText.ToLower().Trim().StartsWith("links") && isDev)
            {
                RemainingText = RemoveString(RemainingText, "links");
                settings.discord.defaultSafeWords = AddStringFromList(settings.discord.defaultSafeWords, RemainingText, "bad links");
            }

            else if (RemainingText.ToLower().Trim().StartsWith("bypass") && IsAdmin(user, isDev))
            {
                RemainingText = RemoveString(RemainingText, "bypass");

                if (Context.Message.MentionedRoles.Count != 0)
                    foreach (var a in Context.Message.MentionedRoles)
                    {
                        discordSettings.BypassRoles = AddUlongFromList(discordSettings.BypassRoles, a.Id, "Bypass roles", "@&");
                    }
                else
                    addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "You need to mention Roles", messageHandler);

            }
        }

        [Command("create")]
        [Alias("c")]
        private async Task Create([Remainder]string RemainingText)
        {
            var isDev = devs.Any(x => x == user.Id);

            if (RemainingText.ToLower().Trim().StartsWith("override"))
            {
                RemainingText = RemoveString(RemainingText, "override");
                discordSettings.overrideChannelsCats = AddOverideChannel(discordSettings.overrideChannelsCats, RemainingText);
            }
        }

        public List<string> RemoveStringFromList(List<string> list, string text, string type)
        {

            if (list.Any(x => x.ToLower() == text.ToLower()))
            {
                list.Remove(text);
                addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "Removed " + text + " from " + type, messageHandler);
            }
            else
                addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "No " + text + " in " + type, messageHandler);


            return list;
        }

        public List<string> AddStringFromList(List<string> list, string text, string type)
        {

            if (!list.Any(x => x.ToLower() == text.ToLower()))
            {
                list.Add(text);
                addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "Added " + text + " to " + type, messageHandler);
            }
            else
                addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "Allready " + text + " in " + type, messageHandler);


            return list;
        }

        public List<ulong> RemoveUlongFromList(List<ulong> list, ulong newUlong, string type, string MentionType = "@")
        {

            if (list.Any(x => x == newUlong))
            {
                list.Remove(newUlong);
                addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "Removed <" + MentionType + newUlong + " from " + type, messageHandler);
            }
            else
                addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "No <" + MentionType + newUlong + " in " + type, messageHandler);


            return list;
        }

        public List<ulong> AddUlongFromList(List<ulong> list, ulong newUlong, string type, string MentionType = "@")
        {

            if (!list.Any(x => x == newUlong))
            {
                list.Add(newUlong);
                addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "Added <" + MentionType + newUlong + "> to " + type, messageHandler);
            }
            else
                addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "Allready <@" + MentionType + newUlong + "> in " + type, messageHandler);


            return list;
        }

        public Dictionary<List<ulong>, OverideChannmels> AddOverideChannel(Dictionary<List<ulong>, OverideChannmels> overideList, string newOveride)
        {
            if (!overideList.Any(x => x.Value.overideCatName.ToLower() == newOveride.ToLower()))
            {
                overideList.Add(new List<ulong> {0}, new OverideChannmels { overideCatName = newOveride});
                addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "Added " + newOveride + " in Overide Channels", messageHandler);
            }
            else
                addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "Allready " + newOveride + " in Overide Channels", messageHandler);



            return overideList;
        }

        public Dictionary<List<ulong>, OverideChannmels> RemoveOverideChannel(Dictionary<List<ulong>, OverideChannmels> overideList, string newOveride)
        {
            if (overideList.Any(x => x.Value.overideCatName.ToLower() == newOveride.ToLower()))
            {
                overideList.Remove(overideList.First(x => x.Value.overideCatName.ToLower() == newOveride.ToLower()).Key);
                addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "Removed " + newOveride + " in Overide Channels", messageHandler);
            }
            else
                addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "" + newOveride + " is not an Overide Channel", messageHandler);



            return overideList;
        }

        public AllowedMessageTypes SetAllowedMessagesTypes(AllowedMessageTypes allowedMessagetypes, string text, string type)
        {
            var swearing = GetBoolFromString(allowedMessagetypes.NoSwearing, text);
            text = swearing.Item2;

            var discordLinks = GetBoolFromString(allowedMessagetypes.NoDiscordLinksMessage, text);
            text = discordLinks.Item2;

            var links = GetBoolFromString(allowedMessagetypes.NoLinks, text);
            text = links.Item2;

            if (swearing.Item3)
            {
                allowedMessagetypes.NoSwearing = swearing.Item1;

                if (discordLinks.Item3)
                    allowedMessagetypes.NoDiscordLinksMessage = discordLinks.Item1;

                if (links.Item3)
                    allowedMessagetypes.NoLinks = links.Item1;


                addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "**Set " + type + " to:**" +
                    "\nNo swearing: " + allowedMessagetypes.NoSwearing.ToString() +
                    "\nNo Discord Links: " + allowedMessagetypes.NoDiscordLinksMessage.ToString() +
                    "\nNo Links: " + allowedMessagetypes.NoLinks.ToString(),
                    messageHandler);
            }
            else
                addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "Failed to set " + type + ". Make sure you are using either true, false or default", messageHandler);


            return allowedMessagetypes;
        }

        public Tuple<bool?, string, bool> GetBoolFromString(bool? _bool, string text, string type = "Dont Reply")//Value, text, ifFailed
        {
            var success = false;

            var dictionary = new Dictionary<string, bool?>
            {
                {"true", true },
                {"false", false },
                {"null", true },
                {"defualt", null },
                {"default", null },
            };


            //Console.WriteLine("text:"+text); //verifying code
            if (dictionary.Any(x => text.ToLower().StartsWith(x.Key)))
            {
                success = true;

                var boolType = dictionary.First(x => text.ToLower().StartsWith(x.Key));

                _bool = boolType.Value;

                text = RemoveString(text, boolType.Key);
            }

            if (type != "Dont Reply")
            {
                if (success)
                    addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "Set " + type + " to " + _bool.ToString(), messageHandler);
                else
                    addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "Failed to set " + type + ". Make sure you are using either true, false or default", messageHandler);
            }

            return new Tuple<bool?, string, bool>(_bool, text, success);
        }

        public string setString(string text, string type)
        {
            addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "Set " + type + " to:\n" + text, messageHandler);

            return text;
        }

        public Dictionary<List<ulong>, OverideChannmels> UpdateDicKey(Dictionary<List<ulong>, OverideChannmels> dic, ulong newKey, string name, string type)
        {
            if (dic.Any(x => x.Value.overideCatName.ToLower() == name.ToLower()))
            {
                var value = dic.First(x => x.Value.overideCatName.ToLower() == name.ToLower());

                if (!value.Key.Contains(newKey))
                {
                    var newKeys = value.Key;
                    newKeys.Add(newKey);


                    foreach (var a in dic.Where(x => x.Key.Contains(newKey) && x.Key != newKeys))
                    {
                        dic = RemoveDicKeys(dic, newKey, a.Key);
                    }

                    addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "Set <#" + newKey + "> as " + name + " in " + type, messageHandler);
                }
                else
                    addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "<#" + newKey + "> is allready set as " + name, messageHandler);

            }
            else
                addtext(Context.Guild, Context.Message.Channel as SocketTextChannel, user, "Cannot find " + name + " in " + type, messageHandler);


            return dic;
        }

        public Dictionary<List<ulong>, OverideChannmels> RemoveDicKeys(Dictionary<List<ulong>, OverideChannmels> dic, ulong key, List<ulong> currentKeys)
        {
            if (dic.Any(x => x.Key == currentKeys && x.Key.Contains(key))) 
            {
                var value = dic.First(x => x.Key == currentKeys);
                var newKeys = value.Key;
                newKeys.Remove(key);
            }

            return dic;
        }


    }
}
