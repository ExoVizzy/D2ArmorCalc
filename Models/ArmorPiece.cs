/*
*   FILE          : ArmorPiece.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Defines ArmorPiece model representing a single piece
*                   of armor including its archetype, stats, slot, mods, & fonts.
*/
using D2ArmorCalc_Data;

namespace D2ArmorCalc_Models {
    //Enum representing whether piece is exotic or legendary.
    public enum ArmorRarity {
        Legendary, Exotic
    }
    //Represents single piece of armor & all its relevant properties.
    public class ArmorPiece(ArmorSlot slot, ArmorRarity rarity){
        //Identity.
        public string? Name {get; set;}
        public ArmorSlot Slot {get; set;} = slot;
        public bool IsCustomRoll {get; set;}
        public StatBlock? CustomStatBlock {get; set;}
        public int StandardPrimary {get; set;}
        public int StandardSecondary {get; set;}
        public int StandardTertiary {get; set;}
        public ArmorRarity Rarity {get; set;} = rarity;
        //Archetype determines primary (30) & secondary (25) stats.
        public Archetype? Archetype {get; set;}
        //Tertiary stat is any of the 4 stats not in the archetype (value: 20).
        public Stat TertiaryStat {get; set;}
        //Focus: +5 to focus stat, -5 from any other stat.
        public Stat FocusStat {get; set;}
        public Stat FocusMinus {get; set;}
        //Stat mod in dedicated stat mod slot (null if none).
        public StatMod? StatMod {get; set;}
        //Up to 3 fonts in general mod slots (slot-locked, validated externally).
        public Font[] Fonts {get; set;} = [];
        //Energy.
        public int TotalEnergy {get;} = rarity == ArmorRarity.Exotic ? 10 : 11;
        public int FontEnergy => Fonts != null ? Fonts.Length  * 3 : 0;
        public int StatModEnergy => StatMod != null ? StatMod.EnergyCost : 0;
        public int RemainingEnergy => TotalEnergy - FontEnergy - StatModEnergy;
        /*
        Method        : GetBaseStat
        Description   : Returns base stat value for given stat based on
                        archetype, tertiary, & focus assignments.
        Parameters    : Stat stat : Stat to retrieve base value for.
        Return Values : int       : Base stat value before mods or fonts.
        */
        public int GetBaseStat(Stat stat){
            if (IsCustomRoll && CustomStatBlock != null) return CustomStatBlock.Get(stat);
            if (Archetype == null) return 0;

            int value;
            if (stat == Archetype.Primary) value = Rarity == ArmorRarity.Exotic ? 30 : 30;
            else if (stat == Archetype.Secondary) value = Rarity == ArmorRarity.Exotic ? 20 : 25;
            else if (stat == TertiaryStat) value = Rarity == ArmorRarity.Exotic ? 12 : 20;
            else value = 5; //masterwork on remaining stats.

            //Focus only on legendary.
            if (Rarity != ArmorRarity.Exotic){
                if (stat == FocusStat) value += 5;
                if (stat == FocusMinus) value -= 5;
            }
            return value;
        }
        /*
        Method        : GetTotalStat
        Description   : Returns total stat value for given stat including
                        base value, stat mod bonus, & font bonuses.
        Parameters    : Stat stat : Stat to calculate total for.
        Return Values : int       : Total stat value after all bonuses.
        */
        public int GetTotalStat(Stat stat){
            int total = GetBaseStat(stat);

            if (StatMod != null && StatMod.Stat == stat) total += StatMod.Bonus;

            int fontCount = 0;
            foreach (Font font in Fonts){
                if (font.Stat == stat) fontCount++;
            }
            total += D2ArmorCalc_Data.Fonts.GetTotalBonus(fontCount);

            return total;
        }
        /*
        Method        : GetAllStats
        Description   : Returns StatBlock containing total value for
                        all 6 stats on this armor piece.
        Parameters    : None.
        Return Values : StatBlock : All 6 stat totals for this piece.
        */
        public StatBlock GetAllStats(){
            return new StatBlock(
                GetTotalStat(Stat.Health), GetTotalStat(Stat.Melee), GetTotalStat(Stat.Grenade),
                GetTotalStat(Stat.Super), GetTotalStat(Stat.Class), GetTotalStat(Stat.Weapons)
            );
        }
    }
}