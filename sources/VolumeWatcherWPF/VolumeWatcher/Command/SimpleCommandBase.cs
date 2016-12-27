using System;
using System.Windows.Input;

namespace VolumeWatcher.Command
{
    abstract class SimpleCommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        abstract public void Execute(object parameter);
    }
}
