/*
*   FILE          : Fragment.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Defines Fragment model, including name, subclass,
*                   & any stat changes the fragment applies.
*/
using D2ArmorCalc_Models;

namespace D2ArmorCalc_Models {
    //Represents single stat change fragment applies (stat + value).
    public class StatChange(Stat stat, int value) {
        public Stat Stat { get; } = stat;
        public int Value { get; } = value;
    }
    //Represents single fragment.
    public class Fragment(string name, string subclass, params StatChange[] statChanges) {
        public string Name { get; } = name;
        public string Subclass { get; } = subclass;
        public StatChange[] StatChanges { get; } = statChanges;
    }
}