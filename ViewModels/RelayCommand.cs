/*
*   FILE          : RelayCommand.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Implements ICommand as relay command, allowing ViewModel
*                   methods to be bound to UI controls without code-behind.
*/
using System.Windows.Input;

namespace D2ArmorCalc_ViewModels {
    public class RelayCommand(Action<object> execute, Func<object, bool> canExecute = null) : ICommand {
        private readonly Action<object> _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        private readonly Func<object, bool> _canExecute = canExecute;

        public event EventHandler CanExecuteChanged {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        /*
        Method        : CanExecute
        Description   : Returns whether command can currently execute.
                        Defaults to true if no canExecute predicate was provided.
        Parameters    : object parameter : Optional command parameter.
        Return Values : bool             : True if the command can execute.
        */
        public bool CanExecute(object parameter){
            return _canExecute == null || _canExecute(parameter);
        }
        /*
        Method        : Execute
        Description   : Executes command action with given parameter.
        Parameters    : object parameter : Optional command parameter.
        Return Values : void
        */
        public void Execute(object parameter){
            _execute(parameter);
        }
    }
}