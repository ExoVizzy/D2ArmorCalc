/*
*   FILE          : ExoticViewModel.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : ViewModel for exotic armor selection, handling slot &
*                   class selection, standard stat assumptions, & custom
*                   roll input for all 6 stats.
*/
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace D2ArmorCalc {
    public class ExoticViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name){
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        //=====================================================================
        //Properties.
        //=====================================================================
        //Class selector.
        public ObservableCollection<string> Classes {get;} =
            new ObservableCollection<string> {"Warlock", "Titan", "Hunter" };

        private string _selectedClass = "Warlock";
        public string SelectedClass {
            get => _selectedClass;
            set {_selectedClass = value; OnPropertyChanged(nameof(SelectedClass));}
        }
        //Exotic slot selector.
        public ObservableCollection<string> Slots {get;} =
            new ObservableCollection<string> {"Helmet", "Arms", "Chestplate", "Boots", "Class Item" };

        private string _selectedSlot = "Helmet";
        public string SelectedSlot {
            get => _selectedSlot;
            set {_selectedSlot = value; OnPropertyChanged(nameof(SelectedSlot));}
        }
        //Custom roll toggle.
        private bool _customRollEnabled;
        public bool CustomRollEnabled {
            get => _customRollEnabled;
            set {
                _customRollEnabled = value;
                OnPropertyChanged(nameof(CustomRollEnabled));
                OnPropertyChanged(nameof(IsStandardRoll));
            }
        }
        public bool IsStandardRoll => !_customRollEnabled;
        //Standard roll display (read-only, shown when custom roll is off).
        public string StandardRollDisplay => "Primary: 30  Secondary: 20  Tertiary: 12";
        //Custom roll stat inputs (all 6 stats, for pre-rework exotics).
        private int _customHealth;
        public int CustomHealth {
            get => _customHealth;
            set {_customHealth = StatHelper.Clamp(value); OnPropertyChanged(nameof(CustomHealth));}
        }
        private int _customMelee;
        public int CustomMelee {
            get => _customMelee;
            set {_customMelee = StatHelper.Clamp(value); OnPropertyChanged(nameof(CustomMelee));}
        }
        private int _customGrenade;
        public int CustomGrenade {
            get => _customGrenade;
            set {_customGrenade = StatHelper.Clamp(value); OnPropertyChanged(nameof(CustomGrenade));}
        }
        private int _customSuper;
        public int CustomSuper {
            get => _customSuper;
            set {_customSuper = StatHelper.Clamp(value); OnPropertyChanged(nameof(CustomSuper));}
        }
        private int _customClass;
        public int CustomClass {
            get => _customClass;
            set {_customClass = StatHelper.Clamp(value); OnPropertyChanged(nameof(CustomClass));}
        }
        private int _customWeapons;
        public int CustomWeapons {
            get => _customWeapons;
            set {_customWeapons = StatHelper.Clamp(value); OnPropertyChanged(nameof(CustomWeapons));}
        }
        //T5 exotic toggle (greyed out until released).
        private bool _t5ExoticEnabled;
        public bool T5ExoticEnabled {
            get => _t5ExoticEnabled;
            set {_t5ExoticEnabled = value; OnPropertyChanged(nameof(T5ExoticEnabled));}
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
            switch (_selectedClass){
                case "Titan": return PlayerClass.Titan;
                case "Hunter": return PlayerClass.Hunter;
                default: return PlayerClass.Warlock;
            }
        }
        /*
        Method        : GetArmorSlot
        Description   : Returns ArmorSlot enum value for currently
                        selected exotic slot string.
        Parameters    : None.
        Return Values : ArmorSlot : Selected exotic armor slot.
        */
        public ArmorSlot GetArmorSlot(){
            switch (_selectedSlot){
                case "Arms":       return ArmorSlot.Arms;
                case "Chestplate": return ArmorSlot.Chestplate;
                case "Boots":      return ArmorSlot.Boots;
                case "Class Item": return ArmorSlot.ClassItem;
                default:           return ArmorSlot.Helmet;
            }
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
            ArmorPiece piece = new ArmorPiece(slot, ArmorRarity.Exotic);

            if (_customRollEnabled){
                //Custom roll — user defines all 6 stats directly
                //Store as special archetype-less piece using flat StatBlock
                piece.CustomStatBlock = new StatBlock(
                    _customHealth, _customMelee, _customGrenade,
                    _customSuper, _customClass, _customWeapons
                );
                piece.IsCustomRoll = true;
            } else {
                //Standard assumption: primary 30, secondary 20, tertiary 12
                //Use archetype matching exotic slot's most common archetype
                //Left to ArmorCalculator to handle since slot doesn't fix archetype
                piece.IsCustomRoll = false;
                piece.StandardPrimary = 30;
                piece.StandardSecondary = 20;
                piece.StandardTertiary = 12;
            }
            return piece;
        }
    }
}