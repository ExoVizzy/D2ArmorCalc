/*
*   FILE          : ExoticViewModel.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : ViewModel for exotic armor selection, handling slot &
*                   class selection, standard stat assumptions, & custom
*                   roll input for all 6 stats.
*/
using D2ArmorCalc_Data;
using D2ArmorCalc_Helpers;
using D2ArmorCalc_Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace D2ArmorCalc_ViewModels {
    public class ExoticViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //=====================================================================
        //Properties.
        //=====================================================================
        public string SelectedClass {
            get;
            set {
                field = value; 
                OnPropertyChanged(nameof(SelectedClass));
            }
        } = "Warlock";
        //Exotic slot selector.
        public string SelectedSlot {
            get;
            set {
                field = value; 
                OnPropertyChanged(nameof(SelectedSlot));
            }
        } = "Helmet";
        public string SelectedArchetype {
            get;
            set {
                field = value; 
                OnPropertyChanged(nameof(SelectedArchetype));
            }
        } = "Gunner";
        //Custom roll toggle.
        public bool CustomRollEnabled {
            get;
            set {
                field = value;
                OnPropertyChanged(nameof(CustomRollEnabled));
                OnPropertyChanged(nameof(IsStandardRoll));
            }
        }
        public bool IsStandardRoll => !CustomRollEnabled;
        //Standard roll display (read-only, shown when custom roll is off).
        public static string StandardRollDisplay => "Primary: 30  Secondary: 20  Tertiary: 12";
        //Custom roll stat inputs (all 6 stats, for pre-rework exotics).
        public int CustomHealth {
            get;
            set {field = StatHelper.Clamp(value); OnPropertyChanged(nameof(CustomHealth));}
        }
        public int CustomMelee {
            get;
            set {field = StatHelper.Clamp(value); OnPropertyChanged(nameof(CustomMelee));}
        }
        public int CustomGrenade {
            get;
            set {field = StatHelper.Clamp(value); OnPropertyChanged(nameof(CustomGrenade));}
        }
        public int CustomSuper {
            get;
            set {field = StatHelper.Clamp(value); OnPropertyChanged(nameof(CustomSuper));}
        }
        public int CustomClass {
            get;
            set {field = StatHelper.Clamp(value); OnPropertyChanged(nameof(CustomClass));}
        }
        public int CustomWeapons {
            get;
            set {field = StatHelper.Clamp(value); OnPropertyChanged(nameof(CustomWeapons));}
        }
        //T5 exotic toggle (greyed out until released).
        public bool T5ExoticEnabled {
            get;
            set {
                field = value; 
                OnPropertyChanged(nameof(T5ExoticEnabled));
            }
        }
        //=====================================================================
        //Helpers.
        //=====================================================================
        /*
        Method        : GetPlayerClass
        Description   : Returns PlayerClass enum value for currently
                        selected class string.
        Parameters    : None.
        Return Values : PlayerClass : Selected player class.
        */
        public PlayerClass GetPlayerClass(){
            return SelectedClass switch {
                "Titan" => PlayerClass.Titan,
                "Hunter" => PlayerClass.Hunter,
                _ => PlayerClass.Warlock,
            };
        }
        /*
        Method        : GetArmorSlot
        Description   : Returns ArmorSlot enum value for currently
                        selected exotic slot string.
        Parameters    : None.
        Return Values : ArmorSlot : Selected exotic armor slot.
        */
        public ArmorSlot GetArmorSlot(){
            return SelectedSlot switch {
                "Arms" => ArmorSlot.Arms,
                "Chestplate" => ArmorSlot.Chestplate,
                "Boots" => ArmorSlot.Boots,
                "Class Item" => ArmorSlot.ClassItem,
                _ => ArmorSlot.Helmet,
            };
        }
        /*
        Method        : BuildExoticPiece
        Description   : Constructs ArmorPiece representing exotic based
                        on current selections. Uses standard 30/20/12 assumption
                        if custom roll is disabled, otherwise uses custom inputs.
        Parameters    : None.
        Return Values : ArmorPiece : Configured exotic armor piece.
        */
        public ArmorPiece BuildExoticPiece(){
            ArmorSlot slot = GetArmorSlot();
            ArmorPiece piece = new(slot, ArmorRarity.Exotic);
            Archetype archetype = Archetypes.AllArchetypes.FirstOrDefault(a => a.Type.ToString() == SelectedArchetype) ?? Archetypes.Gunner;
            piece.Archetype = archetype;

            if (CustomRollEnabled){
                piece.CustomStatBlock = new StatBlock(
                    CustomHealth, CustomMelee, CustomGrenade,
                    CustomSuper, CustomClass, CustomWeapons
                );
                piece.IsCustomRoll = true;
            } else {
                piece.IsCustomRoll = false;
                piece.StandardPrimary = 30;
                piece.StandardSecondary = 20;
                piece.StandardTertiary = 12;
                //Set tertiary to first valid tertiary for this archetype
                piece.TertiaryStat = Archetypes.GetTertiaryStats(archetype)[0];
                piece.FocusStat = archetype.Primary;
                piece.FocusMinus = (Stat)AppSettings.LeastWantedStat;
            }
            return piece;
        }
    }
}