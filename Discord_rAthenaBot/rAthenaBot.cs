using Discord;
using Discord.Commands;
using Discord.Net;
using Discord_rAthenaBot.Commands.Script;
using Discord_rAthenaBot.Const;
using Discord_rAthenaBot.DivinePride;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Xml;

namespace Discord_rAthenaBot
{
    partial class rAthenaBot
    {
        public static rAthenaBot instance { get; private set; }
        public Configuration Config { get; private set; }
        List<ScriptCommand> scriptCommands;
        DiscordClient discord;
        CommandService commands;
        RSSConfiguration RSSConfig;
        DivinePrideService DpService;

        System.Timers.Timer TimerRSS = new System.Timers.Timer();
        long Tick_RSS_Support = DateTime.Now.Ticks;
        long Tick_RSS_Server = DateTime.Now.Ticks;

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[Log] " + e.Message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public void FatalError(string Message, int ExitCode = 1)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(Message);
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            Environment.Exit(ExitCode);
        }

        public rAthenaBot()
        {
            Console.WriteLine("Reading configuration files.");
            #region Process configurations
            string _Result = ProcessConfig();
            if (!String.IsNullOrEmpty(_Result))
            {
                FatalError("Configuration file " + _Result + " is missing.", 0x6E); // ERROR_OPEN_FAILED                
            }
            #endregion
            else {
                Console.WriteLine("Done reading configuration files.");
                Console.Title = Config.ConsoleTitle;

                DpService = new DivinePrideService {
                    BaseUrl = Config.DivinePrideBaseUrl,
                    ApiKey = Config.DivinePrideApiKey
                };

                discord = new DiscordClient(x =>
                {
                    x.LogLevel = LogSeverity.Info;
                    x.LogHandler = Log;
                });

                #region Discord Events - UserJoined
                discord.UserJoined += async (s, e) =>
                {
                    if (e.Server.Name.Equals(Config.ServerName))
                    {
                        Channel channel = e.Server.FindChannels(Config.Channels["General"]).FirstOrDefault();
                        if (channel != null)
                        {
                            await channel.SendMessage("Hello " + e.User.Mention + ", welcome to " + Config.ServerName + " Discord." + Environment.NewLine +
                                "Kindly read the " + e.Server.FindChannels(Config.Channels["Rules"], ChannelType.Text).FirstOrDefault().Mention + " before you start posting. Thank you.");
                        }
                    }
                };
                #endregion

                #region Register Commands
                discord.UsingCommands(x =>
                {
                    x.PrefixChar = Config.PrefixChar.ElementAt(0);
                    x.HelpMode = HelpMode.Public;
                    x.AllowMentionPrefix = Config.AllowMentionPrefix;
                });

                commands = discord.GetService<CommandService>();
                #endregion

                #region List of Commands

                #region Command - stats
                commands.CreateCommand("stats")
                    .Description("This command will return the stats of rAthena Discord. ```Usage: /stats```")
                    .Alias(new string[] { "stat", "status" })
                    .Parameter("args", ParameterType.Unparsed)
                    .Do(async (e) =>
                    {
                        string msg = "**rAthena Discord Stat**:" + System.Environment.NewLine
                                + "```"
                                + "Region - " + e.Server.Region.Name + System.Environment.NewLine
                                + System.Environment.NewLine
                                + "Total Users - " + e.Channel.Server.Users.Where(x => x.IsBot == false).Count() + System.Environment.NewLine
                                + System.Environment.NewLine
                                + "Total Bots - " + e.Channel.Server.Users.Where(x => x.IsBot == true).Count() + System.Environment.NewLine
                                + "Total Online Bots - " + e.Channel.Server.Users.Where(x => x.IsBot == true && x.Status != UserStatus.Offline).Count() + System.Environment.NewLine
                                + "Total Offline Bots - " + e.Channel.Server.Users.Where(x => x.IsBot == true && x.Status == UserStatus.Offline).Count() + System.Environment.NewLine
                                + System.Environment.NewLine
                                + "Total Channels - " + e.Server.ChannelCount + System.Environment.NewLine
                                + "Total Text Channels - " + e.Server.TextChannels.Count() + System.Environment.NewLine
                                + "Total Voice Channels - " + e.Server.VoiceChannels.Count() + System.Environment.NewLine
                                + "```";
                        await e.Channel.SendMessage(msg);
                    });
                #endregion

                #region Command - Search
                commands.CreateCommand("search")
                    .Description("This command will search for contents at rAthena forum. ```Usage: /search <content>```")
                    .Alias(new string[] { "find", "google" })
                    .Parameter("args", ParameterType.Multiple)
                    .Do(async (e) =>
                    {
                        if (e.Args.Length <= 0)
                        {
                            await e.Channel.SendMessage(e.Command.Description);
                        }
                        else
                        {
                            string query = String.Join("%20", e.Args.ToList());
                            string url = "https://rathena.org/board/search/?&q=" + query;
                            await e.Channel.SendMessage("Search result: `" + String.Join(" ", e.Args.ToList()) + "`" + System.Environment.NewLine  + url);
                            Process.Start(url);
                        }
                    });
                #endregion

                #region Command - Purge
                commands.CreateCommand("purge")
                    .Hide()
                    .Description("This command will remove some Chat Log. ```Usage: /purge <amount> <@mention>```")
                    .Alias(new string[] { "remove", "clean", "delete" })
                    .Parameter("arg", ParameterType.Multiple)
                    .Do(async (e) =>
                    {
                        if (e.User.Roles.Any(x => x.Permissions.ManageMessages == true))
                        {
                            if (e.Args.Length < 1)
                            {
                                await e.Channel.SendMessage(e.Command.Description);
                            }
                            else
                            {
                                int amount = Convert.ToInt16(e.Args[0]) + 1;
                                string author = string.Empty;
                                if (e.Args.Length > 1)
                                {
                                    author = String.Join(" ", e.Args.Skip(1).ToList());
                                }
                                if (amount > 0)
                                {
                                    Message[] msg;
                                    msg = await e.Channel.DownloadMessages(amount);
                                    if (!String.IsNullOrEmpty(author))
                                    {
                                        msg = msg.Where(x => x.User.Mention.Equals(author)).ToArray();
                                    }
                                    await e.Channel.DeleteMessages(msg);
                                }
                            }
                        }
                        else
                        {
                            await e.Channel.SendIsTyping();
                            await e.Channel.SendMessage("Invalid Command");
                        }
                    });
                #endregion

                #region Command - WhoIs
                commands.CreateCommand("whois")
                    .Description("This command will return information of user. ```Usage: /whois <@mention>```")
                    .Alias(new string[] { "who", "profile", "check" })
                    .Parameter("arg", ParameterType.Multiple)
                    .Do(async (e) =>
                    {
                        string username = String.Empty;
                        User user = null;
                        if (e.Args.Length == 0)
                        {
                            user = e.Channel.FindUsers(e.User.Name, true).FirstOrDefault();
                        }
                        else
                        {
                            username = String.Join(" ", e.Args).Trim();
                            if (username.Contains("@"))
                            {
                                user = e.Channel.Users.FirstOrDefault(x => x.Mention == username);
                            }
                            else
                            {
                                user = e.Channel.FindUsers(username).FirstOrDefault();
                            }
                        }

                        await e.Channel.SendIsTyping();
                        if (user == null)
                        {
                            await e.Channel.SendMessage("User : '" + String.Join(" ", e.Args) + "' not found.");
                        }
                        else
                        {
                            string msg = "```"
                                        + "Name: " + user.Name + " (" + user.ToString() + ")" + System.Environment.NewLine
                                        + "Role: " + String.Join(",", user.Roles.Select(x => x.Name)) + System.Environment.NewLine
                                        + "Joined Since: " + user.JoinedAt + System.Environment.NewLine
                                        + "Last Activity: " + user.LastActivityAt + System.Environment.NewLine
                                        + "Last Online At: " + user.LastOnlineAt + System.Environment.NewLine
                                        //+ "Status: " + user.Status + System.Environment.NewLine
                                        + "```";
                            await e.Channel.SendMessage(msg);
                        }
                    });
                #endregion

                #region Command - Divine-Pride to rAthena Monster DB
                #region ramob
                commands.CreateCommand("ramob")
                    .Description("This command will return information of a monster in rAthena's mob_db.txt format. ```Usage: /ramob <MonsterID>```")
                    .Parameter("arg", ParameterType.Required)
                    .Do(async (e) =>
                    {
                        int id = Convert.ToInt32(e.Args[0]);
                        await e.Channel.SendIsTyping();
                        if (id <= 0)
                        {
                            await e.Channel.SendMessage(e.Command.Description);
                        }
                        else
                        {
                            Monster mob = DpService.GetMonster(id);
                            if (mob == null)
                            {
                                await e.Channel.SendMessage("Can't retrieve monster info from divine-pride. Either mob-id doesn't exist or divine-pride is down.");
                            }
                            else
                            {
                                await e.Channel.SendMessage("`" + mob.ToString() + "`");
                            }
                        }
                    });
                #endregion

                #region Command - mobinfo
                commands.CreateCommand("mobinfo")
                    .Description("This command will return information of a monster. ```Usage: /mobinfo <MonsterID>```")
                    .Alias(new string[] { "mi", "mobdb", "mobinfo" })
                    .Parameter("arg", ParameterType.Required)
                    .Do(async (e) =>
                    {
                        int id = Convert.ToInt32(e.Args[0]);
                        await e.Channel.SendIsTyping();
                        if (id <= 0)
                        {
                            await e.Channel.SendMessage(e.Command.Description);
                        }
                        else
                        {
                            Monster mob = DpService.GetMonster(id);
                            if (mob == null)
                            {
                                await e.Channel.SendMessage("Can't retrieve monster info from divine-pride. Either mob-id doesn't exist or divine-pride is down.");
                            }
                            else
                            {
                                string template = "Monster info for monster **{0} ({1})** :" + Environment.NewLine +
                                                    "Stats: Lv:{2} HP:{3} SP:{4} STR:{5} AGI:{6} VIT:{7} INT:{8} DEX:{9} LUK:{10}" + Environment.NewLine +
                                                    "Attack: {11}-{12} Def:{13} Mdef:{14} Exp:{15} JobExp:{16} Hit:{17} Flee:{18}" + Environment.NewLine +
                                                    "Race:{19} Size:{20} Element:{21} MVP:{22}" + Environment.NewLine +
                                                    "**Drops**: WIP";
                                await e.Channel.SendMessage(string.Format(template, mob.name, mob.kROName, mob.stats.level, mob.stats.health, mob.stats.sp,
                                    mob.stats.str, mob.stats.agi, mob.stats.vit, mob.stats.Int, mob.stats.dex, mob.stats.luk, mob.stats.attack["minimum"], mob.stats.attack["maximum"],
                                    mob.stats.defense, mob.stats.magicDefense, mob.stats.baseExperience, mob.stats.jobExperience, mob.stats.hit, mob.stats.flee, Monster.Idx2Race(mob.stats.race), Monster.Idx2Size(mob.stats.scale),
                                    "WIP", mob.stats.mvp == 1 ? "Yes" : "No")); // TODO : Implement Element
                            }
                        }
                    });
                #endregion
                #endregion

                #region Command - Divine-Pride to rAthena Item DB
                commands.CreateCommand("raitem")
                    .Alias(new string[] { "ii", "itemdb" })
                    .Description("This command will return information of an item in rAthena's item_db.txt format. ```Usage: /raitem <Item ID>```")
                    .Parameter("arg", ParameterType.Required)
                    .Do(async (e) =>
                    {
                        int nameid = Convert.ToInt32(e.Args[0]);
                        await e.Channel.SendIsTyping();
                        if (nameid <= 0)
                        {
                            await e.Channel.SendMessage(e.Command.Description);
                        }
                        else
                        {
                            Item item = DpService.GetItem(nameid);
                            if (item == null)
                            {
                                await e.Channel.SendMessage("Can't retrieve item info from divine-pride. Either item-id doesn't exist or divine-pride is down.");
                            }
                            else
                            {
                                await e.Channel.SendMessage("`" + item.ToString() + "`");
                            }
                        }
                    });
                #endregion

                #region Command - Member
                commands.CreateCommand("emistry")
                    .Do(async (e) =>
                    {
                        await e.Channel.SendMessage("ZzzzZzzZzzZZZzzzzZZzzzzz...");
                        await e.Channel.SendFile( @"img\emistry.gif" );
                    });
                commands.CreateCommand("aleos")
                    .Do(async (e) =>
                    {
                        await e.Channel.SendFile(@"img\aleos.gif");
                    });
                commands.CreateCommand("akkarin")
                    .Do(async (e) =>
                    {
                        await e.Channel.SendFile(@"img\akkarin.gif");
                    });
                #endregion

                #region Command - Script command documentation
                if (ProcessScriptCommands())
                {
                    commands.CreateCommand("script")
                        .Description("View rAthena script command documentation.")
                        .Parameter("name", ParameterType.Required)
                        .Do(async (e) =>
                        {
                            ScriptCommand cmd = scriptCommands.FirstOrDefault(x => x.Name.Equals(e.Args[0], StringComparison.CurrentCultureIgnoreCase));
                            if (cmd == null)
                            {
                                await e.Channel.SendMessage("Command not found.");
                            }
                            else
                            {
                                string desc = "```";
                                foreach (string x in cmd.Descriptions)
                                {
                                    desc += x + Environment.NewLine;
                                }
                                await e.Channel.SendMessage(desc + "```");
                            }
                        });
                }
                #endregion
                #endregion

                #region RSS Timer
                TimerRSS.Interval = RSSConfig.RefreshInterval;
                TimerRSS.AutoReset = RSSConfig.AutoReset;
                TimerRSS.Enabled = RSSConfig.Enabled;
                TimerRSS.Elapsed += new ElapsedEventHandler(OnCheckRSSFeed);
                TimerRSS.Start();
                #endregion

                instance = this;
                discord.ExecuteAndWait(async () =>
                {
                    try
                    {
                        await discord.Connect(Config.DiscordToken, TokenType.Bot);
                        discord.SetGame("rAthena", GameType.Default, "www.rAthena.org");
                        discord.SetStatus(UserStatus.Online);
                    }
                    catch (HttpException e)
                    {
                        FatalError("HTTP Error : " + e.Message);
                    }
                });
            }
        }

        private string ProcessConfig()
        {
            string _MainConfigFile = "config.json";
            string _RSSConfigFile = "config_rss.json";
            if (!File.Exists(_MainConfigFile))
            {
                return _MainConfigFile;
            }
            else if (!File.Exists(_RSSConfigFile))
            {
                return _RSSConfigFile;
            }
            Config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(_MainConfigFile));
            RSSConfig = JsonConvert.DeserializeObject<RSSConfiguration>(File.ReadAllText(_RSSConfigFile));
            return string.Empty;
        }

        private bool ProcessScriptCommands()
        {
            Console.WriteLine("Processing script_commands.txt");
            ScriptCommandParser scp;
            if (!File.Exists("script_commands.txt"))
            {
                Console.WriteLine("Downloading script_commands.txt from rAthena Git.");
                scp = new ScriptCommandParser(new Uri(@"https://github.com/rathena/rathena/raw/master/doc/script_commands.txt"));
            }
            else
            {
                Console.WriteLine("script_commands.txt already exists.");
                scp = new ScriptCommandParser(File.ReadAllLines("script_commands.txt"));
            }
            Console.Write("Reading file...");
            while (!scp.ready)
            {
                Thread.Sleep(3000);
                Console.Write(".");
            }
            Console.WriteLine();
            if (!scp.Parse())
            {
                Console.WriteLine("Processing failed. Script command doc will be disabled.");
                return false;
            }
            else
            {
                Console.WriteLine("Processed script commands.");
                this.scriptCommands = scp.commands;
                return true;
            }
        }
        #region Rich Site Summary 
        private void OnCheckRSSFeed(object source, ElapsedEventArgs e)
        {
            discord.ExecuteAndWait(async () =>
            {
                TimerRSS.Stop();
                try
                {
                    string message = string.Empty;
                    #region Search for New Support RSS
                    Channel supportChannel = discord.FindServers(Config.ServerName).FirstOrDefault().FindChannels(Config.Channels["Support"], ChannelType.Text).FirstOrDefault();

                    if (supportChannel != null && RSSConfig.SupportFeeds != null && RSSConfig.SupportFeeds.Count > 0)
                    {
                        List<Tuple<string, string, long>> SupportRSSList = new List<Tuple<string, string, long>>();
                        List<string> SupportFeeds = RSSConfig.SupportFeeds;
                        
                        foreach (string rss in SupportFeeds)
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.Load(rss);
                            XmlNodeList elemList = doc.GetElementsByTagName("item");
                            for (int i = 0; i < elemList.Count; i++)
                            {
                                XmlNodeList node = elemList[i].ChildNodes;
                                long timetick = DateTime.Parse(node[(byte)Constant.SupportFeed.PublishDate].InnerText).Ticks;
                                if (timetick > Tick_RSS_Support)
                                {
                                    SupportRSSList.Add(new Tuple<string, string, long>(
                                        node[(byte)Constant.SupportFeed.Title].InnerText,
                                        node[(byte)Constant.SupportFeed.Link].InnerText,
                                        timetick));
                                }
                            }
                        }

                        if (SupportRSSList.Count > 0)
                        {
                            await supportChannel.SendIsTyping();

                            message = "**I've found " + SupportRSSList.Count + " New Topic(s), anybody want to take a look at it? **";
                            foreach (Tuple<string, string, long> rss in SupportRSSList)
                            {
                                message = message + System.Environment.NewLine
                                        //+ rss.Item1 + System.Environment.NewLine 
                                        + rss.Item2 + System.Environment.NewLine;
                            }
                            await supportChannel.SendMessage(message);
                            Tick_RSS_Support = DateTime.Now.Ticks;
                        }
                    }
                    #endregion

                    #region Search for New Server RSS
                    Channel serverChannel = discord.FindServers(Config.ServerName).FirstOrDefault().FindChannels(Config.Channels["ServerAds"], ChannelType.Text).FirstOrDefault();

                    if (serverChannel != null && RSSConfig.ServerFeeds != null && RSSConfig.ServerFeeds.Count > 0)
                    {
                        List<Tuple<string, string, long>> ServerRSSList = new List<Tuple<string, string, long>>();
                        List<string> ServerFeeds = RSSConfig.ServerFeeds;

                        foreach (string rss in ServerFeeds)
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.Load(rss);
                            XmlNodeList elemList = doc.GetElementsByTagName("item");
                            for (int i = 0; i < elemList.Count; i++)
                            {
                                XmlNodeList node = elemList[i].ChildNodes;
                                long timetick = DateTime.Parse(node[(byte)Constant.ServerFeed.PublishDate].InnerText).Ticks;
                                if (timetick > Tick_RSS_Support)
                                {
                                    ServerRSSList.Add(new Tuple<string, string, long>(
                                        node[(byte)Constant.ServerFeed.Title].InnerText,
                                        node[(byte)Constant.ServerFeed.Link].InnerText,
                                        timetick));
                                }
                            }
                        }

                        if (ServerRSSList.Count > 0)
                        {
                            await serverChannel.SendIsTyping();
                            foreach (Tuple<string, string, long> rss in ServerRSSList)
                            {
                                message = rss.Item2 + System.Environment.NewLine;
                                await serverChannel.SendMessage(message);
                            }
                            Tick_RSS_Server = DateTime.Now.Ticks;
                        }
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Exception : " + ex.Message);
                    Console.ForegroundColor = ConsoleColor.White;
                }
                TimerRSS.Start();
            });
        }
        #endregion

    }
}
 
