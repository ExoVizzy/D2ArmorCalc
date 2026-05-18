/*
*   FILE          : ArmorCalculator.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Main entry point for armor calculation algorithm.
*                   Orchestrates combo generation, stat resolution, &
*                   result selection with optional multicore support.
*/
using System.Collections.Concurrent;

namespace D2ArmorCalc {
    //Input parameters for a calculation run.
    public class CalcInput {
        public StatBlock Mins {get; set;}
        public StatBlock Maxs {get; set;}
        public ArmorPiece Exotic {get; set;}
        public Fragment[] Fragments {get; set;} = [];
        public Stat LeastWantedStat {get; set;}
        public bool FontsEnabled {get; set;}
        public Dictionary<ArmorSlot, int> FontCounts {get; set;} = new Dictionary<ArmorSlot, int>();
        public bool ArmorModsEnabled {get; set;}
        public bool MajorMods {get; set;} = true;
        public int MinorModCount {get; set;} //Used when ArmorModsEnabled is false.
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
            //Step 1: Adjust mins/maxs by subtracting fragment bonuses.
            StatBlock fragmentStats = new StatBlock().ApplyFragments(input.Fragments);
            StatBlock adjustedMins = SubtractStats(input.Mins, fragmentStats);
            StatBlock adjustedMaxs = SubtractStats(input.Maxs, fragmentStats);
            //Step 2: Generate candidates for legendary pieces.
            List<ArmorCandidate> candidates = ComboGenerator.GenerateCandidates(adjustedMins, input.LeastWantedStat);
            List<ArmorCandidate[]> combinations = ComboGenerator.GenerateAllCombinations(candidates);
            //Step 3: Resolve all combinations in parallel, collect valid results.
            ConcurrentBag<(ArmorCandidate[] combo, ResolvedResult result)> validResults = [];

            Parallel.ForEach(combinations, combo => {
                ResolvedResult resolved = StatResolver.Resolve(
                    combo, input.Exotic, input.Mins, input.Maxs,
                    adjustedMins, adjustedMaxs, input.Fragments,
                    input.LeastWantedStat, input.FontsEnabled,
                    input.FontCounts, input.MajorMods
                );

                if (resolved.MeetsMinimums) validResults.Add((combo, resolved));
            });
            //Step 4: Pick best result.
            return BuildBestResult(validResults, input, adjustedMins, adjustedMaxs);
        }

        /*
        Method        : BuildBestResult
        Description   : Selects highest scoring valid result & constructs
                        full BuildResult from it. Returns MinsFailed result
                        if no valid combinations were found.
        Parameters    : ConcurrentBag results : All valid resolved combinations.
                        CalcInput    input   : Original calculation input.
                        StatBlock    adjMins : Fragment-adjusted minimums.
                        StatBlock    adjMaxs : Fragment-adjusted maximums.
        Return Values : BuildResult           : Best build result.
        */
        private static BuildResult BuildBestResult(
            ConcurrentBag<(ArmorCandidate[] combo, ResolvedResult result)> results,
            CalcInput input, StatBlock adjMins, StatBlock adjMaxs){

            if (results.IsEmpty){
                return new BuildResult {
                    Status = BuildStatus.MinsFailed,
                    Fragments = input.Fragments
                };
            }
            //Find highest scoring result.
            ArmorCandidate[] bestCombo = null;
            ResolvedResult bestResult = null;

            foreach (var (combo, result) in results){
                if (bestResult == null || result.Score > bestResult.Score){
                    bestCombo = combo;
                    bestResult = result;
                }
            }
            //Assign slots to the 4 legendary candidates.
            ArmorSlot[] slots = [ArmorSlot.Helmet, ArmorSlot.Arms, ArmorSlot.Chestplate, ArmorSlot.Boots];

            //Determine which slot the exotic is NOT occupying.
            List<ArmorSlot> legendarySlots = new List<ArmorSlot>();
            foreach (ArmorSlot slot in slots){
                if (slot != input.Exotic.Slot)
                    legendarySlots.Add(slot);
            }
            //Class item is always legendary.
            legendarySlots.Add(ArmorSlot.ClassItem);

            //Build ArmorPiece objects from candidates.
            ArmorPiece[] pieces = new ArmorPiece[5];
            for (int i = 0; i < 4; i++){
                ArmorCandidate candidate = bestCombo[i];
                Archetype archetype = Archetypes.All[(int)candidate.Archetype];
                pieces[i] = new ArmorPiece(legendarySlots[i], ArmorRarity.Legendary){
                    Archetype = archetype, TertiaryStat = candidate.Tertiary,
                    FocusStat = candidate.FocusStat, FocusMinus = candidate.FocusMinus
                };
            }

            //Place exotic in its slot.
            ArmorPiece exotic = input.Exotic;

            //Build final result.
            BuildResult buildResult = new BuildResult {
                Status = bestResult.MeetsMaximums ? BuildStatus.Success : BuildStatus.MaxsExceeded,
                Fragments = input.Fragments,
                BaseStats = bestResult.BaseStats,
                ModdedStats = bestResult.ModdedStats,
                FinalStats = bestResult.FinalStats,
                OverflowStats = bestResult.FinalStats.GetOverflow(),
                Score = bestResult.Score
            };

            //Assign pieces by slot.
            foreach (ArmorPiece piece in pieces){
                switch (piece.Slot){
                    case ArmorSlot.Helmet: buildResult.Helmet = piece; break;
                    case ArmorSlot.Arms: buildResult.Arms = piece; break;
                    case ArmorSlot.Chestplate: buildResult.Chestplate = piece; break;
                    case ArmorSlot.Boots: buildResult.Boots = piece; break;
                    case ArmorSlot.ClassItem: buildResult.ClassItem = piece; break;
                }
            }
            //Place exotic.
            switch (exotic.Slot){
                case ArmorSlot.Helmet: buildResult.Helmet = exotic; break;
                case ArmorSlot.Arms: buildResult.Arms = exotic; break;
                case ArmorSlot.Chestplate: buildResult.Chestplate = exotic; break;
                case ArmorSlot.Boots: buildResult.Boots = exotic; break;
                case ArmorSlot.ClassItem: buildResult.ClassItem = exotic; break;
            }
            //Flag which stats exceeded maximums.
            buildResult.MaxsExceededStats = GetExceededStats(bestResult.FinalStats, input.Maxs);

            return buildResult;
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
    }
}