/*
*   FILE          : Mod.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Defines base Mod class representing any armor mod
*                   with name & energy cost.
*/
namespace D2ArmorCalc {
    public class Mod {
        public string Name {get;}
        public int EnergyCost {get;}
        public Mod(string name, int energyCost){
            Name = name;
            EnergyCost = energyCost;
        }
    }
}