/*
*   FILE          : StatResolver.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Applies stat mods to armor combinations & resolves
*                   final stat totals against user targets.
*/
using D2ArmorCalc_Data;
using D2ArmorCalc_Helpers;
using D2ArmorCalc_Models;

namespace D2ArmorCalc_Algorithm {
    public static class StatResolver {
        /*
        Method        : ResolveBaseStats
        Description   : Sums raw base stats of all 5 armor pieces.
                        Does not include tuning, fonts, or fragments.
        Parameters    : ArmorCandidate[] legendaries : 4 legendary candidates.
                        ArmorPiece       exotic      : Fixed exotic piece.
        Return Values : StatBlock                    : Combined base stats of all 5 pieces.
        */
        public static StatBlock ResolveBaseStats(ArmorCandidate[] legendaries, ArmorPiece exotic){
            StatBlock total = exotic.IsCustomRoll && exotic.CustomStatBlock != null ? exotic.CustomStatBlock : exotic.GetAllStats();

            foreach(ArmorCandidate piece in legendaries) total = total.Add(piece.BaseStats);

            return total;
        }

        /*
        Method        : ApplyStatMods
        Description   : Assigns best stat mod per slot based on remaining
                        deficit against adjusted minimums. Respects energy
                        constraints from fonts & advanced mods.
        Parameters    : StatBlock                  current       : Current armor stat totals.
                        StatBlock                  adjMins       : Targets with user bonuses subtracted.
                        StatBlock                  adjMaxs       : Stat ceiling with bonuses subtracted.
                        Stat                       leastWanted   : Stat to avoid modding.
                        Dictionary<ArmorSlot, int> fontCounts    : Font counts per slot.
                        Dictionary<ArmorSlot, int> advModPower   : Advanced armor mod energy per slot.
                        int                        minorModCount : Number of minor mods allowed.
                        Dictionary<Stat, int>      perStatFonts  : Fonts counts per slot.
        Return Values : StatBlock                                : Stats after mods applied.
        */
        public static StatBlock ApplyStatMods(StatBlock current, StatBlock adjMins, StatBlock adjMaxs, 
                                              Stat leastWanted, Dictionary<ArmorSlot, int> fontCounts,
                                              Dictionary<ArmorSlot, int> advModPower, int minorModCount,
                                              Dictionary<Stat, int> perStatFonts){
            StatBlock result = new(current.Health, current.Melee, current.Grenade, current.Super, current.Class, current.Weapons);

            int minorsRemaining = minorModCount;
            int majorsRemaining = 5 - minorModCount;

            for (int i = 0; i < GameConstants.SlotOrder.Length; i++){
                Stat bestStat = leastWanted;
                int bestDeficit = 0;

                foreach (Stat stat in GameConstants.StatOrder){
                    if (stat == leastWanted) continue;
                    if (adjMins.Get(stat) == 0) continue;
                    int deficit = StatHelper.GetDeficit(result.Get(stat), adjMins.Get(stat));
                    if (deficit > bestDeficit){bestDeficit = deficit; bestStat = stat;}
                }
                if (bestStat == leastWanted || bestDeficit == 0) break;

                int fontCount = fontCounts.TryGetValue(GameConstants.SlotOrder[i], out int fc) ? fc : 0;
                int advModEnergy = advModPower.TryGetValue(GameConstants.SlotOrder[i], out int ae) ? ae : 0;
                int remainingEnergy = EnergyHelper.GetRemainingGeneralEnergy(ArmorRarity.Legendary, fontCount) - advModEnergy;
                bool majorFits = remainingEnergy >= EnergyHelper.MajorModEnergyCost;
                bool minorFits = remainingEnergy >= EnergyHelper.MinorModEnergyCost;

                if (!minorFits) continue;

                bool energyForced = !majorFits;
                ModType modType;

                if (!majorFits || majorsRemaining == 0){
                    if (!energyForced && minorsRemaining <= 0) continue;
                    if (!energyForced) minorsRemaining--;
                    modType = ModType.Minor;
                } else {
                    modType = ModType.Major;
                    majorsRemaining--;
                }

                StatMod mod = Mods.GetMod(modType, bestStat);
                if (Mods.IsCompatible(mod, remainingEnergy)){
                    int projected = result.Get(bestStat) + mod.Bonus;
                    //Only block mod if it would exceed the adjusted max AND we already meet the min.
                    if (adjMaxs.Get(bestStat) > 0 && projected > adjMaxs.Get(bestStat) 
                        && result.Get(bestStat) >= adjMins.Get(bestStat)) continue;
                    result.Set(bestStat, result.Get(bestStat) + mod.Bonus);
                }
            }
            return result;
        }
        /*
        Method        : ApplyUserBonuses
        Description   : Adds all user-defined bonuses back to stat block.
                        Called after mod assignment for final stat calculation.
        Parameters    : StatBlock                    stats        : Current stat totals.
                        Dictionary<int,(Stat,Stat)>? tuning       : Custom tuning per slot.
                        bool                         fontsEnabled : Whether fonts are on.
                        Dictionary<Stat, int>        perStatFonts : Font counts per stat.
                        Fragment[]                   fragments    : Selected fragments.
        Return Values : StatBlock                                 : Stats with all bonuses applied.
        */
        public static StatBlock ApplyUserBonuses(StatBlock stats,
            Dictionary<int, (Stat FocusStat, Stat FocusMinus)>? tuning,
            bool fontsEnabled, Dictionary<Stat, int> perStatFonts,
            Fragment[] fragments){

            StatBlock result = new(stats.Health, stats.Melee, stats.Grenade, stats.Super, stats.Class, stats.Weapons);
            //Apply custom tuning.
            if (tuning != null){
                for (int i = 0; i < 4; i++){
                    if (!tuning.TryGetValue(i, out (Stat FocusStat, Stat FocusMinus) t)) continue;
                    result.Set(t.FocusStat, result.Get(t.FocusStat) + 5);
                    result.Set(t.FocusMinus, result.Get(t.FocusMinus) - 5);
                }
            }
            //Apply fonts.
            if (fontsEnabled){
                foreach (KeyValuePair<Stat, int> kvp in perStatFonts){
                    if (kvp.Value == 0) continue;
                    int bonus = Fonts.GetTotalBonus(kvp.Value);
                    result.Set(kvp.Key, result.Get(kvp.Key) + bonus);
                }
            }
            //Apply fragments.
            result = result.ApplyFragments(fragments);

            return result;
        }

        /*
        Method        : ApplyStatCap
        Description   : Clamps all stats to 209, redistributing excess to
                        other wanted stats top to bottom, skipping least wanted.
        Parameters    : StatBlock current      : Current stat totals.
                        StatBlock adjMins : Adjusted minimums for wanted stat detection.
                        Stat      leastWanted  : Stat to skip when redistributing.
        Return Values : StatBlock              : Stats with 209 cap applied.
        */
        private static StatBlock ApplyStatCap(StatBlock current, StatBlock adjMins, Stat leastWanted){
            const int Cap = 204;
            StatBlock result = new(current.Health, current.Melee, current.Grenade, current.Super, current.Class, current.Weapons);

            foreach (Stat stat in GameConstants.StatOrder){
                if (result.Get(stat) <= Cap) continue;
                int excess = result.Get(stat) - Cap;
                result.Set(stat, Cap);

                foreach (Stat other in GameConstants.StatOrder){
                    if (other == stat) continue;
                    if (other == leastWanted) continue;
                    if (excess <= 0) break;
                    int space = Cap - result.Get(other);
                    if (space <= 0) continue;
                    int add = Math.Min(excess, space);
                    result.Set(other, result.Get(other) + add);
                    excess -= add;
                }
            }
            return result;
        }
        /*
        Method        : Resolve
        Description   : Resolves 4-piece legendary combination against adjusted
                        targets using only armor base stats & mods. User bonuses
                        applied after for final scoring.
        Parameters    : ArmorCandidate[] legendaries   : 4 legendary candidates.
                        ArmorPiece       exotic        : Fixed exotic piece.
                        StatBlock        ogMins        : Users original minimums.
                        StatBlock        ogMaxs        : Users original maximums.
                        StatBlock        adjMins       : Mins with user bonuses subtracted.
                        StatBlock        adjMaxs       : Maxs with user bonuses subtracted.
                        Fragment[]       fragments     : Selected fragments.
                        Stat             leastWanted   : Stat to avoid modding.
                        bool             fontsEnabled  : Whether fonts are enabled.
                        Dictionary       fontCounts    : Font counts per slot.
                        Dictionary       advModEnergy  : Advanced mod energy per slot.
                        int              minorModCount : Number of minor mods allowed.
                        Dictionary       perStatFonts  : Font counts per slot.
                        Dictionary?      customTuning  : Custom tuning per legendary slot.
        Return Values : ResolvedResult                 : Resolved stats & score.
        */
        public static ResolvedResult Resolve(
            ArmorCandidate[] legendaries, ArmorPiece exotic,
            StatBlock ogMins, StatBlock ogMaxs,
            StatBlock adjMins, StatBlock adjMaxs,
            Fragment[] fragments, Stat leastWanted,
            bool fontsEnabled, Dictionary<ArmorSlot, int> fontCounts,
            Dictionary<ArmorSlot, int> advModEnergy, int minorModCount,
            Dictionary<Stat, int> perStatFonts,
            Dictionary<int, (Stat FocusStat, Stat FocusMinus)>? customTuning = null){

            //Step 1: Pure armor base stats. no tuning, no fonts, no fragments.
            StatBlock stats = ResolveBaseStats(legendaries, exotic);
            //Early exit: if deficit is hopeless even with max mods, skip full resolve.
            int quickDeficit = StatHelper.GetTotalDeficit(stats, adjMins);
            if (quickDeficit > 50) return new ResolvedResult { MeetsMinimums = false };
            //Step 2: Apply stat mods against adjusted targets.
            stats = ApplyStatMods(stats, adjMins, adjMaxs, leastWanted, fontCounts, advModEnergy, minorModCount, perStatFonts);
            //Step 3: Cap armor + mods before adding user bonuses.
            stats = ApplyStatCap(stats, adjMins, leastWanted);
            //Step 4: Store pre-bonus stats for display (ModdedStats).
            StatBlock modded = new(stats.Health, stats.Melee, stats.Grenade, stats.Super, stats.Class, stats.Weapons);
            //Step 5: Apply all user bonuses (tuning + fonts + fragments).
            StatBlock finalStats = ApplyUserBonuses(stats, customTuning, fontsEnabled, perStatFonts, fragments);
            //Step 6: Score against original targets.
            int focusCount = 0;
            foreach (ArmorCandidate piece in legendaries){
                if (piece.FocusStat != piece.FocusMinus) focusCount++; 
            }
            int deficit = StatHelper.GetTotalDeficit(finalStats, ogMins);
            int excess = StatHelper.GetTotalExcess(finalStats, ogMaxs);
            int score = CalculateScore(finalStats, ogMins, ogMaxs, focusCount);

            return new ResolvedResult {
                FinalStats = finalStats, BaseStats = ResolveBaseStats(legendaries, exotic),
                ModdedStats = modded, Deficit = deficit, Excess = excess,
                Score = score, MeetsMinimums = deficit == 0, MeetsMaximums = excess == 0
            };
        }
        /*
        Method        : CalculateScore
        Description   : Scores resolved stat block. Penalizes focus usage & exceeding maximums.
        Parameters    : StatBlock final      : Final resolved stats.
                        StatBlock mins       : Original minimum targets.
                        StatBlock maxs       : Original maximum targets.
                        int       focusCount : Number of focus mods used.
        Return Values : int                  : Build score.
        */
        private static int CalculateScore(StatBlock final, StatBlock mins, StatBlock maxs, int focusCount){
            int score = 0;

            foreach (Stat stat in GameConstants.StatOrder){
                if (mins.Get(stat) > 0) score += final.Get(stat); 
            }
            score -= focusCount * 100;

            foreach (Stat stat in GameConstants.StatOrder){
                if (maxs.Get(stat) > 0 && final.Get(stat) > maxs.Get(stat)){
                    int over = final.Get(stat) - maxs.Get(stat);
                    score -= over * 500;
                }
            }
            return score;
        }
    }
    public class ResolvedResult {
        public StatBlock? BaseStats {get; set;}
        public StatBlock? ModdedStats {get; set;}
        public StatBlock? FinalStats {get; set;}
        public int Deficit {get; set;}
        public int Excess {get; set;}
        public int Score {get; set;}
        public bool MeetsMinimums {get; set;}
        public bool MeetsMaximums {get; set;}
    }
}