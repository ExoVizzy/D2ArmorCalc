/*
*   FILE          : GameConstants.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 24, 2026
*   DESCRIPTION   : Defines GameConstants, providing shared static arrays
*                   for canonical stat & armor slot ordering used
*                   throughout the application.
*/
using System.Collections.ObjectModel;

namespace D2ArmorCalc_Models {
    public static class GameConstants {
        public static readonly Stat[] StatOrder = [
            Stat.Health, Stat.Melee, Stat.Grenade, Stat.Super, Stat.Class, Stat.Weapons
        ];
        public static readonly ArmorSlot[] SlotOrder = [
            ArmorSlot.Helmet, ArmorSlot.Arms, ArmorSlot.Chestplate, ArmorSlot.Boots, ArmorSlot.ClassItem
        ];
    }
    public class ObservableGameConstants {
        public ObservableGameConstants(){}
        //Slot selector.
        public ObservableCollection<string> Slots {get;}  = ["Helmet", "Arms", "Chestplate", "Boots", "Class Item"];
        //Class selector.
        public ObservableCollection<string> Classes {get;}  = ["Warlock", "Titan", "Hunter"];
        //Archetype selector.
        public ObservableCollection<string> Archetypes {get;}  = ["Brawler", "Gunner", "Specialist", "Grenadier", "Paragon", "Bulwark"];
        //Stat selector.
        public ObservableCollection<string> Stats {get;}  = ["Health", "Melee", "Grenade", "Super", "Class", "Weapons"];
        //Subclass dropdown.
        public ObservableCollection<string> Subclasses {get;} = ["None", "Arc", "Solar", "Void", "Stasis", "Strand", "Prismatic"];
    }
}