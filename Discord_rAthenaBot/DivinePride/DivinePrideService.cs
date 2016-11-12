using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Discord_rAthenaBot.DivinePride
{
    public class DivinePrideService
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }

        public string GetData(string type, int id)
        {
            var url = string.Format(BaseUrl, ApiKey, type, id);
            using(WebClient wc = new WebClient())
            {               
                return wc.DownloadString(url);
            }
        }

        public Monster GetMonster(int id)
        {
            try {
                var mob = JsonConvert.DeserializeObject<Monster>(GetData("Monster", id));
                return mob;
            } catch(WebException e)
            {
                return null;
            }
        }
    }
}
