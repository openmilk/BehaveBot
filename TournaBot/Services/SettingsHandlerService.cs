using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Timers;
using static BehaveBot.Services.DiscordSettings;
using static BehaveBot.Services.DiscordSettings.CustomDiscordSetting;

namespace BehaveBot.Services
{
    public class DiscordSettings
    {

        public DefualtDiscordSettings defaultDiscordSettings { get; set; } = new DefualtDiscordSettings ();

        public class DefualtDiscordSettings
        {
            public string botToken { get; set; } = "";
            public bool allowBotTagPrefix { get; set; } = true;
            public string defaultPrefix { get; set; } = "!";
            public List<ulong> botDevs { get; set; } = new List<ulong>();
        }

        

        public List<string> defaultBadWords { get; set; } = new List<string>();
        public List<string> defaultSafeWords { get; set; } = new List<string>();
        public List<string> DiscordInviteLinks { get; set; } = new List<string>();

        public Dictionary<ulong, CustomDiscordSetting> customDiscordSettings { get; set; } = new Dictionary<ulong, CustomDiscordSetting>();

        public class CustomDiscordSetting
        {
            public ulong DiscordID { get; set; } = 0;// used so i dont have to do conversion on bot boot for every server

            public string customPrefix { get; set; } = "";
            public CustomReplies customReplies { get; set; } = new CustomReplies();

            public AllowedMessageTypes notAllowedMessageTypes { get; set; } = new AllowedMessageTypes
            {
                NoDiscordLinksMessage = false,
                NoLinks = false,
                NoSwearing = false
            }; //Null by defualt, hence why setting it to false


            public List<ulong> BypassRoles { get; set; } = new List<ulong>();


            public Dictionary<List<ulong>, OverideChannmels> overrideChannelsCats { get; set; } = new Dictionary<List<ulong>, OverideChannmels>();

            public class OverideChannmels
            {
                public AllowedMessageTypes allowedMessageTypes { get; set; } = new AllowedMessageTypes {}; //swearing, links, DiscordLinks// null means follow defualt
                public string overideCatName { get; set; } = "";
            }

            public class CustomReplies
            {
                public string NoSwearingMessage { get; set; } = "No swearing Thank-you";
                public string NoLinksMessage { get; set; } = "Links Dont Belong Here";
                public string NoDiscordLinksMessage { get; set; } = "Discord Links Dont Belong Here";
            }

            public class AllowedMessageTypes
            {
                public bool? NoSwearing { get; set; } = null;
                public bool? NoLinks { get; set; } = null;
                public bool? NoDiscordLinksMessage { get; set; } = null;
            }
        }    
    }



    public class SettingsHandlerService
    {
        public DiscordSettings discord = new DiscordSettings(); //sets it so it is not null
        private Timer aTimer;

        private class cachedGuildSettings
        {
            public CustomDiscordSetting discordSettingsUncached; //does not need to be cached
            public List<CachedDictionary> discordSettingCached;
            //public List<>

            public class CachedDictionary
            {
                public OverideChannmels overideChanels;
                public List<ulong> overrideKey;

            }

            public CustomDiscordSetting UnCache(cachedGuildSettings settings)
            {
                var value = settings.discordSettingsUncached;

                foreach (var b in discordSettingCached)
                {
                    value.overrideChannelsCats.Add(b.overrideKey, b.overideChanels);
                }


                return value;
            }

            public static cachedGuildSettings CacheSettings(CustomDiscordSetting settings)
            {
                var cache = new List<CachedDictionary>();
                foreach (var a in settings.overrideChannelsCats)
                {
                    var _settings = new CachedDictionary
                    {
                        overrideKey = a.Key,
                        overideChanels = a.Value
                    };

                    cache.Add(_settings);
                }

                var value = new cachedGuildSettings { discordSettingsUncached = settings };

                // value.discordSettingsUncached = removeDictionary(value.discordSettingsUncached);


                value.discordSettingCached = cache;
                //value.discordSettingsUncached.overrideChannelsCats = new Dictionary<List<ulong>, OverideChannmels>();

                return value;
            }

            public static string removeBrokenJson(string json)
            {
                if (json.Contains("(Collection)"))
                {
                    var start = json.IndexOf("(Collection)") - 1; //minus one as i need to remove a bracket as well
                    var end = 0; //abit harder code for it will be below

                    {//seperating code
                       
                        var posEnd = json.LastIndexOf("discordSettingCached") - 7;//idk if its the exact number, but it worked
                        var tempJson = json.Substring(0, posEnd);

                        var pos = tempJson.LastIndexOf("overideCatName");

                        Console.Write(pos+ ","+ posEnd);

                        

                        end = tempJson.LastIndexOf("}");

                    }

                    json = json.Substring(0, start) + json.Substring(end);
                }
                return json;
            }
        }

        private string guildsDirectory = Environment.CurrentDirectory + "\\Guilds";

        public SettingsHandlerService()
        {
            CreateFolder(guildsDirectory);

            LoadSettings();

            aTimer = new System.Timers.Timer(30000); //files updates every 30 seconds
            // Hook up the Elapsed event for the timer. 
            aTimer.Elapsed += SaveSettings;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private void LoadSettings()
        {
            if (System.IO.File.Exists(Environment.CurrentDirectory + "\\globalSettings.json"))
            {
                var loadedString = System.IO.File.ReadAllText(Environment.CurrentDirectory + "\\globalSettings.json");
                discord.defaultDiscordSettings = JsonConvert.DeserializeObject<DiscordSettings.DefualtDiscordSettings>(loadedString);
            }

            if (System.IO.File.Exists(Environment.CurrentDirectory + "/defaultSafeWords.json"))
            {
                var loadedString = System.IO.File.ReadAllText(Environment.CurrentDirectory + "/defaultSafeWords.json");
                discord.defaultSafeWords = JsonConvert.DeserializeObject<List<string>>(loadedString);
            }

            if (System.IO.File.Exists(Environment.CurrentDirectory + "/defaultBadWords.json"))
            {
                var loadedString = System.IO.File.ReadAllText(Environment.CurrentDirectory + "/defaultBadWords.json");
                discord.defaultBadWords = JsonConvert.DeserializeObject<List<string>>(loadedString);
            }

            if (System.IO.File.Exists(Environment.CurrentDirectory + "/discordLinks.json"))
            {
                var loadedString = System.IO.File.ReadAllText(Environment.CurrentDirectory + "/discordLinks.json");
                discord.DiscordInviteLinks = JsonConvert.DeserializeObject<List<string>>(loadedString);
            }


            var GuildSettingsList = System.IO.Directory.GetFiles(guildsDirectory);

            foreach(var a in GuildSettingsList)//load all Custom Discord Settings
            {
                if (a.EndsWith(".json"))//make sore the file is a json
                {
                    var loadedString = System.IO.File.ReadAllText(a);
                    var customDiscordSettingsCache = JsonConvert.DeserializeObject<cachedGuildSettings>(loadedString);

                    var customDiscordSettings = customDiscordSettingsCache.UnCache(customDiscordSettingsCache); //dictionary does not go into json that well, so it goes as a list instead

                    //might be an issue with discord ids allready existing and crashing the bot, key word "might" 
                    discord.customDiscordSettings.Add(customDiscordSettings.DiscordID, customDiscordSettings);
                }
            }
        }

        /*public void ReloadSettings() //kinda obsolete with SaveSettings, maybe there will be a use for it in the future?
        {
            LoadSettings();
        }
        */

        public void SaveSettings(object source, ElapsedEventArgs e) //save all the settings
        {
            aTimer.Enabled = false;//turn timer off so it wont repeat while saving
            Console.WriteLine("Saving settings " + DateTime.Now);

            CreateFolder(guildsDirectory); //make sure folder was not deleted during runtime

            foreach (var a in discord.customDiscordSettings)
            {
                // Console.WriteLine("try Cache " + a.GuildsSettings.GuildID);

                var brokenJson = JsonConvert.SerializeObject(cachedGuildSettings.CacheSettings(a.Value), Formatting.Indented);//idk why its broken plzs fix, i got it working anyway (not a good method imo)
                var fixedJson = cachedGuildSettings.removeBrokenJson(brokenJson); //removed broken part before moving the part to a list instead of a dictionary 
                //if you want to fix give me a DM Milky#0001 and ill explain the issue


                try//try as saving a file sometimes can cause issues, never found the issue so i wraped it in a try loop
                {
                    System.IO.File.WriteAllText(guildsDirectory + "\\" + a.Key + ".json", fixedJson);
                }
                catch (Exception)
                {
                    Console.WriteLine("error 1//");
                }
            }

            //save default bad words file
            {//just so i dont have hirachy problems with jsondata
                var jsonData = JsonConvert.SerializeObject(discord.defaultBadWords, Formatting.Indented);

                try//try as saving a file sometimes can cause issues, never found the issue so i wraped it in a try loop
                {
                    System.IO.File.WriteAllText(Environment.CurrentDirectory + "\\defaultBadWords.json", jsonData);
                }
                catch (Exception)
                {
                    Console.WriteLine("error 2//");
                }
            }

            //save default good words file
            {//just so i dont have hirachy problems with jsondata
                var jsonData = JsonConvert.SerializeObject(discord.defaultSafeWords, Formatting.Indented);

                try//try as saving a file sometimes can cause issues, never found the issue so i wraped it in a try loop
                {
                    System.IO.File.WriteAllText(Environment.CurrentDirectory + "\\defaultSafeWords.json", jsonData);
                }
                catch (Exception)
                {
                    Console.WriteLine("error 3//");
                }
            }

            //save Discord Invite Links type file
            {//just so i dont have hirachy problems with jsondata
                var jsonData = JsonConvert.SerializeObject(discord.DiscordInviteLinks, Formatting.Indented);

                try//try as saving a file sometimes can cause issues, never found the issue so i wraped it in a try loop
                {
                    System.IO.File.WriteAllText(Environment.CurrentDirectory + "\\discordLinks.json", jsonData);
                }
                catch (Exception)
                {
                    Console.WriteLine("error 4//");
                }
            }

            //save default settings
            {//just so i dont have hirachy problems with jsondata
                var jsonData = JsonConvert.SerializeObject(discord.defaultDiscordSettings, Formatting.Indented);

                try//try as saving a file sometimes can cause issues, never found the issue so i wraped it in a try loop
                {
                    System.IO.File.WriteAllText(Environment.CurrentDirectory + "\\globalSettings.json", jsonData);
                }
                catch (Exception)
                {
                    Console.WriteLine("error 5//");
                }
            }

            aTimer.Enabled = true;//turn it back on so it can save
        }

        public void CreateFolder(string directory)
        {
            System.IO.Directory.CreateDirectory(directory);
        }
    }
}