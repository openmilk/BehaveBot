using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Timers;
using Discord.WebSocket;
using System.Linq;
using Discord;

namespace BehaveBot.Services
{

    public class MessageManager
    {
        public Dictionary<ulong, Dictionary<ulong, channelMesages>> MessageHandler = new Dictionary<ulong, Dictionary<ulong, channelMesages>>();
        private Timer aTimer;

        //colours
        public List<Color> DiscordColours = new List<Color> { Color.Blue, Color.Gold, Color.Green, Color.Magenta, Color.Orange, Color.Purple, Color.Red };

        public MessageManager()
        {

            aTimer = new System.Timers.Timer(1000); //files updates every 30 seconds
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += SendMessages;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        public class channelMesages
        {
            public SocketTextChannel channel;
            public List<usermessage> messages = new List<usermessage>();

            public class usermessage
            {
                public SocketGuildUser user;
                public string message { get; set; } = "";
            }
            public int ColorID = 0;
        }

        public int NextColor(int i)
        {
            if (DiscordColours.Count <= i + 1)
                i = 0;
            else
                i += 1;

            return i;
        }

        public void SendMessages(object source, ElapsedEventArgs e)
        {
            aTimer.Enabled = false;//makes sure it cant be called while sending messages

            foreach (var a in MessageHandler)
            {
                foreach (var b in a.Value)
                {
                    var channal = b.Value.channel;

                    var embed = new EmbedBuilder();
                    var footerLength = 0;

                    embed.WithFooter(new EmbedFooterBuilder().Text = "Made by Milky#0001");

                    b.Value.ColorID = NextColor(b.Value.ColorID);
                    embed.WithColor(DiscordColours[b.Value.ColorID]);


                    if (b.Value.messages.Count > 1)
                    {
                        foreach (var c in b.Value.messages)
                        {
                            var field = new EmbedFieldBuilder();
                            field.IsInline = true;
                            field.Value = c.user.Mention + "\n" + c.message;
                            field.Name = embed.Fields.Count.ToString();
                            embed.AddField(field);

                            footerLength += c.message.Count();

                            if (embed.Fields.Count >= 25 || footerLength >= 4000)
                            {
                                channal.SendMessageAsync("", false, embed.Build());

                                embed.Fields = new List<EmbedFieldBuilder>();
                                footerLength = 0;
                            }
                        }

                        if (embed.Fields.Count >= 1)
                        {
                            channal.SendMessageAsync("", false, embed.Build());
                        }
                    }
                    else if (b.Value.messages.Count > 0)
                    {
                        var c = b.Value.messages.First();

                        embed.WithDescription(c.message);

                        channal.SendMessageAsync("", false, embed.Build());

                    }

                    b.Value.messages = new List<channelMesages.usermessage>();
                }
            }

            aTimer.Enabled = true;//make sure it restarts
        }

        public static void addtext(SocketGuild guild, SocketTextChannel channel, SocketGuildUser user, string text, Dictionary<ulong, Dictionary<ulong, channelMesages>> MessageHandler)
        {
            if (!MessageHandler.Any(x => x.Key == guild.Id))
            {
                MessageHandler.Add(guild.Id, new Dictionary<ulong, channelMesages>());
            }

            if (!MessageHandler.First(x => x.Key == guild.Id).Value.Any(x => x.Key == channel.Id))
            {
                var _channelmessage = new channelMesages { channel = channel, messages = new List<channelMesages.usermessage>() };
                _channelmessage.messages.Add(new channelMesages.usermessage { message = text, user = user });
                MessageHandler.First(x => x.Key == guild.Id).Value.Add(channel.Id, _channelmessage);
            }
            else
            {
                var a = new channelMesages.usermessage { message = text, user = user };
                MessageHandler.First(x => x.Key == guild.Id).Value.First(x => x.Key == channel.Id).Value.messages.Add(a);
            }
        }

    }
}