using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FoodopolyClientV2.Views;
using Microsoft.AspNetCore.SignalR.Client;
using GameClasses;
using ConnectivityLibrary.Services;
using CommunityToolkit.Mvvm.Input;
using SetClasses;
using PlayerClasses;
using FoodopolyClasses.Records;
using FoodopolyClasses.MultiplayerClasses;
using BoardClasses;
using Newtonsoft.Json;
using System.Web.Http;

namespace FoodopolyClientV2.ViewModels;

[QueryProperty("GameId", "GameId")]
[QueryProperty("Username", "Username")]
[QueryProperty("Password", "Password")]
public partial class GameViewModel:ObservableObject
{
    //public GameClass game { get; set; }
    private SignalRHubServices _hubServices;
    private GameClass? _gameClass;
    private HubConnection? _hubConnection;

    

    //public EventHandler Loaded;
    //public async Task OnLoad(object sender, EventArgs e)
    //{
    //    await StartConnecting();
    //}

    public GameViewModel(SignalRHubServices hubServices)
    {
        _hubServices = hubServices;
        msgs = new List<string>();
        serverRequestNum = 0;
        currentTurn = false;
        TurnMultiplayer = new TurnMultiplayerClass();

    }
    [ObservableProperty]
    string gameId;
    [ObservableProperty]
    string username;
    [ObservableProperty]
    string password;
    [ObservableProperty]
    List<string> msgs;
    [ObservableProperty]
    bool currentTurn;



    private int serverRequestNum;

    private TurnMultiplayerClass TurnMultiplayer;
    //refenced by OnAppearing in code behind
    public async Task StartConnecting()
    {
        
        Dictionary<string, string> urlArgs = new Dictionary<string, string>();
        urlArgs.Add("username", Username); urlArgs.Add("password", Password); urlArgs.Add("gameId", GameId);
        var connectionAndMessage = await _hubServices.HubBuilder("https://localhost:32772" +
            "/connected/game", urlArgs);
        if (connectionAndMessage.hubConnection == null)
        {
            await App.Current.MainPage.DisplayAlert("Alert", connectionAndMessage.msg, "OK");
            await Shell.Current.GoToAsync(nameof(GameJoinPage));
            return;
        }
        _hubConnection = connectionAndMessage.hubConnection;
        



        //hub method to recieve the fame object from server when joining
        _hubConnection.On<GameClass>("RecieveGame", async (game) =>
        {
            await Task.Run(() =>
            {
                _gameClass = game;

                //makes a copy of the parameters of this object to a local copy here
                TurnMultiplayer.turnMethodCount = game.TurnMultiplayer.turnMethodCount;
                TurnMultiplayer.turnMsgCount = game.TurnMultiplayer.turnMsgCount;

                //Adjust Current turn object
                if (game.CurrentTurnPlayer.Name == Username)
                {
                    CurrentTurn = true;
                }
                else
                {
                    CurrentTurn = false;
                }
            });
            
            
        });

        ////Manually deserialise game object because above wasn't working
        //_hubConnection.On<string>("RecieveGame2", async (gameJsonString) =>
        //{
        //    await Task.Run(() =>
        //    {
        //        GameClass game = JsonConvert.DeserializeObject<GameClass>(gameJsonString);
        //        _gameClass = game;

        //        //makes a copy of the parameters of this object to a local copy here
        //        Turn.turnMethodCount = game.Turn.turnMethodCount;
        //        Turn.turnMsgCount = game.Turn.turnMsgCount;

        //        //Adjust Current turn object
        //        if (game.CurrentTurnPlayer.Name == Username)
        //        {
        //            CurrentTurn = true;
        //        }
        //        else
        //        {
        //            CurrentTurn = false;
        //        }
        //    });


        //});

        //hub method to recieve msgs from server
        _hubConnection.On<string>("RecieveMessage", (msg) =>
        {
            Msgs.Add(msg);
        });

        //hub method that calls a buy
        _hubConnection.On<int, string, int>("Bought", async (methodCount, buyUsername, buyPlayerPos) =>
        {
            //Do something with method count
            if (_gameClass.CurrentTurnPlayer.Name != buyUsername || _gameClass.CurrentTurnPlayer.PlayerPos != buyPlayerPos)
            {
                await ErrorSendBack("Not Synced. Please Reconnect.");
                return;
            }

            //call buy here locally
            string msg = _gameClass.CurrentTurnPlayer.BuyPlayer(_gameClass);
            Msgs.Add(msg);
        });

        _hubConnection.On<(int,GameClass)>("TurnStart", (turnNum) =>
        {
            
        });

        _hubConnection.On<(string Msg, bool Double, bool Diet, int TotalRoll), int>("RecieveStandardDiceRoll", async (msgAndDoubleAndJailAndTotal,methodCount) =>
        {
            //Do something with methodCount

            await Task.Run(async () =>
            {
                //Add Msg To List
                Msgs.Add(msgAndDoubleAndJailAndTotal.Msg);

                (string DoTask, string Result) taskAndMsg;


                if (msgAndDoubleAndJailAndTotal.Diet)
                {
                    _gameClass.goOnADiet.GoOnADietMethod(_gameClass.CurrentTurnPlayer, _gameClass);
                    return;
                }
                else
                {
                    //Moves the Player appropriately, ignore msg and forward, already accounted for
                    _gameClass.CurrentTurnPlayer.FullChangePlayerPos(msgAndDoubleAndJailAndTotal.TotalRoll);

                    //Calls the appropriate landevent-done Independently on server and client side, see if it works
                    taskAndMsg = await _gameClass.CurrentTurnPlayer.LandEventAsync(_gameClass);
                    //Add msg to list if appropriate (list is console in game)
                    if (!string.IsNullOrEmpty(taskAndMsg.Result))
                    {
                        Msgs.Add(taskAndMsg.Result);
                    }
                    
                    if(!msgAndDoubleAndJailAndTotal.Double)
                    {
                        //ends Roll Event
                        _gameClass.Turn.RollEventDone = true;
                    }
                    
                }

                if (taskAndMsg.Result == "CanBuy")
                {
                    //Allow To Buy
                }
                if (!_gameClass.Turn.RollEventDone)
                {
                    //Allow Another Roll
                }

            });

            
        });
        
        

        try
        {
            await _hubConnection.StartAsync();
            serverRequestNum = 0;
        }
        catch (Exception ex)
        {
            await ErrorSendBack(ex.Message);
        }
        //foreach (KeyValuePair<string, Set> entry in _gameClass.setsDict)
        //{

        //}
    }

    //Method to call by methods in this class if error occurs
    private async Task ErrorSendBack(string msg)
    {
        await App.Current.MainPage.DisplayAlert("Could Not Connect", msg, "OK");
        await Shell.Current.GoToAsync($"{nameof(GameJoinPage)}?Username={Username}&Password={Password}");
    }

    //Add Msgs
    



    //Refrenced by ondisappearing in code behind
    public async Task Disconnecting()
    {
        await _hubConnection.DisposeAsync();
    }
    //public IDeviceDisplay deviceDisplay;


    //Command that is triggered by RollDice Button
    [RelayCommand]
    async Task RollDice()
    {
        //Disable Button And Commands Here


        PlayerAuthorisationRecord player = new PlayerAuthorisationRecord(Username,Password);
        try
        {
            //Problem Here!--Believe fixed by making a local copy of turn class from the game class sent
            TurnMultiplayer.turnMethodCount++;
            await _hubConnection.InvokeAsync("CallStandardRollDiceEvent", player, TurnMultiplayer.turnMethodCount);
        }
        catch (Exception ex)
        {
            await ErrorSendBack(ex.Message);
        }
        //await Task.Run(() =>
        //{
        //    player.
        //});
    }


    //Command that is triggered by Buy Button
    [RelayCommand]
    async Task Buy()
    {
        //Disable Button And Commands Here


        //Validation
        await Task.Run(async () =>
        {
            int Price = 1000; //To satisfy error, look into later
            if (_gameClass.CurrentTurnPlayer.Name != Username)
            {
                await ErrorSendBack("Please Reconnect");
                return;
            }
            int pos = _gameClass.CurrentTurnPlayer.PlayerPos;
           
            //using modulo here
            if (pos%10 == 5)
            {
                Station station = _gameClass.stations.Properties.First(station => station.BoardPosition == pos);
                if (station.Owned) 
                {
                    await ErrorSendBack("Please Reconnect");
                    return;
                }
                Price = station.Price;
            }
            if (pos == 12 || pos == 28)
            {
                Utility utility = _gameClass.utilities.Properties.First(utility => utility.BoardPosition == pos);
                if (utility.Owned)
                {
                    await ErrorSendBack("Please Reconnect");
                    return;
                }
                Price = utility.Price;
            }
            else
            {
                try
                {
                    
                    foreach (KeyValuePair<string, SetProp> keyValuePair in _gameClass.setsPropDict)
                    {
                        Property selectedProp;
                        foreach (Property property in keyValuePair.Value.Properties)
                        {
                            if (property.BoardPosition == pos)
                            {
                                selectedProp = property;
                                if (selectedProp.Owned) 
                                {
                                    await ErrorSendBack("Please Reconnect");
                                    return;
                                }
                                Price = selectedProp.Price;
                            }
                            
                        }
                    }
                }
                catch (Exception ex)
                {
                    await ErrorSendBack(ex.Message);
                    return;
                }
            }
            if (Price>_gameClass.CurrentTurnPlayer.Cash)
            {
                await ErrorSendBack("Not Enough Cash. Error. Please Reconnect");
                return;
            }

            //Implementation
            PlayerAuthorisationRecord player = new PlayerAuthorisationRecord(Username, Password);

            await _hubConnection.InvokeAsync("Buy", player, TurnMultiplayer.turnMethodCount, _gameClass.CurrentTurnPlayer.PlayerPos);
        });
        
        
    }



    async Task<PlayerClass> IdentifyPlayer()
    {

        PlayerClass player = _gameClass.PlayerList.First<PlayerClass>((player) => (player.Name == Username));
        return player;
    }


}


//public class DeviceInfo : IDeviceDisplay
//{
//    public DeviceInfo()
//    {

//    }
//    public bool KeepScreenOn { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

//    public DisplayInfo MainDisplayInfo => throw new NotImplementedException();

//    public event EventHandler<DisplayInfoChangedEventArgs> MainDisplayInfoChanged;

//    public void OnMainDisplayInfoChanged()
//    {
//        MainDisplayInfoChanged?.Invoke(this, new DisplayInfoChangedEventArgs);
//    }
//}
