using System;
using System.Collections.Generic;

namespace Discord_rAthenaBot
{
    public class Configuration
    {
        public string ConsoleTitle { get; set; }
        public string DiscordToken { get; set; }
        public string ServerName { get; set; }
        public string PrefixChar { get; set; }
        public bool AllowMentionPrefix { get; set; }
        public Dictionary<String,String> Channels { get; set; }
    }
}
