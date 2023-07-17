using BoardClasses;
using CommunityToolkit.Mvvm.ComponentModel;
using FoodopolyClasses.PlayerClasses;
using GameClasses;

namespace FoodopolyClientV2.ViewModels.Popups;

public partial class PlayerPopupViewModel:ObservableObject
{
    public PlayerClass Player { get; }
    public List<Station> Stations { get; }
    public List<Utility> Utilities { get; }
    public List<Property> Properties { get; }

    public GameClass Game { get; }
    public bool IsMe { get; }
    public bool IsNotMe
    {
        get
        {
            return !IsMe;
        }
    }

    public int LiquidWorth {
        get
        {
            return Player.GetNetWorth(Game);
        }
    }
    public GameViewModel ThisGameViewModel { get; }
    public PlayerPopupViewModel(GameViewModel gameViewModel, PlayerClass player, List<Station> stations, List<Utility> utilities, List<Property> properties, bool isMe) 
    {
        Player = player;
        Stations = stations;
        Utilities = utilities;
        Properties = properties;
        Game = gameViewModel.GameClass;
        IsMe = isMe;
        ThisGameViewModel = gameViewModel;
    }
}
