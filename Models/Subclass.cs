/*
*   FILE          : Subclass.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Defines Subclass model, PlayerClass enum, &
*                   Aspect model used to represent subclass configurations.
*/
namespace D2ArmorCalc {
    //Player classes
    public enum PlayerClass {
        Warlock, Titan, Hunter
    }
    //Represents aspect & how many fragment slots it provides.
    public class Aspect {
        public string Name {get;}
        public int FragmentSlots {get;}
        public Aspect(string name, int fragmentSlots) {
            Name = name;
            FragmentSlots = fragmentSlots;
        }
    }
    //Holds all subclass data for specific class + subclass combination.
    public class Subclass {
        public string Name {get;}
        public PlayerClass Class {get;}
        public Fragment[] Fragments {get;}
        public Aspect[] Aspects {get;}
        public string[] Supers {get;}
        public string[] Melees {get;}
        public string[] Grenades {get;}
        public string[] ClassAbilities {get;}
        public string[] Jumps {get;}
        public Subclass(string name, PlayerClass playerClass, Fragment[] fragments,
                Aspect[] aspects, string[] supers, string[] melees,
                string[] grenades, string[] classAbilities, string[] jumps){
            Name = name;
            Class = playerClass;
            Fragments = fragments;
            Aspects = aspects;
            Supers = supers;
            Melees = melees;
            Grenades = grenades;
            ClassAbilities = classAbilities;
            Jumps = jumps;
        }
    }
}