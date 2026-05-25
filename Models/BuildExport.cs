/*
*   FILE          : BuildExport.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Defines BuildExport model representing serializable
*                   snapshot of full UI state for import & export.
*/
namespace D2ArmorCalc_Models {
    //Serializable snapshot of single armor piece configuration.
    public class ArmorPieceExport {
        public string? Slot {get; set;}
        public string? Rarity {get; set;}
        public string? Archetype {get; set;}
        public string? Tertiary {get; set;}
        public string? FocusStat {get; set;}
        public string? FocusMinus {get; set;}
        public string? StatModType {get; set;} //"Major", "Minor", or null.
        public string? StatModStat {get; set;} //Stat name or null.
        public List<string> Fonts {get; set;} = [];
        public List<string> ArmorMods {get; set;} = [];
    }
    //Serializable snapshot of a subclass configuration.
    public class SubclassExport {
        public string? PlayerClass {get; set;}
        public string? SubclassName {get; set;}
        public string? Super {get; set;}
        public string? Melee {get; set;}
        public string? Grenade {get; set;}
        public string? ClassAbility {get; set;}
        public string? Jump {get; set;}
        public List<string> Aspects {get; set;} = [];
        public List<string> Fragments {get; set;} = [];
    }

    //Serializable snapshot of stat min/max targets.
    public class StatTargetsExport {
        public int HealthMin {get; set;}
        public int HealthMax {get; set;}
        public int MeleeMin {get; set;}
        public int MeleeMax {get; set;}
        public int GrenadeMin {get; set;}
        public int GrenadeMax {get; set;}
        public int SuperMin {get; set;}
        public int SuperMax {get; set;}
        public int ClassMin {get; set;}
        public int ClassMax {get; set;}
        public int WeaponsMin {get; set;}
        public int WeaponsMax {get; set;}
    }
    //Serializable snapshot of single tuning slot (focus/minus stat pair).
    public class TuningSlotExport {
        public string? FocusStat {get; set;}
        public string? FocusMinus {get; set;}
    }
    //Serializable snapshot of all per-stat font slider counts.
    public class FontCountsExport {
        public int Super {get; set;}
        public int Grenade {get; set;}
        public int Melee {get; set;}
        public int Health {get; set;}
        public int Weapons {get; set;}
        public int Class {get; set;}
    }
    //Serializable snapshot of per-slot advanced armor mod selections.
    public class AdvancedModsExport {
        public List<string> Helmet {get; set;} = [];
        public List<string> Arms {get; set;} = [];
        public List<string> Chestplate {get; set;} = [];
        public List<string> Boots {get; set;} = [];
        public List<string> ClassItem {get; set;} = [];
    }
    //Full serializable snapshot of UI state.
    public class BuildExport {
        public string ExportVersion {get; set;} = "2.0";
        //=====================================================================
        //Stat Targets.
        //=====================================================================
        public StatTargetsExport? StatTargets {get; set;}
        //=====================================================================
        //Tuning.
        //=====================================================================
        public bool CustomTuning {get; set;}
        public string? LeastWantedStat {get; set;}
        //Null entries are preserved so slot indices stay stable.
        public List<TuningSlotExport?> TuningSlots {get; set;} = [];
        //=====================================================================
        //Fonts.
        //=====================================================================
        public bool FontsEnabled {get; set;}
        public bool FontsInStats {get; set;}
        public FontCountsExport? FontCounts {get; set;}
        //=====================================================================
        //Stat Mods.
        //=====================================================================
        public bool ArmorModsEnabled {get; set;}
        public int MinorModCount {get; set;} //Used when ArmorModsEnabled is false.
        public AdvancedModsExport? AdvancedMods {get; set;} //Used when ArmorModsEnabled is true.
        //=====================================================================
        //Fragments & Subclass.
        //=====================================================================
        public bool FragmentsEnabled {get; set;}
        public bool ShowFullSubclass {get; set;}
        public string? SelectedClass {get; set;}
        public string? SelectedSubclass {get; set;}
        //Fragments (always stored so stat-only mode restores correctly).
        public List<string> SelectedFragments {get; set;} = [];
        //Full subclass fields. null when ShowFullSubclass is false.
        public SubclassExport? Subclass {get; set;}
        //=====================================================================
        //Exotic.
        //=====================================================================
        public string? ExoticClass {get; set;}
        public string? ExoticSlot {get; set;}
        public bool CustomExoticRoll {get; set;}
        //Custom roll stat values (used when CustomExoticRoll is true).
        public int ExoticStat1Value {get; set;}
        public int ExoticStat2Value {get; set;}
        public int ExoticStat3Value {get; set;}
        public int ExoticStat4Value {get; set;}
        public int ExoticStat5Value {get; set;}
        public int ExoticStat6Value {get; set;}
        //Standard-roll fields used when building ArmorPieces.
        public string? ExoticArchetype {get; set;}
        public string? ExoticTertiary {get; set;}
        public bool T5ExoticEnabled {get; set;}
        //=====================================================================
        //DIM Queries.
        //=====================================================================
        public bool ShowDimQueries {get; set;}
        //=====================================================================
        //Subclass Customization toggle (full-subclass panel toggle).
        //=====================================================================
        public bool SubclassCustomization {get; set;}
        //=====================================================================
        //Result armor pieces (populated after calculation, used for re-display).
        //=====================================================================
        public List<ArmorPieceExport> ArmorPieces {get; set;} = [];
    }
}
