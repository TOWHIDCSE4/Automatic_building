using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DaiwaRentalGD.Gui.ViewModels
{
    /// <summary>
    /// Base class for view models.
    /// </summary>
    public class ViewModelBase : INotifyPropertyChanged
    {
        #region Constructors

        public ViewModelBase()
        { }

        #endregion

        #region Methods

        protected void NotifyPropertyChanged(
            [CallerMemberName] string propertyName = ""
        )
        {
            PropertyChanged?.Invoke(
                this, new PropertyChangedEventArgs(propertyName)
            );
        }

        #endregion

        #region Properties

        public string ViewModelName
        {
            get => _viewModelName;
            set
            {
                _viewModelName = value ??
                    throw new ArgumentNullException(nameof(value));

                NotifyPropertyChanged();
            }
        }

        public Type ViewModelType => GetType();

        #endregion

        #region Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Member variables

        private string _viewModelName = DefaultName;

        #endregion

        #region Constants

        public const string DefaultName = "";

        #endregion
    }
}
