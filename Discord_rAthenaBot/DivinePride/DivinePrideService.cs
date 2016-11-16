using Newtonsoft.Json;
using System.Net;

namespace Discord_rAthenaBot.DivinePride
{
    public class DivinePrideService
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }

        public string GetData(string type, int id)
        {
            string url = string.Format(BaseUrl, ApiKey, type, id);
            using(WebClient wc = new WebClient())
            {               
                return wc.DownloadString(url);
            }
        }

        public Monster GetMonster(int id)
        {
            try
            {
                return JsonConvert.DeserializeObject<Monster>(GetData("Monster", id));
            }
            catch (WebException)
            {
                return null;
            }
        }
    }
}
