/*
*   FILE          : ArmorMod.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Defines ArmorMod model representing general slot
*                   armor mod with slot restriction & energy cost,
*                   extending base Mod class.
*/
namespace D2ArmorCalc_Models {
    //Represents general slot armor mod locked to specific armor slot.
    public class ArmorMod(string name, int energyCost, ArmorSlot slot) : Mod(name, energyCost){
        public ArmorSlot Slot {get;} = slot;
    }
}