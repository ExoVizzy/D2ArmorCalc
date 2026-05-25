/*
*   FILE          : StatMod.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Defines StatMod model representing stat-boosting
*                   armor mod, either major (+10) or minor (+5), extending
*                   base Mod class.
*/
using D2ArmorCalc_Models;

namespace D2ArmorCalc_Models {
    //Enum representing two types of stat mods (Major: +10 Stat & Cost 3 Energy, Minor: +5 Stat & Cost 1 Energy).
    public enum ModType {
        Major, Minor
    }
    //Holds data for single stat mod instance (type + boosted stat).
    public class StatMod(ModType modType, Stat stat) : Mod(modType == ModType.Major ? "Major " + stat : "Minor " + stat, modType == ModType.Major ? 3 : 1){
        public ModType ModType {get;} = modType;
        public Stat Stat {get;} = stat;
        public int Bonus => ModType == ModType.Major ? 10 : 5;
    }
}