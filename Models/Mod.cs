/*
*   FILE          : Mod.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Static definitions for armor stat mods, including
*                   major & minor variants, their costs & bonus values.
*/
namespace D2ArmorCalc {
    //Enum representing two types of stat mods (Major: +10 Stat & Cost 3 Energy, Minor: +5 Stat & Cost 1 Energy).
    public enum ModType {
        Major, Minor
    }
    //Holds data for single stat mod instance (type + boosted stat).
    public class Mod {
        public ModType ModType {get;}
        public Stat Stat {get;}
        public int Bonus {get;}
        public int EnergyCost {get;}
        public Mod(ModType modType, Stat stat) {
            ModType = modType;
            Stat = stat;
            Bonus = modType == ModType.Major ? 10 : 5;
            EnergyCost = modType == ModType.Major ? 3 : 1;
        }
    }
    public static class Mods {
        //All stats as array for iteration.
        private static readonly Stat[] AllStats = {
            Stat.Health, Stat.Melee, Stat.Grenade, Stat.Super, Stat.Class, Stat.Weapons
        };
        /*
        Method        : GetMod
        Description   : Returns mod instance for given type & stat.
        Parameters    : ModType modType : Major or Minor mod type.
                        Stat stat       : Stat the mod applies to.
        Return Values : Mod             : New Mod instance for given type & stat.
        */
        public static Mod GetMod(ModType modType, Stat stat) {
            return new Mod(modType, stat);
        }
        /*
        Method        : GetAllMajorMods
        Description   : Returns one major mod for each of the 6 stats.
        Parameters    : None.
        Return Values : Mod[] : Array of all 6 major stat mods.
        */
        public static Mod[] GetAllMajorMods() {
            var mods = new Mod[AllStats.Length];
            for (int i = 0; i < AllStats.Length; i++) {
                mods[i] = new Mod(ModType.Major, AllStats[i]);
            }
            return mods;
        }
        /*
        Method        : GetAllMinorMods
        Description   : Returns one minor mod for each of the 6 stats.
        Parameters    : None.
        Return Values : Mod[] : Array of all 6 minor stat mods.
        */
        public static Mod[] GetAllMinorMods() {
            var mods = new Mod[AllStats.Length];
            for (int i = 0; i < AllStats.Length; i++) {
                mods[i] = new Mod(ModType.Minor, AllStats[i]);
            }
            return mods;
        }
        /*
        Method        : IsCompatible
        Description   : Checks whether mod fits within remaining energy
                        budget of armor piece. Each piece has dedicated
                        stat mod slot so energy here is always the full slot cost.
        Parameters    : Mod mod          : Mod to check.
                        int energyBudget : Remaining energy available on piece.
        Return Values : bool             : True if mod fits, false otherwise.
        */
        public static bool IsCompatible(Mod mod, int energyBudget) {
            return mod.EnergyCost <= energyBudget;
        }
    }
}