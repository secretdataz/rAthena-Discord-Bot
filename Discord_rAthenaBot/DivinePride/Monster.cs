using Newtonsoft.Json;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Discord_rAthenaBot.DivinePride
{
    public partial class Monster
    {
        public override string ToString()
        {
            return this.rAthenaDB();
        }

        //ID,Sprite_Name,kROName,iROName,LV,HP,SP,EXP,JEXP,Range1,ATK1,ATK2,DEF,MDEF,STR,AGI,VIT,INT,DEX,LUK,Range2,Range3,Scale,Race,Element,Mode,Speed,aDelay,aMotion,dMotion,MEXP,MVP1id,MVP1per,MVP2id,MVP2per,MVP3id,MVP3per,Drop1id,Drop1per,Drop2id,Drop2per,Drop3id,Drop3per,Drop4id,Drop4per,Drop5id,Drop5per,Drop6id,Drop6per,Drop7id,Drop7per,Drop8id,Drop8per,Drop9id,Drop9per,DropCardid,DropCardper
        public string rAthenaDB()
        {
            string template = "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28}"; //,MVP1id,MVP1per,MVP2id,MVP2per,MVP3id,MVP3per,Drop1id,Drop1per,Drop2id,Drop2per,Drop3id,Drop3per,Drop4id,Drop4per,Drop5id,Drop5per,Drop6id,Drop6per,Drop7id,Drop7per,Drop8id,Drop8per,Drop9id,Drop9per,DropCardid,DropCardper
            template = string.Format(template, this.id, this.AegisName, this.name, this.name, this.stats.level, this.stats.health, this.stats.sp, this.stats.baseExperience,
                this.stats.jobExperience, this.stats.attack["minimum"], this.stats.attack["maximum"], this.stats.defense, this.stats.magicDefense, this.stats.str, this.stats.agi, this.stats.vit,
                this.stats.Int, this.stats.dex, this.stats.luk, this.stats.aggroRange, this.stats.escapeRange, this.stats.scale, this.stats.race, this.stats.element, "{Mode}", "{Speed}", "{aDelay}", "{aMotion}", this.stats.attackedSpeed);
            int size = 0;
            if (this.mvpdrops != null)
            {
                foreach (MonsterItemDrop drop in this.mvpdrops)
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
                foreach (MonsterItemDrop drop in this.drops)
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
                MonsterItemDrop cardDrop = this.drops.Where(x => x.stealProtected == true).FirstOrDefault();
                if (cardDrop != null)
                {
                    template += "," + cardDrop.itemId + "," + cardDrop.chance;
                }
                else
                {
                    template += ",0,0";
                }
            }
            else
            {
                template += ",0,0";
            };

            return template;
        }
    }
}
