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
        public List<MonsterItemDrop> drops { get; set; }
        public List<MonsterItemDrop> mvpdrops { get; set; }
        public List<MonsterSpawn> spawn { get; set; }
        public List<MonsterSkill> skill { get; set; }

        //ID,Sprite_Name,kROName,iROName,LV,HP,SP,EXP,JEXP,Range1,ATK1,ATK2,DEF,MDEF,STR,AGI,VIT,INT,DEX,LUK,Range2,Range3,Scale,Race,Element,Mode,Speed,aDelay,aMotion,dMotion,MEXP,MVP1id,MVP1per,MVP2id,MVP2per,MVP3id,MVP3per,Drop1id,Drop1per,Drop2id,Drop2per,Drop3id,Drop3per,Drop4id,Drop4per,Drop5id,Drop5per,Drop6id,Drop6per,Drop7id,Drop7per,Drop8id,Drop8per,Drop9id,Drop9per,DropCardid,DropCardper
        public string ToAthenaFormat()
        {
            var template = "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28}"; //,MVP1id,MVP1per,MVP2id,MVP2per,MVP3id,MVP3per,Drop1id,Drop1per,Drop2id,Drop2per,Drop3id,Drop3per,Drop4id,Drop4per,Drop5id,Drop5per,Drop6id,Drop6per,Drop7id,Drop7per,Drop8id,Drop8per,Drop9id,Drop9per,DropCardid,DropCardper
            template = string.Format(template, this.id, this.AegisName, this.name, this.name, this.stats.level, this.stats.health, this.stats.sp, this.stats.baseExperience,
                this.stats.jobExperience, this.stats.attack["minimum"], this.stats.attack["maximum"], this.stats.defense, this.stats.magicDefense, this.stats.str, this.stats.agi, this.stats.vit,
                this.stats.Int, this.stats.dex, this.stats.luk, this.stats.aggroRange, this.stats.escapeRange, this.stats.scale, this.stats.race, this.stats.element, "{Mode}", "{Speed}", "{aDelay}", "{aMotion}", this.stats.attackedSpeed);
            var size = 0;
            if (this.mvpdrops != null)
            {
                foreach (var drop in this.mvpdrops)
                {
                    template += "," + drop.itemId + "," + drop.chance;
                    size++;
                }
            }
            for (int i = 0; i < 3 - size; ++i)
                template += ",0,0";
            size = 0;
            if (this.drops != null)
            {
                foreach (var drop in this.drops)
                {
                    if (drop.stealProtected) continue;
                    template += "," + drop.itemId + "," + drop.chance;
                    size++;
                }
            }
            for (int i = 0; i < 9 - size; ++i)
                template += ",0,0";
            if (this.drops != null)
            {
                var cardDrop = this.drops.Where(x => x.stealProtected == true).FirstOrDefault();
                if (cardDrop != null)
                {
                    template += "," + cardDrop.itemId + "," + cardDrop.chance;
                }
                else
                {
                    template += ",0,0";
                }
            } else { template += ",0,0"; };

            return template;
        }
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
