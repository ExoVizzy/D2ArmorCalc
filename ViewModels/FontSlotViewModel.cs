/*
*   FILE          : FontSlotViewModel.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : ViewModel for single font slot row, holding font
*                   count for specific armor slot & its available fonts.
*/
using System.ComponentModel;

namespace D2ArmorCalc {
    public class FontSlotViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //=====================================================================
        //Properties.
        //=====================================================================
        public string SlotLabel {get;}
        public ArmorSlot Slot {get;}
        //Max fonts depends on slot. Arms has 2 font types so max 3 still applies.
        public int MaxFonts => 3;
        private int _fontCount;
        public int FontCount {
            get => _fontCount;
            set {
                _fontCount = Math.Max(0, Math.Min(MaxFonts, value));
                OnPropertyChanged(nameof(FontCount));
                OnPropertyChanged(nameof(TotalBonus));
                OnPropertyChanged(nameof(BonusLabel));
            }
        }
        //Total stat bonus from current font count.
        public int TotalBonus => Fonts.GetTotalBonus(_fontCount);
        public string BonusLabel => _fontCount > 0 ? $"+{TotalBonus}" : "None";
        //=====================================================================
        //Constructor.
        //=====================================================================
        public FontSlotViewModel(ArmorSlot slot) {
            Slot = slot;
            SlotLabel = slot.ToString();
            _fontCount = 0;
        }
    }
}