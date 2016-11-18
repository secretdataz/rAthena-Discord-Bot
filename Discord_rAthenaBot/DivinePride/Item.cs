using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discord_rAthenaBot.DivinePride
{
    public partial class Item
    {
        public override string ToString()
        {
            return this.rAthenaDB();
        }

        //ID,AegisName,Name,Type,Buy,Sell,Weight,ATK[:MATK],DEF,Range,Slots,Job,Class,Gender,Loc,wLV,eLV[:maxLevel],Refineable,View,{ Script },{ OnEquip_Script },{ OnUnequip_Script }
        public string rAthenaDB()
        {
            string template = "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21}";
            template = string.Format(template,
                this.id,
                this.aegisName,
                this.aegisName,
                this.itemTypeId,
                this.price,
                (this.price / 2),
                this.weight,
                this.attack,
                this.defense,
                this.range,
                this.slots,
                String.Format("0x{0:X}", this.job),
                "{Class}",
                this.gender,
                this.location,
                this.weaponLevel,
                this.limitLevel,
                this.refinable,
                this.classNum,
                "{Script}",
                "{OnEquip_Script}",
                "{OnUnequip_Script}"
            );
            return template.Replace(",,",",0,");
        }
    }
}
