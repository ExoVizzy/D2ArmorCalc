/*
*   FILE          : Mods.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Static helpers for creating and validating stat mods.
*/
namespace D2ArmorCalc {
    public static class Mods {
        /*
        Method        : GetMod
        Description   : Returns StatMod instance for given type & stat.
        Parameters    : ModType modType : Major or Minor mod type.
                        Stat stat       : Stat mod applies to.
        Return Values : StatMod         : New StatMod for given type & stat.
        */
        public static StatMod GetMod(ModType modType, Stat stat) {
            return new StatMod(modType, stat);
        }
        /*
        Method        : IsCompatible
        Description   : Checks whether stat mod fits within remaining
                        energy budget of dedicated stat mod slot.
        Parameters    : StatMod mod          : Mod to check.
                        int     energyBudget : Remaining energy available.
        Return Values : bool                 : True if mod fits, false otherwise.
        */
        public static bool IsCompatible(StatMod mod, int energyBudget) {
            return mod.EnergyCost <= energyBudget;
        }
    }
}