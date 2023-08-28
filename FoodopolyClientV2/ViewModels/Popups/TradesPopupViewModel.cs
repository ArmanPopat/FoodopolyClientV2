using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodopolyClientV2.ViewModels.Popups;

public partial class TradesPopupViewModel:ObservableObject
{
    public int IncomingTrades { get; }
    public int OutgoingTrades { get; }

    public GameViewModel GameViewModel { get; }

    [RelayCommand]
    async Task ShowIncomingOutgoingTrades(string incomingOrOutgoing)
    {
        await GameViewModel.SeeIncomingOutgoingTrades(incomingOrOutgoing);
    }

    public TradesPopupViewModel(int incomingTrades, int outgoingTrades, GameViewModel gameViewModel)
    {
        IncomingTrades = incomingTrades;
        OutgoingTrades = outgoingTrades;
        GameViewModel = gameViewModel;
    }


}
