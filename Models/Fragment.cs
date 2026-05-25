/*
*   FILE          : Fragment.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Defines Fragment model, including name, subclass,
*                   & any stat changes fragment applies.
*/
using D2ArmorCalc_Models;

namespace D2ArmorCalc_Models {
    //Represents single stat change fragment applies (stat + value).
    public class StatChange(Stat stat, int value){
        public Stat Stat {get;} = stat;
        public int Value {get;} = value;
    }
    //Represents single fragment.
    public class Fragment {
        public string Name {get;}
        public string Subclass {get;}
        public StatChange[] StatChanges {get;}
        public PlayerClass? Class {get;}
        public Fragment(string name, string subclass, params StatChange[] statChanges){
            Name = name;
            Subclass = subclass;
            Class = null;
            StatChanges = statChanges;
        }
        //Represents single class dependant fragment.
        public Fragment(string name, string subclass, PlayerClass? playerClass, params StatChange[] statChanges){
            Name = name;
            Subclass = subclass;
            Class = playerClass;
            StatChanges = statChanges;
        }
    }
}