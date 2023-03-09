using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DaiwaRentalGD.Gui.ValueConverters
{
    /// <summary>
    /// Value converter that converts between <see cref="bool"/> and
    /// <see cref="System.Windows.Visibility"/>.
    /// </summary>
    public class BooleanToVisibilityValueConverter : IValueConverter
    {
        #region Constructors

        public BooleanToVisibilityValueConverter()
        { }

        #endregion

        #region Methods

        public object Convert(
            object value, Type targetType,
            object parameter, CultureInfo culture
        )
        {
            bool boolValue = (bool)value;

            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(
            object value, Type targetType,
            object parameter, CultureInfo culture
        )
        {
            var visibilityValue = (Visibility)value;

            switch (visibilityValue)
            {
                case Visibility.Visible:

                    return true;

                case Visibility.Collapsed:

                    return false;

                default:

                    throw new NotImplementedException();
            }
        }

        #endregion
    }
}
