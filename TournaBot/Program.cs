        using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

using Discord;
using Discord.Commands;
using Discord.WebSocket;

using LlamaBot.Services;
using LlamaBot.Classes;
using LlamaBot.Modules;

namespace LlamaBot
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
            await client.LoginAsync(TokenType.Bot, services.GetService<SettingsHandlerService>().discord.botToken);
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
                .AddSingleton<SUCommander>()
                // Add additional services here...
                .BuildServiceProvider();
        }
    }
}
