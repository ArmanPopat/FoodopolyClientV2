using System.Globalization;

namespace FoodopolyClientV2.ViewModels.Popups.Converters;

public class IntRentDoubledBoolToIntRent : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        int intValue = (int)values[0];
        
        if (bool.Parse((string)values[1]))
        {
            intValue = 2 * intValue;
        }
        return intValue;
    }
    public object[] ConvertBack(object value, Type[] targetType, object parameter, CultureInfo culture)
    {
        //I.E. Not Implemented
        return (object[])Binding.DoNothing;
    }
    public static object GetPropertyValue(object src, string propertyName)
    {
        if (propertyName.Contains("."))
        {
            var splitIndex = propertyName.IndexOf('.');
            var parent = propertyName.Substring(0, splitIndex);
            var child = propertyName.Substring(splitIndex + 1);
            var obj = src?.GetType().GetProperty(parent)?.GetValue(src, null);
            return GetPropertyValue(obj, child);
        }
        return src?.GetType().GetProperty(propertyName)?.GetValue(src, null);
    }
}
