/*
*   FILE          : Subclass.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Defines Subclass model, PlayerClass enum, &
*                   Aspect model used to represent subclass configurations.
*/
namespace D2ArmorCalc_Models {
    //Player classes
    public enum PlayerClass {
        Warlock, Titan, Hunter
    }
    //Represents aspect & how many fragment slots it provides.
    public class Aspect(string name, int fragmentSlots) {
        public string Name { get; } = name;
        public int FragmentSlots { get; } = fragmentSlots;
    }
    //Holds all subclass data for specific class + subclass combination.
    public class Subclass(string name, PlayerClass playerClass, Fragment[] fragments,
            Aspect[] aspects, string[] supers, string[] melees,
            string[] grenades, string[] classAbilities, string[] jumps) {
        public string Name { get; } = name;
        public PlayerClass Class { get; } = playerClass;
        public Fragment[] Fragments { get; } = fragments;
        public Aspect[] Aspects { get; } = aspects;
        public string[] Supers { get; } = supers;
        public string[] Melees { get; } = melees;
        public string[] Grenades { get; } = grenades;
        public string[] ClassAbilities { get; } = classAbilities;
        public string[] Jumps { get; } = jumps;
    }
}