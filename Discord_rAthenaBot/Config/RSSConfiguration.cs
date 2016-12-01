using System;
using System.Collections.Generic;

namespace Discord_rAthenaBot
{
    public class RSSConfiguration
    {
        public bool Enabled;        
        public bool AutoReset { get; set; }
        public long RefreshInterval { get; set; }
        public List<String> SupportFeeds { get; set; }
        public List<String> ServerFeeds { get; set; }
    }
}
