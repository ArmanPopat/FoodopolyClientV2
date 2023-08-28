using BoardClasses;
using CommunityToolkit.Mvvm.ComponentModel;
using FoodopolyClasses.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodopolyClientV2.ViewModels.Popups;

public partial class SeeSpecififcTradePopupViewModel : ObservableObject
{
    public string MyName { get;}
    public int MySelectedCash { get; }
    public List<Station> MyStations { get; }
    public List<Utility> MyUtilities { get; }
    public List<Property> MyProperties { get; }
    public int MySelectedGODCards { get; }

    public string TheirName { get; }
    public int TheirSelectedCash { get; }
    public List<Station> TheirStations { get; }
    public List<Utility> TheirUtilities { get; }
    public List<Property> TheirProperties { get; }
    public int TheirSelectedGodCards { get; }

    private InitiateTradeRecord _tradeRecord;

    public bool Outgoing { get; }
    public bool Incoming { get; }

    public GameViewModel GameViewModel { get; }
    public SeeSpecififcTradePopupViewModel(GameViewModel gameViewModel, string myName, int mySelectedCash, List<Station> myStations, List<Utility> myUtilities,
        List<Property> myProperties, int mySelectedGODCards, string theirName, int theirSelectedCash, List<Station> theirStation, List<Utility> theirUtilities,
        List<Property> theirProperties, int theirSelectedGodCards, InitiateTradeRecord tradeRecord, string incomingOrOutgoing)
    {
        MySelectedCash = mySelectedCash;
        MyStations = myStations;
        MyUtilities = myUtilities;
        MyProperties = myProperties;
        MySelectedGODCards = mySelectedGODCards;
        TheirSelectedCash = theirSelectedCash;
        TheirStations = theirStation;
        TheirUtilities = theirUtilities;
        TheirProperties = theirProperties;
        TheirSelectedGodCards = theirSelectedGodCards;
        GameViewModel = gameViewModel;
        MyName = myName;
        TheirName = theirName;
        _tradeRecord = tradeRecord;
        if (incomingOrOutgoing == "Incoming")
        {
            Incoming = true;
            Outgoing = false;
        }
        else if (incomingOrOutgoing == "Outgoing")
        {
            Incoming = false;
            Outgoing = true;
        }
        else
        {
            throw new ArgumentException($"{incomingOrOutgoing} not Incoming or Outgoing");
        }
    }
}
