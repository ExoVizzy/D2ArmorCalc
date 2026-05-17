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

namespace D2ArmorCalc {
    public static class ImportExportHelper {
        private const string FileExtension = ".d2build";
        private const string FileFilter = "D2 Build Files (*.d2build)|*.d2build|All Files (*.*)|*.*";
        private const string CurrentVersion = "1.0";

        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions {
            WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        //=====================================================================
        //Export.
        //=====================================================================
        /*
        Method        : Export
        Description   : Serializes BuildExport snapshot to a .d2build file.
                        Prompts user with save file dialog each time.
        Parameters    : BuildExport export : Build snapshot to export.
        Return Values : bool               : True if export succeeded, false if
                                             cancelled or error occurred.
        */
        public static bool Export(BuildExport export) {
            SaveFileDialog dialog = new SaveFileDialog {
                Title = "Export Build", Filter = FileFilter,
                DefaultExt = FileExtension, FileName = "MyBuild"
            };
            if (dialog.ShowDialog() != true) return false;

            try {
                string json = JsonSerializer.Serialize(export, JsonOptions);
                File.WriteAllText(dialog.FileName, json);
                return true;
            } catch (Exception ex) {
                MessageBox.Show($"Export failed: {ex.Message}", "Export Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
        Return Values : BuildExport : Deserialized build snapshot, or null
                                      if cancelled or error occurred.
        */
        public static BuildExport Import() {
            OpenFileDialog dialog = new OpenFileDialog {
                Title = "Import Build", Filter = FileFilter
            };
            if (dialog.ShowDialog() != true) return null;

            try {
                string json = File.ReadAllText(dialog.FileName);
                var    export = JsonSerializer.Deserialize<BuildExport>(json, JsonOptions);

                if (!IsVersionCompatible(export.ExportVersion)) {
                    MessageBox.Show(
                        $"This build file was created with version {export.ExportVersion} " +
                        $"and may not be fully compatible with the current version.",
                        "Version Mismatch", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                return export;
            } catch (JsonException) {
                MessageBox.Show("The selected file is not a valid .d2build file.",
                    "Import Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            } catch (Exception ex) {
                MessageBox.Show($"Import failed: {ex.Message}", "Import Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
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
        private static bool IsVersionCompatible(string version) {
            return version == CurrentVersion;
        }
        /*
        Method        : BuildToExport
        Description   : Converts BuildResult & current UI state into
                        serializable BuildExport snapshot ready for export.
        Parameters    : BuildResult result     : Calculated build result.
                        StatTargetsExport targets : Current stat targets.
                        SubclassExport subclass   : Current subclass config, or null.
                        bool fontsEnabled         : Whether fonts toggle is on.
                        bool armorModsEnabled      : Whether armor mods toggle is on.
                        bool subclassCustomization : Whether subclass toggle is on.
                        bool customTuning          : Whether custom tuning toggle is on.
                        bool t5ExoticEnabled       : Whether T5 exotic toggle is on.
                        bool customExoticRoll      : Whether custom exotic roll is on.
                        string leastWantedStat     : Least wanted stat name.
                        int minorModCount          : Minor mod count if mods disabled.
        Return Values : BuildExport               : Populated export snapshot.
        */
        public static BuildExport BuildToExport(
            BuildResult result, StatTargetsExport targets, SubclassExport subclass,
            bool fontsEnabled, bool armorModsEnabled, bool subclassCustomization,
            bool customTuning, bool t5ExoticEnabled, bool customExoticRoll,
            string leastWantedStat, int minorModCount) {

            BuildExport export = new BuildExport {
                ExportVersion = CurrentVersion, StatTargets = targets,
                Subclass = subclassCustomization ? subclass : null,
                FontsEnabled = fontsEnabled, ArmorModsEnabled = armorModsEnabled,
                SubclassCustomization = subclassCustomization, CustomTuning = customTuning,
                T5ExoticEnabled = t5ExoticEnabled, CustomExoticRoll = customExoticRoll,
                LeastWantedStat = leastWantedStat, MinorModCount = minorModCount
            };

            //Serialize each armor piece.
            foreach (var piece in result.GetPieces()) {
                if (piece == null) continue;

                ArmorPieceExport pieceExport = new ArmorPieceExport {
                    Slot = piece.Slot.ToString(), Rarity = piece.Rarity.ToString(),
                    Archetype = piece.Archetype?.Type.ToString(), Tertiary = piece.TertiaryStat.ToString(),
                    FocusStat = piece.FocusStat.ToString(), FocusMinus = piece.FocusMinus.ToString()
                };

                if (piece.StatMod != null) {
                    pieceExport.StatModType = piece.StatMod.ModType.ToString();
                    pieceExport.StatModStat = piece.StatMod.Stat.ToString();
                }

                foreach (var font in piece.Fonts)
                    pieceExport.Fonts.Add(font.Stat.ToString());

                export.ArmorPieces.Add(pieceExport);
            }
            //Exotic custom roll.
            if (customExoticRoll) {
                var exotic = result.Helmet?.Rarity == ArmorRarity.Exotic ? result.Helmet :
                             result.Arms?.Rarity == ArmorRarity.Exotic ? result.Arms :
                             result.Chestplate?.Rarity == ArmorRarity.Exotic ? result.Chestplate :
                             result.Boots?.Rarity == ArmorRarity.Exotic ? result.Boots : result.ClassItem;

                if (exotic != null) {
                    export.ExoticSlot = exotic.Slot.ToString();
                    export.ExoticArchetype = exotic.Archetype?.Type.ToString();
                    export.ExoticTertiary = exotic.TertiaryStat.ToString();
                    export.ExoticStat1Value = exotic.GetBaseStat(exotic.Archetype.Primary);
                    export.ExoticStat2Value = exotic.GetBaseStat(exotic.Archetype.Secondary);
                    export.ExoticStat3Value = exotic.GetBaseStat(exotic.TertiaryStat);
                }
            }
            return export;
        }
    }
}