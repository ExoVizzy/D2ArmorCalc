/*
*   FILE          : ArmorMod.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Defines ArmorMod model representing general slot
*                   armor mod with slot restriction & energy cost,
*                   extending base Mod class.
*/
namespace D2ArmorCalc {
    //Represents general slot armor mod locked to specific armor slot.
    public class ArmorMod : Mod {
        public ArmorSlot Slot {get;}
        public ArmorMod(string name, int energyCost, ArmorSlot slot)
            : base(name, energyCost){
            Slot = slot;
        }
    }
}