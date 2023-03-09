using System;
using System.Windows.Input;

namespace DaiwaRentalGD.Gui.ViewModels
{
    /// <summary>
    /// A helper command that sets up its behavior using properties.
    /// </summary>
    /// <remarks>
    /// Reference: https://docs.microsoft.com/en-us/archive/msdn-magazine/2009/february/patterns-wpf-apps-with-the-model-view-viewmodel-design-pattern
    /// </remarks>
    public class RelayCommand : ICommand
    {
        #region Constructors

        public RelayCommand()
        { }

        #endregion

        #region Methods

        public bool CanExecute(object parameter)
        {
            return CanExecutePredicate?.Invoke(parameter) ?? true;
        }

        public void Execute(object parameter)
        {
            ExecuteAction?.Invoke(parameter);
        }

        #endregion

        #region Properties

        public Predicate<object> CanExecutePredicate { get; set; }

        public Action<object> ExecuteAction { get; set; }

        #endregion

        #region Events

        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        #endregion
    }
}
