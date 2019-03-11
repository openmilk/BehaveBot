using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Timers;

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
        }

        public List<string> defaultBadWords { get; set; } = new List<string>();
        public List<string> defaultSafeWords { get; set; } = new List<string>();
        public List<string> DiscordInviteLinks { get; set; } = new List<string>();

        public Dictionary<ulong, DiscordSetting> customDiscordSettings { get; set; } = new Dictionary<ulong, DiscordSetting>();

        public class DiscordSetting
        {
            public ulong DiscordID { get; set; } = 0;// used so i dont have to do conversion on bot boot for every server

            public string customPrefix { get; set; } = "";
            public CustomReplies customReplies { get; set; } = new CustomReplies();

            public List<bool?> notAllowedMessageTypes { get; set; } = new List<bool?>(); //swearing, links, DiscordLinks

            public List<ulong> BypassRoles { get; set; } = new List<ulong>();


            public Dictionary<List<ulong>, OverideChannmels> overrideChannelsCats { get; set; } = new Dictionary<List<ulong>, OverideChannmels>();

            public class OverideChannmels
            {
                public List<bool?> allowedMessageTypes { get; set; } = new List<bool?> { true, true, true }; //swearing, links, DiscordLinks// null means follow defualt
                public string overideCatName { get; set; } = "";
            }

            public class CustomReplies
            {
                public string NoSwearingMessage { get; set; } = "No swearing Thank-you";
                public string NoLinksMessage { get; set; } = "Links Dont Belong Here";
                public string NoDiscordLinksMessage { get; set; } = "Discord Links Dont Belong Here";
            }
        }    
    }



    public class SettingsHandlerService
    {
        public DiscordSettings discord = new DiscordSettings(); //sets it so it is not null
        private Timer aTimer;

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
                    var customDiscordSettings = JsonConvert.DeserializeObject<DiscordSettings.DiscordSetting>(loadedString);

                    while (customDiscordSettings.notAllowedMessageTypes.Count < 3) //load settings and update them if need be
                        customDiscordSettings.notAllowedMessageTypes.Add(false);

                    foreach(var b in customDiscordSettings.overrideChannelsCats)
                        while (b.Value.allowedMessageTypes.Count < 3) //load settings and update them if need be
                            b.Value.allowedMessageTypes.Add(null);


                    Console.WriteLine(customDiscordSettings.notAllowedMessageTypes.Count);
                    //might be an issue with discord ids allready existing and crashing the bot, key word might
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
                var jsonData = JsonConvert.SerializeObject(a.Value, Formatting.Indented);

                try//try as saving a file sometimes can cause issues, never found the issue so i wraped it in a try loop
                {
                    System.IO.File.WriteAllText(guildsDirectory + "\\" + a.Key + ".json", jsonData);
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