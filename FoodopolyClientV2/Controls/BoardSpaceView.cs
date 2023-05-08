using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodopolyClientV2.Controls;

public class BoardSpaceView : ContentView
{
    public static readonly BindableProperty SpaceNameProperty = BindableProperty.Create(nameof(SpaceName), typeof(string), typeof(BoardSpaceView), string.Empty);
    public string SpaceName
    {
        get => (string)GetValue(SpaceNameProperty);
        set => SetValue(SpaceNameProperty, value);
    }
    public static readonly BindableProperty SetOrTypeProperty = BindableProperty.Create(nameof(SetOrType), typeof(string), typeof(BoardSpaceView), string.Empty);
    public string SetOrType
    {
        get => (string)GetValue(SetOrTypeProperty);
        set => SetValue(SetOrTypeProperty, value);
    }
}
