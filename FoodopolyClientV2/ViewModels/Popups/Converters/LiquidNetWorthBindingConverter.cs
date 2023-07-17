using FoodopolyClasses.PlayerClasses;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodopolyClientV2.ViewModels.Popups.Converters;

public class LiquidNetWorthBindingConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        PlayerClass player = (PlayerClass)value;
        return value;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        //I.E. Not Implemented
        return Binding.DoNothing;
    }
}
