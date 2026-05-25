/*
*   FILE          : AppSettings.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Defines AppSettings model representing user preferences
*                   that persist between sessions, backed by App.config.
*/
using D2ArmorCalc_Models;
using System.Configuration;

namespace D2ArmorCalc_Models {
    //Handles reading & writing persistent user settings via App.config.
    public static class AppSettings {
        //=====================================================================
        //Defaults.
        //=====================================================================
        private const Stat DefaultLeastWantedStat = Stat.Health;
        private const bool DefaultShowDimQueries = false;
        private const bool DefaultCustomExoticRolls = false;
        private const bool DefaultFontsEnabled = false;
        private const bool DefaultFontsInStats = false;
        private const bool DefaultArmorModsEnabled = true;
        private const bool DefaultSubclassCustomization = false;
        private const bool DefaultCustomTuning = false;
        private const bool DefaultT5ExoticEnabled = false;
        private const bool DefaultFragmentsEnabled = false;
        private const bool DefaultShowFullSubclass = false;
        private const string DefaultSelectedClass = "Warlock";
        private const string DefaultSelectedSlot = "Helmet";
        //=====================================================================
        //Properties (read from App.config, fall back to defaults if missing).
        //=====================================================================
        public static Stat LeastWantedStat {
            get => ParseStat(Read("LeastWantedStat"), DefaultLeastWantedStat);
            set => Write("LeastWantedStat", value.ToString());
        }
        public static bool ShowDimQueries {
            get => ParseBool(Read("ShowDimQueries"), DefaultShowDimQueries);
            set => Write("ShowDimQueries", value.ToString());
        }
        public static bool CustomExoticRolls {
            get => ParseBool(Read("CustomExoticRolls"), DefaultCustomExoticRolls);
            set => Write("CustomExoticRolls", value.ToString());
        }
        public static bool FontsEnabled {
            get => ParseBool(Read("FontsEnabled"), DefaultFontsEnabled);
            set => Write("FontsEnabled", value.ToString());
        }
        public static bool FontsInStats {
            get => ParseBool(Read("FontsInStats"), DefaultFontsInStats);
            set => Write("FontsInStats", value.ToString());
        }
        public static bool ArmorModsEnabled {
            get => ParseBool(Read("ArmorModsEnabled"), DefaultArmorModsEnabled);
            set => Write("ArmorModsEnabled", value.ToString());
        }
        public static bool SubclassCustomization {
            get => ParseBool(Read("SubclassCustomization"), DefaultSubclassCustomization);
            set => Write("SubclassCustomization", value.ToString());
        }
        public static bool CustomTuning {
            get => ParseBool(Read("CustomTuning"), DefaultCustomTuning);
            set => Write("CustomTuning", value.ToString());
        }
        public static bool T5ExoticEnabled {
            get => ParseBool(Read("T5ExoticEnabled"), DefaultT5ExoticEnabled);
            set => Write("T5ExoticEnabled", value.ToString());
        }
        public static bool FragmentsEnabled {
            get => ParseBool(Read("FragmentsEnabled"), DefaultFragmentsEnabled);
            set => Write("FragmentsEnabled", value.ToString());
        }
        public static bool ShowFullSubclass {
            get => ParseBool(Read("ShowFullSubclass"), DefaultShowFullSubclass);
            set => Write("ShowFullSubclass", value.ToString());
        }
        public static string SelectedClass {
            get => Read("SelectedClass") ?? DefaultSelectedClass;
            set => Write("SelectedClass", value);
        }
        public static string SelectedSlot {
            get => Read("SelectedSlot") ?? DefaultSelectedSlot;
            set => Write("SelectedSlot", value);
        }
        //=====================================================================
        //Private Helpers.
        //=====================================================================
        /*
        Method        : Read
        Description   : Reads value from App.config by key.
                        Returns null if key does not exist.
        Parameters    : string key : App.config key to read.
        Return Values : string     : Stored value, or null if not found.
        */
        private static string? Read(string key){
            return ConfigurationManager.AppSettings[key];
        }
        /*
        Method        : Write
        Description   : Writes value to App.config by key,
                        creating entry if does not already exist.
        Parameters    : string key   : App.config key to write.
                        string value : Value to store.
        Return Values : void
        */
        private static void Write(string key, string value){
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            if (config.AppSettings.Settings[key] == null) config.AppSettings.Settings.Add(key, value);
            else config.AppSettings.Settings[key].Value = value;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
        /*
        Method        : ParseBool
        Description   : Parses string to bool, returning default value
                        if string is null or cannot be parsed.
        Parameters    : string value      : String to parse.
                        bool defaultValue : Fallback value if parsing fails.
        Return Values : bool              : Parsed or default bool value.
        */
        private static bool ParseBool(string? value, bool defaultValue){
            return bool.TryParse(value, out bool result) ? result : defaultValue;
        }
        /*
        Method        : ParseStat
        Description   : Parses string to Stat enum value, returning
                        default value if string is null or cannot be parsed.
        Parameters    : string value      : String to parse.
                        Stat defaultValue : Fallback Stat if parsing fails.
        Return Values : Stat              : Parsed or default Stat value.
        */
        private static Stat ParseStat(string? value, Stat defaultValue){
            return Enum.TryParse(value, out Stat result) ? result : defaultValue;
        }
        /*
        Method        : ResetToDefaults
        Description   : Resets all settings to default values &
                        saves to App.config.
        Parameters    : None.
        Return Values : void
        */
        public static void ResetToDefaults(){
            LeastWantedStat = DefaultLeastWantedStat;
            ShowDimQueries = DefaultShowDimQueries;
            CustomExoticRolls = DefaultCustomExoticRolls;
            FontsEnabled = DefaultFontsEnabled;
            FontsInStats = DefaultFontsInStats;
            ArmorModsEnabled = DefaultArmorModsEnabled;
            SubclassCustomization = DefaultSubclassCustomization;
            CustomTuning = DefaultCustomTuning;
            T5ExoticEnabled = DefaultT5ExoticEnabled;
            FragmentsEnabled = DefaultFragmentsEnabled;
            ShowFullSubclass = DefaultShowFullSubclass;
            SelectedClass = DefaultSelectedClass;
            SelectedSlot = DefaultSelectedSlot;
        }
    }
}