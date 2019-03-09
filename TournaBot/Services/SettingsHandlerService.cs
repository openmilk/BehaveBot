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
using System.Runtime.Serialization;

using LlamaBot.Classes;
using System.IO;
using Discord.Rest;
using LlamaBot.Modules;

namespace LlamaBot.Services
{
    public class DiscordSettings
    {
        public string botToken { get; set; } = "";
        public bool allowBotTagPrefix { get; set; } = false;
        public List<string> prefixes { get; set; } = new List<string>();
    }

    public class SettingsHandlerService
    {
        public DiscordSettings discord;

        public SettingsHandlerService()
        {
            ReloadSettings();
        }

        private void LoadSettings()
        {
            if (System.IO.File.Exists(Environment.CurrentDirectory + "/globalSettings.json"))
            {
                var loadedString = System.IO.File.ReadAllText(Environment.CurrentDirectory + "/globalSettings.json");
                discord = JsonConvert.DeserializeObject<DiscordSettings>(loadedString);
            }
        }

        public void ReloadSettings()
        {
            LoadSettings();
        }
    }
}