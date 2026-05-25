/*
*   FILE          : FragmentViewModel.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : ViewModel for subclass & fragment selection, managing
*                   subclass dropdown, fragment filtering, aspect selection,
*                   & full subclass customization when toggled.
*/
using D2ArmorCalc_Data;
using D2ArmorCalc_Models;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace D2ArmorCalc_ViewModels {
    //Wraps Fragment with selection state for UI.
    public class FragmentSelectionItem(Fragment fragment) : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public Fragment Fragment {get;} = fragment;
        public string Name => Fragment.Name;
        public string StatInfo {get;} = BuildStatInfo(fragment);
        public bool IsSelected {
            get;
            set {
                field = value; 
                OnPropertyChanged(nameof(IsSelected));
            }
        }
        private static string BuildStatInfo(Fragment fragment){
            if (fragment.StatChanges.Length == 0) return string.Empty;
            List<string> parts = [];
            foreach (StatChange change in fragment.StatChanges) parts.Add($"{(change.Value > 0 ? "+" : "")}{change.Value}{change.Stat}");
            return string.Join(", ", parts);
        }
    }
    //Wraps Aspect with selection state for UI.
    public class AspectSelectionItem(Aspect aspect) : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        public Aspect Aspect {get;} = aspect;
        public string Name => Aspect.Name;
        public string FragmentSlots => $"({Aspect.FragmentSlots} fragment slots)";
        public bool IsSelected {
            get;
            set {
                field = value; 
                OnPropertyChanged(nameof(IsSelected));
            }
        }
    }
    public class FragmentViewModel : INotifyPropertyChanged {
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //=====================================================================
        //Properties.
        //=====================================================================
        public string SelectedSubclass {
            get;
            set {
                field = value;
                OnPropertyChanged(nameof(SelectedSubclass));
                RefreshFragments();
                RefreshAspects();
                RefreshSubclassOptions();
            }
        } = "None";
        public bool FragmentsEnabled {
            get;
            set {
                field = value;
                OnPropertyChanged(nameof(FragmentsEnabled));
            }
        }
        public string SelectedClass {
            get;
            set {
                if (field == value) return;
                field = value;
                OnPropertyChanged(nameof(SelectedClass));
                //Update CurrentClass enum.
                if (Enum.TryParse(value, out PlayerClass pc)) CurrentClass = pc;
                //Keep subclass name but reload for new class.
                string current = SelectedSubclass;
                SelectedSubclass = "None";
                RefreshFragments();
                RefreshAspects();
                RefreshSubclassOptions();
                SelectedSubclass = current;
                RefreshFragments();
                RefreshAspects();
                RefreshSubclassOptions();
                //Notify class changed for two-way sync with ExoticVM.
                ClassChanged?.Invoke(this, value);
            }
        } = "Warlock";
        //Event for two-way class sync.
        public event EventHandler<string>? ClassChanged;
        //Show full subclass toggle.
        public bool ShowFullSubclass {
            get;
            set {
                field = value;
                OnPropertyChanged(nameof(ShowFullSubclass));
                OnPropertyChanged(nameof(MaxFragmentSlots));
                RefreshFragments();
                EnforceFragmentLimit();
            }
        }
        //Fragment list (filtered based on ShowFullSubclass).
        public ObservableCollection<FragmentSelectionItem> Fragments {get;} = [];
        //Aspect list (only shown in full subclass mode).
        public ObservableCollection<AspectSelectionItem> Aspects {get;} = [];
        //Full subclass options (only shown in full subclass mode).
        public ObservableCollection<string> Supers {get;} = [];
        public ObservableCollection<string> Melees {get;} = [];
        public ObservableCollection<string> Grenades {get;} = [];
        public ObservableCollection<string> ClassAbilities {get;} = [];
        public ObservableCollection<string> Jumps {get;} = [];
        public string? SelectedSuper {
            get;
            set {field = value; OnPropertyChanged(nameof(SelectedSuper));}
        }
        public string? SelectedMelee {
            get;
            set {field = value; OnPropertyChanged(nameof(SelectedMelee));}
        }
        public string? SelectedGrenade {
            get;
            set {field = value; OnPropertyChanged(nameof(SelectedGrenade));}
        }
        public string? SelectedClassAbility {
            get;
            set {field = value; OnPropertyChanged(nameof(SelectedClassAbility));}
        }
        public string? SelectedJump {
            get;
            set {field = value; OnPropertyChanged(nameof(SelectedJump));}
        }
        //Fragment slot tracking.
        public int MaxFragmentSlots => GetMaxFragmentSlots();
        public int SelectedFragmentCount => GetSelectedFragmentCount();
        //Needed externally to filter by class.
        public PlayerClass CurrentClass {get; set;} = PlayerClass.Warlock;
        //=====================================================================
        //Refresh Methods.
        //=====================================================================
        /*
        Method        : RefreshFragments
        Description   : Repopulates fragment list based on selected
                        subclass. If ShowFullSubclass is false, only fragments
                        with stat changes are shown. Preserves existing selections.
        Parameters    : None.
        Return Values : void
        */
        private void RefreshFragments(){
            HashSet<string> previousSelections = [];
            foreach (FragmentSelectionItem item in Fragments){
                if (item.IsSelected) previousSelections.Add(item.Name);
            }
            Fragments.Clear();

            if (SelectedSubclass == "None") return;
            Subclass? subclass = Subclasses.GetSubclass(CurrentClass, SelectedSubclass);
            if (subclass == null) return;

            foreach (Fragment fragment in subclass.Fragments){
                if (!ShowFullSubclass && fragment.StatChanges.Length == 0) continue;
                FragmentSelectionItem item = new(fragment){
                    IsSelected = previousSelections.Contains(fragment.Name)
                };
                item.PropertyChanged += (s, e) => {
                    if (e.PropertyName == nameof(FragmentSelectionItem.IsSelected)) OnFragmentSelectionChanged();
                };
                Fragments.Add(item);
            }
            OnPropertyChanged(nameof(SelectedFragmentCount));
            OnPropertyChanged(nameof(MaxFragmentSlots));
        }
        /*
        Method        : RefreshAspects
        Description   : Repopulates aspect list for selected subclass.
        Parameters    : None.
        Return Values : void
        */
        private void RefreshAspects(){
            Aspects.Clear();
            if (SelectedSubclass == "None") return;

            Subclass? subclass = Subclasses.GetSubclass(CurrentClass, SelectedSubclass);
            if (subclass == null) return;

            foreach (Aspect aspect in subclass.Aspects){
                AspectSelectionItem item = new(aspect);
                item.PropertyChanged += (s, e) => {
                    if (e.PropertyName == nameof(AspectSelectionItem.IsSelected)){
                        OnPropertyChanged(nameof(MaxFragmentSlots));
                        EnforceFragmentLimit();
                    }
                };
                Aspects.Add(item);
            }
        }
        /*
        Method        : RefreshSubclassOptions
        Description   : Repopulates super, melee, grenade, class ability, &
                        jump options for selected subclass.
        Parameters    : None.
        Return Values : void
        */
        private void RefreshSubclassOptions(){
            Supers.Clear();
            Melees.Clear();
            Grenades.Clear();
            ClassAbilities.Clear();
            Jumps.Clear();

            if (SelectedSubclass == "None") return;

            Subclass? subclass = Subclasses.GetSubclass(CurrentClass, SelectedSubclass);
            if (subclass == null) return;

            foreach (string s in subclass.Supers) Supers.Add(s);
            foreach (string m in subclass.Melees) Melees.Add(m);
            foreach (string g in subclass.Grenades) Grenades.Add(g);
            foreach (string c in subclass.ClassAbilities) ClassAbilities.Add(c);
            foreach (string j in subclass.Jumps) Jumps.Add(j);

            SelectedSuper = Supers.Count > 0 ? Supers[0] : null;
            SelectedMelee = Melees.Count > 0 ? Melees[0] : null;
            SelectedGrenade = Grenades.Count > 0 ? Grenades[0] : null;
            SelectedClassAbility = ClassAbilities.Count > 0 ? ClassAbilities[0] : null;
            SelectedJump = Jumps.Count > 0 ? Jumps[0] : null;
        }
        //=====================================================================
        //Fragment Slot Logic.
        //=====================================================================
        /*
        Method        : GetMaxFragmentSlots
        Description   : Returns total fragment slots available based on
                        selected aspects. Defaults to 6 if no aspects
                        selected (no subclass chosen).
        Parameters    : None.
        Return Values : int : Total available fragment slots.
        */
        private int GetMaxFragmentSlots(){
            //When full subclass is off, always return 6.
            if (!ShowFullSubclass) return 6;
            if (SelectedSubclass == "None") return 6;

            int total = 0, selected = 0;
            foreach (AspectSelectionItem item in Aspects){
                if (item.IsSelected){
                    total += item.Aspect.FragmentSlots;
                    selected++;
                    if (selected >= 2) break;
                }
            }
            return selected == 0 ? 6 : total;
        }
        /*
        Method        : GetSelectedFragmentCount
        Description   : Returns number of currently selected fragments.
        Parameters    : None.
        Return Values : int : Number of selected fragments.
        */
        private int GetSelectedFragmentCount(){
            int count = 0;
            foreach (FragmentSelectionItem item in Fragments){
                if (item.IsSelected) count++;
            }
            return count;
        }
        /*
        Method        : OnFragmentSelectionChanged
        Description   : Called when fragment selection changes. Enforces
                        fragment slot limit & updates counts.
        Parameters    : None.
        Return Values : void
        */
        private void OnFragmentSelectionChanged(){
            EnforceFragmentLimit();
            OnPropertyChanged(nameof(SelectedFragmentCount));
        }
        /*
        Method        : EnforceFragmentLimit
        Description   : Deselects most recently selected fragment if
                        selected count exceeds available fragment slots.
        Parameters    : None.
        Return Values : void
        */
        private void EnforceFragmentLimit(){
            int max = GetMaxFragmentSlots();
            int count = GetSelectedFragmentCount();
            if (count <= max) return;
            //Deselect last selected fragment to bring back under limit
            for (int i = Fragments.Count - 1; i >= 0; i--){
                if (Fragments[i].IsSelected){
                    Fragments[i].IsSelected = false;
                    break;
                }
            }
        }
        //=====================================================================
        //Output Helpers.
        //=====================================================================
        /*
        Method        : GetSelectedFragments
        Description   : Returns array of all currently selected Fragment
                        objects for use by algorithm.
        Parameters    : None.
        Return Values : Fragment[] : All selected fragments.
        */
        public Fragment[] GetSelectedFragments(){
            if (!FragmentsEnabled) return [];
            List<Fragment> selected = [];
            foreach (FragmentSelectionItem item in Fragments){
                if (item.IsSelected) selected.Add(item.Fragment);
            }
            return [.. selected];
        }
        /*
        Method        : GetSelectedAspects
        Description   : Returns array of selected Aspect objects
                        (max 2) for export purposes.
        Parameters    : None.
        Return Values : Aspect[] : Selected aspects.
        */
        public Aspect[] GetSelectedAspects(){
            List<Aspect> selected = [];
            foreach (AspectSelectionItem item in Aspects){
                if (item.IsSelected){
                    selected.Add(item.Aspect);
                    if (selected.Count >= 2) break;
                }
            }
            return [.. selected];
        }
    }
}