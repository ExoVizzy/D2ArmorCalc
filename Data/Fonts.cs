/*
*   FILE          : Fonts.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Static definitions for all armor fonts, including slot
*                   restrictions, energy costs, & stacking bonus helpers.
*/
using D2ArmorCalc_Models;

namespace D2ArmorCalc_Data {
    public static class Fonts {
        //One font definition per stat (slot-locked per game rules).
        public static readonly Font Super = new(Stat.Super, ArmorSlot.Helmet);
        public static readonly Font Grenade = new(Stat.Grenade, ArmorSlot.Arms);
        public static readonly Font Melee = new(Stat.Melee, ArmorSlot.Arms);
        public static readonly Font Health = new(Stat.Health, ArmorSlot.Chestplate);
        public static readonly Font Weapons = new(Stat.Weapons, ArmorSlot.Boots);
        public static readonly Font Class = new(Stat.Class, ArmorSlot.ClassItem);
        //All fonts as array for iteration.
        public static readonly Font[] All = [Super, Grenade, Melee, Health, Weapons, Class];
        //Stacking bonus values per font count (index = number of that font, 1-based).
        //1st font: +20, 2nd font: +20, 3rd font: +10.
        public static readonly int[] StackingBonus = [0, 20, 20, 10];
        /*
        Method        : GetTotalBonus
        Description   : Returns total stat bonus for given number of
                        stacked fonts of same type on armor piece.
        Parameters    : int count : Number of fonts of same type (1-3).
        Return Values : int       : Total stat bonus granted by the fonts.
        */
        public static int GetTotalBonus(int count){
            if (count < 1 || count > 3) return 0;
            int total = 0;
            for (int i = 1; i <= count; i++) total += StackingBonus[i];
            return total;
        }
        /*
        Method        : GetFontsBySlot
        Description   : Returns all fonts available for given armor slot.
        Parameters    : ArmorSlot slot : Armor slot to filter by.
        Return Values : Font[]         : Array of fonts available in slot.
        */
        public static Font[] GetFontsBySlot(ArmorSlot slot){
            return Array.FindAll(All, f => f.Slot == slot);
        }
        /*
        Method        : GetMaxFontEnergy
        Description   : Returns maximum energy cost of fonts for given
                        slot based on max 3 fonts per piece rule.
        Parameters    : ArmorSlot slot : Armor slot to check.
        Return Values : int            : Maximum energy fonts can use in that slot.
        */
        public static int GetMaxFontEnergy(ArmorSlot slot){
            return GetFontsBySlot(slot).Length * 3;
        }
    }
}