using Newtonsoft.Json;
using System.Net;
using System;

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
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);
                Console.ForegroundColor = ConsoleColor.White;
            }
            return null;
        }

        public Item GetItem(int id)
        {
            try
            {
                return JsonConvert.DeserializeObject<Item>(GetData("Item", id));
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);
                Console.ForegroundColor = ConsoleColor.White;
            }
            return null;
        }
    }
}
