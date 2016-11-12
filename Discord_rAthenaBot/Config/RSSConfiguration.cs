using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_rAthenaBot
{
    public class RSSConfiguration
    {
        public bool Enabled;        
        public bool AutoReset { get; set; }
        public long RefreshInterval { get; set; }
        public List<String> Feeds { get; set; }
    }
}
