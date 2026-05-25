/*
*   FILE          : ArmorCalculator.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Main entry point for armor calculation algorithm.
*                   Orchestrates combo generation, stat resolution, &
*                   result selection with optional multicore support.
*/
using D2ArmorCalc_Data;
using D2ArmorCalc_Helpers;
using D2ArmorCalc_Models;
using System.Collections.Concurrent;

namespace D2ArmorCalc_Algorithm {
    //Input parameters for a calculation run.
    public class CalcInput {
        public StatBlock Mins {get; set;} = new();
        public StatBlock Maxs {get; set;} = new();
        public ArmorPiece Exotic {get; set;} = new(ArmorSlot.Helmet, ArmorRarity.Exotic);
        public Fragment[] Fragments {get; set;} = [];
        public Stat LeastWantedStat {get; set;}
        public bool FontsEnabled {get; set;}
        public bool FontsInStats {get; set;}
        public bool ArmorModsEnabled {get; set;}
        public int MinorModCount {get; set;} = 0; //0 = all major, 1-5 = N minor rest major.
        public Dictionary<ArmorSlot, int> AdvModEnergy {get; set;} = [];
        public Dictionary<ArmorSlot, int> FontCounts {get; set;} = [];
        public Dictionary<int, (Stat FocusStat, Stat FocusMinus)>? CustomTuning {get; set;}
        public Dictionary<Stat, int> PerStatFonts {get; set;} = [];
    }
    public static class ArmorCalculator {
        /*
        Method        : Calculate
        Description   : Main entry point for armor calculator. Adjusts
                        targets for fragments, generates combinations, resolves
                        stats, & returns best valid BuildResult.
        Parameters    : CalcInput input : All user-configured calculation parameters.
        Return Values : BuildResult     : Best result found, or failed result
                                          if no valid combination exists.
        */
        public static BuildResult Calculate(CalcInput input){
            //Step 1: Build all user-controlled bonus StatBlock upfront.
            StatBlock fragStats = new StatBlock().ApplyFragments(input.Fragments);
            StatBlock adjMins = SubtractStats(input.Mins, fragStats);
            StatBlock adjMaxs = SubtractStats(input.Maxs, fragStats);
            //Subtract font bonuses if FontsInStats is enabled.
            if (input.FontsInStats && input.FontsEnabled){
                StatBlock fontBonus = BuildFontStatBlock(input.PerStatFonts);
                adjMins = SubtractStats(adjMins, fontBonus);
                adjMaxs = SubtractStats(adjMaxs, fontBonus);
            }
            //Subtract custom tuning bonuses. algorithm doesn't need to fill what tuning provides.
            if (input.CustomTuning != null){
                foreach ((Stat FocusStat, Stat FocusMinus) tuning in input.CustomTuning.Values){
                    //+5 to focus stat means armor needs 5 less of that stat.
                    adjMins.Set(tuning.FocusStat, Math.Max(0, adjMins.Get(tuning.FocusStat) - 5));
                    //-5 from focus minus means that stat effectively loses 5.
                    //don't subtract from mins for minus stat. its a penalty not a bonus.
                }
            }
            //Generate legendary candidates.
            List<ArmorCandidate> candidates = input.CustomTuning != null 
                ? ComboGenerator.GenerateCandidatesNoFocus(adjMins, input.LeastWantedStat) : ComboGenerator.GenerateCandidates(adjMins, input.LeastWantedStat);
            const int HighMinThreshold = 20; //Only keep candidates that have 20+ in a wanted stat.
            candidates = [.. candidates.Where(c =>
                GameConstants.StatOrder.Any(stat => adjMins.Get(stat) > 0 && c.BaseStats.Get(stat) >= HighMinThreshold))];
            //Generate exotic candidates separately.
            List<ArmorCandidate>? exoticCandidates = input.Exotic.IsCustomRoll && input.Exotic.CustomStatBlock != null 
                ? null : input.CustomTuning != null
                    ? ComboGenerator.GenerateCandidatesNoFocus(adjMins, input.LeastWantedStat) : ComboGenerator.GenerateCandidates(adjMins, input.LeastWantedStat);
            //Search combinations with branch pruning.
            int maxModBoost = 50 - (input.MinorModCount * 5); //5 major mods x 10 each.
            StatBlock bestPerSlot = ComboGenerator.GetBestPossibleContribution(candidates);
            List<ArmorCandidate[]> combinations = [];

            StatBlock bestExoticContribution = exoticCandidates != null
                ? ComboGenerator.GetBestPossibleContribution(exoticCandidates) : input.Exotic.IsCustomRoll && input.Exotic.CustomStatBlock != null
                    ? input.Exotic.CustomStatBlock : input.Exotic.GetAllStats();

            //Subtract exotic contribution from mins so search only needs to cover the remainder.
            StatBlock searchMins = input.Exotic.IsCustomRoll && input.Exotic.CustomStatBlock != null
                ? SubtractStats(adjMins, input.Exotic.CustomStatBlock) : SubtractStats(adjMins, bestExoticContribution);

            ComboGenerator.SearchCombinations(candidates, new ArmorCandidate?[4], 0,
                new StatBlock(), searchMins, bestPerSlot, maxModBoost, combinations);

            ConcurrentBag<(ArmorCandidate[] combo, ArmorCandidate? exoticCandidate, ResolvedResult result)> validResults = [];
            Parallel.ForEach(combinations, combo => {
                //Build combo base stats once.
                StatBlock comboBase = new();
                foreach (ArmorCandidate c in combo) comboBase = comboBase.Add(c.BaseStats);

                if (exoticCandidates == null){
                    StatBlock exoticStats = input.Exotic.IsCustomRoll && input.Exotic.CustomStatBlock != null
                        ? input.Exotic.CustomStatBlock : input.Exotic.GetAllStats();

                    StatBlock debugTotal = comboBase.Add(exoticStats);

                    if (StatHelper.GetTotalDeficit(comboBase.Add(exoticStats), adjMins) > maxModBoost) return;
                    ResolvedResult resolved = StatResolver.Resolve(combo, input.Exotic, input.Mins, input.Maxs,
                        adjMins, adjMaxs, input.Fragments, input.LeastWantedStat,
                        input.FontsEnabled, input.FontCounts, input.AdvModEnergy,
                        input.MinorModCount, input.PerStatFonts, input.CustomTuning);
                    if (resolved.MeetsMinimums) validResults.Add((combo, null, resolved));
                } else {
                    //Find the best exotic candidate for this specific combo
                    //rather than trying all 96.
                    ArmorCandidate? bestExotic = null;
                    int bestExoticScore = int.MinValue;

                    foreach (ArmorCandidate exoticCandidate in exoticCandidates){
                        //Quick feasibility check first.
                        ArmorPiece exoticPiece = BuildExoticFromCandidate(exoticCandidate, input.Exotic.Slot);
                        StatBlock withExotic = comboBase.Add(exoticPiece.GetAllStats());
                        int deficit = StatHelper.GetTotalDeficit(withExotic, adjMins);
                        if (deficit > maxModBoost) continue;

                        //Score this exotic by how much it covers the combo's deficit.
                        int score = 0;
                        foreach (Stat stat in GameConstants.StatOrder){
                            int need = Math.Max(0, adjMins.Get(stat) - comboBase.Get(stat));
                            score += Math.Min(need, exoticPiece.GetAllStats().Get(stat));
                        }
                        if (score > bestExoticScore){
                            bestExoticScore = score;
                            bestExotic = exoticCandidate;
                        }
                    }
                    if (bestExotic == null) return;

                    ArmorPiece bestExoticPiece = BuildExoticFromCandidate(bestExotic, input.Exotic.Slot);
                    ResolvedResult resolved = StatResolver.Resolve(combo, bestExoticPiece, input.Mins, input.Maxs,
                        adjMins, adjMaxs, input.Fragments, input.LeastWantedStat,
                        input.FontsEnabled, input.FontCounts, input.AdvModEnergy,
                        input.MinorModCount, input.PerStatFonts, input.CustomTuning);
                    if (resolved.MeetsMinimums) validResults.Add((combo, bestExotic, resolved));
                }
            });
            return BuildBestResult(validResults, input, adjMins, adjMaxs);
        }
        /*
        Method        : BuildExoticFromCandidate
        Description   : Builds temporary ArmorPiece for exotic from
                        ArmorCandidate, using exotic stat values (30/20/12).
        Parameters    : ArmorCandidate candidate : Candidate to build from.
                        ArmorSlot      slot      : Exotic armor slot.
        Return Values : ArmorPiece               : Exotic piece.
        */
        private static ArmorPiece BuildExoticFromCandidate(ArmorCandidate candidate, ArmorSlot slot){
            Archetype archetype = Archetypes.AllArchetypes[(int)candidate.Archetype];
            ArmorPiece piece = new(slot, ArmorRarity.Exotic){
                Archetype = archetype, TertiaryStat = candidate.Tertiary
                //No FocusStat or FocusMinus for exotics
            };
            return piece;
        }
        /*
        Method        : BuildBestResult
        Description   : Selects highest scoring valid result & constructs
                        full BuildResult from it. Returns MinsFailed result
                        if no valid combinations found.
        Parameters    : ConcurrentBag results : All valid resolved combinations.
                        CalcInput   input   : Original calculation input.
                        StatBlock   adjMins : Fragment-adjusted minimums.
                        StatBlock   adjMaxs : Fragment-adjusted maximums.
        Return Values : BuildResult         : Best build result.
        */
        private static BuildResult BuildBestResult(ConcurrentBag<(ArmorCandidate[] combo, ArmorCandidate? exoticCandidate, 
            ResolvedResult result)> results, CalcInput input, StatBlock adjMins, StatBlock adjMaxs){

            if (results.IsEmpty){
                return new BuildResult {
                    Status = BuildStatus.MinsFailed, Fragments = input.Fragments
                };
            }
            //Find highest scoring result.
            ArmorCandidate[]? bestCombo = null;
            ArmorCandidate? bestExoticCandidate = null;
            ResolvedResult? bestResult = null;

            foreach ((ArmorCandidate[]? combo, ArmorCandidate? exoticCandidate, ResolvedResult? result) in results){
                if (bestResult == null || result.Score > bestResult.Score ||
                   (result.Score == bestResult.Score && StatHelper.IsMoreDeterministic(combo, bestCombo))){
                    bestCombo = combo;
                    bestExoticCandidate = exoticCandidate;
                    bestResult = result;
                }
            }
            ArmorCandidate[] finalCombo = bestCombo  ?? throw new InvalidOperationException("bestCombo was null after iterating non-empty results.");
            ResolvedResult finalResult = bestResult ?? throw new InvalidOperationException("bestResult was null after iterating non-empty results.");
            //Assign slots to 4 legendary candidates.
            ArmorSlot[] slots = [ArmorSlot.Helmet, ArmorSlot.Arms, ArmorSlot.Chestplate, ArmorSlot.Boots];
            //Determine which slot exotic is NOT occupying.
            List<ArmorSlot> legendarySlots = [];
            foreach (ArmorSlot slot in slots){
                if (slot != input.Exotic.Slot) legendarySlots.Add(slot);
            }
            legendarySlots.Add(ArmorSlot.ClassItem);
            //Build ArmorPiece objects from legendary candidates.
            ArmorPiece[] pieces = new ArmorPiece[4];
            for (int i = 0; i < 4; i++){
                ArmorCandidate candidate = finalCombo[i];
                Archetype archetype = Archetypes.AllArchetypes[(int)candidate.Archetype];
                pieces[i] = new ArmorPiece(legendarySlots[i], ArmorRarity.Legendary){
                    Archetype = archetype, TertiaryStat = candidate.Tertiary,
                    FocusStat = candidate.FocusStat, FocusMinus = candidate.FocusMinus
                };
            }
            //Resolve exotic piece — either from best candidate or custom roll.
            ArmorPiece exotic = bestExoticCandidate != null ? BuildExoticFromCandidate(bestExoticCandidate, input.Exotic.Slot) : input.Exotic;
            //Apply custom tuning if enabled.
            if (input.CustomTuning != null){
                for (int i = 0; i < pieces.Length; i++){
                    if (input.CustomTuning.TryGetValue(i, out (Stat FocusStat, Stat FocusMinus) tuning)){
                        pieces[i].FocusStat = tuning.FocusStat;
                        pieces[i].FocusMinus = tuning.FocusMinus;
                    }
                }
            }
            AssignStatMods(pieces, exotic, adjMins, adjMaxs, input.LeastWantedStat, input.MinorModCount, input.FontCounts, input.AdvModEnergy, input.PerStatFonts);
            //Build final result.
            BuildResult buildResult = new(){
                Status = finalResult.MeetsMaximums ? BuildStatus.Success : BuildStatus.MaxsExceeded,
                Fragments = input.Fragments, BaseStats = finalResult.BaseStats,
                ModdedStats = finalResult.ModdedStats, FinalStats = finalResult.FinalStats,
                OverflowStats = finalResult.FinalStats?.GetOverflow(), Score = finalResult.Score
            };
            //Assign legendary pieces by slot.
            foreach (ArmorPiece piece in pieces){
                switch (piece.Slot){
                    case ArmorSlot.Helmet: buildResult.Helmet = piece; break;
                    case ArmorSlot.Arms: buildResult.Arms = piece; break;
                    case ArmorSlot.Chestplate: buildResult.Chestplate = piece; break;
                    case ArmorSlot.Boots: buildResult.Boots = piece; break;
                    case ArmorSlot.ClassItem: buildResult.ClassItem = piece; break;
                }
            }
            //Place exotic in its slot.
            switch (exotic.Slot){
                case ArmorSlot.Helmet: buildResult.Helmet = exotic; break;
                case ArmorSlot.Arms: buildResult.Arms = exotic; break;
                case ArmorSlot.Chestplate: buildResult.Chestplate = exotic; break;
                case ArmorSlot.Boots: buildResult.Boots = exotic; break;
                case ArmorSlot.ClassItem: buildResult.ClassItem = exotic; break;
            }
            if (finalResult.FinalStats != null) buildResult.MaxsExceededStats = GetExceededStats(finalResult.FinalStats, input.Maxs);

            return buildResult;
        }
        /*
        Method        : BuildFontStatBlock
        Description   : Builds StatBlock representing total font bonuses
                        across all slots based on font counts.
        Parameters    : Dictionary<ArmorSlot, int> fontCounts : Font counts per slot.
        Return Values : StatBlock                             : Total font bonuses.
        */
        private static StatBlock BuildFontStatBlock(Dictionary<Stat, int> perStatFontCounts){
            StatBlock stats = new();
            foreach (Stat stat in GameConstants.StatOrder){
                if (perStatFontCounts.TryGetValue(stat, out int count)) stats.Set(stat, Fonts.GetTotalBonus(count));
            }
            return stats;
        }
        /*
        Method        : SubtractStats
        Description   : Subtracts fragment stat bonuses from min/max targets
                        so armor just needs to hit adjusted values.
                        Clamps results to 0 minimum to avoid negative targets.
        Parameters    : StatBlock targets   : Original min or max targets.
                        StatBlock fragments : Fragment stat bonuses to subtract.
        Return Values : StatBlock           : Adjusted targets.
        */
        private static StatBlock SubtractStats(StatBlock targets, StatBlock fragments){
            return new StatBlock(
                Math.Max(0, targets.Health - fragments.Health),
                Math.Max(0, targets.Melee - fragments.Melee),
                Math.Max(0, targets.Grenade - fragments.Grenade),
                Math.Max(0, targets.Super - fragments.Super),
                Math.Max(0, targets.Class - fragments.Class),
                Math.Max(0, targets.Weapons - fragments.Weapons)
            );
        }
        /*
        Method        : GetExceededStats
        Description   : Returns StatBlock containing only amounts that
                        each stat exceeds its maximum target.
        Parameters    : StatBlock final : Final resolved stats.
                        StatBlock maxs  : Maximum stat targets.
        Return Values : StatBlock       : Excess amounts per stat above maximum.
        */
        private static StatBlock GetExceededStats(StatBlock final, StatBlock maxs){
            return new StatBlock(
                maxs.Health > 0 ? Math.Max(0, final.Health - maxs.Health) : 0,
                maxs.Melee > 0 ? System.Math.Max(0, final.Melee - maxs.Melee) : 0,
                maxs.Grenade > 0 ? Math.Max(0, final.Grenade - maxs.Grenade) : 0,
                maxs.Super > 0 ? Math.Max(0, final.Super - maxs.Super) : 0,
                maxs.Class > 0 ? Math.Max(0, final.Class - maxs.Class) : 0,
                maxs.Weapons > 0 ? Math.Max(0, final.Weapons - maxs.Weapons) : 0
            );
        }
        /*
        Method        : AssignStatMods
        Description   : Re-runs mod assignment logic & stores chosen StatMod
                        on each ArmorPiece for display in results panel.
        Parameters    : ArmorPiece[] pieces        : 4 legendary pieces.
                      : ArmorPiece   exotic        : Exotic piece.
                      : StatBlock    adjMins       : Minimum stat targets.
                      : StatBlock    adjMaxs       : Maximum stat targets.
                      : Stat         leastWanted   : Stat to avoid modding.
                      : int          minorModCount : Number of minor mods to apply.
                      : Dictionary   fontCounts    : Number of fonts per slot.
                      : Dictionary   advModEnergy  : Armor energy per slot.
                      : Dictionary   perStatFonts  : Number of fonts per stat.
        Return Values : void
        */
        private static void AssignStatMods(ArmorPiece[] pieces, ArmorPiece exotic, StatBlock adjMins, StatBlock adjMaxs, 
                                           Stat leastWanted, int minorModCount, Dictionary<ArmorSlot, int> fontCounts, 
                                           Dictionary<ArmorSlot, int> advModEnergy, Dictionary<Stat, int> perStatFonts){
            List<ArmorPiece> allPieces = [..pieces, exotic];
            int minorsRemaining = minorModCount;
            int majorsRemaining = 5 - minorModCount;
            //Build running total from pure armor stats only. no focus/tuning
            //Use ArmorCandidate base logic: primary=30, secondary=25, tertiary=20, rest=5
            //Since pieces already have Archetype/TertiaryStat set, GetAllStats() includes
            //focus from FocusStat/FocusMinus. We need to exclude that.
            //Solution: rebuild stats without focus.
            StatBlock running = new();
            foreach (ArmorPiece p in allPieces){
                //Get stats without focus by temporarily zeroing focus effect.
                Stat savedFocusStat = p.FocusStat;
                Stat savedFocusMinus = p.FocusMinus;
                //Set focus to same stat so net effect is zero.
                p.FocusStat = p.FocusMinus;
                running = running.Add(p.GetAllStats());
                p.FocusStat = savedFocusStat;
                p.FocusMinus = savedFocusMinus;
            }
            foreach (ArmorPiece piece in allPieces){
                Stat bestStat = leastWanted;
                int bestDeficit = 0;

                foreach (Stat stat in GameConstants.StatOrder){
                    if (stat == leastWanted) continue;
                    if (adjMins.Get(stat) == 0) continue;
                    int deficit = StatHelper.GetDeficit(running.Get(stat), adjMins.Get(stat));
                    if (deficit > bestDeficit){bestDeficit = deficit; bestStat = stat;}
                }

                if (bestStat != leastWanted && bestDeficit > 0){
                    int fontCount = fontCounts.TryGetValue(piece.Slot, out int fc) ? fc : 0;
                    int advEnergy = advModEnergy.TryGetValue(piece.Slot, out int ae) ? ae : 0;
                    int remainingEnergy = EnergyHelper.GetRemainingGeneralEnergy(piece.Rarity, fontCount) - advEnergy;
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
                    int fontBonus = perStatFonts.TryGetValue(bestStat, out int fsc) ? Fonts.GetTotalBonus(fsc) : 0;
                    int projected = running.Get(bestStat) + mod.Bonus;
                    if (adjMaxs.Get(bestStat) > 0 && projected > adjMaxs.Get(bestStat)) continue;
                    piece.StatMod = mod;
                    running.Set(bestStat, running.Get(bestStat) + mod.Bonus);
                }
            }
        }
    }
}