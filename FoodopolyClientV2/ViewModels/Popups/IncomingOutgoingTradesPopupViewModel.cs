using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodopolyClasses.Records;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodopolyClientV2.ViewModels.Popups;

public partial class IncomingOutgoingTradesPopupViewModel : ObservableObject
{

    public string IncomingOrOutgoing { get; }
    public ObservableCollection<InitiateTradeRecord> TradeRecords { get; }

    public GameViewModel GameViewModel { get; }

    [RelayCommand]
    async Task SeeSpecificTrade(InitiateTradeRecord tradeRecord)
    {
        await GameViewModel.SeeSpecificTrade(tradeRecord, IncomingOrOutgoing);
    }
    public IncomingOutgoingTradesPopupViewModel(List<InitiateTradeRecord> tradeRecords, string incomingOrOutgoing, GameViewModel gameViewModel) 
    {
        TradeRecords = new ObservableCollection<InitiateTradeRecord>(tradeRecords);
        IncomingOrOutgoing = incomingOrOutgoing;
        GameViewModel = gameViewModel;
    }
}
