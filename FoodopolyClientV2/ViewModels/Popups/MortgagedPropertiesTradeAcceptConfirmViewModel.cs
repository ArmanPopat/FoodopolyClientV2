using BoardClasses;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CoreText;
using FoodopolyClasses.PlayerClasses;
using FoodopolyClasses.Records;
using FoodopolyClientV2.Views.Popups.DataTypeClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodopolyClientV2.ViewModels.Popups;

public partial class MortgagedPropertiesTradeAcceptConfirmViewModel:ObservableObject
{
    public IEnumerable<MortgageTuple> ListOfMortagedOwned { get; }
    public List<MortgageTuple> ToUnmortgage { get; set; } = new List<MortgageTuple>();
    public GameViewModel ThisGameViewModel { get; }

    private InitiateTradeRecord _tradeRecord;

    public bool Accepting { get; }
    public bool Responding
    {
        get
        {
            return !Accepting;
        }
    }
    //private PlayerClass _localPlayer;
    //private int _mySelectedCash;
    //private List<Station> _mySelectedStations;
    //private List<Utility> _mySelectedUtilities;
    //private List<Property> _mySelectedProperties;
    //private int _mySelectedGODCards;
    //private PlayerClass _otherplayer;
    //private int _theirSelectedCash;
    //private List<Station> _theirSelectedStations;
    //private List<Utility> _theirSelectedUtilities;
    //private List<Property> _theirSelectedProperties;
    //private int _theirSelectedGODCard;

    //To determine whether it's being initiated or Confirmed


    //[RelayCommand]
    //async Task SendTrade()
    //{
    //    await ThisGameViewModel.SendTrade(_localPlayer, _mySelectedCash, _mySelectedStations, _mySelectedUtilities, _mySelectedProperties, _mySelectedGODCards,
    //        _otherplayer ,_theirSelectedCash, _theirSelectedStations, _theirSelectedUtilities, _theirSelectedProperties, _theirSelectedGODCard);
    //}

    private async Task GetMortPropsToDo()
    {

    }

    [RelayCommand]
    async Task RespondUnmortgageOrFeeTrade()
    {
        
    }


    [RelayCommand]
    async Task SendConfirmTrade()
    {
        var mortPropsToDo = await Task<List<(int BoardPosition, bool ToUnmortgage)>>.Run(() =>
        {
            List<(int BoardPosition, bool ToUnmortgage)> mortPropsToDo = new();
            List<int> toUnmortgageBP = ToUnmortgage.Select(o=>o.BoardPosition).ToList();
            foreach (var prop in ListOfMortagedOwned)
            {
                if (toUnmortgageBP.Any(o => o == prop.BoardPosition))
                {
                    mortPropsToDo.Add((prop.BoardPosition, true));
                }
                else
                {
                    mortPropsToDo.Add((prop.BoardPosition, false));
                }
                
            }
            return mortPropsToDo;
        });
        await ThisGameViewModel.SendConfirmTrade(_tradeRecord, mortPropsToDo);
    }
    public MortgagedPropertiesTradeAcceptConfirmViewModel(GameViewModel gameViewModel, List<Station> theirSelectedStations, List<Utility> theirSelectedUtilities,
        List<Property> theirSelectedProperties, InitiateTradeRecord tradeRecord, bool accepting = true) 
    {
        ListOfMortagedOwned = theirSelectedStations.FindAll(o => o.Mortgaged == true).Select(o => new MortgageTuple(o.Name,o.Price/2,o.BoardPosition)).ToList();
        ListOfMortagedOwned = ListOfMortagedOwned.Concat(theirSelectedUtilities.FindAll(o => o.Mortgaged == true).Select(o => new MortgageTuple(o.Name, o.Price / 2, o.BoardPosition)).ToList());
        ListOfMortagedOwned = ListOfMortagedOwned.Concat(theirSelectedProperties.FindAll(o => o.Mortgaged == true).Select(o => new MortgageTuple(o.Name, o.Price / 2, o.BoardPosition)).ToList());
        ThisGameViewModel = gameViewModel;
        _tradeRecord = tradeRecord;
        Accepting = accepting;
        //_localPlayer = localPlayer;
        //_mySelectedCash = mySelectedCash;
        //_mySelectedStations = mySelectedStations;
        //_mySelectedUtilities = mySelectedUtilities;
        //_mySelectedProperties = mySelectedProperties;
        //_mySelectedGODCards = mySelectedGODCards;
        //_theirSelectedCash = mySelectedCash;
        //_theirSelectedStations = theirSelectedStations;
        //_theirSelectedUtilities = theirSelectedUtilities;
        //_theirSelectedProperties = theirSelectedProperties;
        //_theirSelectedGODCard = theirSelectedGODCard;
    }
}
