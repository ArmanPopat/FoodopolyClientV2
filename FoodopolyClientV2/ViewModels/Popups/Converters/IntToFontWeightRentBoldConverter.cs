using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodopolyClientV2.ViewModels.Popups.Converters;

public class IntToFontWeightRentBoldConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isBold = ((int)value == int.Parse((string)parameter));
        return isBold ? FontAttributes.Bold : FontAttributes.None;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        //I.E. Not Implemented
        return Binding.DoNothing;
    }
}