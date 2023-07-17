
using FoodopolyClasses.PlayerClasses;
using Microsoft.Maui.Graphics.Converters;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodopolyClientV2.ViewModels.Popups.Converters;

public class PropertySetToColourConverter : IValueConverter
{
    ColorTypeConverter colorTypeConverter = new ColorTypeConverter();
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string colourString = value.ToString();
        Color colour = (Color)colorTypeConverter.ConvertFromInvariantString(String.Concat(colourString.Where(c => !Char.IsWhiteSpace(c))));
        return value;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        //I.E. Not Implemented
        return Binding.DoNothing;
    }
}

