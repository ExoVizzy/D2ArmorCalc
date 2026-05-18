/*
*   FILE          : FontStatViewModel.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : ViewModel for single font slot row, holding font
*                   count for specific armor slot & its available fonts.
*/
using System.ComponentModel;

namespace D2ArmorCalc {
    public class FontStatViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //=====================================================================
        //Properties.
        //=====================================================================
        public Stat Stat {get;}
        public ArmorSlot Slot {get;}
        public string? StatLabel {get;}
        public string? SlotLabel {get;}
        public int MaxFonts  => 3;
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
        public FontStatViewModel(Stat stat, ArmorSlot slot) {
            Stat = stat;
            Slot = slot;
            StatLabel = stat.ToString();
            SlotLabel = slot.ToString();
            _fontCount = 0;
        }
    }
}