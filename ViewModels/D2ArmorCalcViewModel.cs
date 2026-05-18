/*
*   FILE          : D2ArmorCalcViewModel.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Main ViewModel for the D2 Armor Calculator, orchestrating
*                   all sub-ViewModels, commands, toggle states, and the
*                   calculation pipeline from input to result.
*/
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

namespace D2ArmorCalc {
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
        public StatSliderViewModel HealthSlider {get;} = new StatSliderViewModel(Stat.Health);
        public StatSliderViewModel MeleeSlider {get;} = new StatSliderViewModel(Stat.Melee);
        public StatSliderViewModel GrenadeSlider {get;} = new StatSliderViewModel(Stat.Grenade);
        public StatSliderViewModel SuperSlider {get;} = new StatSliderViewModel(Stat.Super);
        public StatSliderViewModel ClassSlider {get;} = new StatSliderViewModel(Stat.Class);
        public StatSliderViewModel WeaponsSlider {get;} = new StatSliderViewModel(Stat.Weapons);
        public ExoticViewModel ExoticVM {get;} = new ExoticViewModel();
        public FragmentViewModel FragmentVM {get;} = new FragmentViewModel();
        public ModViewModel ModVM {get;} = new ModViewModel();
        public ResultViewModel ResultVM {get;} = new ResultViewModel();
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

            TuningSlots = new ObservableCollection<TuningSlotViewModel> {
                new("Slot 1", "Weapons", "Health"),
                new("Slot 2", "Weapons", "Health"),
                new("Slot 3", "Weapons", "Health"),
                new("Slot 4", "Weapons", "Health")
            };

            FontStats = new ObservableCollection<FontStatViewModel> {
                new(Stat.Super,   ArmorSlot.Helmet),
                new(Stat.Grenade, ArmorSlot.Arms),
                new(Stat.Melee,   ArmorSlot.Arms),
                new(Stat.Health,  ArmorSlot.Chestplate),
                new(Stat.Weapons, ArmorSlot.Boots),
                new(Stat.Class,   ArmorSlot.ClassItem)
            };
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

                //Load into result ViewModel
                StatBlock mins = BuildMinStatBlock();
                StatBlock maxs = BuildMaxStatBlock();
                ResultVM.LoadResult(result, mins, maxs, _showDimQueries);

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
            foreach (Stat stat in Enum.GetValues(typeof(Stat))){
                if (mins.Get(stat) > 0){
                    anyMin = true; 
                    break;
                }
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
            Enum.TryParse(_leastWantedStat, out Stat leastWanted);

            return new CalcInput {
                Mins = mins,
                Maxs = BuildMaxStatBlock(),
                Exotic = exotic,
                Fragments = FragmentVM.GetSelectedFragments(),
                LeastWantedStat = leastWanted,
                FontsEnabled = _fontsEnabled,
                FontCounts = BuildFontCounts(),
                ArmorModsEnabled = _armorModsEnabled,
                MajorMods = !_armorModsEnabled || ModVM.MinorModCount == 0,
                MinorModCount = ModVM.MinorModCount
            };
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
            //Default to 0 fonts per slot when fonts are disabled
            Dictionary<ArmorSlot, int> counts = new(){
                {ArmorSlot.Helmet, 0}, {ArmorSlot.Arms, 0},
                {ArmorSlot.Chestplate, 0}, {ArmorSlot.Boots, 0},
                {ArmorSlot.ClassItem, 0}
            };
            //When fonts enabled, default to 1 font per slot as baseline.
            //Full font customization can be added as future enhancement.
            if (_fontsEnabled){
                foreach (ArmorSlot slot in counts.Keys) counts[slot] = 1;
            }

            return counts;
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
        private void RunImport() {
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
        private void ApplyStatTarget(StatSliderViewModel slider, int min, int max){
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
        private void RunExport() {
            if (!ResultVM.HasResult) return;
            if (_lastResult == null) return;

            StatTargetsExport targets = new() {
                HealthMin = HealthSlider.ToMinValue(), HealthMax = HealthSlider.ToMaxValue(),
                MeleeMin = MeleeSlider.ToMinValue(), MeleeMax = MeleeSlider.ToMaxValue(),
                GrenadeMin = GrenadeSlider.ToMinValue(), GrenadeMax = GrenadeSlider.ToMaxValue(),
                SuperMin = SuperSlider.ToMinValue(), SuperMax = SuperSlider.ToMaxValue(),
                ClassMin = ClassSlider.ToMinValue(), ClassMax = ClassSlider.ToMaxValue(),
                WeaponsMin = WeaponsSlider.ToMinValue(), WeaponsMax = WeaponsSlider.ToMaxValue()
            };

            SubclassExport? subclassExport = null;
            if (_subclassCustomization && FragmentVM.SelectedSubclass != "None") {
                Aspect[] aspects = FragmentVM.GetSelectedAspects();
                Fragment[] fragments = FragmentVM.GetSelectedFragments();

                List<string> aspectNames = new();
                List<string> fragmentNames = new();

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