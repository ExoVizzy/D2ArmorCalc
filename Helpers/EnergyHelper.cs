/*
*   FILE          : EnergyHelper.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Provides energy validation & calculation utilities for
*                   armor pieces, including mod & font compatibility checks
*                   against available energy budgets.
*/
namespace D2ArmorCalc {
    public static class EnergyHelper {
        //=====================================================================
        //Constants.
        //=====================================================================
        public const int LegendaryEnergy = 11;
        public const int ExoticEnergy = 10;
        public const int GeneralSlots = 3;
        public const int FontEnergyCost = 3;
        public const int MajorModEnergyCost = 3;
        public const int MinorModEnergyCost = 1;
        //=====================================================================
        //Energy Calculations.
        //=====================================================================
        /*
        Method        : GetTotalEnergy
        Description   : Returns total energy available for armor piece
                        based on its rarity.
        Parameters    : ArmorRarity rarity : Rarity of armor piece.
        Return Values : int                : Total energy available.
        */
        public static int GetTotalEnergy(ArmorRarity rarity) {
            return rarity == ArmorRarity.Exotic ? ExoticEnergy : LegendaryEnergy;
        }
        /*
        Method        : GetFontEnergy
        Description   : Returns total energy used by set of fonts on
                        single armor piece.
        Parameters    : int fontCount : Number of fonts equipped on piece.
        Return Values : int           : Total energy consumed by fonts.
        */
        public static int GetFontEnergy(int fontCount) {
            return fontCount * FontEnergyCost;
        }

        /*
        Method        : GetRemainingGeneralEnergy
        Description   : Returns remaining general slot energy on piece
                        after accounting for equipped fonts.
        Parameters    : ArmorRarity rarity    : Rarity of armor piece.
                        int         fontCount : Number of fonts equipped.
        Return Values : int                   : Remaining general slot energy.
        */
        public static int GetRemainingGeneralEnergy(ArmorRarity rarity, int fontCount) {
            return GetTotalEnergy(rarity) - GetFontEnergy(fontCount);
        }
        //=====================================================================
        //Validation.
        //=====================================================================
        /*
        Method        : IsFontCountValid
        Description   : Checks whether given number of fonts is valid for
                        armor piece. Max 3 fonts per piece.
        Parameters    : int fontCount : Number of fonts to validate.
        Return Values : bool          : True if font count is within limits.
        */
        public static bool IsFontCountValid(int fontCount) {
            return fontCount >= 0 && fontCount <= GeneralSlots;
        }
        /*
        Method        : IsStatModCompatible
        Description   : Checks whether stat mod fits within remaining
                        energy after fonts are accounted for. Stat mod uses
                        its own slot but still draws from total energy.
        Parameters    : StatMod     mod       : Stat mod to validate.
                        ArmorRarity rarity    : Rarity of armor piece.
                        int         fontCount : Number of fonts equipped.
        Return Values : bool                  : True if the stat mod fits.
        */
        public static bool IsStatModCompatible(StatMod mod, ArmorRarity rarity, int fontCount) {
            int remaining = GetRemainingGeneralEnergy(rarity, fontCount);
            return mod.EnergyCost <= remaining;
        }
        /*
        Method        : IsArmorModCompatible
        Description   : Checks whether single armor mod fits within
                        remaining general slot energy on piece.
        Parameters    : ArmorMod    mod            : Armor mod to validate.
                        ArmorRarity rarity         : Rarity of armor piece.
                        int         fontCount      : Number of fonts equipped.
                        int         statModCost    : Energy cost of equipped stat mod (0 if none).
                        int         currentModCost : Total energy already used by other armor mods.
        Return Values : bool                       : True if mod fits.
        */
        public static bool IsArmorModCompatible(ArmorMod mod, ArmorRarity rarity, int fontCount, int statModCost, int currentModCost) {
            int total     = GetTotalEnergy(rarity);
            int used      = GetFontEnergy(fontCount) + statModCost + currentModCost;
            int remaining = total - used;
            return mod.EnergyCost <= remaining;
        }
        /*
        Method        : IsArmorModLoadoutValid
        Description   : Checks whether full set of armor mods fits within
                        energy budget of piece alongside fonts a stat mod.
        Parameters    : ArmorMod[]  mods        : Armor mods to validate.
                        ArmorRarity rarity      : Rarity of armor piece.
                        int         fontCount   : Number of fonts equipped.
                        int         statModCost : Energy cost of equipped stat mod (0 if none).
        Return Values : bool                    : True if full loadout fits.
        */
        public static bool IsArmorModLoadoutValid(ArmorMod[] mods, ArmorRarity rarity, int fontCount, int statModCost) {
            int total = GetTotalEnergy(rarity);
            int fontCost = GetFontEnergy(fontCount);
            int modCost = 0;
            foreach (var mod in mods) modCost += mod.EnergyCost;
            return fontCost + statModCost + modCost <= total;
        }
        /*
        Method        : IsSlotValid
        Description   : Checks whether all mods in loadout match
                        expected armor slot.
        Parameters    : ArmorMod[] mods : Armor mods to validate.
                        ArmorSlot  slot : Expected armor slot.
        Return Values : bool            : True if all mods belong to slot.
        */
        public static bool IsSlotValid(ArmorMod[] mods, ArmorSlot slot) {
            foreach (var mod in mods) {
                if (mod.Slot != slot) return false;
            }
            return true;
        }
        /*
        Method        : IsPieceConfigValid
        Description   : Validates full configuration of armor piece
                        including slot compatibility, font count, & total
                        energy usage.
        Parameters    : ArmorPiece piece : Armor piece to validate.
        Return Values : bool             : True if piece configuration is valid.
        */
        public static bool IsPieceConfigValid(ArmorPiece piece) {
            if (!IsFontCountValid(piece.Fonts.Length)) return false;

            if (!IsSlotValid(new ArmorMod[0], piece.Slot)) return false;

            int statModCost = piece.StatMod != null ? piece.StatMod.EnergyCost : 0;
            int fontCost = GetFontEnergy(piece.Fonts.Length);
            int total = GetTotalEnergy(piece.Rarity);

            return fontCost + statModCost <= total;
        }
        /*
        Method        : GetAvailableArmorMods
        Description   : Returns all armor mods for given slot that fit
                        within remaining energy budget of piece.
        Parameters    : ArmorSlot   slot        : Armor slot to filter by.
                        ArmorRarity rarity      : Rarity of armor piece.
                        int         fontCount   : Number of fonts equipped.
                        int         statModCost : Energy cost of equipped stat mod (0 if none).
                        int         usedEnergy  : Energy already used by other equipped armor mods.
        Return Values : List<ArmorMod>          : All mods that fit within remaining budget.
        */
        public static List<ArmorMod> GetAvailableArmorMods(ArmorSlot slot, ArmorRarity rarity, int fontCount, int statModCost, int usedEnergy) {
            var available = new List<ArmorMod>();
            var slotMods = ArmorMods.GetModsBySlot(slot);
            int total = GetTotalEnergy(rarity);
            int used = GetFontEnergy(fontCount) + statModCost + usedEnergy;
            int remaining = total - used;

            foreach (var mod in slotMods) {
                if (mod.EnergyCost <= remaining) available.Add(mod);
            }
            return available;
        }
    }
}