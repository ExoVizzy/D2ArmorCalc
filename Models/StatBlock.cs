/*
*   FILE          : StatBlock.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Defines StatBlock model representing a complete set
*                   of all 6 armor stats, with arithmetic & helper methods.
*/
namespace D2ArmorCalc {
    //Represents a complete set of all 6 armor stats.
    public class StatBlock {
        public int Health {get; set;}
        public int Melee {get; set;}
        public int Grenade {get; set;}
        public int Super {get; set;}
        public int Class {get; set;}
        public int Weapons {get; set;}

        public StatBlock(int health, int melee, int grenade, int super, int classVal, int weapons) {
            Health  = health;
            Melee   = melee;
            Grenade = grenade;
            Super   = super;
            Class   = classVal;
            Weapons = weapons;
        }
        //Empty StatBlock defaulting all stats to 0.
        public StatBlock() { }
        /*
        Method        : Get
        Description   : Returns value of specific stat by Stat enum.
        Parameters    : Stat stat : Stat to retrieve.
        Return Values : int       : Value of requested stat.
        */
        public int Get(Stat stat) {
            switch (stat) {
                case Stat.Health: return Health;
                case Stat.Melee: return Melee;
                case Stat.Grenade: return Grenade;
                case Stat.Super: return Super;
                case Stat.Class: return Class;
                case Stat.Weapons: return Weapons;
                default: return 0;
            }
        }
        /*
        Method        : Set
        Description   : Sets value of specific stat by Stat enum.
        Parameters    : Stat stat : Stat to set.
                        int value : Value to assign.
        Return Values : void
        */
        public void Set(Stat stat, int value) {
            switch (stat) {
                case Stat.Health: Health = value; break;
                case Stat.Melee: Melee = value; break;
                case Stat.Grenade: Grenade = value; break;
                case Stat.Super: Super = value; break;
                case Stat.Class: Class = value; break;
                case Stat.Weapons: Weapons = value; break;
            }
        }
        /*
        Method        : Add
        Description   : Returns new StatBlock with another StatBlock's
                        values added to this one.
        Parameters    : StatBlock other : StatBlock to add.
        Return Values : StatBlock       : New StatBlock with summed values.
        */
        public StatBlock Add(StatBlock other) {
            return new StatBlock(
                Health + other.Health, Melee + other.Melee, Grenade + other.Grenade,
                Super + other.Super, Class + other.Class, Weapons + other.Weapons
            );
        }
        /*
        Method        : MeetsMinimums
        Description   : Checks whether all stats in this StatBlock meet or
                        exceed minimums defined in another StatBlock.
        Parameters    : StatBlock mins : Minimum stat targets to check against.
        Return Values : bool           : True if all stats meet minimums, false otherwise.
        */
        public bool MeetsMinimums(StatBlock mins) {
            return Health >= mins.Health && Melee >= mins.Melee && Grenade >= mins.Grenade &&
                   Super >= mins.Super && Class >= mins.Class && Weapons >= mins.Weapons;
        }

        /*
        Method        : MeetsMaximums
        Description   : Checks whether all stats in this StatBlock are at or
                        below the maximums defined in another StatBlock.
                        Ignores stats where the maximum is 0 (not set).
        Parameters    : StatBlock maxs : The maximum stat targets to check against.
        Return Values : bool           : True if all set stats are within maximums.
        */
        public bool MeetsMaximums(StatBlock maxs) {
            return (maxs.Health == 0 || Health <= maxs.Health) && (maxs.Melee == 0 || Melee <= maxs.Melee) &&
                   (maxs.Grenade == 0 || Grenade <= maxs.Grenade) && (maxs.Super == 0 || Super <= maxs.Super) &&
                   (maxs.Class == 0 || Class <= maxs.Class) && (maxs.Weapons == 0 || Weapons <= maxs.Weapons);
        }
        /*
        Method        : ApplyFragments
        Description   : Returns new StatBlock with fragment stat changes
                        applied from array of selected fragments.
        Parameters    : Fragment[] fragments : Fragments to apply.
        Return Values : StatBlock            : New StatBlock with fragment bonuses applied.
        */
        public StatBlock ApplyFragments(Fragment[] fragments) {
            StatBlock result = new StatBlock(Health, Melee, Grenade, Super, Class, Weapons);
            foreach (var fragment in fragments) {
                foreach (var change in fragment.StatChanges) {
                    result.Set(change.Stat, result.Get(change.Stat) + change.Value);
                }
            }
            return result;
        }
        /*
        Method        : GetOverflow
        Description   : Returns new StatBlock containing only the amount
                        each stat exceeds 100, for UI display purposes.
        Parameters    : None.
        Return Values : StatBlock : Overflow values per stat (0 if stat is <= 100).
        */
        public StatBlock GetOverflow() {
            return new StatBlock(
                Health > 100 ? Health - 100 : 0, Melee > 100 ? Melee - 100 : 0,
                Grenade > 100 ? Grenade - 100 : 0, Super > 100 ? Super - 100 : 0,
                Class > 100 ? Class - 100 : 0, Weapons > 100 ? Weapons - 100 : 0
            );
        }
    }
}