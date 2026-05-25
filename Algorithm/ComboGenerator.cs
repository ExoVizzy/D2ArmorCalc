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
        private StatBlock CalculateBaseStats(){
            Archetype archetype = Archetypes.AllArchetypes[(int)Archetype];
            StatBlock stats = new();
            foreach (Stat stat in GameConstants.StatOrder){
                if (stat == archetype.Primary) stats.Set(stat, 30);
                else if (stat == archetype.Secondary) stats.Set(stat, 25);
                else if (stat == Tertiary) stats.Set(stat, 20);
                else stats.Set(stat, 5); //Masterwork.
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
        public static List<Archetype> GetValidArchetypes(StatBlock mins){
            List<Archetype> primaryMatches = [];
            List<Archetype> secondaryMatches = [];

            foreach (Archetype archetype in Archetypes.AllArchetypes){
                if (mins.Get(archetype.Primary) > 0) primaryMatches.Add(archetype);
                else if (mins.Get(archetype.Secondary) > 0) secondaryMatches.Add(archetype);
            }
            //Prefer primary matches. only use secondary matches if no primary matches exist.
            if (primaryMatches.Count > 0) return primaryMatches;
            if (secondaryMatches.Count > 0) return secondaryMatches;
            //No minimums set. allow all.
            return [..Archetypes.AllArchetypes]; //Technically Not possible since user must set at least one minimum, but just in case.
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
        public static List<Stat> GetValidTertiaryStats(Archetype archetype, StatBlock mins, Stat leastWanted){
            Stat[] all = Archetypes.GetTertiaryStats(archetype);
            List<Stat> valid = [];
            //First try stats with non-zero minimums.
            foreach (Stat stat in all){
                if (mins.Get(stat) > 0) valid.Add(stat);
            }
            if (valid.Count > 0) return valid;
            //Fall back. pick first valid tertiary top-down, skipping least wanted.
            foreach (Stat stat in GameConstants.StatOrder){
                if (!all.Contains(stat)) continue;
                if (stat == leastWanted) continue;
                return [stat];
            }
            //Last resort — if least wanted is the only option, use it.
            return [all[0]]; //Technically not possible but yknow, just in case.
        }
        /*
        Method        : GetValidFocusStats
        Description   : Returns focus stat options, only including stats with
                        non-zero minimums since +5 is wasted on unwanted stats.
                        Falls back to all stats if no minimums.
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
        public static List<ArmorCandidate> GenerateCandidates(StatBlock mins, Stat focusMinus){
            List<ArmorCandidate> candidates = [];
            List<Archetype> validArchetypes = GetValidArchetypes(mins);
            List<Stat> validFocusStats = GetValidFocusStats(mins);

            //Max stat boost from 4 major mods = +40 total.
            const int MaxModBoost = 40;

            foreach (Archetype archetype in validArchetypes){
                List<Stat> validTertiaries = GetValidTertiaryStats(archetype, mins, focusMinus);
                foreach (Stat tertiary in validTertiaries){
                    //Check if mods alone can cover all deficits for this archetype+tertiary.
                    StatBlock baseForCheck = new();
                    baseForCheck.Set(archetype.Primary, 30);
                    baseForCheck.Set(archetype.Secondary, 25);
                    baseForCheck.Set(tertiary, 20);

                    bool modsCanCover = StatHelper.GetTotalDeficit(baseForCheck, mins) <= MaxModBoost;
                    //Always add no-focus candidate.
                    candidates.Add(new ArmorCandidate(archetype.Type, tertiary, focusMinus, focusMinus));
                    //Only add focus candidates if mods alone can't cover deficit.
                    if (!modsCanCover){
                        foreach (Stat focusStat in validFocusStats){
                            if (focusStat == focusMinus) continue;
                            candidates.Add(new ArmorCandidate(archetype.Type, tertiary, focusStat, focusMinus));
                        }
                    }
                }
            }
            return [.. candidates.Where(c => {
                    Archetype arch = Archetypes.AllArchetypes[(int)c.Archetype];
                    return mins.Get(arch.Primary) >= 30;
                }).GroupBy(c => (c.Archetype, c.Tertiary, c.FocusStat, c.FocusMinus)).Select(g => g.First())];
        }
        /*
        Method        : GenerateCandidatesNoFocus
        Description   : Generates candidates without any focus stat variation.
                        Used when custom tuning is enabled so algorithm
                        doesnt search focus combinations.
        Parameters    : StatBlock mins       : Minimum stat targets.
                        Stat      focusMinus : Least wanted stat.
        Return Values : List<ArmorCandidate> : Candidates with no focus applied.
        */
        public static List<ArmorCandidate> GenerateCandidatesNoFocus(StatBlock mins, Stat focusMinus){
            List<ArmorCandidate> candidates = [];
            List<Archetype> validArchetypes = GetValidArchetypes(mins);

            foreach (Archetype archetype in validArchetypes){
                List<Stat> validTertiaries = GetValidTertiaryStats(archetype, mins, focusMinus);
                //Only add no-focus candidate (focusStat == focusMinus = net zero).
                foreach (Stat tertiary in validTertiaries) candidates.Add(new ArmorCandidate(archetype.Type, tertiary, focusMinus, focusMinus));
            }
            return [.. candidates.GroupBy(c => (c.Archetype, c.Tertiary)).Select(g => g.First())];
        }
        /*
        Method        : GetBestPossibleContribution
        Description   : Returns StatBlock representing maximum any single
                        candidate can contribute to each stat. Used by
                        feasibility check to prune impossible branches early.
        Parameters    : List<ArmorCandidate> candidates : All valid candidates.
        Return Values : StatBlock                       : Per-stat maximum contribution.
        */
        public static StatBlock GetBestPossibleContribution(List<ArmorCandidate> candidates){
            StatBlock best = new();
            foreach (ArmorCandidate c in candidates){
                foreach (Stat stat in GameConstants.StatOrder){
                    if (c.BaseStats.Get(stat) > best.Get(stat)) best.Set(stat, c.BaseStats.Get(stat));
                }
            }
            return best;
        }
        /*
        Method        : CouldPossiblyMeetMins
        Description   : Checks whether remaining slots could possibly cover
                        deficit in running stat total, given best
                        contribution any single candidate can make per stat.
                        Used to prune impossible branches during combo search.
        Parameters    : StatBlock running     : Accumulated stats so far.
                        StatBlock adjMins     : Minimum targets to meet.
                        StatBlock bestPerSlot : Max any one candidate contributes per stat.
                        int       slotsLeft   : Number of slots still to be filled.
        Return Values : bool                  : True if meeting minimums is still possible.
        */
        public static bool CouldPossiblyMeetMins(StatBlock running, StatBlock adjMins, StatBlock bestPerSlot, int slotsLeft, int maxModBoost){
            foreach (Stat stat in GameConstants.StatOrder){
                int deficit = Math.Max(0, adjMins.Get(stat) - running.Get(stat));
                if (deficit == 0) continue;
                int maxPossible = (bestPerSlot.Get(stat) * slotsLeft) + maxModBoost;
                if (maxPossible < deficit) return false;
            }
            return true;
        }
        /*
        Method        : SearchCombinations
        Description   : Recursively fills 4 legendary slots one at a time,
                        pruning branches where remaining slots cannot possibly
                        meet adjusted minimums. Dramatically reduces
                        combinations evaluated vs brute-force O(n^4) approach.
        Parameters    : List<ArmorCandidate>   candidates  : All valid single-piece candidates.
                        ArmorCandidate?[]      current     : Current combo being built (4 slots).
                        int                    slot        : Current slot index being filled (0-3).
                        StatBlock              running     : Accumulated base stats so far.
                        StatBlock              adjMins     : Minimum targets adjusted for bonuses.
                        StatBlock              bestPerSlot : Max per-stat contribution any candidate makes.
                        int                    maxModBoost : Maximum total stat boost mods can provide.
                        List<ArmorCandidate[]> results     : Collected valid combinations.
        Return Values : void
        */
        public static void SearchCombinations(List<ArmorCandidate> candidates, ArmorCandidate?[] current,
                                              int slot, StatBlock running, StatBlock adjMins,
                                              StatBlock bestPerSlot, int maxModBoost,
                                              List<ArmorCandidate[]> results){
            if (slot == 4){
                //All slots filled. check if mods can cover any remaining deficit.
                int deficit = StatHelper.GetTotalDeficit(running, adjMins);
                if (deficit <= maxModBoost) results.Add([current[0]!, current[1]!, current[2]!, current[3]!]);
                return;
            }
            int slotsRemaining = 4 - slot;

            foreach (ArmorCandidate candidate in candidates){
                StatBlock next = running.Add(candidate.BaseStats);

                //Feasibility check: prune if remaining slots can't cover deficit.
                if (!CouldPossiblyMeetMins(next, adjMins, bestPerSlot, slotsRemaining - 1, maxModBoost)) continue;

                current[slot] = candidate;
                SearchCombinations(candidates, current, slot + 1, next, adjMins, bestPerSlot, maxModBoost, results);
            }
        }
        /*
        Method        : SearchCombinations
        Description   : Recursively fills 4 legendary slots one at a time,
                        pruning branches where remaining slots cannot possibly
                        meet adjusted minimums or satisfy required archetype
                        counts. Dramatically reduces combinations evaluated vs
                        brute-force O(n^4) approach.
        Parameters    : List<ArmorCandidate>          candidates  : All valid single-piece candidates.
                        ArmorCandidate?[]             current     : Current combo being built (4 slots).
                        int                           slot        : Current slot index being filled (0-3).
                        StatBlock                     running     : Accumulated base stats so far.
                        StatBlock                     adjMins     : Minimum targets adjusted for bonuses.
                        StatBlock                     bestPerSlot : Max per-stat contribution any candidate makes.
                        int                           maxModBoost : Maximum total stat boost mods can provide.
                        List<ArmorCandidate[]>        results     : Collected valid combinations.
                        Dictionary<ArchetypeType,int> reqCounts   : Min pieces needed per archetype.
                        Dictionary<ArchetypeType,int> currCounts  : Archetype counts placed so far.
        Return Values : void
        */
        public static void SearchCombinations(List<ArmorCandidate> candidates, ArmorCandidate?[] current,
                                              int slot, StatBlock running, StatBlock adjMins,
                                              StatBlock bestPerSlot, int maxModBoost, List<ArmorCandidate[]> results,
                                              Dictionary<ArchetypeType, int> reqCounts,
                                              Dictionary<ArchetypeType, int> currCounts){
            if (slot == 4){
                //Check all required archetype counts are met.
                foreach (var kvp in reqCounts){
                    currCounts.TryGetValue(kvp.Key, out int count);
                    if (count < kvp.Value) return;
                }
                int deficit = StatHelper.GetTotalDeficit(running, adjMins);
                if (deficit <= maxModBoost) results.Add([current[0]!, current[1]!, current[2]!, current[3]!]);
                return;
            }
            int slotsRemaining = 4 - slot;

            foreach (ArmorCandidate candidate in candidates){
                StatBlock next = running.Add(candidate.BaseStats);
                //Stat feasibility check.
                if (!CouldPossiblyMeetMins(next, adjMins, bestPerSlot, slotsRemaining - 1, maxModBoost)) continue;

                //Archetype feasibility check. can remaining slots still satisfy required counts?
                bool archetypeFeasible = true;
                foreach (var kvp in reqCounts){
                    currCounts.TryGetValue(kvp.Key, out int currentCount);
                    int willHave = currentCount + (candidate.Archetype == kvp.Key ? 1 : 0);
                    int stillNeeded = kvp.Value - willHave;
                    if (stillNeeded > slotsRemaining - 1){archetypeFeasible = false; break;}
                }
                if (!archetypeFeasible) continue;
                //Update counts & recurse.
                currCounts.TryGetValue(candidate.Archetype, out int existingCount);
                currCounts[candidate.Archetype] = existingCount + 1;
                current[slot] = candidate;

                SearchCombinations(candidates, current, slot + 1, next, adjMins, bestPerSlot, maxModBoost, results, reqCounts, currCounts);
                //Backtrack.
                currCounts[candidate.Archetype] = existingCount;
            }
        }
    }
}