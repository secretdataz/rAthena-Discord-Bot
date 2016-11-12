﻿using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Timers;
using System.Xml;
using Newtonsoft.Json;
using Discord.Net;

namespace Discord_rAthenaBot
{
    partial class rAthenaBot
    {
        DiscordClient discord;
        CommandService commands;
        Configuration Config;
        RSSConfiguration RSSConfig;
        long rssTick = DateTime.Now.Ticks;

        private void Log(object sender, LogMessageEventArgs e)
        {
            Console.WriteLine(e.Message);
        }

        private void FatalError(string Message, int ExitCode = 1)
        {
            Console.WriteLine(Message);
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
            Environment.Exit(ExitCode);
        }

        public rAthenaBot()
        {
            #region Process configurations
            Console.WriteLine("Reading configuration files.");
            var _Result = ProcessConfig();
            if (!string.IsNullOrEmpty(_Result))
            {
                FatalError("Configuration file " + _Result + " is missing.", 0x6E); // ERROR_OPEN_FAILED                
            }
            Console.WriteLine("Done reading configuration files.");
            Console.Title = Config.ConsoleTitle;
            #endregion

            discord = new DiscordClient(x =>
            {
                x.LogLevel = LogSeverity.Info;
                x.LogHandler = Log;
            });

            #region Discord Events - UserJoined
            discord.UserJoined += async (s, e) => {
                if (e.Server.Name.Equals(Config.ServerName))
                {
                    var channel = e.Server.FindChannels(Config.Channels["General"]).FirstOrDefault();
                    if (channel != null)
                    {
                        await channel.SendMessage("Hello " + e.User.Mention + ", welcome to " + Config.ServerName  + " Discord." + Environment.NewLine +
                            "Kindly read the " + e.Server.FindChannels(Config.Channels["Rules"], ChannelType.Text).FirstOrDefault().Mention + " before you start posting.Thank you.");
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
                        string query = String.Join("+", e.Args.ToList());
                        // string url = "www.google.com/?#newwindow=1&safe=off&q=rathena.org+" + query;
                        string url = "cse.google.com/cse/publicurl?cx=009409555530237933087:hphsio1cods&q=" + query;
                        await e.Channel.SendMessage("Search result: `" + String.Join(" ", e.Args.ToList()) + "`" + System.Environment.NewLine + "https://" + url);
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
                                    await e.Channel.DeleteMessages(msg);
                                    //await e.Channel.SendIsTyping();
                                    //if (msg.Length > 1)
                                    //{
                                    //    await e.Channel.SendMessage(e.Message.User.Name + " removed " + author + "'s " + (msg.Length - 1) + " chat(s).");
                                    //}
                                }
                                else
                                {
                                    await e.Channel.DeleteMessages(msg);
                                    //await e.Channel.SendIsTyping();
                                    //if (msg.Length > 1)
                                    //{
                                    //    await e.Channel.SendMessage(e.Message.User.Name + " removed " + (msg.Length - 1) + " chat(s).");
                                    //}
                                }

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
                            user = e.Channel.FindUsers(username, true).FirstOrDefault();
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
                                    + "Role: " + String.Join(",", e.User.Roles.Select(x => x.Name)) + System.Environment.NewLine
                                    + "Joined Since: " + user.JoinedAt + System.Environment.NewLine
                                    + "Last Activity: " + user.LastActivityAt + System.Environment.NewLine
                                    + "Last Online At: " + user.LastOnlineAt + System.Environment.NewLine
                                    //+ "Status: " + user.Status + System.Environment.NewLine
                                    + "```";
                        await e.Channel.SendMessage(msg);
                    }
                });
            #endregion

            #region Command - Item Info
            commands.CreateCommand("iteminfo")
                .Description("This command will return information of an Item. ```Usage: /iteminfo <ItemID>```")
                .Alias(new string[] { "ii", "itemdb" })
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
                        string divinepride = "www.divine-pride.net/database/item/" + id;
                        Process.Start(divinepride);
                    }

                });
            #endregion

            #region Command - Monster Info
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
                        string divinepride = "www.divine-pride.net/database/monster/" + id;
                        Process.Start(divinepride);
                    }
                });
            #endregion

            #endregion 

            #region RSS Timer
            Timer rssTimer = new Timer();
            rssTimer.Interval = RSSConfig.RefreshInterval;
            rssTimer.AutoReset = RSSConfig.AutoReset;
            rssTimer.Enabled = RSSConfig.Enabled;
            rssTimer.Elapsed += new ElapsedEventHandler(OnTriggerRSS);
            rssTimer.Start();
            #endregion

            discord.ExecuteAndWait(async () =>
            {
                try
                {
                    await discord.Connect(Config.DiscordToken, TokenType.Bot);
                }catch(HttpException e)
                {
                    FatalError("HTTP Error : " + e.Message);
                }
            });
        }

        private string ProcessConfig()
        {
            var _MainConfigFile = "config.json";
            var _RSSConfigFile = "config_rss.json";
            if (!File.Exists(_MainConfigFile))
            {
                return _MainConfigFile;
            } else if (!File.Exists(_RSSConfigFile))
            {
                return _RSSConfigFile;
            }
            Config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(_MainConfigFile));
            RSSConfig = JsonConvert.DeserializeObject<RSSConfiguration>(File.ReadAllText(_RSSConfigFile));
            return null;
        }

        #region Rich Site Summary 
        enum RSSItemType {
            Title = 0,
            Link = 1,
            Description = 2,
            GUID = 3,
            pubDate = 4
        }

        private void OnTriggerRSS(object source, ElapsedEventArgs e)
        {
            discord.ExecuteAndWait(async () =>
            {
                Channel channel = discord.FindServers(Config.ServerName).FirstOrDefault().FindChannels(Config.Channels["Support"],ChannelType.Text).FirstOrDefault();

                if (channel != null)
                {
                    if (RSSConfig.Feeds.Count > 0)
                    {
                        List<Tuple<string, string, long>> newRSS = new List<Tuple<string, string, long>>();
                        var rssList = RSSConfig.Feeds;

                        #region Search for New RSS
                        foreach (var rss in rssList)
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.Load(rss);
                            XmlNodeList elemList = doc.GetElementsByTagName("item");
                            for (int i = 0; i < elemList.Count; i++)
                            {
                                var node = elemList[i].ChildNodes;
                                long timetick = DateTime.Parse(node[(byte)RSSItemType.pubDate].InnerText).Ticks;
                                if (timetick > rssTick)
                                {
                                    newRSS.Add(new Tuple<string, string, long>(
                                        node[(byte)RSSItemType.Title].InnerText,
                                        node[(byte)RSSItemType.Link].InnerText,
                                        timetick));
                                }
                            }
                        }
                        #endregion

                        #region Post RSS if found
                        if (newRSS.Count > 0)
                        {
                            await channel.SendIsTyping();

                            string msg = "**" + newRSS.Count + " New Topic(s): **";
                            foreach (var rss in newRSS)
                            {
                                msg = msg + System.Environment.NewLine
                                    //+ rss.Item1 + System.Environment.NewLine 
                                    + rss.Item2 + System.Environment.NewLine; 
                            }
                            await channel.SendMessage(msg);
                            rssTick = DateTime.Now.Ticks;
                        }
                        #endregion

                    }

                }
            });
        }
        #endregion

    }
}
 