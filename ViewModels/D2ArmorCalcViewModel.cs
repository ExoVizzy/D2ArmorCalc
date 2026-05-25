/*
*   FILE          : D2ArmorCalcViewModel.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Main ViewModel for the D2 Armor Calculator, orchestrating
*                   all sub-ViewModels, commands, toggle states, &
*                   calculation pipeline from input to result.
*/
using D2ArmorCalc;
using D2ArmorCalc.Helpers;
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
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //=====================================================================
        //Sub-ViewModels.
        //=====================================================================
        public ObservableGameConstants Constants {get;} = new();
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
        public bool FontsEnabled {
            get;
            set {
                field = value;
                OnPropertyChanged(nameof(FontsEnabled));
                AppSettings.FontsEnabled = value;
            }
        }
        public bool FontsInStats {
            get;
            set {
                field = value;
                OnPropertyChanged(nameof(FontsInStats));
                AppSettings.FontsInStats = value;
            }
        }
        public bool ArmorModsEnabled {
            get;
            set {
                field = value;
                OnPropertyChanged(nameof(ArmorModsEnabled));
                ModVM.ArmorModsEnabled = value;
                AppSettings.ArmorModsEnabled = value;
            }
        }
        public bool SubclassCustomization {
            get;
            set {
                field = value;
                OnPropertyChanged(nameof(SubclassCustomization));
                AppSettings.SubclassCustomization = value;
            }
        }
        public bool CustomTuning {
            get;
            set {
                field = value;
                OnPropertyChanged(nameof(CustomTuning));
                AppSettings.CustomTuning = value;
            }
        }
        public bool ShowDimQueries {
            get;
            set {
                field = value;
                OnPropertyChanged(nameof(ShowDimQueries));
                AppSettings.ShowDimQueries = value;
            }
        }
        public bool T5ExoticEnabled {
            get;
            set {
                field = value;
                OnPropertyChanged(nameof(T5ExoticEnabled));
                ExoticVM.T5ExoticEnabled = value;
                AppSettings.T5ExoticEnabled = value;
            }
        }
        public bool FragmentsEnabled {
            get;
            set {
                field = value;
                FragmentVM.FragmentsEnabled = value;
                OnPropertyChanged(nameof(FragmentsEnabled));
                AppSettings.FragmentsEnabled = value;
            }
        }
        //=====================================================================
        //Least Wanted Stat.
        //=====================================================================
        public string? LeastWantedStat {
            get;
            set {
                field = value;
                OnPropertyChanged(nameof(LeastWantedStat));
                if (Enum.TryParse(value, out Stat stat)) AppSettings.LeastWantedStat = stat;
            }
        }
        //=====================================================================
        //Calculation State.
        //=====================================================================
        public bool IsCalculating {
            get;
            set {
                field = value;
                OnPropertyChanged(nameof(IsCalculating));
                OnPropertyChanged(nameof(CanCalculate));
            }
        }
        public bool CanCalculate => !IsCalculating;
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
                if (_lastResult != null){
                    DimQueryBuilder.BuildQueries(_lastResult, ExoticVM.GetPlayerClass());
                    ResultVM.UpdateDimQueries(_lastResult); 
                }
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
            //Keep ExoticVM & FragmentVM class selections in sync.
            FragmentVM.ClassChanged += (s, className) => {
                if (ExoticVM.SelectedClass != className)
                    ExoticVM.SelectedClass = className;
            };
            ExoticVM.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(ExoticViewModel.SelectedClass)){
                    if (FragmentVM.SelectedClass != ExoticVM.SelectedClass)
                        FragmentVM.SelectedClass = ExoticVM.SelectedClass;
                    //Persist class selection.
                    AppSettings.SelectedClass = ExoticVM.SelectedClass ?? AppSettings.SelectedClass;
                }
                if (e.PropertyName == nameof(ExoticViewModel.SelectedSlot)){
                    SyncExoticSlotToMods();
                    //Persist slot selection.
                    AppSettings.SelectedSlot = ExoticVM.SelectedSlot ?? AppSettings.SelectedSlot;
                }
            };
            //Persist FragmentsEnabled sub-state: ShowFullSubclass.
            FragmentVM.PropertyChanged += (s, e) => {
                if (e.PropertyName == nameof(FragmentViewModel.ShowFullSubclass))
                    AppSettings.ShowFullSubclass = FragmentVM.ShowFullSubclass;
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
            FontStats[1].PropertyChanged += OnArmsFontChanged; //Grenade.
            FontStats[2].PropertyChanged += OnArmsFontChanged; //Melee.

            foreach (FontStatViewModel fontStat in FontStats){
                fontStat.PropertyChanged += (s, e) => {
                    if (e.PropertyName == nameof(FontStatViewModel.FontCount)) SyncFontCountsToMods();
                }; 
            }
            SyncExoticSlotToMods();
        }
        //=====================================================================
        //Settings.
        //=====================================================================
        /*
        Method        : LoadSettings
        Description   : Loads persisted AppSettings & applies them to all
                        toggle states & sub-ViewModels on startup.
        Parameters    : None.
        Return Values : void
        */
        private void LoadSettings(){
            //Load all persisted toggles directly into backing fields to avoid
            //setters writing them back before everything is initialised.
            FontsEnabled = AppSettings.FontsEnabled;
            FontsInStats = AppSettings.FontsInStats;
            ArmorModsEnabled = AppSettings.ArmorModsEnabled;
            SubclassCustomization = AppSettings.SubclassCustomization;
            CustomTuning = AppSettings.CustomTuning;
            ShowDimQueries = AppSettings.ShowDimQueries;
            T5ExoticEnabled = AppSettings.T5ExoticEnabled;
            FragmentsEnabled = AppSettings.FragmentsEnabled;
            LeastWantedStat = AppSettings.LeastWantedStat.ToString();
            //Restore class & slot selections.
            ExoticVM.SelectedClass = AppSettings.SelectedClass;
            ExoticVM.SelectedSlot = AppSettings.SelectedSlot;
            //Restore ShowFullSubclass toggle in FragmentVM.
            FragmentVM.ShowFullSubclass = AppSettings.ShowFullSubclass;
            //Sync dependent sub-ViewModels.
            ModVM.ArmorModsEnabled = ArmorModsEnabled;
            ExoticVM.T5ExoticEnabled = T5ExoticEnabled;
            FragmentVM.FragmentsEnabled = FragmentsEnabled;
            //Notify all toggle & dropdown properties so bindings refresh.
            OnPropertyChanged(nameof(FontsEnabled));
            OnPropertyChanged(nameof(FontsInStats));
            OnPropertyChanged(nameof(ArmorModsEnabled));
            OnPropertyChanged(nameof(SubclassCustomization));
            OnPropertyChanged(nameof(CustomTuning));
            OnPropertyChanged(nameof(ShowDimQueries));
            OnPropertyChanged(nameof(T5ExoticEnabled));
            OnPropertyChanged(nameof(FragmentsEnabled));
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
        private Dictionary<ArmorSlot, int> BuildAdvancedModEnergy(){
            if (!ArmorModsEnabled) return [];
            Dictionary<ArmorSlot, int> energy = [];
            foreach (KeyValuePair<ArmorSlot, ArmorMod[]> kvp in ModVM.GetAllSelectedMods()){
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
                CalcInput? input = BuildCalcInput();
                if (input == null) return;

                BuildResult result = await Task.Run(() => ArmorCalculator.Calculate(input));
                DimQueryBuilder.BuildQueries(result, ExoticVM.GetPlayerClass());

                _lastResult = result;

                StatBlock mins = BuildMinStatBlock();
                StatBlock maxs = BuildMaxStatBlock();
                ResultVM.LoadResult(result, mins, maxs, ShowDimQueries, BuildPerStatFontCounts(), FontsEnabled);
            } catch (Exception ex){
                MessageBox.Show($"Calculation error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
        private CalcInput? BuildCalcInput(){
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

            ArmorPiece exotic = ExoticVM.BuildExoticPiece();
            FragmentVM.CurrentClass = ExoticVM.GetPlayerClass();
            _ = Enum.TryParse(LeastWantedStat, out Stat leastWanted);

            return new CalcInput {
                Mins = mins, Maxs = BuildMaxStatBlock(), Exotic = exotic,
                Fragments = FragmentsEnabled ? FragmentVM.GetSelectedFragments() : [],
                LeastWantedStat = leastWanted, CustomTuning = CustomTuning ? BuildTuningSlots() : null,
                FontsEnabled = FontsEnabled, FontsInStats = FontsInStats, FontCounts = BuildFontCounts(),
                ArmorModsEnabled = ArmorModsEnabled, MinorModCount = ModVM.MinorModCount,
                PerStatFonts = BuildPerStatFontCounts(), AdvModEnergy = BuildAdvancedModEnergy(),
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
            for (int i = 0; i < TuningSlots.Count; i++) tuning[i] = (TuningSlots[i].GetFocusStat(), TuningSlots[i].GetFocusMinus());
            return tuning;
        }
        private StatBlock BuildMinStatBlock() => new(
            HealthSlider.ToMinValue(), MeleeSlider.ToMinValue(),
            GrenadeSlider.ToMinValue(), SuperSlider.ToMinValue(),
            ClassSlider.ToMinValue(), WeaponsSlider.ToMinValue()
        );
        private StatBlock BuildMaxStatBlock() => new(
            HealthSlider.ToMaxValue(), MeleeSlider.ToMaxValue(),
            GrenadeSlider.ToMaxValue(), SuperSlider.ToMaxValue(),
            ClassSlider.ToMaxValue(), WeaponsSlider.ToMaxValue()
        );
        private Dictionary<ArmorSlot, int> BuildFontCounts(){
            Dictionary<ArmorSlot, int> counts = new(){
                {ArmorSlot.Helmet, 0}, {ArmorSlot.Arms, 0}, {ArmorSlot.Chestplate, 0}, 
                {ArmorSlot.Boots, 0}, {ArmorSlot.ClassItem, 0}
            };
            if (!FontsEnabled) return counts;

            counts[ArmorSlot.Helmet] = FontStats[0].FontCount;
            counts[ArmorSlot.Arms] = FontStats[1].FontCount + FontStats[2].FontCount;
            counts[ArmorSlot.Chestplate] = FontStats[3].FontCount;
            counts[ArmorSlot.Boots] = FontStats[4].FontCount;
            counts[ArmorSlot.ClassItem] = FontStats[5].FontCount;
            return counts;
        }
        private Dictionary<Stat, int> BuildPerStatFontCounts(){
            return !FontsEnabled
                ? [] : new Dictionary<Stat, int> {
                {Stat.Super, FontStats[0].FontCount}, {Stat.Grenade, FontStats[1].FontCount},
                {Stat.Melee, FontStats[2].FontCount}, {Stat.Health, FontStats[3].FontCount},
                {Stat.Weapons, FontStats[4].FontCount}, {Stat.Class, FontStats[5].FontCount}
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
        private void OnArmsFontChanged(object? sender, PropertyChangedEventArgs e){
            if (e.PropertyName != nameof(FontStatViewModel.FontCount)) return;

            FontStatViewModel grenade = FontStats[1];
            FontStatViewModel melee = FontStats[2];

            if (grenade.FontCount + melee.FontCount > 3){
                if (sender == grenade) grenade.FontCount = 3 - melee.FontCount;
                else melee.FontCount = 3 - grenade.FontCount;
            }
        }
        private void SyncFontCountsToMods(){
            ModVM.HelmetMods.FontCount = FontStats[0].FontCount;
            ModVM.ArmsMods.FontCount = FontStats[1].FontCount + FontStats[2].FontCount;
            ModVM.ChestplateMods.FontCount = FontStats[3].FontCount;
            ModVM.BootsMods.FontCount = FontStats[4].FontCount;
            ModVM.ClassItemMods.FontCount = FontStats[5].FontCount;
        }
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
                        restores all UI states from imported build.
        Parameters    : None.
        Return Values : void
        */
        private void RunImport(){
            BuildExport? export = ImportExportHelper.Import();
            if (export == null) return;
            ApplyImport(export);
        }
        /*
        Method        : ApplyImport
        Description   : Applies all settings & state from imported
                        BuildExport to all relevant sub-ViewModels.
        Parameters    : BuildExport export : Imported build data.
        Return Values : void
        */
        private void ApplyImport(BuildExport export){
            //Toggles.
            FontsEnabled = export.FontsEnabled;
            FontsInStats = export.FontsInStats;
            ArmorModsEnabled = export.ArmorModsEnabled;
            SubclassCustomization = export.SubclassCustomization;
            CustomTuning = export.CustomTuning;
            T5ExoticEnabled = export.T5ExoticEnabled;
            ShowDimQueries = export.ShowDimQueries;
            FragmentsEnabled = export.FragmentsEnabled;
            LeastWantedStat = export.LeastWantedStat;
            //Stat targets.
            if (export.StatTargets != null){
                ApplyStatTarget(HealthSlider, export.StatTargets.HealthMin, export.StatTargets.HealthMax);
                ApplyStatTarget(MeleeSlider, export.StatTargets.MeleeMin, export.StatTargets.MeleeMax);
                ApplyStatTarget(GrenadeSlider, export.StatTargets.GrenadeMin, export.StatTargets.GrenadeMax);
                ApplyStatTarget(SuperSlider, export.StatTargets.SuperMin, export.StatTargets.SuperMax);
                ApplyStatTarget(ClassSlider, export.StatTargets.ClassMin, export.StatTargets.ClassMax);
                ApplyStatTarget(WeaponsSlider, export.StatTargets.WeaponsMin, export.StatTargets.WeaponsMax);
            }
            //Tuning slots.
            if (export.TuningSlots?.Count > 0){
                for (int i = 0; i < TuningSlots.Count && i < export.TuningSlots.Count; i++){
                    TuningSlotExport? slot = export.TuningSlots[i];
                    if (slot == null) continue;
                    if (!string.IsNullOrEmpty(slot.FocusStat)) TuningSlots[i].FocusStat = slot.FocusStat;
                    if (!string.IsNullOrEmpty(slot.FocusMinus)) TuningSlots[i].FocusMinus = slot.FocusMinus;
                }
            }
            //Font counts.
            if (export.FontCounts != null){
                FontStats[0].FontCount = export.FontCounts.Super;
                FontStats[1].FontCount = export.FontCounts.Grenade;
                FontStats[2].FontCount = export.FontCounts.Melee;
                FontStats[3].FontCount = export.FontCounts.Health;
                FontStats[4].FontCount = export.FontCounts.Weapons;
                FontStats[5].FontCount = export.FontCounts.Class;
                SyncFontCountsToMods();
            }
            //Mods.
            ModVM.MinorModCount = export.MinorModCount;
            if (export.ArmorModsEnabled && export.AdvancedMods != null){
                ApplyAdvancedMods(ModVM.HelmetMods, export.AdvancedMods.Helmet);
                ApplyAdvancedMods(ModVM.ArmsMods, export.AdvancedMods.Arms);
                ApplyAdvancedMods(ModVM.ChestplateMods, export.AdvancedMods.Chestplate);
                ApplyAdvancedMods(ModVM.BootsMods, export.AdvancedMods.Boots);
                ApplyAdvancedMods(ModVM.ClassItemMods, export.AdvancedMods.ClassItem);
            }
            //Fragments & subclass.
            if (!string.IsNullOrEmpty(export.SelectedClass)) FragmentVM.SelectedClass = export.SelectedClass;
            if (!string.IsNullOrEmpty(export.SelectedSubclass)) FragmentVM.SelectedSubclass = export.SelectedSubclass;
            FragmentVM.ShowFullSubclass = export.ShowFullSubclass;
            //Restore fragment checkbox states (works for both stat-only & full-subclass modes).
            if (export.SelectedFragments?.Count > 0){
                foreach (FragmentSelectionItem item in FragmentVM.Fragments) item.IsSelected = export.SelectedFragments.Contains(item.Name);
            }
            //Restore full-subclass fields when applicable.
            if (export.SubclassCustomization && export.Subclass != null){
                if (!string.IsNullOrEmpty(export.Subclass.PlayerClass)) ExoticVM.SelectedClass = export.Subclass.PlayerClass;
                FragmentVM.CurrentClass = ExoticVM.GetPlayerClass();
                if (!string.IsNullOrEmpty(export.Subclass.SubclassName)) FragmentVM.SelectedSubclass = export.Subclass.SubclassName;
                //Subclass Options.
                if (!string.IsNullOrEmpty(export.Subclass.Super)) FragmentVM.SelectedSuper = export.Subclass.Super;
                if (!string.IsNullOrEmpty(export.Subclass.Melee)) FragmentVM.SelectedMelee = export.Subclass.Melee;
                if (!string.IsNullOrEmpty(export.Subclass.Grenade)) FragmentVM.SelectedGrenade = export.Subclass.Grenade;
                if (!string.IsNullOrEmpty(export.Subclass.ClassAbility)) FragmentVM.SelectedClassAbility = export.Subclass.ClassAbility;
                if (!string.IsNullOrEmpty(export.Subclass.Jump)) FragmentVM.SelectedJump = export.Subclass.Jump;
                //Fragments & Aspects.
                if (FragmentVM.Aspects != null){
                    foreach (var item in FragmentVM.Aspects) item.IsSelected = export.Subclass.Aspects.Contains(item.Name); 
                }
                if (export.Subclass.Fragments != null){
                    foreach (FragmentSelectionItem item in FragmentVM.Fragments) item.IsSelected = export.Subclass.Fragments.Contains(item.Name); 
                }
            }
            //Exotic.
            if (!string.IsNullOrEmpty(export.ExoticClass)) ExoticVM.SelectedClass = export.ExoticClass;
            if (!string.IsNullOrEmpty(export.ExoticSlot)) ExoticVM.SelectedSlot = export.ExoticSlot;
            ExoticVM.CustomRollEnabled = export.CustomExoticRoll;
            if (export.CustomExoticRoll){
                ExoticVM.CustomHealth = export.ExoticStat1Value;
                ExoticVM.CustomMelee = export.ExoticStat2Value;
                ExoticVM.CustomGrenade = export.ExoticStat3Value;
                ExoticVM.CustomSuper = export.ExoticStat4Value;
                ExoticVM.CustomClass = export.ExoticStat5Value;
                ExoticVM.CustomWeapons = export.ExoticStat6Value;
            }
            SyncExoticSlotToMods();
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
        Method        : ApplyAdvancedMods
        Description   : Restores per-slot mod selections from list of
                        mod name strings into given SlotModViewModel.
        Parameters    : SlotModViewModel slotVM   : Mod slot ViewModel to populate.
                        List<string>     modNames : Mod names to restore.
        Return Values : void
        */
        private static void ApplyAdvancedMods(SlotModViewModel slotVM, List<string> modNames){
            if (modNames == null || modNames.Count == 0) return;
            slotVM.RestoreSelections(modNames);
        }
        /*
        Method        : RunExport
        Description   : Builds BuildExport snapshot from complete current
                        UI state & result, then prompts user to save it.
        Parameters    : None.
        Return Values : void
        */
        private void RunExport(){
            if (!ResultVM.HasResult || _lastResult == null) return;
            //Stat targets.
            StatTargetsExport targets = new(){
                HealthMin = HealthSlider.ToMinValue(), HealthMax = HealthSlider.ToMaxValue(),
                MeleeMin = MeleeSlider.ToMinValue(), MeleeMax = MeleeSlider.ToMaxValue(),
                GrenadeMin = GrenadeSlider.ToMinValue(), GrenadeMax = GrenadeSlider.ToMaxValue(),
                SuperMin = SuperSlider.ToMinValue(), SuperMax = SuperSlider.ToMaxValue(),
                ClassMin = ClassSlider.ToMinValue(), ClassMax = ClassSlider.ToMaxValue(),
                WeaponsMin = WeaponsSlider.ToMinValue(), WeaponsMax = WeaponsSlider.ToMaxValue()
            };
            //Tuning slots.
            List<TuningSlotExport?> tuning = [];
            foreach (TuningSlotViewModel ts in TuningSlots) tuning.Add(new TuningSlotExport{FocusStat = ts.FocusStat, FocusMinus = ts.FocusMinus});
            //Font counts.
            FontCountsExport fontCounts = new(){
                Super = FontStats[0].FontCount, Grenade = FontStats[1].FontCount,
                Melee = FontStats[2].FontCount, Health = FontStats[3].FontCount,
                Weapons = FontStats[4].FontCount, Class = FontStats[5].FontCount
            };
            //Advanced mod selections.
            AdvancedModsExport advancedMods = new(){
                Helmet = ModVM.HelmetMods.GetSelectedModNames(), Arms = ModVM.ArmsMods.GetSelectedModNames(),
                Chestplate = ModVM.ChestplateMods.GetSelectedModNames(), Boots = ModVM.BootsMods.GetSelectedModNames(),
                ClassItem = ModVM.ClassItemMods.GetSelectedModNames()
            };
            //Fragment selections (always captured, used in both modes).
            List<string> selectedFragments = [];
            foreach (FragmentSelectionItem item in FragmentVM.Fragments){
                if (item.IsSelected) selectedFragments.Add(item.Name);
            }
            //Full subclass export (only when SubclassCustomization is on).
            SubclassExport? subclassExport = null;
            if (SubclassCustomization && FragmentVM.SelectedSubclass != "None"){
                List<string> aspectNames = [];
                List<string> fragmentNames = [];
                foreach (Aspect a in FragmentVM.GetSelectedAspects()) aspectNames.Add(a.Name);
                foreach (Fragment f in FragmentVM.GetSelectedFragments()) fragmentNames.Add(f.Name);

                subclassExport = new SubclassExport {
                    PlayerClass = ExoticVM.SelectedClass, SubclassName = FragmentVM.SelectedSubclass,
                    Super = FragmentVM.SelectedSuper, Melee = FragmentVM.SelectedMelee,
                    Grenade = FragmentVM.SelectedGrenade, ClassAbility = FragmentVM.SelectedClassAbility,
                    Jump = FragmentVM.SelectedJump, Aspects = aspectNames, Fragments = fragmentNames
                };
            }
            //All six custom exotic roll values.
            int[] customRollStats = [
                ExoticVM.CustomHealth, ExoticVM.CustomMelee,
                ExoticVM.CustomGrenade, ExoticVM.CustomSuper,
                ExoticVM.CustomClass, ExoticVM.CustomWeapons
            ];
            BuildExport export = ImportExportHelper.BuildToExport(
                result: _lastResult, targets: targets, customTuning: CustomTuning,
                leastWantedStat: LeastWantedStat ?? Stat.Health.ToString(), tuning: tuning, 
                fontsEnabled: FontsEnabled, fontsInStats: FontsInStats, fontCounts: fontCounts,
                armorModsEnabled: ArmorModsEnabled, minorModCount: ModVM.MinorModCount, advancedMods: advancedMods,
                fragmentsEnabled: FragmentsEnabled, showFullSubclass: FragmentVM.ShowFullSubclass,
                selectedClass: FragmentVM.SelectedClass, selectedSubclass: FragmentVM.SelectedSubclass,
                selectedFragments: selectedFragments, subclass: subclassExport,
                exoticClass: ExoticVM.SelectedClass, exoticSlot: ExoticVM.SelectedSlot,
                customExoticRoll: ExoticVM.CustomRollEnabled, customRollStats: customRollStats,
                t5ExoticEnabled: T5ExoticEnabled, showDimQueries: ShowDimQueries,
                subclassCustomization: SubclassCustomization
            );
            ImportExportHelper.Export(export);
        }
        //=====================================================================
        //Reset.
        //=====================================================================
        /*
        Method        : Reset
        Description   : Resets every UI element to default/unselected
                        state & clears result panel. Also resets all
                        persistent AppSettings to defaults.
        Parameters    : None.
        Return Values : void
        */
        private void Reset(){
            //Stat sliders.
            HealthSlider.Reset();
            MeleeSlider.Reset();
            GrenadeSlider.Reset();
            SuperSlider.Reset();
            ClassSlider.Reset();
            WeaponsSlider.Reset();
            //Result panel.
            ResultVM.Clear();
            _lastResult = null;
            //Tuning.
            CustomTuning = false;
            LeastWantedStat = Stat.Health.ToString();
            foreach (TuningSlotViewModel ts in TuningSlots){
                ts.FocusStat = "Weapons";
                ts.FocusMinus = "Health";
            }
            //Fonts.
            FontsEnabled = false;
            FontsInStats = false;
            foreach (FontStatViewModel fs in FontStats) fs.FontCount = 0;
            SyncFontCountsToMods();
            //Mods.
            ArmorModsEnabled = false;
            ModVM.MinorModCount = 0;
            ModVM.HelmetMods.RestoreSelections([]);
            ModVM.ArmsMods.RestoreSelections([]);
            ModVM.ChestplateMods.RestoreSelections([]);
            ModVM.BootsMods.RestoreSelections([]);
            ModVM.ClassItemMods.RestoreSelections([]);
            //Fragments & subclass.
            FragmentsEnabled = false;
            FragmentVM.ShowFullSubclass = false;
            FragmentVM.SelectedSubclass = "None";
            foreach (FragmentSelectionItem item in FragmentVM.Fragments) item.IsSelected = false;
            if (FragmentVM.Aspects != null){
                foreach (AspectSelectionItem item in FragmentVM.Aspects) item.IsSelected = false;
            }
            //Full subclass abilities.
            SubclassCustomization = false;
            //Exotic.
            ExoticVM.SelectedClass = "Warlock";
            ExoticVM.SelectedSlot = "Helmet";
            ExoticVM.CustomRollEnabled = false;
            ExoticVM.CustomHealth = 0;
            ExoticVM.CustomMelee = 0;
            ExoticVM.CustomGrenade = 0;
            ExoticVM.CustomSuper = 0;
            ExoticVM.CustomClass = 0;
            ExoticVM.CustomWeapons = 0;
            T5ExoticEnabled = false;
            //DIM Queries.
            ShowDimQueries = false;
            //Persist all defaults.
            AppSettings.ResetToDefaults();
 
            SyncExoticSlotToMods();
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