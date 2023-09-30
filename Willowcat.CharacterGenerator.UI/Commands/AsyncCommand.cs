using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Willowcat.CharacterGenerator.UI.Commands
{
    public class AsyncCommand : ICommand
    {
        private readonly Func<Task> _Action;
        private readonly Func<bool> _CanExecute;

        public event EventHandler CanExecuteChanged;

        public AsyncCommand(Func<Task> action)
        {
            _Action = action;
        }

        public AsyncCommand(Func<Task> action, Func<bool> canExecute)
        {
            _Action = action;
            _CanExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _CanExecute?.Invoke() ?? true;
        }

        public async void Execute(object parameter)
        {
            await _Action?.Invoke();
        }

        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}
