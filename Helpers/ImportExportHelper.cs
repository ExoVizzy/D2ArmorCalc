/*
*   FILE          : ImportExportHelper.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Provides import & export functionality for build files,
*                   serializing & deserializing full UI state to &
*                   from .d2build files using JSON.
*/
using System.IO;
using System.Windows;
using Microsoft.Win32;
using System.Text.Json;
using System.Text.Json.Serialization;
using D2ArmorCalc_Models;

namespace D2ArmorCalc_Helpers {
    public static class ImportExportHelper {
        private const string FileExtension = ".d2build";
        private const string FileFilter = "D2 Build Files (*.d2build)|*.d2build|All Files (*.*)|*.*";
        private const string CurrentVersion = "2.0";

        private static readonly JsonSerializerOptions JsonOptions = new(){
            WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        //=====================================================================
        //Export.
        //=====================================================================
        /*
        Method        : Export
        Description   : Serializes BuildExport snapshot to .d2build file.
                        Prompts user with save file dialog each time.
        Parameters    : BuildExport export : Build snapshot to export.
        Return Values : bool               : True if export succeeded, false if
                                             cancelled or error occurred.
        */
        public static bool Export(BuildExport export){
            SaveFileDialog dialog = new(){
                Title = "Export Build", Filter = FileFilter, DefaultExt = FileExtension, FileName = "MyBuild"
            };
            if (dialog.ShowDialog() != true) return false;

            try {
                string json = JsonSerializer.Serialize(export, JsonOptions);
                File.WriteAllText(dialog.FileName, json);
                return true;
            } catch (Exception ex){
                MessageBox.Show($"Export failed: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        //=====================================================================
        //Import.
        //=====================================================================
        /*
        Method        : Import
        Description   : Deserializes a .d2build file into BuildExport snapshot.
                        Prompts user with open file dialog each time.
        Parameters    : None.
        Return Values : BuildExport : Build snapshot, or null if cancelled or errored.
        */
        public static BuildExport? Import(){
            OpenFileDialog dialog = new(){Title = "Import Build", Filter = FileFilter};
            if (dialog.ShowDialog() != true) return null;

            try {
                string json = File.ReadAllText(dialog.FileName);
                BuildExport export = JsonSerializer.Deserialize<BuildExport>(json, JsonOptions) ?? throw new JsonException("Deserialized build was null.");

                if (!IsVersionCompatible(export.ExportVersion)){
                    MessageBox.Show($"This build file was created with version {export.ExportVersion} " +
                                    $"& may not be fully compatible with current version.", 
                                    "Version Mismatch", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                return export;
            } catch (JsonException){
                MessageBox.Show("The selected file is not a valid .d2build file.", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            } catch (Exception ex){
                MessageBox.Show($"Import failed: {ex.Message}", "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }
        //=====================================================================
        //Helpers.
        //=====================================================================
        /*
        Method        : IsVersionCompatible
        Description   : Checks whether build file version string is compatible
                        with current application version.
        Parameters    : string version : Version string from build file.
        Return Values : bool           : True if version is compatible.
        */
        private static bool IsVersionCompatible(string version){
            return version == CurrentVersion;
        }
        /*
        Method        : BuildToExport
        Description   : Converts the full current UI state into a serializable
                        BuildExport snapshot ready for export or persistence.
        Parameters    : BuildResult result               : Calculated build result (may be null
                                                           if exporting without a result).
                        StatTargetsExport targets        : Current stat slider targets.
                        bool customTuning                : Whether custom tuning is enabled.
                        string leastWantedStat           : Least wanted stat name.
                        List<TuningSlotExport> tuning    : Focus/minus pairs for each tuning slot.
                        bool fontsEnabled                : Whether fonts section is enabled.
                        bool fontsInStats                : Whether fonts are included in calc.
                        FontCountsExport fontCounts      : Per-stat font slider values.
                        bool armorModsEnabled            : Whether advanced mod mode is on.
                        int minorModCount                : Minor mod count (simple mode).
                        AdvancedModsExport advancedMods  : Per-slot mod selections (advanced mode).
                        bool fragmentsEnabled            : Whether fragments section is enabled.
                        bool showFullSubclass            : Whether full-subclass mode is on.
                        string selectedClass             : Currently selected class (Fragment panel).
                        string selectedSubclass          : Currently selected subclass.
                        List<string> selectedFragments   : Names of checked fragments.
                        SubclassExport subclass          : Full subclass config, or null.
                        string exoticClass               : Selected class in Exotic panel.
                        string exoticSlot                : Selected exotic slot.
                        bool customExoticRoll            : Whether custom roll is enabled.
                        int[] customRollStats            : Six custom stat values [H,M,G,S,C,W].
                        bool t5ExoticEnabled             : Whether T5 exotic toggle is on.
                        bool showDimQueries              : Whether DIM queries are shown.
                        bool subclassCustomization       : Whether subclass panel toggle is on.
        Return Values : BuildExport                      : Populated export snapshot.
        */
        public static BuildExport BuildToExport(
            BuildResult? result, StatTargetsExport targets, 
            bool customTuning, string leastWantedStat, List<TuningSlotExport?> tuning,
            bool fontsEnabled, bool fontsInStats, FontCountsExport fontCounts,
            bool armorModsEnabled, int minorModCount, AdvancedModsExport advancedMods,
            bool fragmentsEnabled, bool showFullSubclass, string selectedClass, 
            string selectedSubclass, List<string> selectedFragments, SubclassExport? subclass,
            string exoticClass, string exoticSlot, bool customExoticRoll, int[] customRollStats,
            bool t5ExoticEnabled, bool showDimQueries, bool subclassCustomization){

            BuildExport export = new(){
                ExportVersion = CurrentVersion,
                //Stat targets.
                StatTargets = targets,
                //Tuning.
                CustomTuning = customTuning,
                LeastWantedStat = leastWantedStat,
                TuningSlots = tuning,
                //Fonts.
                FontsEnabled = fontsEnabled,
                FontsInStats = fontsInStats,
                FontCounts = fontsEnabled ? fontCounts : null,
                //Mods.
                ArmorModsEnabled = armorModsEnabled,
                MinorModCount = minorModCount,
                AdvancedMods = armorModsEnabled ? advancedMods : null,
                //Fragments & subclass.
                FragmentsEnabled = fragmentsEnabled,
                ShowFullSubclass = showFullSubclass,
                SelectedClass = selectedClass,
                SelectedSubclass = selectedSubclass,
                SelectedFragments = selectedFragments,
                Subclass = subclassCustomization ? subclass : null,
                //Exotic.
                ExoticClass = exoticClass,
                ExoticSlot = exoticSlot,
                CustomExoticRoll = customExoticRoll,
                T5ExoticEnabled = t5ExoticEnabled,
                ShowDimQueries = showDimQueries,
                SubclassCustomization = subclassCustomization,
            };
            //Custom exotic roll stat values (all 6 fields).
            if (customExoticRoll && customRollStats?.Length == 6){
                export.ExoticStat1Value = customRollStats[0];
                export.ExoticStat2Value = customRollStats[1];
                export.ExoticStat3Value = customRollStats[2];
                export.ExoticStat4Value = customRollStats[3];
                export.ExoticStat5Value = customRollStats[4];
                export.ExoticStat6Value = customRollStats[5];
            }
            //Serialize each armor piece from the last calculated result (if any).
            if (result != null){
                foreach (ArmorPiece? piece in result.GetPieces()){
                    if (piece == null) continue;

                    ArmorPieceExport pieceExport = new(){
                        Slot = piece.Slot.ToString(), Rarity = piece.Rarity.ToString(),
                        Archetype = piece.Archetype?.Type.ToString(), Tertiary = piece.TertiaryStat.ToString(),
                        FocusStat = piece.FocusStat.ToString(), FocusMinus = piece.FocusMinus.ToString()
                    };

                    if (piece.StatMod != null){
                        pieceExport.StatModType = piece.StatMod.ModType.ToString();
                        pieceExport.StatModStat = piece.StatMod.Stat.ToString();
                    }

                    foreach (Font font in piece.Fonts) pieceExport.Fonts.Add(font.Stat.ToString());

                    export.ArmorPieces.Add(pieceExport);
                }

                //Store standard-roll exotic archetype/tertiary for reference.
                ArmorPiece? exotic = result.Helmet?.Rarity == ArmorRarity.Exotic ? result.Helmet :
                                     result.Arms?.Rarity == ArmorRarity.Exotic ? result.Arms :
                                     result.Chestplate?.Rarity == ArmorRarity.Exotic ? result.Chestplate :
                                     result.Boots?.Rarity == ArmorRarity.Exotic ? result.Boots : result.ClassItem;

                if (exotic != null){
                    export.ExoticArchetype = exotic.Archetype?.Type.ToString();
                    export.ExoticTertiary = exotic.TertiaryStat.ToString();
                }
            }
            return export;
        }
    }
}
