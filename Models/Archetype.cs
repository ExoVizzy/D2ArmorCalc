/*
*   FILE          : Archetype.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Defines Archetype model, ArchetypeType enum, &
*                   Stat enum used throughout the application.
*/
namespace D2ArmorCalc {
    //Enum representing all 6 armor stats.
    public enum Stat {
        Health, Melee, Grenade, Super, Class, Weapons
    }
    //Enum representing all 6 armor archetypes.
    //Brawler: Melee/Health, Gunner: Weapons/Grenade, Specialist: Class/Weapons, Grenadier: Grenade/Super, Paragon: Super/Melee, Bulwark: Health/Class.
    public enum ArchetypeType {
        Brawler, Gunner, Specialist, Grenadier, Paragon, Bulwark
    }
    //Holds stat layout for given archetype.
    public class Archetype {
        public ArchetypeType Type {get;}
        public Stat Primary {get;}
        public Stat Secondary {get;}
        public Archetype(ArchetypeType type, Stat primary, Stat secondary) {
            Type = type;
            Primary = primary;
            Secondary = secondary;
        }
    }
}
