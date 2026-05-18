/*
*   FILE          : RelayCommand.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Implements ICommand as relay command, allowing ViewModel
*                   methods to be bound to UI controls without code-behind.
*/
using System.Windows.Input;

namespace D2ArmorCalc {
    public class RelayCommand : ICommand {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null){
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
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