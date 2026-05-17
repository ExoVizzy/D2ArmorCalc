/*
*   FILE          : BuildResult.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Defines BuildResult model representing output
*                   of completed armor calculation, including chosen pieces,
*                   stat totals, DIM queries, & outcome status.
*/
namespace D2ArmorCalc {
    //Represents outcome status of a build calculation.
    //Success: mins & maxs met, MinsFailed: target minimums not achievable, MaxsExceeded: mins met but one or more stats exceed max.
    public enum BuildStatus {
        Success, MinsFailed, MaxsExceeded
    }
    //Represents full output of a completed armor calculation.
    public class BuildResult {
        //Status of calculation.
        public BuildStatus Status {get; set;}
        //The 5 chosen armor pieces (Helmet, Arms, Chest, Boots, ClassItem).
        public ArmorPiece Helmet {get; set;}
        public ArmorPiece Arms {get; set;}
        public ArmorPiece Chestplate {get; set;}
        public ArmorPiece Boots {get; set;}
        public ArmorPiece ClassItem {get; set;}

        //Stat totals at each stage.
        public StatBlock BaseStats {get; set;} //Raw armor stats before mods/fonts/fragments.
        public StatBlock ModdedStats {get; set;} //After mods & fonts applied.
        public StatBlock FinalStats {get; set;} //After fragments applied (display value).
        public StatBlock OverflowStats {get; set;} //Amount each stat exceeds 100.

        //Stats that exceeded their maximum target (for disclaimer display).
        public StatBlock MaxsExceededStats {get; set;}

        //Selected fragments applied to this build.
        public Fragment[] Fragments {get; set;}
        //DIM query strings.
        public string DimQueryAll {get; set;} //All 5 pieces together.
        public string DimQueryExotic {get; set;} //Exotic piece only.
        public string DimQueryLegendary {get; set;} //4 legendary pieces only.
        // Score used internally by algorithm to rank and compare results.
        // Lower is better: represents how far total stats are from targets.
        public int Score {get; set;}
        public BuildResult() {
            Fragments = new Fragment[0];
            MaxsExceededStats = new StatBlock();
        }
        /*
        Method        : GetPieces
        Description   : Returns all 5 armor pieces as array in slot order.
        Parameters    : None.
        Return Values : ArmorPiece[] : Array of all 5 pieces in slot order.
        */
        public ArmorPiece[] GetPieces() {
            return new ArmorPiece[] { Helmet, Arms, Chestplate, Boots, ClassItem };
        }
        /*
        Method        : IsValid
        Description   : Returns true if build result has valid status,
                        meaning minimums were met regardless of max outcome.
        Parameters    : None.
        Return Values : bool : True if mins were met, false if build is impossible.
        */
        public bool IsValid() {
            return Status != BuildStatus.MinsFailed;
        }
        /*
        Method        : GetStatusMessage
        Description   : Returns user-facing message string based on
                        current build status for display in UI.
        Parameters    : None.
        Return Values : string : Message describing build outcome.
        */
        public string GetStatusMessage() {
            switch (Status) {
                case BuildStatus.Success:
                    return "Build found. All stat targets met.";
                case BuildStatus.MinsFailed:
                    return "No valid build found. Target minimums are not achievable with the current configuration.";
                case BuildStatus.MaxsExceeded:
                    return "Build found. Minimum targets met, but one or more stats could not stay within the set maximums.";
                default:
                    return string.Empty;
            }
        }
    }
}