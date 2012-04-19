using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Parameter_Filter
{
    class RelayCommand : ICommand
    {
        bool canExecute = true;
        Action command;

        private bool Executable
        {
            get { return canExecute; }
            set
            {
                if (value != canExecute)
                {
                    canExecute = value;
                    EventHandler eHandler = CanExecuteChanged;
                    if (eHandler != null)
                        eHandler(this, EventArgs.Empty);
                }
            }
        }

        public bool CanExecute(object parameter)
        {
            return Executable;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            command();
        }

        public RelayCommand(Action command, IObservable<bool> executable = null)
        {
            this.command = command;

            if (executable != null)
            {
                executable.Subscribe(v => Executable = v);
            }
        }
    }
}
