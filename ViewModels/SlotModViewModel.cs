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
        public ObservableCollection<ModSelectionItem> AvailableMods {get;}
        public ArmorSlot Slot {get;}
        public string SlotLabel {get;}
        public int FontCount {
            get;
            set {
                field = value;
                OnPropertyChanged(nameof(FontCount));
                OnPropertyChanged(nameof(Slot1Available));
                OnPropertyChanged(nameof(Slot2Available));
                OnPropertyChanged(nameof(Slot3Available));
            }
        }
        //Each general slot is available if its index is beyond font count.
        public bool Slot1Available => FontCount < 1;
        public bool Slot2Available => FontCount < 2;
        public bool Slot3Available => FontCount < 3;
        //Up to 3 general mod slots.
        public ModSelectionItem ModSlot1 {
            get;
            set {
                field = value; 
                OnPropertyChanged(nameof(ModSlot1)); 
                OnPropertyChanged(nameof(UsedEnergy)); 
                OnPropertyChanged(nameof(RemainingEnergy));}
        }
        public ModSelectionItem ModSlot2 {
            get;
            set {
                field = value; 
                OnPropertyChanged(nameof(ModSlot2)); 
                OnPropertyChanged(nameof(UsedEnergy)); 
                OnPropertyChanged(nameof(RemainingEnergy));}
        }
        public ModSelectionItem ModSlot3 {
            get;
            set {
                field = value; 
                OnPropertyChanged(nameof(ModSlot3)); 
                OnPropertyChanged(nameof(UsedEnergy)); 
                OnPropertyChanged(nameof(RemainingEnergy));}
        }
        public bool IsExotic {
            get;
            set {
                field = value;
                OnPropertyChanged(nameof(IsExotic));
                OnPropertyChanged(nameof(TotalEnergy));
                OnPropertyChanged(nameof(RemainingEnergy));
                OnPropertyChanged(nameof(EnergyLabel));
            }
        }
        public string EnergyLabel => IsExotic ? "10 energy (Exotic)" : $"{TotalEnergy} energy";
        //Energy tracking (assumes legendary 11 energy, no fonts or stat mods for display).
        public int TotalEnergy => IsExotic ? EnergyHelper.ExoticEnergy : EnergyHelper.LegendaryEnergy;
        public int UsedEnergy => (ModSlot1?.Mod.EnergyCost ?? 0) + (ModSlot2?.Mod.EnergyCost ?? 0) + (ModSlot3?.Mod.EnergyCost ?? 0);
        public int RemainingEnergy => TotalEnergy - UsedEnergy;
        public SlotModViewModel(ArmorSlot slot){
            Slot = slot;
            SlotLabel = slot.ToString();

            //Populate available mods for this slot plus blank "None" option.
            AvailableMods = [new ModSelectionItem(new("None", 0, slot))];
            foreach (ArmorMod mod in ArmorMods.GetModsBySlot(slot)) AvailableMods.Add(new ModSelectionItem(mod));

            ModSlot1 = AvailableMods[0];
            ModSlot2 = AvailableMods[0];
            ModSlot3 = AvailableMods[0];
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
            if (ModSlot1?.Mod.EnergyCost > 0) mods.Add(ModSlot1.Mod);
            if (ModSlot2?.Mod.EnergyCost > 0) mods.Add(ModSlot2.Mod);
            if (ModSlot3?.Mod.EnergyCost > 0) mods.Add(ModSlot3.Mod);
            return [.. mods];
        }
        /*
        Method        : GetSelectedModNames
        Description   : Returns display names of all non-None selected mods
                        for this slot, for use during export serialization.
        Parameters    : None.
        Return Values : List<string> : Names of selected mods (excludes "None"/empty slots).
        */
        public List<string> GetSelectedModNames(){
            List<string> names = [];
            if (ModSlot1?.Mod.EnergyCost > 0) names.Add(ModSlot1.Mod.Name);
            if (ModSlot2?.Mod.EnergyCost > 0) names.Add(ModSlot2.Mod.Name);
            if (ModSlot3?.Mod.EnergyCost > 0) names.Add(ModSlot3.Mod.Name);
            return names;
        }
        /*
        Method        : RestoreSelections
        Description   : Restores mod slot selections from list of mod names,
                        matching each name against AvailableMods for this slot.
                        Unrecognized names are silently skipped.
        Parameters    : List<string> modNames : Mod names to restore (in slot order).
        Return Values : void
        */
        public void RestoreSelections(List<string> modNames){
            //Reset all slots to None first.
            ModSelectionItem none = AvailableMods[0];
            ModSlot1 = none;
            ModSlot2 = none;
            ModSlot3 = none;

            int slotIndex = 0;
            foreach (string name in modNames){
                ModSelectionItem? match = AvailableMods.FirstOrDefault(m => m.Mod.Name == name);
                if (match == null) continue;
                if (slotIndex == 0) ModSlot1 = match;
                else if (slotIndex == 1) ModSlot2 = match;
                else if (slotIndex == 2) ModSlot3 = match;
                else break; //Only 3 slots available.

                slotIndex++;
            }
        }
    }
}