using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace DaiwaRentalGD.Gui.Utilities
{
    /// <summary>
    /// Data template selector for subclasses of
    /// <see cref="ViewModels.ViewModelBase"/>.
    /// </summary>
    public class ViewModelDataTemplateSelector : DataTemplateSelector
    {
        #region Constructors

        public ViewModelDataTemplateSelector() : base()
        { }

        #endregion

        #region Methods

        public static Type GetViewType(Type viewModelType)
        {
            var viewModelName = viewModelType.FullName;
            var viewName = viewModelName.Replace("ViewModel", "View");

            var uiAssemblyName = viewModelType.Assembly.FullName;

            var viewQualifiedName = string.Format(
                CultureInfo.InvariantCulture,
                "{0}, {1}",
                viewName, uiAssemblyName
            );

            var viewType = Type.GetType(viewQualifiedName);

            return viewType;
        }

        public override DataTemplate SelectTemplate(
            object item, DependencyObject container
        )
        {
            if (item == null)
            {
                return null;
            }

            var viewType = GetViewType(item.GetType());

            if (viewType == null)
            {
                return null;
            }

            var dataTemplate = new DataTemplate
            {
                VisualTree = new FrameworkElementFactory(viewType)
            };

            return dataTemplate;
        }

        #endregion
    }
}
