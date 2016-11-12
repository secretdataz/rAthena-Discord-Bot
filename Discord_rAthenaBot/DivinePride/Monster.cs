using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_rAthenaBot.DivinePride
{
    public class Monster
    {
        public static string Idx2Size(int idx)
        {
            switch (idx)
            {
                case 0:
                    return "Small";
                case 1:
                    return "Medium";
                case 2:
                    return "Large";
                default:
                    return "Unknown";
            }
        }

        public static string Idx2Race(int idx)
        {
            switch (idx) {
                case 0:
                    return "Formless";
                case 1:
                    return "Undead";
                case 2:
                    return "Brute";
                case 3:
                    return "Plant";
                case 4:
                    return "Insect";
                case 5:
                    return "Fish";
                case 6:
                    return "Demon";
                case 7:
                    return "Demi-Human";
                case 8:
                    return "Angel";
                case 9:
                    return "Dragon";
                case 10:
                    return "Player";
                default:
                    return "Unknown";
            }
        }

        public static TextInfo _TextInfo = new CultureInfo("en-US", false).TextInfo;
        public int id { get; set; }
        [JsonProperty("dbname")]
        public string AegisName { get; set; }
        public string name { get; set; }
        public string kROName
        {
            get
            {
                return _TextInfo.ToTitleCase(AegisName.Replace('_', ' '));
            }
        }
        public MonsterStat stats { get; set; }
    }

    public class MonsterStat
    {
        public int attackRange { get; set; }
        public int level { get; set; }
        public int health { get; set; }
        public int sp { get; set; }
        public int str { get; set; }
        [JsonProperty("int")]
        public int Int { get; set; }
        public int vit { get; set; }
        public int dex { get; set; }
        public int agi { get; set; }
        public int luk { get; set; }
        public int rechargeTime { get; set; }
        public Dictionary<string, int> attack { get; set; }
        public int defense { get; set; }
        public int baseExperience { get; set; }
        public int jobExperience { get; set; }
        public int aggroRange { get; set; }
        public int escapeRange { get; set; }
        public double movementSpeed { get; set; }
        public double attackSpeed { get; set; }
        public double attackedSpeed { get; set; }
        public int element { get; set; }
        public int scale { get; set; }
        public int race { get; set; }
        public int magicDefense { get; set; }
        public int hit { get; set; }
        public int flee { get; set; }
        public string ai { get; set; }
        public int mvp { get; set; }
        [JsonProperty("class")]
        public int Class { get; set; }
        public int attr { get; set; }
        public List<MonsterItemDrop> drops { get; set; }
        public List<MonsterItemDrop> mvpdrops { get; set; }
        public List<MonsterSpawn> spawn { get; set; }
        public List<MonsterSkill> skill { get; set; }
    }

    public class MonsterItemDrop
    {
        public int itemId { get; set; }
        public int chance { get; set; }
        public bool stealProtected { get; set; }
    }

    public class MonsterSpawn
    {
        public string mapname { get; set; }
        public int amount { get; set; }
        public long respawnTime { get; set; }
    }

    public class MonsterSkill
    {
        public int idx { get; set; }
        public int skillId { get; set; }
        public string status { get; set; }
        public int level { get; set; }
        public int chance { get; set; }
        public int casttime { get; set; }
        public int delay { get; set; }
        public bool interruptable { get; set; }
        public string sendType { get; set; }
        public int value { get; set; }
    }
}
