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

public partial class MortgagedPropertiesTradeConfirmViewModel:ObservableObject
{
    public IEnumerable<string> ListOfMortagedOwned { get; }
    public GameViewModel ThisGameViewModel { get; }
    private PlayerClass _localPlayer;
    private int _mySelectedCash;
    private List<Station> _mySelectedStations;
    private List<Utility> _mySelectedUtilities;
    private List<Property> _mySelectedProperties;
    private int _mySelectedGODCards;
    private PlayerClass _otherplayer;
    private int _theirSelectedCash;
    private List<Station> _theirSelectedStations;
    private List<Utility> _theirSelectedUtilities;
    private List<Property> _theirSelectedProperties;
    private int _theirSelectedGODCard;

    //To determine whether it's being initiated or Confirmed
    public bool Initiating { get; }
    public bool Confirming
    {
        get
        {
            return !Initiating;
        }
    }

    [RelayCommand]
    async Task SendTrade()
    {
        await ThisGameViewModel.SendTrade(_localPlayer, _mySelectedCash, _mySelectedStations, _mySelectedUtilities, _mySelectedProperties, _mySelectedGODCards,
            _otherplayer ,_theirSelectedCash, _theirSelectedStations, _theirSelectedUtilities, _theirSelectedProperties, _theirSelectedGODCard);
    }

    public MortgagedPropertiesTradeConfirmViewModel(GameViewModel gameViewModel, 
        PlayerClass localPlayer, int mySelectedCash, List<Station> mySelectedStations, List<Utility> mySelectedUtilities, List<Property> mySelectedProperties, int mySelectedGODCards,
         PlayerClass otherPlayer, int theirSelectedCash, List<Station> theirSelectedStations, List<Utility> theirSelectedUtilities, List<Property> theirSelectedProperties, int theirSelectedGODCard,
         bool initiate = true) 
    {
        ListOfMortagedOwned = theirSelectedStations.FindAll(o => o.Mortgaged == true).Select(o => o.Name).ToList();
        ListOfMortagedOwned = ListOfMortagedOwned.Concat(theirSelectedUtilities.FindAll(o => o.Mortgaged == true).Select(o => o.Name).ToList());
        ListOfMortagedOwned = ListOfMortagedOwned.Concat(theirSelectedProperties.FindAll(o => o.Mortgaged == true).Select(o => o.Name).ToList());
        ThisGameViewModel = gameViewModel;
        _localPlayer = localPlayer;
        _mySelectedCash = mySelectedCash;
        _mySelectedStations = mySelectedStations;
        _mySelectedUtilities = mySelectedUtilities;
        _mySelectedProperties = mySelectedProperties;
        _mySelectedGODCards = mySelectedGODCards;
        _theirSelectedCash = theirSelectedCash;
        _theirSelectedStations = theirSelectedStations;
        _theirSelectedUtilities = theirSelectedUtilities;
        _theirSelectedProperties = theirSelectedProperties;
        _theirSelectedGODCard = theirSelectedGODCard;
        Initiating = initiate;
    }
}
