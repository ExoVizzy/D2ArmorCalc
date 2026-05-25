/*
*   FILE          : Archetypes.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Static definitions for all 6 armor archetypes &
*                   helpers for resolving tertiary stat options.
*/
using D2ArmorCalc_Models;

namespace D2ArmorCalc_Data {
    //Static lookup for all archetypes.
    public static class Archetypes {
        public static readonly Archetype Brawler = new(ArchetypeType.Brawler, Stat.Melee, Stat.Health);
        public static readonly Archetype Gunner = new(ArchetypeType.Gunner, Stat.Weapons, Stat.Grenade);
        public static readonly Archetype Specialist = new(ArchetypeType.Specialist, Stat.Class, Stat.Weapons);
        public static readonly Archetype Grenadier = new(ArchetypeType.Grenadier, Stat.Grenade, Stat.Super);
        public static readonly Archetype Paragon = new(ArchetypeType.Paragon, Stat.Super, Stat.Melee);
        public static readonly Archetype Bulwark = new(ArchetypeType.Bulwark, Stat.Health, Stat.Class);
        //All archetypes as array for iteration.
        public static readonly Archetype[] AllArchetypes = [Brawler, Gunner, Specialist, Grenadier, Paragon, Bulwark];
        /*
        Method        : GetTertiaryStats
        Description   : Returns 4 stats that arent primary or secondary
                        for given archetype (valid tertiary options).
        Parameters    : Archetype archetype : Archetype to check against.
        Return Values : Stat[]              : Array of 4 valid tertiary stats.
        */
        public static Stat[] GetTertiaryStats(Archetype archetype){
            Stat[] all = [Stat.Health, Stat.Melee, Stat.Grenade, Stat.Super, Stat.Class, Stat.Weapons];
            return Array.FindAll(all, s => s != archetype.Primary && s != archetype.Secondary);
        }
    }
}