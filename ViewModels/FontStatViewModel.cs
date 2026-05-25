/*
*   FILE          : FontStatViewModel.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : ViewModel for single font slot row, holding font
*                   count for specific armor slot & its available fonts.
*/
using D2ArmorCalc_Data;
using D2ArmorCalc_Models;
using System.ComponentModel;

namespace D2ArmorCalc_ViewModels {
    public class FontStatViewModel(Stat stat, ArmorSlot slot) : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //=====================================================================
        //Properties.
        //=====================================================================
        public Stat Stat {get;} = stat;
        public ArmorSlot Slot {get;} = slot;
        public string? StatLabel {get;} = stat.ToString();
        public string? SlotLabel {get;} = slot.ToString();
        public static int MaxFonts => 3;
        public int FontCount {
            get;
            set {
                field = Math.Max(0, Math.Min(MaxFonts, value));
                OnPropertyChanged(nameof(FontCount));
                OnPropertyChanged(nameof(TotalBonus));
                OnPropertyChanged(nameof(BonusLabel));
            }
        } = 0;
        //Total stat bonus from current font count.
        public int TotalBonus => Fonts.GetTotalBonus(FontCount);
        public string BonusLabel => FontCount > 0 ? $"+{TotalBonus}" : "None";
    }
}