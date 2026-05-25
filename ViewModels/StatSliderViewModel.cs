/*
*   FILE          : StatSliderViewModel.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : ViewModel for single stat row, managing min & max
*                   slider & text box values with two-way live sync.
*/
using D2ArmorCalc_Helpers;
using D2ArmorCalc_Models;
using System.ComponentModel;

namespace D2ArmorCalc_ViewModels {
    public class StatSliderViewModel(Stat stat) : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //=====================================================================
        //Properties.
        //=====================================================================
        public Stat Stat {get;} = stat;
        public string Label {get;} = stat.ToString();
        public bool IsEnabled {
            get;
            set {
                field = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        } = false;
        public int MinValue {
            get;
            set {
                field = Math.Max(0, Math.Min(value, MaxValue > 0 ? MaxValue : StatHelper.StatMax));
                IsEnabled = MinValue > 0 || MaxValue < StatHelper.StatMax;
                OnPropertyChanged(nameof(MinValue));
                OnPropertyChanged(nameof(IsEnabled));
            }
        } = 0;
        public int MaxValue {
            get;
            set {
                field = value == 0 ? 200 : Math.Max(MinValue, Math.Min(value, StatHelper.StatMax));
                IsEnabled = MinValue > 0 || MaxValue < StatHelper.StatMax;
                OnPropertyChanged(nameof(MaxValue));
                OnPropertyChanged(nameof(IsEnabled));
            }
        } = 200;
        //Slider proxies: keep sliders & text boxes in sync.
        public int MinSlider {
            get => MinValue;
            set {MinValue = value;}
        }
        public int MaxSlider {
            get => MaxValue;
            set {MaxValue = value;}
        }
        //=====================================================================
        //Helpers.
        //=====================================================================
        /*
        Method        : ToMinStatBlock
        Description   : Returns stats minimum value as contribution
                        to StatBlock. Returns 0 if not enabled.
        Parameters    : None.
        Return Values : int : Minimum value or 0 if disabled.
        */
        public int ToMinValue(){
            return IsEnabled ? MinValue : 0;
        }
        /*
        Method        : ToMaxValue
        Description   : Returns stats maximum value as contribution
                        to StatBlock. Returns 0 if not enabled or not set.
        Parameters    : None.
        Return Values : int : Maximum value or 0 if disabled/unset.
        */
        public int ToMaxValue(){
            return IsEnabled ? MaxValue : 0;
        }
        /*
        Method        : Reset
        Description   : Resets stat row to default disabled state
                        with min & max values cleared to 0.
        Parameters    : None.
        Return Values : void
        */
        public void Reset(){
            MinValue = 0;
            MaxValue = 200;
            IsEnabled = false;
            OnPropertyChanged(nameof(MinValue));
            OnPropertyChanged(nameof(MaxValue));
            OnPropertyChanged(nameof(IsEnabled));
        }
    }
}