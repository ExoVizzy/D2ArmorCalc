/*
*   FILE          : ModViewModel.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : ViewModel for armor mod selection, supporting both simple
*                   minor mod count mode & full per-slot mod customization
*                   with live energy tracking.
*/
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace D2ArmorCalc {
    //Wraps ArmorMod with selection state for slot's mod list.
    public class ModSelectionItem {
        public ArmorMod Mod {get;}
        public string Name => Mod.Name;
        public string EnergyCost => $"{Mod.EnergyCost} energy";
        public ModSelectionItem(ArmorMod mod){
            Mod = mod;
        }
    }
    //Holds mod selection state for single armor slot.
    public class SlotModViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public ArmorSlot Slot {get;}
        public string SlotLabel {get;}
        public ObservableCollection<ModSelectionItem> AvailableMods {get;}
        //Up to 3 general mod slots.
        private ModSelectionItem _modSlot1;
        private ModSelectionItem _modSlot2;
        private ModSelectionItem _modSlot3;
        public ModSelectionItem ModSlot1 {
            get => _modSlot1;
            set {_modSlot1 = value; OnPropertyChanged(nameof(ModSlot1)); OnPropertyChanged(nameof(UsedEnergy)); OnPropertyChanged(nameof(RemainingEnergy));}
        }
        public ModSelectionItem ModSlot2 {
            get => _modSlot2;
            set {_modSlot2 = value; OnPropertyChanged(nameof(ModSlot2)); OnPropertyChanged(nameof(UsedEnergy)); OnPropertyChanged(nameof(RemainingEnergy));}
        }
        public ModSelectionItem ModSlot3 {
            get => _modSlot3;
            set {_modSlot3 = value; OnPropertyChanged(nameof(ModSlot3)); OnPropertyChanged(nameof(UsedEnergy)); OnPropertyChanged(nameof(RemainingEnergy));}
        }
        //Energy tracking (assumes legendary 11 energy, no fonts or stat mods for display).
        public int TotalEnergy => EnergyHelper.LegendaryEnergy;
        public int UsedEnergy => (_modSlot1?.Mod.EnergyCost ?? 0) +
                                      (_modSlot2?.Mod.EnergyCost ?? 0) +
                                      (_modSlot3?.Mod.EnergyCost ?? 0);
        public int RemainingEnergy => TotalEnergy - UsedEnergy;
        public SlotModViewModel(ArmorSlot slot){
            Slot = slot;
            SlotLabel = slot.ToString();

            //Populate available mods for this slot plus a blank "None" option.
            AvailableMods = new ObservableCollection<ModSelectionItem>();
            AvailableMods.Add(new ModSelectionItem(new ArmorMod("None", 0, slot)));
            foreach (ArmorMod mod in ArmorMods.GetModsBySlot(slot))
                AvailableMods.Add(new ModSelectionItem(mod));

            _modSlot1 = AvailableMods[0];
            _modSlot2 = AvailableMods[0];
            _modSlot3 = AvailableMods[0];
        }
        /*
        Method        : GetSelectedMods
        Description   : Returns all non-null, non-None mods selected for
                        this slot as array.
        Parameters    : None.
        Return Values : ArmorMod[] : Selected mods for slot.
        */
        public ArmorMod[] GetSelectedMods(){
            List<ArmorMod> mods = new List<ArmorMod>();
            if (_modSlot1?.Mod.EnergyCost > 0) mods.Add(_modSlot1.Mod);
            if (_modSlot2?.Mod.EnergyCost > 0) mods.Add(_modSlot2.Mod);
            if (_modSlot3?.Mod.EnergyCost > 0) mods.Add(_modSlot3.Mod);
            return mods.ToArray();
        }
    }
    public class ModViewModel : INotifyPropertyChanged {
        public int MajorModCount => 5 - MinorModCount;

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //=====================================================================
        //Properties.
        //=====================================================================
        //Toggle between simple minor mod count & full mod customization.
        private bool _armorModsEnabled;
        public bool ArmorModsEnabled {
            get => _armorModsEnabled;
            set {
                _armorModsEnabled = value;
                OnPropertyChanged(nameof(ArmorModsEnabled));
                OnPropertyChanged(nameof(IsSimpleMode));
            }
        }
        public bool IsSimpleMode => !_armorModsEnabled;
        //Simple mode: how many of the 5 stat mod slots will be minor mods.
        private int _minorModCount;
        public int MinorModCount {
            get => _minorModCount;
            set {
                _minorModCount = Math.Max(0, Math.Min(5, value));
                OnPropertyChanged(nameof(MinorModCount));
            }
        }
        //Per-slot mod ViewModels (full customization mode).
        public SlotModViewModel HelmetMods {get;} = new SlotModViewModel(ArmorSlot.Helmet);
        public SlotModViewModel ArmsMods {get;} = new SlotModViewModel(ArmorSlot.Arms);
        public SlotModViewModel ChestplateMods {get;} = new SlotModViewModel(ArmorSlot.Chestplate);
        public SlotModViewModel BootsMods {get;} = new SlotModViewModel(ArmorSlot.Boots);
        public SlotModViewModel ClassItemMods {get;} = new SlotModViewModel(ArmorSlot.ClassItem);
        //=====================================================================
        //Output Helpers.
        //=====================================================================
        /*
        Method        : GetAllSelectedMods
        Description   : Returns dictionary of all selected armor mods per
                        slot for use by energy validator & export.
        Parameters    : None.
        Return Values : Dictionary<ArmorSlot, ArmorMod[]> : Mods per slot.
        */
        public Dictionary<ArmorSlot, ArmorMod[]> GetAllSelectedMods(){
            return new Dictionary<ArmorSlot, ArmorMod[]> {
                {ArmorSlot.Helmet, HelmetMods.GetSelectedMods()},
                {ArmorSlot.Arms, ArmsMods.GetSelectedMods()},
                {ArmorSlot.Chestplate, ChestplateMods.GetSelectedMods()},
                {ArmorSlot.Boots, BootsMods.GetSelectedMods()},
                {ArmorSlot.ClassItem, ClassItemMods.GetSelectedMods()}
            };
        }
    }
}