/*
*   FILE          : StatResolver.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Applies stat mods, fonts, & fragments to combination
*                   of armor candidates & resolves final stat totals,
*                   checking them against user's min & max targets.
*/
using D2ArmorCalc_Data;
using D2ArmorCalc_Helpers;
using D2ArmorCalc_Models;

namespace D2ArmorCalc_Algorithm {
    public static class StatResolver {
        /*
        Method        : ResolveBaseStats
        Description   : Sums base stats of all 5 armor pieces 
                        (4 legendary candidates + 1 exotic) into single StatBlock.
        Parameters    : ArmorCandidate[] legendaries : 4 legendary candidates.
                        ArmorPiece       exotic      : Fixed exotic piece.
        Return Values : StatBlock                    : Combined base stats of all 5 pieces.
        */
        public static StatBlock ResolveBaseStats(ArmorCandidate[] legendaries, ArmorPiece exotic){
            StatBlock total = exotic.GetAllStats();
            foreach (ArmorCandidate piece in legendaries) total = total.Add(piece.BaseStats);
            return total;
        }
        /*
        Method        : ApplyStatMods
        Description   : Determines best stat mod for each legendary piece
                        based on remaining deficit against minimums, avoiding
                        least wanted stat. Returns modded stat block.
        Parameters    : StatBlock   current     : Current stat totals.
                        StatBlock   mins        : Minimum stat targets.
                        Stat        leastWanted : Stat to avoid modding.
                        ArmorRarity rarity      : Rarity of legendary pieces.
                        int         fontCount   : Fonts per piece (for energy check).
                        bool        majorMods   : True = major mods, false = minor.
        Return Values : StatBlock               : Stats after best mods applied.
        */
        public static StatBlock ApplyStatMods(StatBlock current, StatBlock mins, Stat leastWanted, 
                                              ArmorRarity rarity, int fontCount, bool majorMods){
            StatBlock result = new(current.Health, current.Melee, current.Grenade,
                                         current.Super, current.Class, current.Weapons);
            ModType modType = majorMods ? ModType.Major : ModType.Minor;
            Stat[] allStats = [Stat.Health, Stat.Melee, Stat.Grenade, Stat.Super, Stat.Class, Stat.Weapons];

            //Apply one stat mod per legendary piece (4 pieces).
            for (int i = 0; i < 4; i++){
                Stat bestStat = leastWanted;
                int bestDeficit = 0;

                foreach (Stat stat in allStats){
                    if (stat == leastWanted) continue;
                    if (mins.Get(stat) == 0) continue;

                    int deficit = StatHelper.GetDeficit(result.Get(stat), mins.Get(stat));
                    if (deficit > bestDeficit){
                        bestDeficit = deficit;
                        bestStat = stat;
                    }
                }
                //Only apply mod if fits within energy budget.
                StatMod mod = Mods.GetMod(modType, bestStat);
                if (Mods.IsCompatible(mod, EnergyHelper.GetRemainingGeneralEnergy(rarity, fontCount)))
                    result.Set(bestStat, result.Get(bestStat) + mod.Bonus);
            }
            return result;
        }
        /*
        Method        : ApplyFonts
        Description   : Applies font bonuses to stat block based on
                        user's selected font configuration per slot.
        Parameters    : StatBlock                  current : Current stat totals.
                        Dictionary<ArmorSlot, int> fonts   : Font count per slot.
        Return Values : StatBlock                          : Stats after fonts applied.
        */
        public static StatBlock ApplyFonts(StatBlock current, Dictionary<ArmorSlot, int> fonts){
            StatBlock result = new(current.Health, current.Melee, current.Grenade,
                                       current.Super, current.Class, current.Weapons);

            foreach (var kvp in fonts){
                Font[] slotFonts = Fonts.GetFontsBySlot(kvp.Key);
                foreach (Font font in slotFonts){
                    int bonus = Fonts.GetTotalBonus(kvp.Value);
                    result.Set(font.Stat, result.Get(font.Stat) + bonus);
                }
            }
            return result;
        }
        /*
        Method        : Resolve
        Description   : Fully resolves 4-piece legendary combination against
                        exotic & user targets, applying mods & fonts,
                        & returns scored StatBlock result.
        Parameters    : ArmorCandidate[]          legendaries  : 4 legendary candidates.
                        ArmorPiece                exotic       : Fixed exotic piece.
                        StatBlock                 mins         : Minimum stat targets.
                        StatBlock                 maxs         : Maximum stat targets.
                        StatBlock                 adjustedMins : Mins after fragment subtraction.
                        StatBlock                 adjustedMaxs : Maxs after fragment subtraction.
                        Fragment[]                fragments    : Selected fragments.
                        Stat                      leastWanted  : Least wanted stat.
                        bool                      fontsEnabled : Whether fonts are enabled.
                        Dictionary<ArmorSlot,int> fontCounts   : Font counts per slot.
                        bool                      majorMods    : True = major, false = minor.
        Return Values : ResolvedResult                         : Resolved stats & score.
        */
        public static ResolvedResult Resolve(ArmorCandidate[] legendaries, ArmorPiece exotic, StatBlock mins, StatBlock maxs, 
            StatBlock adjustedMins, StatBlock adjustedMaxs, Fragment[] fragments, Stat leastWanted, 
            bool fontsEnabled, Dictionary<ArmorSlot,int> fontCounts, bool majorMods){

            int fontCount = fontsEnabled ? 1 : 0; //Average fonts per piece for energy calc.

            //Step 1: Base stats.
            StatBlock stats = ResolveBaseStats(legendaries, exotic);

            //Step 2: Apply stat mods.
            stats = ApplyStatMods(stats, adjustedMins, leastWanted, ArmorRarity.Legendary, fontCount, majorMods);

            //Step 3: Apply fonts if enabled.
            if (fontsEnabled) stats = ApplyFonts(stats, fontCounts);

            //Step 4: Apply fragments.
            StatBlock finalStats = stats.ApplyFragments(fragments);

            //Step 5: Score.
            int deficit = StatHelper.GetTotalDeficit(finalStats, mins);
            int excess = StatHelper.GetTotalExcess(finalStats, maxs);
            int score = CalculateScore(finalStats, mins);

            return new ResolvedResult {
                FinalStats = finalStats, BaseStats = ResolveBaseStats(legendaries, exotic),
                ModdedStats = stats, Deficit = deficit, Excess = excess, Score = score,
                MeetsMinimums = deficit == 0, MeetsMaximums = excess == 0
            };
        }
        /*
        Method        : CalculateScore
        Description   : Scores resolved stat block based on sum of all
                        stats with non-zero minimum target. Higher is better.
        Parameters    : StatBlock final : Final resolved stats.
                        StatBlock mins  : Minimum stat targets.
        Return Values : int             : Build score (higher = better).
        */
        private static int CalculateScore(StatBlock final, StatBlock mins){
            int score = 0;
            Stat[] allStats = [Stat.Health, Stat.Melee, Stat.Grenade,
                               Stat.Super, Stat.Class, Stat.Weapons];
            foreach (Stat stat in allStats){
                if (mins.Get(stat) > 0) score += final.Get(stat);
            }
            return score;
        }
    }
    //Lightweight result struct from a single resolve pass.
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