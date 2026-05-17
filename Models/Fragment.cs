/*
*   FILE          : Fragment.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Defines Fragment model, including name, subclass,
*                   & any stat changes the fragment applies.
*/
namespace D2ArmorCalc {
    //Represents single stat change fragment applies (stat + value).
    public class StatChange {
        public Stat Stat {get;}
        public int Value {get;} //Positive = boost, Negative = reduction.
        public StatChange(Stat stat, int value) {
            Stat = stat;
            Value = value;
        }
    }
    //Represents single fragment.
    public class Fragment {
        public string Name {get;}
        public string Subclass {get;}
        public StatChange[] StatChanges {get;} //Empty if no stat changes.
        public Fragment(string name, string subclass, params StatChange[] statChanges) {
            Name = name;
            Subclass = subclass;
            StatChanges = statChanges;
        }
    }
}