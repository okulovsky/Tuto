using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Tuto.Navigator
{
    public class RelayCommand<T> : ICommand where T : class
    {
        #region Fields

        private readonly Action<T> _execute;
        private readonly Predicate<T> _canExecute;

        #endregion

        #region Constructors

        public RelayCommand(Action<T> execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter as T);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute(parameter as T);
        }

        #endregion
    }

    public class RelayCommand : ICommand
    {
        #region Fields

        private readonly Action _execute;
        private readonly Func<bool> _canExecute;
        #endregion

        #region Constructors

        public RelayCommand(Action execute)
            : this(execute, null)
        {
        }

        public RelayCommand(Action execute, Func<bool> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            _execute();
        }

        #endregion
    }
    
// To use this class within your viewmodel class:
/*RelayCommand _myCommand;
public ICommand MyCommand
{
    get
    {
        if (_myCommand == null)
        {
            _myCommand = new RelayCommand(p => this.DoMyCommand(p),
                p => this.CanDoMyCommand(p) );
        }
        return _myCommand;
    }
}*/
}
