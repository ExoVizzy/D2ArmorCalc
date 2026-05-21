/*
*   FILE          : D2ArmorCalcViewModel.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Main ViewModel for the D2 Armor Calculator, orchestrating
*                   all sub-ViewModels, commands, toggle states, and the
*                   calculation pipeline from input to result.
*/
using D2ArmorCalc;
using D2ArmorCalc_Algorithm;
using D2ArmorCalc_Helpers;
using D2ArmorCalc_Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace D2ArmorCalc_ViewModels {
    public class D2ArmorCalcViewModel : INotifyPropertyChanged {
        public ObservableCollection<TuningSlotViewModel> TuningSlots {get;}
        public ObservableCollection<FontStatViewModel> FontStats {get;}
        public RelayCommand GenerateQueriesCommand {get;}
        private BuildResult? _lastResult;
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //=====================================================================
        //Sub-ViewModels.
        //=====================================================================
        public StatSliderViewModel HealthSlider {get;} = new(Stat.Health);
        public StatSliderViewModel MeleeSlider {get;} = new(Stat.Melee);
        public StatSliderViewModel GrenadeSlider {get;} = new(Stat.Grenade);
        public StatSliderViewModel SuperSlider {get;} = new(Stat.Super);
        public StatSliderViewModel ClassSlider {get;} = new(Stat.Class);
        public StatSliderViewModel WeaponsSlider {get;} = new(Stat.Weapons);
        public ExoticViewModel ExoticVM {get;} = new();
        public FragmentViewModel FragmentVM {get;} = new();
        public ModViewModel ModVM {get;} = new();
        public ResultViewModel ResultVM {get;} = new();
        //=====================================================================
        //Toggle States.
        //=====================================================================
        private bool _fontsEnabled;
        public bool FontsEnabled {
            get => _fontsEnabled;
            set {
                _fontsEnabled = value;
                OnPropertyChanged(nameof(FontsEnabled));
                AppSettings.FontsEnabled = value;
            }
        }
        private bool _armorModsEnabled;
        public bool ArmorModsEnabled {
            get => _armorModsEnabled;
            set {
                _armorModsEnabled = value;
                OnPropertyChanged(nameof(ArmorModsEnabled));
                ModVM.ArmorModsEnabled = value;
                AppSettings.ArmorModsEnabled = value;
            }
        }
        private bool _subclassCustomization;
        public bool SubclassCustomization {
            get => _subclassCustomization;
            set {
                _subclassCustomization = value;
                OnPropertyChanged(nameof(SubclassCustomization));
                AppSettings.SubclassCustomization = value;
            }
        }
        private bool _customTuning;
        public bool CustomTuning {
            get => _customTuning;
            set {
                _customTuning = value;
                OnPropertyChanged(nameof(CustomTuning));
                AppSettings.CustomTuning = value;
            }
        }
        private bool _showDimQueries;
        public bool ShowDimQueries {
            get => _showDimQueries;
            set {
                _showDimQueries = value;
                OnPropertyChanged(nameof(ShowDimQueries));
                AppSettings.ShowDimQueries = value;
            }
        }
        private bool _t5ExoticEnabled;
        public bool T5ExoticEnabled {
            get => _t5ExoticEnabled;
            set {
                _t5ExoticEnabled = value;
                OnPropertyChanged(nameof(T5ExoticEnabled));
                ExoticVM.T5ExoticEnabled = value;
                AppSettings.T5ExoticEnabled = value;
            }
        }
        private bool _fontsInStats;
        public bool FontsInStats {
            get => _fontsInStats;
            set {_fontsInStats = value; OnPropertyChanged(nameof(FontsInStats));}
        }
        private bool _fragmentsEnabled;
        public bool FragmentsEnabled {
            get => _fragmentsEnabled;
            set {
                _fragmentsEnabled = value;
                FragmentVM.FragmentsEnabled = value;
                OnPropertyChanged(nameof(FragmentsEnabled));
            }
        }
        //=====================================================================
        //Least Wanted Stat.
        //=====================================================================
        public ObservableCollection<string> StatOptions {get;} = ["Health", "Melee", "Grenade", "Super", "Class", "Weapons"];
        private string? _leastWantedStat;
        public string LeastWantedStat {
            get => _leastWantedStat;
            set {
                _leastWantedStat = value;
                OnPropertyChanged(nameof(LeastWantedStat));
                if (Enum.TryParse(value, out Stat stat))
                    AppSettings.LeastWantedStat = stat;
            }
        }
        //=====================================================================
        //Calculation State.
        //=====================================================================
        private bool _isCalculating;
        public bool IsCalculating {
            get => _isCalculating;
            set {
                _isCalculating = value;
                OnPropertyChanged(nameof(IsCalculating));
                OnPropertyChanged(nameof(CanCalculate));
            }
        }
        public bool CanCalculate => !_isCalculating;
        //=====================================================================
        //Commands.
        //=====================================================================
        public RelayCommand CalculateCommand {get;}
        public RelayCommand ImportCommand {get;}
        public RelayCommand ExportCommand {get;}
        public RelayCommand ResetCommand {get;}
        public RelayCommand OpenAboutCommand {get;}
        //=====================================================================
        //Constructor.
        //=====================================================================
        public D2ArmorCalcViewModel(){
            GenerateQueriesCommand = new RelayCommand(_ => {
                if (_lastResult != null)
                    DimQueryBuilder.BuildQueries(_lastResult, ExoticVM.GetPlayerClass());
                    ResultVM.UpdateDimQueries(_lastResult); //Make UpdateDimQueries public.
            }, _ => ResultVM.HasResult);

            CalculateCommand = new RelayCommand(
                async _ => await RunCalculationAsync(),
                _ => CanCalculate
            );

            ImportCommand = new RelayCommand(_ => RunImport());
            ExportCommand = new RelayCommand(_ => RunExport(), _ => ResultVM.HasResult);
            ResetCommand = new RelayCommand(_ => Reset());
            OpenAboutCommand = new RelayCommand(_ => OpenAbout());

            LoadSettings();

            FragmentVM.ClassChanged += (s, className) => {
                if (ExoticVM.SelectedClass != className)
                    ExoticVM.SelectedClass = className;
            };
            ExoticVM.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(ExoticViewModel.SelectedClass)){
                    if (FragmentVM.SelectedClass != ExoticVM.SelectedClass) FragmentVM.SelectedClass = ExoticVM.SelectedClass;
                }
                if (e.PropertyName == nameof(ExoticViewModel.SelectedSlot)) SyncExoticSlotToMods();
            };

            TuningSlots = [
                new("Slot 1", "Weapons", "Health"), new("Slot 2", "Weapons", "Health"),
                new("Slot 3", "Weapons", "Health"), new("Slot 4", "Weapons", "Health")
            ];

            FontStats = [
                new(Stat.Super, ArmorSlot.Helmet), new(Stat.Grenade, ArmorSlot.Arms),
                new(Stat.Melee, ArmorSlot.Arms), new(Stat.Health, ArmorSlot.Chestplate),
                new(Stat.Weapons, ArmorSlot.Boots), new(Stat.Class, ArmorSlot.ClassItem)
            ];
            FontStats[1].PropertyChanged += OnArmsFontChanged; // Grenade
            FontStats[2].PropertyChanged += OnArmsFontChanged; // Melee

            foreach (FontStatViewModel fontStat in FontStats)
                fontStat.PropertyChanged += (s, e) => {
                    if (e.PropertyName == nameof(FontStatViewModel.FontCount)) SyncFontCountsToMods();
                };

            SyncExoticSlotToMods();
        }
        //=====================================================================
        //Settings.
        //=====================================================================
        /*
        Method        : LoadSettings
        Description   : Loads persisted AppSettings and applies them to all
                        toggle states and sub-ViewModels on startup.
        Parameters    : None.
        Return Values : void
        */
        private void LoadSettings(){
            _fontsEnabled = AppSettings.FontsEnabled;
            _armorModsEnabled = AppSettings.ArmorModsEnabled;
            _subclassCustomization = AppSettings.SubclassCustomization;
            _customTuning = AppSettings.CustomTuning;
            _showDimQueries = AppSettings.ShowDimQueries;
            _t5ExoticEnabled = AppSettings.T5ExoticEnabled;
            _leastWantedStat = AppSettings.LeastWantedStat.ToString();

            //Sync sub-ViewModels.
            ModVM.ArmorModsEnabled = _armorModsEnabled;
            ExoticVM.T5ExoticEnabled = _t5ExoticEnabled;
            //Notify all toggle properties.
            OnPropertyChanged(nameof(FontsEnabled));
            OnPropertyChanged(nameof(ArmorModsEnabled));
            OnPropertyChanged(nameof(SubclassCustomization));
            OnPropertyChanged(nameof(CustomTuning));
            OnPropertyChanged(nameof(ShowDimQueries));
            OnPropertyChanged(nameof(T5ExoticEnabled));
            OnPropertyChanged(nameof(LeastWantedStat));
        }
        //=====================================================================
        //Calculation.
        //=====================================================================
        /*
        Method        : BuildAdvancedModEnergy
        Description   : Builds dictionary of total armor mod energy used per slot
                        when advanced mod mode is enabled.
        Parameters    : None.
        Return Values : Dictionary<ArmorSlot, int> : Energy used by armor mods per slot.
        */
        private Dictionary<ArmorSlot, int> BuildAdvancedModEnergy() {
            if (!_armorModsEnabled) return [];
            Dictionary<ArmorSlot, int> energy = [];
            foreach (var kvp in ModVM.GetAllSelectedMods()) {
                int total = 0;
                foreach (ArmorMod mod in kvp.Value) total += mod.EnergyCost;
                energy[kvp.Key] = total;
            }
            return energy;
        }
        /*
        Method        : RunCalculationAsync
        Description   : Builds CalcInput from all sub-ViewModels, runs
                        armor calculator asynchronously to keep UI responsive,
                        generates DIM queries, & loads result into ResultVM.
        Parameters    : None.
        Return Values : Task
        */
        private async Task RunCalculationAsync(){
            IsCalculating = true;
            ResultVM.Clear();

            try {
                CalcInput input = BuildCalcInput();
                if (input == null) return;

                //Run heavy calculation off UI thread.
                BuildResult result = await Task.Run(() => ArmorCalculator.Calculate(input));

                //Generate DIM queries on result.
                DimQueryBuilder.BuildQueries(result, ExoticVM.GetPlayerClass());

                //Store last result for potential export.
                _lastResult = result;

                //Load into result ViewModel.
                StatBlock mins = BuildMinStatBlock();
                StatBlock maxs = BuildMaxStatBlock();
                ResultVM.LoadResult(result, mins, maxs, _showDimQueries, BuildFontCounts(), _fontsEnabled);
            } catch (Exception ex){
                MessageBox.Show($"Calculation error: {ex.Message}", "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            } finally {
                IsCalculating = false;
            }
        }
        /*
        Method        : BuildCalcInput
        Description   : Constructs CalcInput object from current state
                        of all sub-ViewModels. Returns null if input is invalid.
        Parameters    : None.
        Return Values : CalcInput : Populated calculation input, or null.
        */
        private CalcInput BuildCalcInput(){
            //Validate at least one stat has minimum set.
            StatBlock mins = BuildMinStatBlock();
            bool anyMin = false;
            foreach (Stat stat in Enum.GetValues<Stat>()){
                if (mins.Get(stat) > 0){anyMin = true; break;}
            }

            if (!anyMin){
                MessageBox.Show("Please set at least one minimum stat target before calculating.",
                    "No Targets Set", MessageBoxButton.OK, MessageBoxImage.Warning);
                return null;
            }
            //Build exotic piece.
            ArmorPiece exotic = ExoticVM.BuildExoticPiece();
            //Sync fragment VM class with exotic VM class.
            FragmentVM.CurrentClass = ExoticVM.GetPlayerClass();
            //Parse least wanted stat.
            _ = Enum.TryParse(_leastWantedStat, out Stat leastWanted);

            return new CalcInput {
                Mins = mins, Maxs = BuildMaxStatBlock(),
                Exotic = exotic, Fragments = _fragmentsEnabled ? FragmentVM.GetSelectedFragments() : [],
                LeastWantedStat = leastWanted, FontsEnabled = _fontsEnabled, FontsInStats = _fontsInStats,
                FontCounts = BuildFontCounts(), ArmorModsEnabled = _armorModsEnabled,
                MinorModCount = ModVM.MinorModCount, CustomTuning = _customTuning ? BuildTuningSlots() : null,
                PerStatFontCounts = BuildPerStatFontCounts(), AdvancedModEnergy = BuildAdvancedModEnergy(),
            };
        }
        /*
        Method        : BuildTuningSlots
        Description   : Builds dictionary of custom focus stat assignments
                        per slot index from TuningSlots ViewModels.
        Parameters    : None.
        Return Values : Dictionary<int, (Stat, Stat)> : Focus assignments per slot.
        */
        private Dictionary<int, (Stat FocusStat, Stat FocusMinus)> BuildTuningSlots(){
            Dictionary<int, (Stat, Stat)> tuning = [];
            for (int i = 0; i < TuningSlots.Count; i++){
                tuning[i] = (TuningSlots[i].GetFocusStat(), TuningSlots[i].GetFocusMinus());
            }
            return tuning;
        }
        /*
        Method        : BuildMinStatBlock
        Description   : Constructs StatBlock from minimum values of all
                        6 stat slider ViewModels.
        Parameters    : None.
        Return Values : StatBlock : Minimum stat targets.
        */
        private StatBlock BuildMinStatBlock(){
            return new StatBlock(
                HealthSlider.ToMinValue(),
                MeleeSlider.ToMinValue(),
                GrenadeSlider.ToMinValue(),
                SuperSlider.ToMinValue(),
                ClassSlider.ToMinValue(),
                WeaponsSlider.ToMinValue()
            );
        }
        /*
        Method        : BuildMaxStatBlock
        Description   : Constructs StatBlock from maximum values of all
                        6 stat slider ViewModels.
        Parameters    : None.
        Return Values : StatBlock : Maximum stat targets.
        */
        private StatBlock BuildMaxStatBlock(){
            return new StatBlock(
                HealthSlider.ToMaxValue(),
                MeleeSlider.ToMaxValue(),
                GrenadeSlider.ToMaxValue(),
                SuperSlider.ToMaxValue(),
                ClassSlider.ToMaxValue(),
                WeaponsSlider.ToMaxValue()
            );
        }
        /*
        Method        : BuildFontCounts
        Description   : Constructs font count dictionary per armor slot
                        for use by algorithm. Returns empty counts if
                        fonts are disabled.
        Parameters    : None.
        Return Values : Dictionary<ArmorSlot, int> : Font counts per slot.
        */
        private Dictionary<ArmorSlot, int> BuildFontCounts(){
            Dictionary<ArmorSlot, int> counts = new(){
                {ArmorSlot.Helmet, 0}, {ArmorSlot.Arms, 0},
                {ArmorSlot.Chestplate, 0}, {ArmorSlot.Boots, 0},
                {ArmorSlot.ClassItem, 0}
            };

            if (!_fontsEnabled) return counts;

            //FontStats: Super(Helmet), Grenade(Arms), Melee(Arms), Health(Chest), Weapons(Boots), Class(ClassItem).
            counts[ArmorSlot.Helmet] = FontStats[0].FontCount;
            counts[ArmorSlot.Arms] = FontStats[1].FontCount + FontStats[2].FontCount;
            counts[ArmorSlot.Chestplate] = FontStats[3].FontCount;
            counts[ArmorSlot.Boots] = FontStats[4].FontCount;
            counts[ArmorSlot.ClassItem] = FontStats[5].FontCount;

            return counts;
        }
        private Dictionary<Stat, int> BuildPerStatFontCounts(){
            if (!_fontsEnabled) return [];
            return new Dictionary<Stat, int> {
                {Stat.Super,FontStats[0].FontCount},
                {Stat.Grenade, FontStats[1].FontCount},
                {Stat.Melee, FontStats[2].FontCount},
                {Stat.Health, FontStats[3].FontCount},
                {Stat.Weapons, FontStats[4].FontCount},
                {Stat.Class, FontStats[5].FontCount}
            };
        }
        /*
        Method        : OnArmsFontChanged
        Description   : Enforces 3-font cap shared between Grenade & Melee
                        fonts on Arms. If combined count exceeds 3, clamps
                        slot that just changed.
        Parameters    : object sender              : FontStatViewModel that changed.
                        PropertyChangedEventArgs e : Property change event.
        Return Values : void
        */
        private void OnArmsFontChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e){
            if (e.PropertyName != nameof(FontStatViewModel.FontCount)) return;

            FontStatViewModel grenade = FontStats[1];
            FontStatViewModel melee   = FontStats[2];

            if (grenade.FontCount + melee.FontCount > 3){
                //Clamp whichever one just changed.
                if (sender == grenade) grenade.FontCount = 3 - melee.FontCount;
                else melee.FontCount = 3 - grenade.FontCount;
            }
        }
        /*
        Method        : SyncFontCountsToMods
        Description   : Updates SlotModViewModel font counts so mod slot
                        availability stays synced with font selections.
        Parameters    : None.
        Return Values : void
        */
        private void SyncFontCountsToMods(){
            //FontStats order: Super(Helmet), Grenade(Arms), Melee(Arms), Health(Chest), Weapons(Boots), Class(ClassItem).
            ModVM.HelmetMods.FontCount = FontStats[0].FontCount;
            ModVM.ArmsMods.FontCount = FontStats[1].FontCount + FontStats[2].FontCount;
            ModVM.ChestplateMods.FontCount = FontStats[3].FontCount;
            ModVM.BootsMods.FontCount = FontStats[4].FontCount;
            ModVM.ClassItemMods.FontCount  = FontStats[5].FontCount;
        }
        /*
        Method        : SyncExoticSlotToMods
        Description   : Updates SlotModViewModel for exotic slot to use
                        10 energy & marks all others as legendary (11 energy).
        Parameters    : None.
        Return Values : void
        */
        private void SyncExoticSlotToMods(){
            ArmorSlot exoticSlot = ExoticVM.GetArmorSlot();
            ModVM.HelmetMods.IsExotic = exoticSlot == ArmorSlot.Helmet;
            ModVM.ArmsMods.IsExotic = exoticSlot == ArmorSlot.Arms;
            ModVM.ChestplateMods.IsExotic = exoticSlot == ArmorSlot.Chestplate;
            ModVM.BootsMods.IsExotic = exoticSlot == ArmorSlot.Boots;
            ModVM.ClassItemMods.IsExotic = exoticSlot == ArmorSlot.ClassItem;
        }
        //=====================================================================
        //Import / Export.
        //=====================================================================
        /*
        Method        : RunImport
        Description   : Prompts user to select a .d2build file &
                        restores all UI state from imported build.
        Parameters    : None.
        Return Values : void
        */
        private void RunImport(){
            BuildExport export = ImportExportHelper.Import();
            if (export == null) return;
            ApplyImport(export);
        }
        /*
        Method        : ApplyImport
        Description   : Applies all settings & state from imported
                        BuildExport to relevant sub-ViewModels.
        Parameters    : BuildExport export : Imported build data.
        Return Values : void
        */
        private void ApplyImport(BuildExport export){
            //Apply toggles.
            FontsEnabled = export.FontsEnabled;
            ArmorModsEnabled = export.ArmorModsEnabled;
            SubclassCustomization = export.SubclassCustomization;
            CustomTuning = export.CustomTuning;
            T5ExoticEnabled = export.T5ExoticEnabled;
            LeastWantedStat = export.LeastWantedStat;
            ModVM.MinorModCount = export.MinorModCount;

            //Apply stat targets.
            if (export.StatTargets != null){
                ApplyStatTarget(HealthSlider, export.StatTargets.HealthMin, export.StatTargets.HealthMax);
                ApplyStatTarget(MeleeSlider, export.StatTargets.MeleeMin, export.StatTargets.MeleeMax);
                ApplyStatTarget(GrenadeSlider, export.StatTargets.GrenadeMin, export.StatTargets.GrenadeMax);
                ApplyStatTarget(SuperSlider, export.StatTargets.SuperMin, export.StatTargets.SuperMax);
                ApplyStatTarget(ClassSlider, export.StatTargets.ClassMin, export.StatTargets.ClassMax);
                ApplyStatTarget(WeaponsSlider, export.StatTargets.WeaponsMin, export.StatTargets.WeaponsMax);
            }
            //Apply exotic.
            if (!string.IsNullOrEmpty(export.ExoticSlot)) ExoticVM.SelectedSlot = export.ExoticSlot;

            ExoticVM.CustomRollEnabled = export.CustomExoticRoll;
            if (export.CustomExoticRoll){
                ExoticVM.CustomHealth = export.ExoticStat1Value;
                ExoticVM.CustomMelee = export.ExoticStat2Value;
                ExoticVM.CustomGrenade = export.ExoticStat3Value;
            }
            //Apply subclass.
            if (export.SubclassCustomization && export.Subclass != null){
                ExoticVM.SelectedClass = export.Subclass.PlayerClass;
                FragmentVM.CurrentClass = ExoticVM.GetPlayerClass();
                FragmentVM.SelectedSubclass = export.Subclass.SubclassName;

                if (!string.IsNullOrEmpty(export.Subclass.Super)) FragmentVM.SelectedSuper = export.Subclass.Super;
                if (!string.IsNullOrEmpty(export.Subclass.Melee)) FragmentVM.SelectedMelee = export.Subclass.Melee;
                if (!string.IsNullOrEmpty(export.Subclass.Grenade)) FragmentVM.SelectedGrenade = export.Subclass.Grenade;
                if (!string.IsNullOrEmpty(export.Subclass.ClassAbility)) FragmentVM.SelectedClassAbility = export.Subclass.ClassAbility;
                if (!string.IsNullOrEmpty(export.Subclass.Jump)) FragmentVM.SelectedJump = export.Subclass.Jump;

                //Restore fragment selections.
                foreach (FragmentSelectionItem fragmentItem in FragmentVM.Fragments){
                    fragmentItem.IsSelected = export.Subclass.Fragments.Contains(fragmentItem.Name);
                }
            }
        }
        /*
        Method        : ApplyStatTarget
        Description   : Applies imported min & max values to stat slider
                        ViewModel, enabling it if either value is non-zero.
        Parameters    : StatSliderViewModel slider : Slider to update.
                        int                 min    : Minimum value to apply.
                        int                 max    : Maximum value to apply.
        Return Values : void
        */
        private static void ApplyStatTarget(StatSliderViewModel slider, int min, int max){
            slider.IsEnabled = min > 0 || max > 0;
            slider.MinValue = min;
            slider.MaxValue = max;
        }
        /*
        Method        : RunExport
        Description   : Builds BuildExport snapshot from current UI
                        state & result, then prompts user to save it.
        Parameters    : None.
        Return Values : void
        */
        private void RunExport(){
            if (!ResultVM.HasResult) return;
            if (_lastResult == null) return;

            StatTargetsExport targets = new(){
                HealthMin = HealthSlider.ToMinValue(), HealthMax = HealthSlider.ToMaxValue(),
                MeleeMin = MeleeSlider.ToMinValue(), MeleeMax = MeleeSlider.ToMaxValue(),
                GrenadeMin = GrenadeSlider.ToMinValue(), GrenadeMax = GrenadeSlider.ToMaxValue(),
                SuperMin = SuperSlider.ToMinValue(), SuperMax = SuperSlider.ToMaxValue(),
                ClassMin = ClassSlider.ToMinValue(), ClassMax = ClassSlider.ToMaxValue(),
                WeaponsMin = WeaponsSlider.ToMinValue(), WeaponsMax = WeaponsSlider.ToMaxValue()
            };

            SubclassExport? subclassExport = null;
            if (_subclassCustomization && FragmentVM.SelectedSubclass != "None"){
                Aspect[] aspects = FragmentVM.GetSelectedAspects();
                Fragment[] fragments = FragmentVM.GetSelectedFragments();

                List<string> aspectNames = [];
                List<string> fragmentNames = [];

                foreach (Aspect a in aspects) aspectNames.Add(a.Name);
                foreach (Fragment f in fragments) fragmentNames.Add(f.Name);

                subclassExport = new SubclassExport {
                    PlayerClass = ExoticVM.SelectedClass, SubclassName = FragmentVM.SelectedSubclass,
                    Super = FragmentVM.SelectedSuper, Melee = FragmentVM.SelectedMelee,
                    Grenade = FragmentVM.SelectedGrenade, ClassAbility = FragmentVM.SelectedClassAbility,
                    Jump = FragmentVM.SelectedJump, Aspects = aspectNames, Fragments = fragmentNames
                };
            }

            var export = ImportExportHelper.BuildToExport(
                _lastResult,
                targets,
                subclassExport,
                _fontsEnabled,
                _armorModsEnabled,
                _subclassCustomization,
                _customTuning,
                _t5ExoticEnabled,
                ExoticVM.CustomRollEnabled,
                _leastWantedStat,
                ModVM.MinorModCount
            );

            ImportExportHelper.Export(export);
        }
        //=====================================================================
        //Reset.
        //=====================================================================
        /*
        Method        : Reset
        Description   : Resets all stat sliders, clears result panel,
                        & resets sub-ViewModels to their default states.
        Parameters    : None.
        Return Values : void
        */
        private void Reset(){
            HealthSlider.Reset();
            MeleeSlider.Reset();
            GrenadeSlider.Reset();
            SuperSlider.Reset();
            ClassSlider.Reset();
            WeaponsSlider.Reset();
            ResultVM.Clear();
            FragmentVM.SelectedSubclass = "None";
            ModVM.MinorModCount = 0;
        }
        //=====================================================================
        //About.
        //=====================================================================
        /*
        Method        : OpenAbout
        Description   : Opens About dialog window.
        Parameters    : None.
        Return Values : void
        */
        private static void OpenAbout(){
            AboutBox about = new();
            about.ShowDialog();
        }
    }
}