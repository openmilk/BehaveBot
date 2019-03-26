using System.Threading.Tasks;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace BehaveBot.Classes
{
    using Discord.Net;
    using System.IO;
    using System.Net;
    public static class DiscordExtensions
    {
        public static string UrlImageType(string url)//used to see the filetype
        {
            var dot = url.LastIndexOf('.');
            return url.Substring(dot);
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

        public static bool IsAdmin(SocketGuildUser user, bool isDev)
        {
            var isAdmin = false;

            if (isDev)
                isAdmin = true;

            else if (user.GuildPermissions.Administrator == true)
                isAdmin = true;

            return isAdmin;
        }



    }
}
