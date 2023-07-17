using BoardClasses;
using CommunityToolkit.Mvvm.ComponentModel;
using FoodopolyClasses.PlayerClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodopolyClientV2.ViewModels.Popups;

public partial class InitiateTradePopupViewModel:ObservableObject
{
    public List<Station> MyStations { get; }
    public List<Utility> MyUtilities { get; }
    public List<Property> MyProperties { get; }
    public int MyCash { get; }
    public List<Station> TheirStations { get; }
    public List<Utility> TheirUtilities { get; }
    public List<Property> TheirProperties { get; }
    public int TheirCash { get; }

    [ObservableProperty]
    public List<Station> mySelectedStations = new List<Station>();
    [ObservableProperty]
    public List<Station> theirSelectedStations = new List<Station>();
    [ObservableProperty]
    public List<Utility> mySelectedUtilities = new List<Utility>();
    [ObservableProperty]
    public List<Utility> theirSelectedUtilities = new List<Utility>();
    [ObservableProperty]
    public List<Property> mySelectedProperties = new List<Property>();
    [ObservableProperty]
    public List<Property> theirSelectedProperties = new List<Property>();

    [ObservableProperty]
    public int mySelectedCash = 0;

    [ObservableProperty]
    public int theirSelectedCash = 0;

    public GameViewModel ThisGameViewModel { get; }

    public InitiateTradePopupViewModel(GameViewModel gameViewModel, PlayerClass me, PlayerClass otherPlayer)
    {
        MyCash = me.Cash;
        var myOwnedStuff = me.GetOwnedPropsAndStuff(gameViewModel.GameClass);
        MyStations = myOwnedStuff.Stations;
        MyUtilities = myOwnedStuff.Utilities;
        MyProperties = myOwnedStuff.Properties;
        TheirCash = otherPlayer.Cash;
        var theirOwnedStuff = otherPlayer.GetOwnedPropsAndStuff(gameViewModel.GameClass);
        TheirStations = theirOwnedStuff.Stations;
        TheirUtilities = theirOwnedStuff.Utilities;
        TheirProperties = theirOwnedStuff.Properties;
        ThisGameViewModel = gameViewModel;
    }
}
