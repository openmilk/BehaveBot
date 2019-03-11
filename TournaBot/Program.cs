using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using BehaveBot.Services;
using BehaveBot.Classes;
using BehaveBot.Modules;

namespace BehaveBot
{
    public class Program : ModuleBase<SocketCommandContext>
    {
        static void Main(string[] args)
        => new Program().MainAsync().GetAwaiter().GetResult();

        private DiscordSocketClient client;
        public async Task MainAsync()
        {
            Console.WriteLine("");
            Console.WriteLine(("█▀▀█ █▀▀█ ▒█▀▀█ █▀▀█ ▀▀█▀▀ ").CenterString());
            Console.WriteLine(("█▄▄█ █░░█ ▒█▀▀▄ █░░█ ░░█░░ ").CenterString());
            Console.WriteLine(("▀░░▀ ▀▀▀▀ ▒█▄▄█ ▀▀▀▀ ░░▀░░ ").CenterString());
            Console.WriteLine("");
            client = new DiscordSocketClient();
            var services = ConfigureServices();
            await services.GetRequiredService<CommandHandlerService>().InitializeAsync();
            services.GetRequiredService<SettingsHandlerService>();
            await client.LoginAsync(TokenType.Bot, services.GetService<SettingsHandlerService>().discord.defaultDiscordSettings.botToken);
            await client.StartAsync();
            await Task.Delay(-1);
        }

        private IServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                // Base
                .AddSingleton(client)
                .AddSingleton<CommandService>()
                .AddSingleton<SettingsHandlerService>()
                .AddSingleton<CommandHandlerService>()
                .AddSingleton<GeneralCommander>()
                .AddSingleton<MessageManager>()
                // Add additional services here...
                .BuildServiceProvider();
        }
    }
}
