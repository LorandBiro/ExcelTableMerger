using System;
using System.Windows.Input;

namespace ExcelTableMerger.ViewModels.Common
{
    public sealed class Command : ICommand
    {
        private bool canExecute;
        private Action action;

        public Command(Action action): this(action, true) { }

        public Command(Action action, bool canExecute)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute
        {
            get => this.canExecute;
            set
            {
                if (this.canExecute == value)
                {
                    return;
                }

                this.canExecute = value;
                this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            return this.canExecute;
        }

        void ICommand.Execute(object parameter)
        {
            this.action();
        }
    }
}
