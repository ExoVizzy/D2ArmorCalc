/*
*   FILE          : ResultViewModel.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : ViewModel for results panel, binding BuildResult data
*                   to UI including stat totals, overflow, buff strings,
*                   build status, & DIM query outputs.
*/
using D2ArmorCalc_Helpers;
using D2ArmorCalc_Models;
using D2ArmorCalc_ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace D2ArmorCalc_ViewModels {
    //Represents single stat row in results display.
    public class StatResultItem(Stat stat) : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public Stat Stat { get; } = stat;
        public string Label { get; } = stat.ToString();
        private int _baseValue;
        public int BaseValue {
            get => _baseValue;
            set {_baseValue = value; OnPropertyChanged(nameof(BaseValue));}
        }
        private int _moddedValue;
        public int ModdedValue {
            get => _moddedValue;
            set {_moddedValue = value; OnPropertyChanged(nameof(ModdedValue));}
        }
        private int _finalValue;
        public int FinalValue {
            get => _finalValue;
            set {
                _finalValue = value;
                OnPropertyChanged(nameof(FinalValue));
                OnPropertyChanged(nameof(OverflowValue));
                OnPropertyChanged(nameof(HasOverflow));
                OnPropertyChanged(nameof(BuffString));
                OnPropertyChanged(nameof(IsOverCap));
            }
        }
        private int _targetMin;
        public int TargetMin {
            get => _targetMin;
            set {_targetMin = value; OnPropertyChanged(nameof(TargetMin)); OnPropertyChanged(nameof(MeetsMin));}
        }
        private int _targetMax;
        public int TargetMax {
            get => _targetMax;
            set {_targetMax = value; OnPropertyChanged(nameof(TargetMax)); OnPropertyChanged(nameof(ExceedsMax));}
        }
        //Overflow above 100.
        public int OverflowValue => StatHelper.GetOverflow(_finalValue);
        public bool HasOverflow => OverflowValue > 0;
        //Whether stat is over 200 cap.
        public bool IsOverCap => _finalValue > StatHelper.StatMax;
        //Whether stat meets its minimum target.
        public bool MeetsMin => _targetMin == 0 || _finalValue >= _targetMin;
        //Whether stat exceeds its maximum target.
        public bool ExceedsMax => _targetMax > 0 && _finalValue > _targetMax;
        //Buff string from StatHelper.
        public string BuffString => StatHelper.GetBuff(Stat, _finalValue);
    }
    //Represents single armor piece row in results display.
    public class PieceResultItem {
        public string? SlotLabel {get; set;}
        public string? Rarity {get; set;}
        public string? Archetype {get; set;}
        public string? Tertiary {get; set;}
        public string? Focus {get; set;}
        public string? StatMod {get; set;}
        public string? Fonts {get; set;}
        public string? EnergyUsed {get; set;}
        public int TotalAnyPoints { get; set; }
        public int SlotNumber { get; set; }
        public bool IsExotic { get; set; }
    }
    public class ResultViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //=====================================================================
        //Properties.
        //=====================================================================
        //Stat result rows (one per stat).
        public ObservableCollection<StatResultItem> StatResults {get;} = [];
        //Piece result rows (one per armor slot).
        public ObservableCollection<PieceResultItem> PieceResults {get;} = [];
        //Build status.
        private BuildStatus _status;
        public BuildStatus Status {
            get => _status;
            set {
                _status = value;
                OnPropertyChanged(nameof(Status));
                OnPropertyChanged(nameof(StatusMessage));
                OnPropertyChanged(nameof(IsSuccess));
                OnPropertyChanged(nameof(IsMinsFailed));
                OnPropertyChanged(nameof(IsMaxsExceeded));
                OnPropertyChanged(nameof(HasResult));
            }
        }
        public string StatusMessage => _buildResult?.GetStatusMessage() ?? string.Empty;
        public bool IsSuccess => _status == BuildStatus.Success;
        public bool IsMinsFailed => _status == BuildStatus.MinsFailed;
        public bool IsMaxsExceeded => _status == BuildStatus.MaxsExceeded;
        public bool HasResult => _buildResult != null && !IsMinsFailed;
        //DIM queries.
        private string? _dimQueryAll;
        public string DimQueryAll {
            get => _dimQueryAll;
            set {_dimQueryAll = value; OnPropertyChanged(nameof(DimQueryAll));}
        }
        private string? _dimQueryLegendary;
        public string DimQueryLegendary {
            get => _dimQueryLegendary;
            set {_dimQueryLegendary = value; OnPropertyChanged(nameof(DimQueryLegendary));}
        }
        private string? _dimQueryExotic;
        public string DimQueryExotic {
            get => _dimQueryExotic;
            set {_dimQueryExotic = value; OnPropertyChanged(nameof(DimQueryExotic));}
        }
        //DIM query visibility toggle.
        private bool _showDimQueries;
        public bool ShowDimQueries {
            get => _showDimQueries;
            set {_showDimQueries = value; OnPropertyChanged(nameof(ShowDimQueries));}
        }
        //Clipboard commands.
        public RelayCommand CopyAllQueryCommand {get;}
        public RelayCommand CopyLegendaryQueryCommand {get;}
        public RelayCommand CopyExoticQueryCommand {get;}
        //=====================================================================
        //Private State.
        //=====================================================================
        private BuildResult? _buildResult;
        //=====================================================================
        //Constructor.
        //=====================================================================
        public ResultViewModel(){
            //Initialize stat rows.
            foreach (Stat stat in Enum.GetValues<Stat>())
                StatResults.Add(new StatResultItem(stat));
            //Clipboard commands.
            CopyAllQueryCommand = new RelayCommand(_ => {
                if (!string.IsNullOrEmpty(_dimQueryAll)) Clipboard.SetText(_dimQueryAll);
            });
            CopyLegendaryQueryCommand = new RelayCommand(_ => {
                if (!string.IsNullOrEmpty(_dimQueryLegendary)) Clipboard.SetText(_dimQueryLegendary);
            });
            CopyExoticQueryCommand = new RelayCommand(_ => {
                if (!string.IsNullOrEmpty(_dimQueryExotic)) Clipboard.SetText(_dimQueryExotic);
            });
        }
        //=====================================================================
        //Public Methods.
        //=====================================================================
        /*
        Method        : LoadResult
        Description   : Populates all result display data from BuildResult,
                        updating stat rows, piece rows, status, & DIM queries.
        Parameters    : BuildResult result  : Build result to display.
                        StatBlock mins    : Minimum stat targets for display.
                        StatBlock maxs    : Maximum stat targets for display.
                        bool      showDim : Whether to show DIM query section.
        Return Values : void
        */
        public void LoadResult(BuildResult result, StatBlock mins, StatBlock maxs, bool showDim){
            _buildResult = result;
            ShowDimQueries = showDim;
            Status = result.Status;

            UpdateStatRows(result, mins, maxs);
            UpdatePieceRows(result, mins);
            UpdateDimQueries(result);
        }
        /*
        Method        : Clear
        Description   : Clears all result data & resets ViewModel to
                        default empty state.
        Parameters    : None.
        Return Values : void
        */
        public void Clear(){
            _buildResult = null;
            foreach (StatResultItem row in StatResults){
                row.BaseValue = 0;
                row.ModdedValue = 0;
                row.FinalValue = 0;
                row.TargetMin = 0;
                row.TargetMax = 0;
            }
            PieceResults.Clear();
            DimQueryAll = string.Empty;
            DimQueryLegendary = string.Empty;
            DimQueryExotic = string.Empty;
            Status = BuildStatus.MinsFailed;
            OnPropertyChanged(nameof(HasResult));
        }
        //=====================================================================
        //Private Helpers.
        //=====================================================================
        /*
        Method        : UpdateStatRows
        Description   : Updates all 6 stat result rows with base, modded, &
                        final values from build result.
        Parameters    : BuildResult result : Build result to read stats from.
                        StatBlock   mins   : Minimum targets for met/unmet display.
                        StatBlock   maxs   : Maximum targets for exceeded display.
        Return Values : void
        */
        private void UpdateStatRows(BuildResult result, StatBlock mins, StatBlock maxs) {
            foreach (StatResultItem row in StatResults) {
                row.BaseValue = result.BaseStats != null ? result.BaseStats.Get(row.Stat) : 0;
                row.ModdedValue = result.ModdedStats != null ? result.ModdedStats.Get(row.Stat) : 0;
                row.FinalValue = result.FinalStats != null ? result.FinalStats.Get(row.Stat) : 0;
                row.TargetMin = mins.Get(row.Stat);
                row.TargetMax = maxs.Get(row.Stat);
            }
        }
        /*
        Method        : UpdatePieceRows
        Description   : Populates piece result rows from build result's
                        armor pieces, formatting archetype, tertiary, focus,
                        stat mod, fonts, & energy usage per piece.
        Parameters    : BuildResult result : Build result to read pieces from.
                      : StatBlock   mins   : Minimum targets for determining "Any" display on focus/tertiary.
        Return Values : void
        */
        private void UpdatePieceRows(BuildResult result, StatBlock mins) {
            PieceResults.Clear();
            int slotNumber = 1;
            foreach (ArmorPiece piece in result.GetPieces()) {
                if (piece == null) continue;

                string fonts = string.Empty;
                if (piece.Fonts != null && piece.Fonts.Length > 0) {
                    List<string> fontNames = [];
                    foreach (Font font in piece.Fonts) fontNames.Add(font.Stat.ToString());
                    fonts = string.Join(", ", fontNames);
                }

                string statMod = piece.StatMod != null ? $"{piece.StatMod.ModType} {piece.StatMod.Stat} (+{piece.StatMod.Bonus})" : "None";

                bool tertiaryIsAny = mins.Get(piece.TertiaryStat) == 0;
                bool focusIsAny = piece.Rarity == ArmorRarity.Exotic || piece.FocusStat == piece.FocusMinus || mins.Get(piece.FocusStat) == 0;

                PieceResults.Add(new PieceResultItem {
                    SlotLabel = piece.Slot.ToString(), SlotNumber = slotNumber, IsExotic = piece.Rarity == ArmorRarity.Exotic,
                    Rarity = piece.Rarity.ToString(), Archetype = piece.Archetype?.Type.ToString() ?? "Custom",
                    Tertiary = tertiaryIsAny ? $"Any ({piece.TertiaryStat})" : piece.TertiaryStat.ToString(),
                    Focus = focusIsAny ? (piece.Rarity == ArmorRarity.Exotic ? "N/A" : "Any") : $"+{piece.FocusStat} / -{piece.FocusMinus}",
                    StatMod = statMod, Fonts = string.IsNullOrEmpty(fonts) ? "None" : fonts,
                    EnergyUsed = $"{piece.FontEnergy + piece.StatModEnergy} / {piece.TotalEnergy}"
                });
                slotNumber++;
            }
        }
        /*
        Method        : UpdateDimQueries
        Description   : Updates three DIM query strings from build result.
        Parameters    : BuildResult result : Build result containing query strings.
        Return Values : void
        */
        public void UpdateDimQueries(BuildResult result){
            DimQueryAll = result.DimQueryAll ?? string.Empty;
            DimQueryLegendary = result.DimQueryLegendary ?? string.Empty;
            DimQueryExotic = result.DimQueryExotic ?? string.Empty;
        }
    }
}