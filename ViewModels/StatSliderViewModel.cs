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
        private void OnPropertyChanged(string name){
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        //=====================================================================
        //Properties.
        //=====================================================================
        public Stat Stat {get;} = stat;
        public string Label {get;} = stat.ToString();
        private bool _isEnabled = false;
        public bool IsEnabled {
            get => _isEnabled;
            set {
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
            }
        }
        private int _minValue = 0;
        public int MinValue {
            get => _minValue;
            set {
                _minValue = Math.Max(0, Math.Min(value, _maxValue > 0 ? _maxValue : StatHelper.StatMax));
                _isEnabled = _minValue > 0 || _maxValue < StatHelper.StatMax;
                OnPropertyChanged(nameof(MinValue));
                OnPropertyChanged(nameof(IsEnabled));
            }
        }
        private int _maxValue = 200;
        public int MaxValue {
            get => _maxValue;
            set {
                _maxValue = value == 0 ? 200 : Math.Max(_minValue, Math.Min(value, StatHelper.StatMax));
                _isEnabled = _minValue > 0 || _maxValue < StatHelper.StatMax;
                OnPropertyChanged(nameof(MaxValue));
                OnPropertyChanged(nameof(IsEnabled));
            }
        }
        //Slider proxies: keep sliders & text boxes in sync.
        public int MinSlider {
            get => _minValue;
            set {MinValue = value;}
        }
        public int MaxSlider {
            get => _maxValue;
            set {MaxValue = value;}
        }
        //=====================================================================
        //Helpers.
        //=====================================================================
        /*
        Method        : ToMinStatBlock
        Description   : Returns stat's minimum value as contribution
                        to StatBlock. Returns 0 if not enabled.
        Parameters    : None.
        Return Values : int : Minimum value or 0 if disabled.
        */
        public int ToMinValue(){
            return _isEnabled ? _minValue : 0;
        }
        /*
        Method        : ToMaxValue
        Description   : Returns stat's maximum value as contribution
                        to StatBlock. Returns 0 if not enabled or not set.
        Parameters    : None.
        Return Values : int : Maximum value or 0 if disabled/unset.
        */
        public int ToMaxValue(){
            return _isEnabled ? _maxValue : 0;
        }
        /*
        Method        : Reset
        Description   : Resets stat row to default disabled state
                        with min & max values cleared to 0.
        Parameters    : None.
        Return Values : void
        */
        public void Reset(){
            _minValue  = 0;
            _maxValue  = 200;
            _isEnabled = false;
            OnPropertyChanged(nameof(MinValue));
            OnPropertyChanged(nameof(MaxValue));
            OnPropertyChanged(nameof(IsEnabled));
        }
    }
}