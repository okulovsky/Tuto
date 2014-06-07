using System;
using System.Windows.Input;

namespace Tuto.Navigator
{
    public class Command : ICommand
    {

        public Command(Action action, bool canExecute = true)
        {
            this.action = action;
            this.canExecute = canExecute;
        }

        bool ICommand.CanExecute(object parameter)
        {
            return canExecute;
        }

        public void Execute(object parameter)
        {
            if(CanExecute)
                action();
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute
        {
            get { return canExecute; }
            set
            {
                if (canExecute != value)
                {
                    canExecute = value;
                    if (CanExecuteChanged != null)
                        CanExecuteChanged(this, EventArgs.Empty);
                }
                
            }
        }

        private readonly Action action;
        private bool canExecute;

    }
}