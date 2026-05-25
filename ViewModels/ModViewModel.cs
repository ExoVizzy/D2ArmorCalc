/*
*   FILE          : ModViewModel.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : ViewModel for armor mod selection, supporting both simple
*                   minor mod count mode & full per-slot mod customization
*                   with live energy tracking.
*/
using D2ArmorCalc_Models;
using System.ComponentModel;

namespace D2ArmorCalc_ViewModels {
    //Wraps ArmorMod with selection state for slot's mod list.
    public class ModSelectionItem(ArmorMod mod){
        public ArmorMod Mod {get;} = mod;
        public string Name => Mod.Name;
        public string EnergyCost => $"{Mod.EnergyCost} energy";
        public override string ToString() => $"{Mod.Name} ({Mod.EnergyCost})";
    }
    public class ModViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //=====================================================================
        //Properties.
        //=====================================================================
        public int MajorModCount => 5 - MinorModCount;
        //Toggle between simple minor mod count & full mod customization.
        public bool ArmorModsEnabled {
            get;
            set {
                field = value;
                OnPropertyChanged(nameof(ArmorModsEnabled));
                OnPropertyChanged(nameof(IsSimpleMode));
            }
        }
        public bool IsSimpleMode => !ArmorModsEnabled;
        //Simple mode: how many of the 5 stat mod slots will be minor mods.
        public int MinorModCount {
            get;
            set {
                field = Math.Max(0, Math.Min(5, value));
                OnPropertyChanged(nameof(MinorModCount));
                OnPropertyChanged(nameof(MajorModCount));
            }
        }
        //Per-slot mod ViewModels (full customization mode).
        public SlotModViewModel HelmetMods {get;} = new(ArmorSlot.Helmet);
        public SlotModViewModel ArmsMods {get;} = new(ArmorSlot.Arms);
        public SlotModViewModel ChestplateMods {get;} = new(ArmorSlot.Chestplate);
        public SlotModViewModel BootsMods {get;} = new(ArmorSlot.Boots);
        public SlotModViewModel ClassItemMods {get;} = new(ArmorSlot.ClassItem);
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