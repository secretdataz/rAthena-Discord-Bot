using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Discord_rAthenaBot.DivinePride
{
    public partial class Item
    {
        public int id { get; set; }
        public string aegisName { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public int slots { get; set; }
        public int itemTypeId { get; set; }
        public int itemSubTypeId { get; set; }
        public int matk { get; set; }
        public int attack { get; set; }
        public int defense { get; set; }
        public int attribute { get; set; }
        public int range { get; set; }
        public string weight { get; set; }
        public int limitLevel { get; set; }
        public int weaponLevel { get; set; }
        public int price { get; set; }
        public int job { get; set; }
        public int gender { get; set; }
        public string location { get; set; }
        public bool refinable { get; set; }
        public bool indestructible { get; set; }
        public int classNum { get; set; }

        public ItemMoveInfo itemMoveInfo { get; set; }
        public class ItemMoveInfo
        {
            public bool drop { get; set; }
            public bool trade { get; set; }
            public bool store { get; set; }
            public bool cart { get; set; }
            public bool sell { get; set; }
            public bool mail { get; set; }
            public bool auction { get; set; }
            public bool guildStore { get; set; }
        }

        public List<ItemSetList> sets { get; set; }
        public class ItemSetList
        {
            public string name { get; set; }
            public List<ItemSet> items { get; set; }
        }

        public class ItemSet
        {
            public int itemId { get; set; }
            public string name { get; set; }
        }

    }
}
