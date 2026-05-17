/*
*   FILE          : BuildExport.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Defines BuildExport model representing serializable
*                   snapshot of full UI state for import & export.
*/
namespace D2ArmorCalc {
    //Serializable snapshot of single armor piece configuration.
    public class ArmorPieceExport {
        public string Slot {get; set;}
        public string Rarity {get; set;}
        public string Archetype {get; set;}
        public string Tertiary {get; set;}
        public string FocusStat {get; set;}
        public string FocusMinus {get; set;}
        public string StatModType {get; set;} //"Major", "Minor", or null.
        public string StatModStat {get; set;} //Stat name or null.
        public List<string> Fonts {get; set;} = new List<string>();
        public List<string> ArmorMods {get; set;} = new List<string>();
    }
    //Serializable snapshot of a subclass configuration.
    public class SubclassExport {
        public string PlayerClass {get; set;}
        public string SubclassName {get; set;}
        public string Super {get; set;}
        public string Melee {get; set;}
        public string Grenade {get; set;}
        public string ClassAbility {get; set;}
        public string Jump {get; set;}
        public List<string> Aspects {get; set;} = new List<string>();
        public List<string> Fragments {get; set;} = new List<string>();
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
    //Full serializable snapshot of UI state.
    public class BuildExport {
        public string ExportVersion {get; set;} = "1.0";
        public StatTargetsExport StatTargets {get; set;}
        public SubclassExport Subclass {get; set;} //null if not configured.
        public List<ArmorPieceExport> ArmorPieces {get; set;} = new List<ArmorPieceExport>();
        //Exotic.
        public bool CustomExoticRoll {get; set;}
        public string ExoticSlot {get; set;} //null if not selected.
        public string ExoticArchetype {get; set;}
        public string ExoticTertiary {get; set;}
        public int ExoticStat1Value {get; set;}
        public int ExoticStat2Value {get; set;}
        public int ExoticStat3Value {get; set;}

        //Toggle states.
        public bool FontsEnabled {get; set;}
        public bool ArmorModsEnabled {get; set;}
        public bool SubclassCustomization {get; set;}
        public bool CustomTuning {get; set;}
        public bool T5ExoticEnabled {get; set;}

        //Misc.
        public string LeastWantedStat {get; set;}
        public int MinorModCount {get; set;} //Used when ArmorModsEnabled is false.
    }
}