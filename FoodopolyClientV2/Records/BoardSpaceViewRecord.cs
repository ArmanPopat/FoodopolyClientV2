using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodopolyClientV2.Views.Popups;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodopolyClientV2.Records;

//Changed from record to class to use ObservableObject
public partial class BoardSpaceViewRecord:ObservableObject
{
    public string Name { get; }
    public int BoardPosition { get; }
    public string TypeOrSet { get; }
    public Color Colour { get; }
    public BoardSpaceViewRecord(string nameArg, int boardPositionArg, string typeOrSetArg, Color colourArg) 
    {
        Name = nameArg;
        BoardPosition = boardPositionArg;
        TypeOrSet = typeOrSetArg;
        Colour = colourArg;
    }
    [ObservableProperty]
    public bool p1Here = false;
    [ObservableProperty]
    public bool p2Here = false;
    [ObservableProperty]
    public bool p3Here = false;
    [ObservableProperty]
    public bool p4Here = false;

    //PlayerOwnerNum is an int, 0 means bank
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PlayerColor))]
    public int playerOwnerNum = 0;

    private Color[] _playerColors = new Color[] {Colors.White, Colors.Red, Colors.Green, Colors.Yellow, Colors.Blue};

    //Provides colour for player
    public Color PlayerColor
    {
        get 
        {
            return _playerColors[PlayerOwnerNum];
        }
    }

    static Page Page => Application.Current?.MainPage ?? throw new NullReferenceException();

    [RelayCommand]
    async Task ShowPropertyModal()
    {
        var popup = new PropertyPopup();
        Page.ShowPopup(popup);
    }
}
