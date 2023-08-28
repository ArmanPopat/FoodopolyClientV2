using BoardClasses;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    public List<Property> MyUnupgradedProperties { get; }
    //public List<Property> MyUpgradedProperties { get; }
    public int MyCash { get; }
    public List<Station> TheirStations { get; }
    public List<Utility> TheirUtilities { get; }
    public List<Property> TheirUnupgradedProperties { get; }
    //public List<Property> TheirUpgradedProperties { get; }
    public int TheirCash { get; }

    public int MyGODCards { get; }
    public int TheirGODCards { get; }

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

    [ObservableProperty]
    public int mySelectedGODCards = 0;

    [ObservableProperty]
    public int theirSelectedGODCards = 0;

    public GameViewModel ThisGameViewModel { get; }

    private PlayerClass _localPlayer;
    private PlayerClass _otherPlayer;

    public InitiateTradePopupViewModel(GameViewModel gameViewModel, PlayerClass me, PlayerClass otherPlayer)
    {
        MyCash = me.Cash;
        var myOwnedStuff = me.GetOwnedPropsAndStuff(gameViewModel.GameClass);
        MyStations = myOwnedStuff.Stations;
        MyUtilities = myOwnedStuff.Utilities;
        MyUnupgradedProperties = myOwnedStuff.Properties.Where(o => o.NumOfUpgrades <= 0).ToList();
        TheirCash = otherPlayer.Cash;
        var theirOwnedStuff = otherPlayer.GetOwnedPropsAndStuff(gameViewModel.GameClass);
        TheirStations = theirOwnedStuff.Stations;
        TheirUtilities = theirOwnedStuff.Utilities;
        MyUnupgradedProperties = theirOwnedStuff.Properties.Where(o => o.NumOfUpgrades <= 0).ToList();
        ThisGameViewModel = gameViewModel;
        MyGODCards = me.NumOfGODCards;
        TheirGODCards = otherPlayer.NumOfGODCards;
        _localPlayer = me;
        _otherPlayer = otherPlayer;
    }

    [RelayCommand]
    async Task InitiateTradeSend()
    {
        await ThisGameViewModel.StartSendTrade(_localPlayer, MySelectedCash, MySelectedStations, MySelectedUtilities, MySelectedProperties, MySelectedGODCards,
            _otherPlayer, TheirSelectedCash, TheirSelectedStations, TheirSelectedUtilities, TheirSelectedProperties, TheirSelectedGODCards);
    }
}
