/*
*   FILE          : ResultViewModel.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : ViewModel for results panel, binding BuildResult data
*                   to UI including stat totals, overflow, buff strings,
*                   build status, & DIM query outputs.
*/
using D2ArmorCalc_Data;
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
        public Stat Stat {get;} = stat;
        public int BaseStatValue {
            get;
            set {field = value; OnPropertyChanged(nameof(BaseStatValue));}
        }
        public int FontBonus {
            get;
            set {
                field = value; 
                OnPropertyChanged(nameof(FontBonus)); 
                OnPropertyChanged(nameof(HasFontBonus)); 
                OnPropertyChanged(nameof(BaseStatValue));
            }
        }
        public bool HasFontBonus => FontBonus > 0;
        public string Label {get;} = stat.ToString();
        public int BaseValue {
            get;
            set {
                field = value; 
                OnPropertyChanged(nameof(BaseValue));
            }
        }
        public int ModdedValue {
            get;
            set {
                field = value; 
                OnPropertyChanged(nameof(ModdedValue));
            }
        }
        public int FinalValue {
            get;
            set {
                field = value;
                OnPropertyChanged(nameof(FinalValue));
                OnPropertyChanged(nameof(OverflowValue));
                OnPropertyChanged(nameof(HasOverflow));
                OnPropertyChanged(nameof(BuffString));
                OnPropertyChanged(nameof(IsOverCap));
            }
        }
        public int TargetMin {
            get;
            set {
                field = value; 
                OnPropertyChanged(nameof(TargetMin)); 
                OnPropertyChanged(nameof(MeetsMin));
            }
        }
        public int TargetMax {
            get;
            set {
                field = value; 
                OnPropertyChanged(nameof(TargetMax)); 
                OnPropertyChanged(nameof(ExceedsMax));
            }
        }
        //Overflow above 100.
        public int OverflowValue => StatHelper.GetOverflow(FinalValue);
        public bool HasOverflow => OverflowValue > 0;
        //Whether stat is over 200 cap.
        public bool IsOverCap => FinalValue > StatHelper.StatMax;
        //Whether stat meets its minimum target.
        public bool MeetsMin => TargetMin == 0 || FinalValue >= TargetMin;
        //Whether stat exceeds its maximum target.
        public bool ExceedsMax => TargetMax > 0 && FinalValue > TargetMax;
        //Buff string from StatHelper.
        public string BuffString => StatHelper.GetBuff(Stat, FinalValue);
    }
    //Represents single armor piece row in results display.
    public class PieceResultItem {
        public string? SlotLabel {get; set;}
        public string? Rarity {get; set;}
        public string? Archetype {get; set;}
        public string? Tertiary {get; set;}
        public List<TertiaryItem> Tertiaries {get; set;} = [];
        public string? Focus {get; set;}
        public string? StatMod {get; set;}
        public string? Fonts {get; set;}
        public string? EnergyUsed {get; set;}
        public int TotalAnyPoints {get; set;}
        public int SlotNumber {get; set;}
        public bool IsExotic {get; set;} 
        public int GroupCount { get; set; } = 1;
        public string CountBadge => $"×{GroupCount}";
        public Visibility CountBadgeVisibility => GroupCount > 1 ? Visibility.Visible : Visibility.Collapsed;
    }
    //Represents single tertiary tag with optional count for duplicates.
    public class TertiaryItem {
        public string Label {get; set;} = string.Empty;
        public int Count {get; set;}
        public string CountBadge => $"×{Count}";
        public Visibility CountBadgeVisibility => Count > 1 ? Visibility.Visible : Visibility.Collapsed;
    }
    public class ResultViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //=====================================================================
        //Properties.
        //=====================================================================
        private StatBlock _fontBonuses = new();
        //Stat result rows (one per stat).
        public ObservableCollection<StatResultItem> StatResults {get;} = [];
        //Piece result rows (one per armor slot).
        public ObservableCollection<PieceResultItem> PieceResults {get;} = [];
        public ObservableCollection<PieceResultItem> PieceResultsUngrouped {get;} = [];
        //Build status.
        public BuildStatus Status {
            get;
            set {
                field = value;
                OnPropertyChanged(nameof(Status));
                OnPropertyChanged(nameof(StatusMessage));
                OnPropertyChanged(nameof(IsSuccess));
                OnPropertyChanged(nameof(IsMinsFailed));
                OnPropertyChanged(nameof(IsMaxsExceeded));
                OnPropertyChanged(nameof(HasResult));
            }
        }
        public string StatusMessage => _buildResult?.GetStatusMessage() ?? string.Empty;
        public bool IsSuccess => Status == BuildStatus.Success;
        public bool IsMinsFailed => Status == BuildStatus.MinsFailed;
        public bool IsMaxsExceeded => Status == BuildStatus.MaxsExceeded;
        public bool HasResult => _buildResult != null && !IsMinsFailed;

        //DIM queries.
        public string DimQueryAll {
            get;
            set {field = value; OnPropertyChanged(nameof(DimQueryAll));}
        } = string.Empty;
        public string DimQueryLegendary {
            get;
            set {field = value; OnPropertyChanged(nameof(DimQueryLegendary));}
        } = string.Empty;
        public string DimQueryExotic {
            get;
            set {field = value; OnPropertyChanged(nameof(DimQueryExotic));}
        } = string.Empty;
        //DIM query visibility toggle.
        public bool ShowDimQueries {
            get;
            set {field = value; OnPropertyChanged(nameof(ShowDimQueries));}
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
            foreach (Stat stat in Enum.GetValues<Stat>()) StatResults.Add(new StatResultItem(stat));
            //Clipboard commands.
            CopyAllQueryCommand = new RelayCommand(_ => {
                if (!string.IsNullOrEmpty(DimQueryAll)) Clipboard.SetText(DimQueryAll);
            });
            CopyLegendaryQueryCommand = new RelayCommand(_ => {
                if (!string.IsNullOrEmpty(DimQueryLegendary)) Clipboard.SetText(DimQueryLegendary);
            });
            CopyExoticQueryCommand = new RelayCommand(_ => {
                if (!string.IsNullOrEmpty(DimQueryExotic)) Clipboard.SetText(DimQueryExotic);
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
                        StatBlock   mins    : Minimum stat targets for display.
                        StatBlock   maxs    : Maximum stat targets for display.
                        bool        showDim : Whether to show DIM query section.
        Return Values : void
        */
        public void LoadResult(BuildResult result, StatBlock mins, StatBlock maxs, bool showDim, Dictionary<ArmorSlot, int> fontCounts, bool fontsEnabled){
            _fontBonuses = (fontsEnabled && fontCounts != null) ? CalculateFontBonuses(fontCounts) : new StatBlock();
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
        Method        : CalculateFontBonuses
        Description   : Calculates total font bonuses per stat from font counts.
        Parameters    : Dictionary<ArmorSlot, int> fontCounts : Font counts per slot.
        Return Values : StatBlock                             : Font bonuses per stat.
        */
        private static StatBlock CalculateFontBonuses(Dictionary<ArmorSlot, int> fontCounts){
            StatBlock result = new();
            (Stat, ArmorSlot)[] allFonts = [
                (Stat.Super, ArmorSlot.Helmet), (Stat.Grenade, ArmorSlot.Arms),
                (Stat.Melee, ArmorSlot.Arms), (Stat.Health, ArmorSlot.Chestplate),
                (Stat.Weapons, ArmorSlot.Boots), (Stat.Class, ArmorSlot.ClassItem)
            ];
            foreach ((Stat stat, ArmorSlot slot) in allFonts){
                if (fontCounts.TryGetValue(slot, out int count)) result.Set(stat, result.Get(stat) + Fonts.GetTotalBonus(count));
            }
            return result;
        }
        /*
        Method        : UpdateStatRows
        Description   : Updates all 6 stat result rows with base, modded, &
                        final values from build result.
        Parameters    : BuildResult result : Build result to read stats from.
                        StatBlock   mins   : Minimum targets for met/unmet display.
                        StatBlock   maxs   : Maximum targets for exceeded display.
        Return Values : void
        */
        private void UpdateStatRows(BuildResult result, StatBlock mins, StatBlock maxs){
            foreach (StatResultItem row in StatResults){
                int fontBonus = _fontBonuses.Get(row.Stat);
                int finalVal = result.FinalStats != null ? result.FinalStats.Get(row.Stat) : 0;

                row.BaseStatValue = finalVal - fontBonus;
                row.FontBonus = fontBonus;
                row.FinalValue = finalVal;
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
        private void UpdatePieceRows(BuildResult result, StatBlock mins){
            PieceResults.Clear();
            PieceResultsUngrouped.Clear();
            int slotNumber = 1;
            List<(PieceResultItem item, int count)> groups = [];

            foreach (ArmorPiece ?piece in result.GetPieces()){
                if (piece == null){slotNumber++; continue;}

                string fonts = string.Empty;
                if (piece.Fonts != null && piece.Fonts.Length > 0){
                    List<string> fontNames = [];
                    foreach (Font font in piece.Fonts) fontNames.Add(font.Stat.ToString());
                    fonts = string.Join(", ", fontNames);
                }
                string statMod = piece.StatMod != null ? $"{piece.StatMod.ModType} {piece.StatMod.Stat} (+{piece.StatMod.Bonus})" : "None";

                bool tertiaryIsAny = mins.Get(piece.TertiaryStat) == 0;
                bool focusIsAny = piece.Rarity == ArmorRarity.Exotic || piece.FocusStat == piece.FocusMinus || mins.Get(piece.FocusStat) == 0;

                string tertiaryLabel = tertiaryIsAny ? $"Any ({piece.TertiaryStat})" : piece.TertiaryStat.ToString();

                PieceResultItem item = new(){
                    SlotLabel = piece.Slot.ToString(), SlotNumber = slotNumber,
                    IsExotic = piece.Rarity == ArmorRarity.Exotic, Rarity = piece.Rarity.ToString(),
                    Archetype = piece.Archetype?.Type.ToString() ?? "Custom",
                    Tertiaries = [new TertiaryItem{Label = tertiaryLabel, Count = 1}],
                    Focus = piece.Rarity == ArmorRarity.Exotic ? "Exotic (No Tuning)" : focusIsAny ? "Any" : $"+{piece.FocusStat} / -{piece.FocusMinus}",
                    StatMod = statMod, Fonts = string.IsNullOrEmpty(fonts) ? "None" : fonts,
                    EnergyUsed = $"{piece.FontEnergy + piece.StatModEnergy} / {piece.TotalEnergy}"
                };
                //Always add to ungrouped.
                PieceResultItem ungroupedItem = new(){
                    SlotLabel = piece.Slot.ToString(), SlotNumber = slotNumber,
                    IsExotic = piece.Rarity == ArmorRarity.Exotic, Rarity = piece.Rarity.ToString(),
                    Archetype = piece.Archetype?.Type.ToString() ?? "Custom",
                    Tertiaries = [new TertiaryItem{Label = tertiaryLabel, Count = 1}],
                    Focus = piece.Rarity == ArmorRarity.Exotic ? "Exotic (No Tuning)" : focusIsAny ? "Any" : $"+{piece.FocusStat} / -{piece.FocusMinus}",
                    StatMod = statMod, Fonts = string.IsNullOrEmpty(fonts) ? "None" : fonts,
                    EnergyUsed = $"{piece.FontEnergy + piece.StatModEnergy} / {piece.TotalEnergy}"
                };
                PieceResultsUngrouped.Add(ungroupedItem);
                //Check if this matches an existing group (same archetype + rarity, tertiary collected separately).
                bool merged = false;
                for (int i = 0; i < groups.Count; i++){
                    (PieceResultItem? existing, int count) = groups[i];
                    if (existing.Archetype == item.Archetype && existing.Rarity == item.Rarity){
                        //Increment or add the tertiary count within this group.
                        TertiaryItem? existing_t = existing.Tertiaries.FirstOrDefault(t => t.Label == tertiaryLabel);
                        if (existing_t == null) existing.Tertiaries.Add(new TertiaryItem{Label = tertiaryLabel, Count = 1});
                        else existing_t.Count++;
                        groups[i] = (existing, count + 1);
                        merged = true;
                        break;
                    }
                }
                if (!merged) groups.Add((item, 1));
                slotNumber++;
            }
            //Add grouped items with count suffix.
            foreach ((PieceResultItem? item, int count) in groups){
                if (count > 1) item.GroupCount = count;
                PieceResults.Add(item);
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