/*
*   FILE          : Font.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Defines Font model & ArmorSlot enum
*                   representing armor fonts & their slot restrictions.
*/
namespace D2ArmorCalc {
    //Enum representing which armor slot fonts are restricted to.
    public enum ArmorSlot {
        Helmet, Arms, Chestplate, Boots, ClassItem
    }
    //Holds data for single font type.
    public class Font {
        public Stat Stat {get;}
        public ArmorSlot Slot {get;}
        public int EnergyCost {get;}
        public Font(Stat stat, ArmorSlot slot){
            Stat = stat;
            Slot = slot;
            EnergyCost = 3;
        }
    }
}
