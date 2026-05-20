/*
*   FILE          : ComboGenerator.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Generates all valid armor piece combinations for
*                   4 legendary slots based on pruned archetype, tertiary,
*                   & focus options derived from user's stat targets.
*/
using D2ArmorCalc_Data;
using D2ArmorCalc_Helpers;
using D2ArmorCalc_Models;

namespace D2ArmorCalc_Algorithm {
    //Represents lightweight candidate armor configuration for piece.
    public class ArmorCandidate {
        public ArchetypeType Archetype {get;}
        public Stat Tertiary {get;}
        public Stat FocusStat {get;}
        public Stat FocusMinus {get;}
        public StatBlock BaseStats {get;}
        public ArmorCandidate(ArchetypeType archetype, Stat tertiary, Stat focusStat, Stat focusMinus){
            Archetype = archetype;
            Tertiary = tertiary;
            FocusStat = focusStat;
            FocusMinus = focusMinus;
            BaseStats = CalculateBaseStats();
        }
        /*
        Method        : CalculateBaseStats
        Description   : Calculates the base stat block for this candidate
                        based on archetype, tertiary, & focus assignments.
        Parameters    : None.
        Return Values : StatBlock : Base stats for this candidate piece.
        */
        private StatBlock CalculateBaseStats() {
            Archetype archetype = Archetypes.All[(int)Archetype];
            StatBlock stats = new();
            Stat[] allStats = [Stat.Health, Stat.Melee, Stat.Grenade,
                               Stat.Super, Stat.Class, Stat.Weapons];

            foreach (Stat stat in allStats) {
                if (stat == archetype.Primary) stats.Set(stat, 30);
                else if (stat == archetype.Secondary) stats.Set(stat, 25);
                else if (stat == Tertiary) stats.Set(stat, 20);
                else stats.Set(stat, 5); //masterwork.
            }

            //Focus (legendary only. no focus on exotics).
            stats.Set(FocusStat, stats.Get(FocusStat) + 5);
            stats.Set(FocusMinus, stats.Get(FocusMinus) - 5);

            return stats;
        }
    }
    public static class ComboGenerator {
        /*
        Method        : GetValidArchetypes
        Description   : Returns archetypes whose primary stat has non-zero
                        minimum target, pruning archetypes that cannot contribute
                        to any required stat.
        Parameters    : StatBlock mins  : Minimum stat targets.
        Return Values : List<Archetype> : Valid archetypes for consideration.
        */
        public static List<Archetype> GetValidArchetypes(StatBlock mins) {
            List<Archetype> primaryMatches = [];
            List<Archetype> secondaryMatches = [];

            foreach (Archetype archetype in Archetypes.All) {
                if (mins.Get(archetype.Primary) > 0) primaryMatches.Add(archetype);
                else if (mins.Get(archetype.Secondary) > 0) secondaryMatches.Add(archetype);
            }
            //Prefer primary matches. only use secondary matches if no primary matches exist.
            if (primaryMatches.Count > 0) return primaryMatches;
            if (secondaryMatches.Count > 0) return secondaryMatches;

            //No minimums set. allow all.
            return [..Archetypes.All]; //Technically Not possible since user must set at least one minimum, but just in case.
        }
        /*
        Method        : GetValidTertiaryStats
        Description   : Returns tertiary stat options for given archetype,
                        preferring stats with non-zero minimums. Falls back
                        to all valid tertiary options if none have minimums.
        Parameters    : Archetype archetype : The archetype to get tertiary options for.
                        StatBlock mins      : Minimum stat targets.
                      : Stat leastWanted    : Stat to avoid if possible (used for focus -5).
        Return Values : List<Stat>          : Valid tertiary stats for consideration.
        */
        public static List<Stat> GetValidTertiaryStats(Archetype archetype, StatBlock mins, Stat leastWanted) {
            Stat[] all = Archetypes.GetTertiaryStats(archetype);
            Stat[] order = [Stat.Health, Stat.Melee, Stat.Grenade,
                            Stat.Super,  Stat.Class,  Stat.Weapons];

            List<Stat> valid = [];

            //First try stats with non-zero minimums.
            foreach (Stat stat in all) {
                if (mins.Get(stat) > 0) valid.Add(stat);
            }
            if (valid.Count > 0) return valid;

            //Fall back. pick first valid tertiary top-down, skipping least wanted.
            foreach (Stat stat in order) {
                if (!all.Contains(stat))   continue;
                if (stat == leastWanted)   continue;
                return [stat];
            }

            //Last resort — if least wanted is the only option, use it.
            return [all[0]]; //Technically not possible but yknow, just in case.
        }
        /*
        Method        : GetValidFocusStats
        Description   : Returns focus stat options, only including stats with
                        non-zero minimums since +5 is wasted on unwanted stats.
                        Falls back to all stats if none have minimums.
        Parameters    : StatBlock mins : Minimum stat targets.
        Return Values : List<Stat>     : Valid focus stats for consideration.
        */
        public static List<Stat> GetValidFocusStats(StatBlock mins){
            List<Stat> valid = [];
            Stat[] all = [Stat.Health, Stat.Melee, Stat.Grenade, Stat.Super, Stat.Class, Stat.Weapons];

            foreach (Stat stat in all){
                if (mins.Get(stat) > 0) valid.Add(stat);
            }
            if (valid.Count == 0) valid.AddRange(all);
            return valid;
        }
        /*
        Method        : GenerateCandidates
        Description   : Generates all valid ArmorCandidate combinations for
                        single legendary armor piece based on pruned options.
        Parameters    : StatBlock mins       : Minimum stat targets.
                        Stat      focusMinus : Least wanted stat (always used for -5).
        Return Values : List<ArmorCandidate> : All valid candidates for one piece.
        */
        public static List<ArmorCandidate> GenerateCandidates(StatBlock mins, Stat focusMinus) {
            List<ArmorCandidate> candidates = [];
            List<Archetype> validArchetypes = GetValidArchetypes(mins);
            List<Stat> validFocusStats = GetValidFocusStats(mins);

            //Max stat boost from 4 major mods = +40 total.
            const int MaxModBoost = 40;

            foreach (Archetype archetype in validArchetypes) {
                List<Stat> validTertiaries = GetValidTertiaryStats(archetype, mins, focusMinus);
                foreach (Stat tertiary in validTertiaries) {
                    //Check if mods alone can cover all deficits for this archetype+tertiary.
                    StatBlock baseForCheck = new();
                    baseForCheck.Set(archetype.Primary, 30);
                    baseForCheck.Set(archetype.Secondary, 25);
                    baseForCheck.Set(tertiary, 20);

                    bool modsCanCover = StatHelper.GetTotalDeficit(baseForCheck, mins) <= MaxModBoost;

                    //Always add no-focus candidate.
                    candidates.Add(new ArmorCandidate(archetype.Type, tertiary, focusMinus, focusMinus));

                    //Only add focus candidates if mods alone can't cover deficit.
                    if (!modsCanCover) {
                        foreach (Stat focusStat in validFocusStats) {
                            if (focusStat == focusMinus) continue;
                            candidates.Add(new ArmorCandidate(archetype.Type, tertiary, focusStat, focusMinus));
                        }
                    }
                }
            }
            return candidates;
        }
        /*
        Method        : GenerateAllCombinations
        Description   : Generates all 4-piece legendary combinations from
                        candidate list as arrays of 4 ArmorCandidates.
        Parameters    : List<ArmorCandidate> candidates : All valid single-piece candidates.
        Return Values : List<ArmorCandidate[]>          : All 4-piece combinations.
        */
        public static List<ArmorCandidate[]> GenerateAllCombinations(List<ArmorCandidate> candidates){
            List<ArmorCandidate[]> combinations = [];
            int count = candidates.Count;

            for (int a = 0; a < count; a++)
                for (int b = 0; b < count; b++)
                    for (int c = 0; c < count; c++)
                        for (int d = 0; d < count; d++){
                            combinations.Add([candidates[a], candidates[b], candidates[c], candidates[d]]);
                        }

            return combinations;
        }
    }
}