/*
*   FILE          : Converters.cs
*   PROJECT       : D2ArmorCalc
*   PROGRAMMER    : ExoVizzy
*   FIRST VERSION : May 17, 2026
*   DESCRIPTION   : Value converters for XAML bindings including bool to
*                   visibility & non-zero int to visibility conversions.
*/
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace D2ArmorCalc {
    public class InverseBoolToVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value is bool b && b ? Visibility.Collapsed : Visibility.Visible;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value is Visibility v && v == Visibility.Collapsed;
        }
    }
    //Converts int to bool — true if value is negative (for energy over-budget warning).
    public class IsNegativeConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value is int i && i < 0;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
    //Converts bool to Visibility (true = Visible, false = Collapsed).
    public class BoolToVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value is bool b && b ? Visibility.Visible : Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            return value is Visibility v && v == Visibility.Visible;
        }
    }
    //Converts int to Visibility (non-zero = Visible, zero = Collapsed).
    public class NonZeroToVisibilityConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            return value is int i && i != 0 ? Visibility.Visible : Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}