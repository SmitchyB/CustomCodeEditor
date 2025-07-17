using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CodeEditor.Components
{
    /// <summary>
    /// Margin converter that converts an indent level to a margin.
    /// This is used for the Task Tracking feature to create a visual alignment of Main and Sub Tasks.
    /// </summary>
    public class IndentToMarginConverter : IValueConverter
    {
        // Method for converting an indent level to a margin
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int indent) //Check if the value is an integer representing the indent level
            {
                return new Thickness(indent * 20, 0, 0, 0); // Convert the indent level to a margin, multiplying by 20 for spacing
            }
            return new Thickness(0); // Return a default margin of 0 if the value is not an integer
        }
        // This method is not implemented as we are only converting from indent level to margin but is required by the IValueConverter interface
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
