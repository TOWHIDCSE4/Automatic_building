using System;
using System.Globalization;
using System.Windows.Data;

namespace DaiwaRentalGD.Gui.ValueConverters
{
    /// <summary>
    /// Value converter that negates a <see cref="bool"/> value.
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class BooleanNegationValueConverter : IValueConverter
    {
        #region Constructors

        public BooleanNegationValueConverter()
        { }

        #endregion

        #region Methods

        public object Convert(
            object value, Type targetType,
            object parameter, CultureInfo culture
        )
        {
            bool boolValue = System.Convert.ToBoolean(value);

            return !boolValue;
        }

        public object ConvertBack(
            object value, Type targetType,
            object parameter, CultureInfo culture
        )
        {
            bool boolValue = System.Convert.ToBoolean(value);

            return !boolValue;
        }

        #endregion
    }
}
