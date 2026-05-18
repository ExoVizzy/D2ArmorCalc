/*
*   FILE          : StatSliderViewModel.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : ViewModel for single stat row, managing min & max
*                   slider & text box values with two-way live sync.
*/
using System.ComponentModel;

namespace D2ArmorCalc {
    public class StatSliderViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name){
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        //=====================================================================
        //Properties.
        //=====================================================================
        public Stat   Stat {get;}
        public string Label {get;}
        private bool _isEnabled;
        public bool IsEnabled {
            get => _isEnabled;
            set {
                _isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
                if (!value){
                    MinValue = 0;
                    MaxValue = 0;
                }
            }
        }
        private int _minValue;
        public int MinValue {
            get => _minValue;
            set {
                //Clamp between 0 & current max (or StatMax if max is 0).
                int clampedMax = _maxValue > 0 ? _maxValue : StatHelper.StatMax;
                _minValue = Math.Max(0, Math.Min(value, clampedMax));
                OnPropertyChanged(nameof(MinValue));
                OnPropertyChanged(nameof(MinSlider));
            }
        }
        private int _maxValue;
        public int MaxValue {
            get => _maxValue;
            set {
                //Max must be >= min & <= 200, 0 means not set.
                _maxValue = value == 0 ? 0 : Math.Max(_minValue, Math.Min(value, StatHelper.StatMax));
                OnPropertyChanged(nameof(MaxValue));
                OnPropertyChanged(nameof(MaxSlider));
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
        //Constructor.
        //=====================================================================
        public StatSliderViewModel(Stat stat){
            Stat = stat;
            Label = stat.ToString();
            _isEnabled = false;
            _minValue = 0;
            _maxValue = 0;
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
            IsEnabled = false;
            _minValue = 0;
            _maxValue = 0;
            OnPropertyChanged(nameof(MinValue));
            OnPropertyChanged(nameof(MaxValue));
            OnPropertyChanged(nameof(MinSlider));
            OnPropertyChanged(nameof(MaxSlider));
        }
    }
}