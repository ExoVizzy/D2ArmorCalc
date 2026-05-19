
using D2ArmorCalc_Data;
using D2ArmorCalc_Helpers;
using D2ArmorCalc_Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace D2ArmorCalc_ViewModels {
    //Holds mod selection state for single armor slot.
    public class SlotModViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public ArmorSlot Slot {get;}
        public string SlotLabel {get;}
        public ObservableCollection<ModSelectionItem> AvailableMods {get;}
        private int _fontCount;
        public int FontCount {
            get => _fontCount;
            set {
                _fontCount = value;
                OnPropertyChanged(nameof(FontCount));
                OnPropertyChanged(nameof(Slot1Available));
                OnPropertyChanged(nameof(Slot2Available));
                OnPropertyChanged(nameof(Slot3Available));
            }
        }
        //Each general slot is available if its index is beyond the font count.
        public bool Slot1Available => _fontCount < 1;
        public bool Slot2Available => _fontCount < 2;
        public bool Slot3Available => _fontCount < 3;
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
        private bool _isExotic;
        public bool IsExotic {
            get => _isExotic;
            set {
                _isExotic = value;
                OnPropertyChanged(nameof(IsExotic));
                OnPropertyChanged(nameof(TotalEnergy));
                OnPropertyChanged(nameof(RemainingEnergy));
                OnPropertyChanged(nameof(EnergyLabel));
            }
        }
        public string EnergyLabel => _isExotic ? "10 energy (Exotic)" : $"{TotalEnergy} energy";
        //Energy tracking (assumes legendary 11 energy, no fonts or stat mods for display).
        public int TotalEnergy => _isExotic ? EnergyHelper.ExoticEnergy : EnergyHelper.LegendaryEnergy;
        public int UsedEnergy => (_modSlot1?.Mod.EnergyCost ?? 0) + (_modSlot2?.Mod.EnergyCost ?? 0) + (_modSlot3?.Mod.EnergyCost ?? 0);
        public int RemainingEnergy => TotalEnergy - UsedEnergy;
        public SlotModViewModel(ArmorSlot slot){
            Slot = slot;
            SlotLabel = slot.ToString();

            //Populate available mods for this slot plus blank "None" option.
            AvailableMods = [new ModSelectionItem(new("None", 0, slot))];
            foreach (ArmorMod mod in ArmorMods.GetModsBySlot(slot)) AvailableMods.Add(new ModSelectionItem(mod));

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
            List<ArmorMod> mods = [];
            if (_modSlot1?.Mod.EnergyCost > 0) mods.Add(_modSlot1.Mod);
            if (_modSlot2?.Mod.EnergyCost > 0) mods.Add(_modSlot2.Mod);
            if (_modSlot3?.Mod.EnergyCost > 0) mods.Add(_modSlot3.Mod);
            return [.. mods];
        }
    }
}
