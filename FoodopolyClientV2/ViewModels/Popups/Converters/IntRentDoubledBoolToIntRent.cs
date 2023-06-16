using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodopolyClientV2.ViewModels.Popups.Converters;

public class IntRentDoubledBoolToIntRent : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        int intValue = int.Parse((string)value);
        if (bool.Parse((string)parameter))
        {
            intValue = 2 * intValue;
        }
        return intValue;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        //I.E. Not Implemented
        return Binding.DoNothing;
    }
}
