/*
*   FILE          : DimQueryBuilder.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Builds DIM search query strings from BuildResult,
*                   generating separate legendary, exotic, & combined
*                   queries based on archetype, tertiary, & focus stats.
*/
using System.Text;

namespace D2ArmorCalc {
    public static class DimQueryBuilder {
        //=====================================================================
        //DIM Keyword Maps.
        //=====================================================================
        private static readonly Dictionary<ArchetypeType, string> ArchetypeKeywords =
            new Dictionary<ArchetypeType, string> {
                {ArchetypeType.Brawler, "brawler"}, {ArchetypeType.Gunner, "gunner"},
                {ArchetypeType.Specialist, "specialist"}, {ArchetypeType.Grenadier, "grenadier"},
                {ArchetypeType.Paragon, "paragon"}, {ArchetypeType.Bulwark, "bulwark"}
            };
        private static readonly Dictionary<Stat, string> StatKeywords =
            new Dictionary<Stat, string> {
                {Stat.Health, "health"}, {Stat.Melee, "melee"},
                {Stat.Grenade, "grenade"}, {Stat.Super, "super"},
                {Stat.Class, "class"}, {Stat.Weapons, "weapons"}
            };
        private static readonly Dictionary<ArmorSlot, string> SlotKeywords =
            new Dictionary<ArmorSlot, string> {
                {ArmorSlot.Helmet, "helmet"}, {ArmorSlot.Arms, "gauntlets"},
                {ArmorSlot.Chestplate, "chest"}, {ArmorSlot.Boots, "leg"},
                {ArmorSlot.ClassItem, "classitem"}
            };
        private static readonly Dictionary<PlayerClass, string> ClassKeywords =
            new Dictionary<PlayerClass, string> {
                {PlayerClass.Warlock, "warlock"}, {PlayerClass.Titan, "titan"},
                {PlayerClass.Hunter, "hunter"}
            };
        //=====================================================================
        //Public Entry Points.
        //=====================================================================
        /*
        Method        : BuildQueries
        Description   : Builds all three DIM query strings (legendary, exotic,
                        combined) from BuildResult & stores them on the result.
        Parameters    : BuildResult result      : Build result to generate queries for.
                        PlayerClass playerClass : Selected player class.
        Return Values : void
        */
        public static void BuildQueries(BuildResult result, PlayerClass playerClass){
            result.DimQueryLegendary = BuildLegendaryQuery(result, playerClass);
            result.DimQueryExotic = BuildExoticQuery(result, playerClass);
            result.DimQueryAll = BuildCombinedQuery(result.DimQueryLegendary, result.DimQueryExotic);
        }
        //=====================================================================
        //Query Builders.
        //=====================================================================
        /*
        Method        : BuildLegendaryQuery
        Description   : Builds DIM query string for all 4 legendary armor
                        pieces, prefixed with is:legendary, player class,
                        & (not is:slot) exclusion for exotic slot.
        Parameters    : BuildResult result      : Build result containing piece data.
                        PlayerClass playerClass : Selected player class.
        Return Values : string                  : Legendary DIM query string.
        */
        public static string BuildLegendaryQuery(BuildResult result, PlayerClass playerClass){
            StringBuilder sb = new StringBuilder();
            //Prefix.
            string classKeyword = ClassKeywords[playerClass];
            string exoticSlot = SlotKeywords[result.Helmet?.Rarity == ArmorRarity.Exotic ? ArmorSlot.Helmet :
                                               result.Arms?.Rarity == ArmorRarity.Exotic ? ArmorSlot.Arms :
                                               result.Chestplate?.Rarity == ArmorRarity.Exotic ? ArmorSlot.Chestplate :
                                               result.Boots?.Rarity == ArmorRarity.Exotic ? ArmorSlot.Boots : ArmorSlot.ClassItem];

            sb.Append($"is:legendary is:{classKeyword} (not is:{exoticSlot})");

            //Build per-piece query segments.
            List<string> pieceSegments = new List<string>();
            foreach (ArmorPiece piece in result.GetPieces()){
                if (piece == null || piece.Rarity == ArmorRarity.Exotic) continue;
                pieceSegments.Add(BuildPieceSegment(piece));
            }
            //Join with OR if pieces differ, otherwise just append.
            if (pieceSegments.Count > 0){
                sb.Append(" ");
                sb.Append(JoinSegments(pieceSegments));
            }
            return sb.ToString();
        }
        /*
        Method        : BuildExoticQuery
        Description   : Builds DIM query string for exotic armor piece,
                        prefixed with is:exotic, player class, & slot.
        Parameters    : BuildResult result      : Build result containing piece data.
                        PlayerClass playerClass : Selected player class.
        Return Values : string                  : Exotic DIM query string.
        */
        public static string BuildExoticQuery(BuildResult result, PlayerClass playerClass){
            //Find the exotic piece.
            ArmorPiece exotic = null;
            foreach (ArmorPiece piece in result.GetPieces()){
                if (piece?.Rarity == ArmorRarity.Exotic){
                    exotic = piece;
                    break;
                }
            }
            if (exotic == null) return string.Empty;

            string classKeyword = ClassKeywords[playerClass];
            string slotKeyword = SlotKeywords[exotic.Slot];
            string segment = BuildPieceSegment(exotic);

            return $"is:exotic is:{classKeyword} is:{slotKeyword} {segment}";
        }
        /*
        Method        : BuildCombinedQuery
        Description   : Combines legendary & exotic query strings into
                        a single DIM query using OR.
        Parameters    : string legendaryQuery : Legendary DIM query string.
                        string exoticQuery    : Exotic DIM query string.
        Return Values : string                : Combined DIM query string.
        */
        public static string BuildCombinedQuery(string legendaryQuery, string exoticQuery){
            if (string.IsNullOrEmpty(legendaryQuery)) return exoticQuery;
            if (string.IsNullOrEmpty(exoticQuery)) return legendaryQuery;
            return $"({legendaryQuery}) or ({exoticQuery})";
        }
        //=====================================================================
        //Helpers.
        //=====================================================================
        /*
        Method        : BuildPieceSegment
        Description   : Builds archetype, tertiary, & focus portion of
                        DIM query for single armor piece.
        Parameters    : ArmorPiece piece : Armor piece to build a segment for.
        Return Values : string           : DIM query segment for this piece.
        */
        private static string BuildPieceSegment(ArmorPiece piece){
            string archetype = ArchetypeKeywords[piece.Archetype.Type];
            string tertiary = StatKeywords[piece.TertiaryStat];
            string focus = StatKeywords[piece.FocusStat];
            return $"exactperk:{archetype} tertiarystat:{tertiary} tunedstat:{focus}";
        }
        /*
        Method        : JoinSegments
        Description   : Joins list of piece query segments with OR if they
                        differ, or returns single segment if they are all
                        identical.
        Parameters    : List<string> segments : Query segments to join.
        Return Values : string                : Joined query string.
        */
        private static string JoinSegments(List<string> segments){
            //Check if all segments are identical.
            bool allSame = true;
            for (int i = 1; i < segments.Count; i++){
                if (segments[i] != segments[0]){allSame = false; break;}
            }
            if (allSame) return segments[0];

            //Deduplicate segments before joining.
            List<string> unique = new List<string>();
            foreach (string seg in segments){
                if (!unique.Contains(seg)) unique.Add(seg);
            }
            if (unique.Count == 1) return unique[0];

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < unique.Count; i++){
                if (i > 0) sb.Append(" or ");
                sb.Append($"({unique[i]})");
            }
            return sb.ToString();
        }
    }
}