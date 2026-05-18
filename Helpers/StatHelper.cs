/*
*   FILE          : StatHelper.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Provides stat math utilities including overflow calculation,
*                   stat deficit scoring, clamping, & 100+ buff calculations
*                   for all 6 armor stats.
*/
namespace D2ArmorCalc {
    public static class StatHelper {
        //=====================================================================
        //Constants.
        //=====================================================================
        public const int StatMin = 0;
        public const int StatMax = 200;
        public const int BuffThreshold = 100; //Buff only applies above this value
        //=====================================================================
        //General Stat Math.
        //=====================================================================
        /*
        Method        : Clamp
        Description   : Clamps stat value between valid stat range of 0 & 200.
        Parameters    : int value : Stat value to clamp.
        Return Values : int       : Clamped stat value.
        */
        public static int Clamp(int value){
            return Math.Max(StatMin, Math.Min(StatMax, value));
        }
        /*
        Method        : GetOverflow
        Description   : Returns amount a stat value exceeds 100.
                        Returns 0 if the value is 100 or below.
        Parameters    : int value : Stat value to check.
        Return Values : int       : Overflow amount above 100.
        */
        public static int GetOverflow(int value){
            return value > BuffThreshold ? value - BuffThreshold : 0;
        }
        /*
        Method        : GetDeficit
        Description   : Returns how far stat value falls short of minimum
                        target. Returns 0 if value meets or exceeds target.
        Parameters    : int value  : Current stat value.
                        int target : Minimum target to reach.
        Return Values : int        : Deficit amount below the target.
        */
        public static int GetDeficit(int value, int target){
            return value < target ? target - value : 0;
        }
        /*
        Method        : GetTotalDeficit
        Description   : Returns total deficit across all stats in
                        StatBlock against set of minimum targets.
                        Used by algorithm to score & rank builds.
        Parameters    : StatBlock current : Current stat totals.
                        StatBlock mins    : Minimum stat targets.
        Return Values : int               : Sum of all stat deficits.
        */
        public static int GetTotalDeficit(StatBlock current, StatBlock mins){
            int total = 0;
            total += GetDeficit(current.Health, mins.Health);
            total += GetDeficit(current.Melee, mins.Melee);
            total += GetDeficit(current.Grenade, mins.Grenade);
            total += GetDeficit(current.Super, mins.Super);
            total += GetDeficit(current.Class, mins.Class);
            total += GetDeficit(current.Weapons, mins.Weapons);
            return total;
        }
        /*
        Method        : GetTotalExcess
        Description   : Returns total amount all stats exceed their maximum
                        targets in StatBlock. Ignores stats with max of 0.
                        Used by algorithm to score & rank builds.
        Parameters    : StatBlock current : Current stat totals.
                        StatBlock maxs    : Maximum stat targets.
        Return Values : int               : Sum of all stat excesses over max.
        */
        public static int GetTotalExcess(StatBlock current, StatBlock maxs){
            int total = 0;
            if (maxs.Health > 0) total += Math.Max(0, current.Health - maxs.Health);
            if (maxs.Melee > 0) total += Math.Max(0, current.Melee - maxs.Melee);
            if (maxs.Grenade > 0) total += Math.Max(0, current.Grenade - maxs.Grenade);
            if (maxs.Super > 0) total += Math.Max(0, current.Super - maxs.Super);
            if (maxs.Class > 0) total += Math.Max(0, current.Class - maxs.Class);
            if (maxs.Weapons > 0) total += Math.Max(0, current.Weapons - maxs.Weapons);
            return total;
        }
        //=====================================================================
        //100+ Buff Calculations.
        //=====================================================================
        /*
        Method        : GetHealthBuff
        Description   : Calculates Health stat 100+ buff values scaled
                        linearly from 0 at 100 to max at 200.
                        Max buff at 200: +45% shield recharge rate, +20 Shield HP.
        Parameters    : int statValue : Current Health stat value.
        Return Values : string        : Formatted buff string, or empty if no buff.
        */
        public static string GetHealthBuff(int statValue){
            int overflow = GetOverflow(statValue);
            if (overflow == 0) return string.Empty;
            double scale = overflow / 100.0;
            double rechargeRate = Math.Round(45.0 * scale, 1);
            double shieldHp = Math.Round(20.0 * scale, 1);
            return $"+{rechargeRate}% Shield Recharge Rate, +{shieldHp} Shield HP";
        }
        /*
        Method        : GetMeleeBuff
        Description   : Calculates Melee stat 100+ buff value scaled
                        linearly from 0 at 100 to max at 200.
                        Max buff at 200: +30% Melee Damage.
        Parameters    : int statValue : Current Melee stat value.
        Return Values : string        : Formatted buff string, or empty if no buff.
        */
        public static string GetMeleeBuff(int statValue){
            int overflow = GetOverflow(statValue);
            if (overflow == 0) return string.Empty;
            double scale = overflow / 100.0;
            double damage = Math.Round(30.0 * scale, 1);
            return $"+{damage}% Melee Damage";
        }
        /*
        Method        : GetGrenadeBuff
        Description   : Calculates Grenade stat 100+ buff value scaled
                        linearly from 0 at 100 to max at 200.
                        Max buff at 200: +65% Grenade Damage.
        Parameters    : int statValue : Current Grenade stat value.
        Return Values : string        : Formatted buff string, or empty if no buff.
        */
        public static string GetGrenadeBuff(int statValue){
            int overflow = GetOverflow(statValue);
            if (overflow == 0) return string.Empty;
            double scale = overflow / 100.0;
            double damage = Math.Round(65.0 * scale, 1);
            return $"+{damage}% Grenade Damage";
        }
        /*
        Method        : GetSuperBuff
        Description   : Calculates Super stat 100+ buff value scaled
                        linearly from 0 at 100 to max at 200.
                        Max buff at 200: +45% Super Damage.
        Parameters    : int statValue : Current Super stat value.
        Return Values : string        : Formatted buff string, or empty if no buff.
        */
        public static string GetSuperBuff(int statValue){
            int overflow = GetOverflow(statValue);
            if (overflow == 0) return string.Empty;
            double scale = overflow / 100.0;
            double damage = Math.Round(45.0 * scale, 1);
            return $"+{damage}% Super Damage";
        }
        /*
        Method        : GetClassBuff
        Description   : Calculates Class stat 100+ buff value scaled
                        linearly from 0 at 100 to max at 200.
                        Max buff at 200: 40 HP Overshield on class ability use.
        Parameters    : int statValue : Current Class stat value.
        Return Values : string        : Formatted buff string, or empty if no buff.
        */
        public static string GetClassBuff(int statValue){
            int overflow = GetOverflow(statValue);
            if (overflow == 0) return string.Empty;
            double scale = overflow / 100.0;
            double overshield = Math.Round(40.0 * scale, 1);
            return $"+{overshield} HP Overshield on Class Ability Use";
        }
        /*
        Method        : GetWeaponsBuff
        Description   : Calculates Weapons stat 100+ buff values scaled
                        linearly from 0 at 100 to max at 200.
                        Max buff at 200: +15% primary/special dmg to bosses,
                        +10% heavy ammo dmg to bosses.
        Parameters    : int statValue : Current Weapons stat value.
        Return Values : string        : Formatted buff string, or empty if no buff.
        */
        public static string GetWeaponsBuff(int statValue){
            int overflow = GetOverflow(statValue);
            if (overflow == 0) return string.Empty;
            double scale = overflow / 100.0;
            double primaryDmg = Math.Round(15.0 * scale, 1);
            double heavyDmg = Math.Round(10.0 * scale, 1);
            return $"+{primaryDmg}% Primary/Special Dmg to Bosses, +{heavyDmg}% Heavy Dmg to Bosses";
        }
        /*
        Method        : GetBuff
        Description   : Returns 100+ buff string for any stat by Stat enum.
                        Useful for iterating over all stats in the UI.
        Parameters    : Stat stat     : Stat to get the buff for.
                        int statValue : Current value of that stat.
        Return Values : string        : Formatted buff string, or empty if no buff.
        */
        public static string GetBuff(Stat stat, int statValue){
            switch (stat){
                case Stat.Health: return GetHealthBuff(statValue);
                case Stat.Melee: return GetMeleeBuff(statValue);
                case Stat.Grenade: return GetGrenadeBuff(statValue);
                case Stat.Super: return GetSuperBuff(statValue);
                case Stat.Class: return GetClassBuff(statValue);
                case Stat.Weapons: return GetWeaponsBuff(statValue);
                default: return string.Empty;
            }
        }
    }
}