/*
*   FILE          : Font.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Defines Font model & ArmorSlot enum
*                   representing armor fonts & their slot restrictions.
*/
using D2ArmorCalc_Models;

namespace D2ArmorCalc_Models {
    //Enum representing which armor slot fonts are restricted to.
    public enum ArmorSlot {
        Helmet, Arms, Chestplate, Boots, ClassItem
    }
    //Holds data for single font type.
    public class Font(Stat stat, ArmorSlot slot){
        public Stat Stat {get;} = stat;
        public ArmorSlot Slot {get;} = slot;
        public int EnergyCost {get;} = 3;
    }
}
