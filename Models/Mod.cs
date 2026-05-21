/*
*   FILE          : Mod.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Defines base Mod class representing any armor mod
*                   with name & energy cost.
*/
namespace D2ArmorCalc_Models {
    public class Mod(string name, int energyCost){
        public string Name {get;} = name;
        public int EnergyCost {get;} = energyCost;
    }
}