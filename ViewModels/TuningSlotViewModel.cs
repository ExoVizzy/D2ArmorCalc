/*
*   FILE          : TuningSlotViewModel.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : ViewModel for single tuning slot row, holding
*                   selected +5 focus stat & -5 focus stat per armor piece.
*/
using D2ArmorCalc_Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace D2ArmorCalc_ViewModels {
    public class TuningSlotViewModel(string slotLabel, string defaultFocusStat, string defaultFocusMinus) : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //=====================================================================
        //Properties.
        //=====================================================================
        public string SlotLabel {get;} = slotLabel;
        public ObservableCollection<string> StatOptions {get;} = ["Health", "Melee", "Grenade", "Super", "Class", "Weapons"];
        private string _focusStat = defaultFocusStat;
        public string FocusStat {
            get => _focusStat;
            set {
                _focusStat = value;
                OnPropertyChanged(nameof(FocusStat));
            }
        }
        private string _focusMinus = defaultFocusMinus;
        public string FocusMinus {
            get => _focusMinus;
            set {
                _focusMinus = value;
                OnPropertyChanged(nameof(FocusMinus));
            }
        }

        //=====================================================================
        //Helpers.
        //=====================================================================
        /*
        Method        : GetFocusStat
        Description   : Returns selected focus stat as Stat enum value.
        Parameters    : None.
        Return Values : Stat : Selected +5 focus stat.
        */
        public Stat GetFocusStat(){
            _ = Enum.TryParse(_focusStat, out Stat stat);
            return stat;
        }
        /*
        Method        : GetFocusMinus
        Description   : Returns selected focus minus stat as Stat enum value.
        Parameters    : None.
        Return Values : Stat : Selected -5 focus stat.
        */
        public Stat GetFocusMinus(){
            _ = Enum.TryParse(_focusMinus, out Stat stat);
            return stat;
        }
    }
}