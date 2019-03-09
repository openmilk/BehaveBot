using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using Newtonsoft.Json;

using TournaBot.Classes;
using LlamaBot.Services;

namespace LlamaBot.Classes
{
    using Discord.Net;
    using System.IO;
    using System.Net;
    using static JsonExtensions;
    public static class DiscordExtensions
    {
        public async static Task TimedReplyAsync(SocketCommandContext context, string message, bool isTTS = false, Embed embed = null, int time = 5)
        {
            await TimedDeleteAsync(await context.Channel.SendMessageAsync(message, isTTS, embed), time);
        }

        public async static Task TimedReplyAsync(SocketCommandContext context, string message, int time = 5, bool isTTS = false, Embed embed = null)
        {
            await TimedDeleteAsync(await context.Channel.SendMessageAsync(message, isTTS, embed), time);
        }

        public async static Task TimedReplyAsync(SocketCommandContext context, string message)
        {
            await TimedDeleteAsync(await context.Channel.SendMessageAsync(message));
        }

        public async static Task TimedReplyAsync(SocketGuildUser user, string message, bool isTTS = false, Embed embed = null, int time = 5)
        {
            await TimedDeleteAsync(await user.SendMessageAsync(message, isTTS, embed), time);
        }

        public async static Task TimedReplyAsync(SocketGuildUser user, string message, int time = 5, bool isTTS = false, Embed embed = null)
        {
            await TimedDeleteAsync(await user.SendMessageAsync(message, isTTS, embed), time);
        }

        public async static Task TimedReplyAsync(SocketGuildUser user, string message)
        {
            await TimedDeleteAsync(await user.SendMessageAsync(message));
        }

        public async static Task TimedDeleteAsync(IMessage message, int time = 5)
        {
            await Task.Run(() => DeleteMessageAsync(message, time * 1000));
        }

        private async static void DeleteMessageAsync(IMessage message, int time = 5)
        {
            await Task.Delay(time);

            var mes = message.Channel.GetMessageAsync(message.Id).IsCompletedSuccessfully;
            if (mes)
                await message.DeleteAsync();
        }

        public static async Task CheckIfFolerExistAndCreate(string directory)
        {

            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
        }

        public static async Task UpdateOrCreateJson(string directory, string file, string jsonData)
        {
            if (System.IO.Directory.Exists(directory))
            {
                System.IO.File.WriteAllText(directory + "/" + file, jsonData);
            }
        }

        public static async Task CheckifJsonExistAndCreate(string directory, string file, string jsonData)
        {
            var doit = true;
            Console.WriteLine(directory);
            await CheckIfFolerExistAndCreate(directory);
            if (System.IO.Directory.GetFiles(directory).Count() != 0)
            {
                Console.WriteLine("qwe");
                foreach (string a in System.IO.Directory.GetFiles(directory))
                {
                    Console.WriteLine(a.Replace("\\", "/") + "()" + directory.Replace("\\", "/") + file);
                    if (a.Replace("\\", "/") == directory.Replace("\\", "/") + file.Replace("\\", "/"))
                    {
                        doit = false;
                    }
                }
            }
            if (doit == true)
            {
                Console.WriteLine("qwe");
                System.IO.File.WriteAllText(directory + file, jsonData);
                Console.WriteLine("qwe");
            }

        }


        public static string UrlImageType(string url)
        {
            var dot = url.LastIndexOf('.');
            return url.Substring(dot);
        }

        public async static Task DownLoadImage(string url, string directory)
        {
            int fCount = Directory.GetFiles(directory + "/images/", "*", SearchOption.AllDirectories).Length;
            var imageType = UrlImageType(url);

            using (WebClient client = new WebClient())
                client.DownloadFileAsync(new Uri(url), directory + "/images/score" + fCount.ToString() + imageType);
        }

        public async static Task BuildEmbed(EmbedBuilder embed, SocketTextChannel Channel)
        {
            await Channel.SendMessageAsync("", false, embed.Build());
        }

        public static async Task<IUserMessage> TrySendDMAsync(this IUser user, string message = null, Embed embed = null)
        {
            try
            {
                return await user.SendMessageAsync(message, embed: embed);
            }
            catch (HttpException e) when (e.HttpCode == HttpStatusCode.Forbidden)
            {
                return null;
            }
        }

    }
}
