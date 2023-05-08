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
using FoodopolyClientV2.Records;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using Microsoft.Maui.Graphics.Converters;
using System.ComponentModel;
using Microsoft.Maui.ApplicationModel;
using FoodopolyClientV2.Views.Popups;
using CommunityToolkit.Maui.Views;

namespace FoodopolyClientV2.ViewModels;

[QueryProperty("GameId", "GameId")]
[QueryProperty("Username", "Username")]
[QueryProperty("Password", "Password")]
public partial class GameViewModel:ObservableObject
{
    //public GameClass game { get; set; }
    private SignalRHubServices _hubServices;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanRoll))]
    [NotifyPropertyChangedFor(nameof(CanEnd))]
    [NotifyPropertyChangedFor(nameof(CurrentTurn))]
    private GameClass? _gameClass;

    private void ManualNotifyGameClassChanged()
    {
        OnPropertyChanged(global::CommunityToolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedArgs.CanRoll);
        OnPropertyChanged(global::CommunityToolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedArgs.CanEnd);
        OnPropertyChanged(global::CommunityToolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedArgs.CurrentTurn);
    }

    private HubConnection? _hubConnection;

    



    //Props dependent on gameClass
    public bool CanRoll
    {
        get
        {
            if (GameClass == null || !CurrentTurn)
            {
                return false;

            }
            return (!GameClass.Turn.RollEventDone);
        }
    }
    public bool CanEnd
    {
        get
        {
            if (GameClass == null || !CurrentTurn)
            {
                return false;

            }
            //Need Adjustment!!!!!
            return (GameClass.Turn.RollEventDone);
        }
    }
    public bool CurrentTurn
    {
        get
        {
            if (GameClass == null)
            {
                return false;
            }
            if (GameClass.CurrentTurnPlayer.Name == Username)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    

    [ObservableProperty]
    public bool canBuy = false;
    //public EventHandler Loaded;
    //public async Task OnLoad(object sender, EventArgs e)
    //{
    //    await StartConnecting();
    //}
    //public List<BoardSpaceViewRecord> boardSpaces;
    [ObservableProperty]
    public ObservableCollection<BoardSpaceViewRecord> boardSpaces = new ObservableCollection<BoardSpaceViewRecord> (Enumerable.Repeat(new BoardSpaceViewRecord("name",1000,"set", Colors.White),40).ToArray());
    //[ObservableProperty]
    //public BoardSpaceViewRecord[] boardSpaces = Enumerable.Repeat(new BoardSpaceViewRecord("name",1000,"set"),40).ToArray();
    //[ObservableProperty]
    //public BoardSpaceViewRecord testBoardSpace = new BoardSpaceViewRecord("name", 1000, "set");
    public GameViewModel(SignalRHubServices hubServices)
    {
        _hubServices = hubServices;
        serverRequestNum = 0;
        TurnMultiplayer = new TurnMultiplayerClass();

        //Test
        //TestBoardSpace = new BoardSpaceViewRecord("Test1", 1, "Brown");

        //implementing BoardSpaceViewRecords
        //BoardSpaces = new BoardSpaceViewRecord[40];
        XElement root = XElement.Load("Properties.xml");
        IEnumerable<XElement> sets = root.Elements("set");

        ColorTypeConverter colorTypeConverter = new ColorTypeConverter();
        foreach (XElement set in sets)
        {
            string setOrType = (string)set.Attribute("set");
            
            Color colour;
            if (setOrType == "Stations" ||setOrType == "Utilities")
            {
                colour = (Color)colorTypeConverter.ConvertFromInvariantString("White");
            }
            else
            {
                colour = (Color)colorTypeConverter.ConvertFromInvariantString(String.Concat(setOrType.Where(c => !Char.IsWhiteSpace(c))));
            }

            IEnumerable<XElement> properties = set.Elements("property");
            foreach (XElement property in properties)
            {
                int boardPosition = (int)property.Element("boardPosition");
                BoardSpaceViewRecord boardSpace = new BoardSpaceViewRecord((string)property.Element("name"), boardPosition, setOrType, colour);
                BoardSpaces[boardPosition] = boardSpace;
            }

            
        }
        foreach (int n in new int[] { 7, 22, 36 })
        {
            BoardSpaceViewRecord boardSpaceChance = new BoardSpaceViewRecord("Chance", n, "Chance", (Color)colorTypeConverter.ConvertFromInvariantString("White"));
            BoardSpaces[n] = boardSpaceChance;
        }

        foreach (int n in new int[] { 2, 17, 33 })
        {
            BoardSpaceViewRecord boardSpaceCChest = new BoardSpaceViewRecord("CChest", n, "CChest", (Color)colorTypeConverter.ConvertFromInvariantString("White"));
            BoardSpaces[n] = boardSpaceCChest;
        }

        //Fines
        BoardSpaces[4] = new BoardSpaceViewRecord("FoodTax", 4, "Fines", (Color)colorTypeConverter.ConvertFromInvariantString("White"));
        BoardSpaces[38] = new BoardSpaceViewRecord("FoodWasteTax", 38, "Fines", (Color)colorTypeConverter.ConvertFromInvariantString("White"));

        //OddOnes
        BoardSpaces[0] = new BoardSpaceViewRecord("Start", 0, "Start", (Color)colorTypeConverter.ConvertFromInvariantString("White"));
        BoardSpaces[10] = new BoardSpaceViewRecord("Dieting", 10, "Dieting", (Color)colorTypeConverter.ConvertFromInvariantString("White"));
        BoardSpaces[20] = new BoardSpaceViewRecord("Buffet", 20, "Buffet", (Color)colorTypeConverter.ConvertFromInvariantString("White"));
        BoardSpaces[30] = new BoardSpaceViewRecord("Go On A Diet", 30, "Go On A Diet", (Color)colorTypeConverter.ConvertFromInvariantString("White"));


    }
    [ObservableProperty]
    string gameId;
    [ObservableProperty]
    string username;
    [ObservableProperty]
    string password;
    [ObservableProperty]
    ObservableCollection<string> msgs = new ObservableCollection<string>();





    private int serverRequestNum;

    private TurnMultiplayerClass TurnMultiplayer;
    //refenced by OnAppearing in code behind
    public async Task StartConnecting()
    {
        
        Dictionary<string, string> urlArgs = new Dictionary<string, string>();
        urlArgs.Add("username", Username); urlArgs.Add("password", Password); urlArgs.Add("gameId", GameId);
        var connectionAndMessage = await _hubServices.HubBuilder("https://localhost:32776" +
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
                //Reset dependable view props
                CanBuy = false;

                GameClass = game;

                //makes a copy of the parameters of this object to a local copy here
                TurnMultiplayer.turnMethodCount = game.TurnMultiplayer.turnMethodCount;
                TurnMultiplayer.turnMsgCount = game.TurnMultiplayer.turnMsgCount;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdateAllPos();
                });
                
                //Adjust Current turn object
                //NOT NEEDED ANYMORE
            });
            
            
        });

        ////Manually deserialise game object because above wasn't working
        //_hubConnection.On<string>("RecieveGame2", async (gameJsonString) =>
        //{
        //    await Task.Run(() =>
        //    {
        //        GameClass game = JsonConvert.DeserializeObject<GameClass>(gameJsonString);
        //        GameClass = game;

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
            if (GameClass.CurrentTurnPlayer.Name != buyUsername || GameClass.CurrentTurnPlayer.PlayerPos != buyPlayerPos)
            {
                await ErrorSendBack("Not Synced. Please Reconnect.");
                return;
            }

            //call buy here locally
            string msg = GameClass.CurrentTurnPlayer.BuyPlayer(GameClass);
            CanBuy = false;
            ManualNotifyGameClassChanged();
            Msgs.Add(msg);
            
            //Adjust PlayerColor on Prop
            BoardSpaceViewRecord tempSpaceVar = BoardSpaces[GameClass.CurrentTurnPlayer.PlayerPos];
            //Remember CurrentTurnPos starts from 1
            tempSpaceVar.PlayerOwnerNum = GameClass.CurrentTurnPos;
        });

        _hubConnection.On<int , GameClass, string>("StartTurn", async (turnNum, game, msg) =>
        {
            //Do something with method count
            await Task.Run(() =>
            {
                //reset All view dependable props Props
                CanBuy = false;

                GameClass = game;

                //makes a copy of the parameters of this object to a local copy here
                TurnMultiplayer.turnMethodCount = game.TurnMultiplayer.turnMethodCount;
                TurnMultiplayer.turnMsgCount = game.TurnMultiplayer.turnMsgCount;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    UpdateAllPos();
                });
                //Adjust Current turn object
                //NOT NEEDED ANYMORE
                Msgs.Add(msg);
            });
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
                    GameClass.goOnADiet.GoOnADietMethod(GameClass.CurrentTurnPlayer, GameClass);
                    ManualNotifyGameClassChanged();
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        UpdatePos(GameClass.CurrentTurnPos);
                    });
                    
                    return;
                }
                else
                {
                    //Moves the Player appropriately, ignore msg and forward, already accounted for
                    GameClass.CurrentTurnPlayer.FullChangePlayerPos(msgAndDoubleAndJailAndTotal.TotalRoll);
                    ManualNotifyGameClassChanged();
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        UpdatePos(GameClass.CurrentTurnPos);
                    });

                    //Calls the appropriate landevent-done Independently on server and client side, see if it works
                    taskAndMsg = await GameClass.CurrentTurnPlayer.LandEventAsync(GameClass);
                    ManualNotifyGameClassChanged();
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        UpdatePos(GameClass.CurrentTurnPos);
                    });
                    //Add msg to list if appropriate (list is console in game)
                    if (!string.IsNullOrEmpty(taskAndMsg.Result))
                    {
                        Msgs.Add(taskAndMsg.Result);
                    }
                    
                    if(!msgAndDoubleAndJailAndTotal.Double)
                    {
                        //ends Roll Event

                        GameClass.Turn.RollEventDone = true;
                        ManualNotifyGameClassChanged();

                    }
                    
                }

                if (taskAndMsg.DoTask == "CanBuy" && CurrentTurn)
                {
                    //Allow To Buy
                    CanBuy = true;
                }
                if (!GameClass.Turn.RollEventDone)
                {
                    //Allow Another Roll
                    //Should already be covered by property
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
        //foreach (KeyValuePair<string, Set> entry in GameClass.setsDict)
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

    [RelayCommand]
    async Task EndTurn()
    {
        //validation
        if (!CurrentTurn || !CanEnd)
        {
            await ErrorSendBack("Please Reconnect!");
        }

        //Implementation
        PlayerAuthorisationRecord player = new PlayerAuthorisationRecord(Username, Password);

        await _hubConnection.InvokeAsync("EndTurn", player, TurnMultiplayer.turnMethodCount);

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
            if (GameClass.CurrentTurnPlayer.Name != Username)
            {
                await ErrorSendBack("Please Reconnect");
                return;
            }
            int pos = GameClass.CurrentTurnPlayer.PlayerPos;
           
            //using modulo here
            if (pos%10 == 5)
            {
                Station station = GameClass.stations.Properties.First(station => station.BoardPosition == pos);
                if (station.Owned) 
                {
                    await ErrorSendBack("Please Reconnect");
                    return;
                }
                Price = station.Price;
            }
            if (pos == 12 || pos == 28)
            {
                Utility utility = GameClass.utilities.Properties.First(utility => utility.BoardPosition == pos);
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
                    
                    foreach (KeyValuePair<string, SetProp> keyValuePair in GameClass.setsPropDict)
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
            if (Price>GameClass.CurrentTurnPlayer.Cash)
            {
                await ErrorSendBack("Not Enough Cash. Error. Please Reconnect");
                return;
            }

            //Implementation
            PlayerAuthorisationRecord player = new PlayerAuthorisationRecord(Username, Password);

            await _hubConnection.InvokeAsync("Buy", player, TurnMultiplayer.turnMethodCount, GameClass.CurrentTurnPlayer.PlayerPos);
        });
        
        
    }

    //public void AddMsg(string msg)
    //{
    //    Msgs.Add(msg);
    //    OnPropertyChanged(global::CommunityToolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedArgs.Msgs);

    //}



    async Task<PlayerClass> IdentifyPlayer()
    {

        PlayerClass player = GameClass.PlayerList.First<PlayerClass>((player) => (player.Name == Username));
        return player;
    }


    private void UpdatePos(int playerPos)
    {
        if (playerPos == 1)
        {
            foreach (BoardSpaceViewRecord viewSpace in BoardSpaces)
            {
                viewSpace.P1Here = false;
            }
            BoardSpaces[GameClass.PlayerList[0].PlayerPos].P1Here = true;
        }
        else if (playerPos == 2)
        {
            foreach (BoardSpaceViewRecord viewSpace in BoardSpaces)
            {
                viewSpace.P2Here = false;
            }
            BoardSpaces[GameClass.PlayerList[1].PlayerPos].P2Here = true;
        }
        else if (playerPos == 3)
        {
            foreach (BoardSpaceViewRecord viewSpace in BoardSpaces)
            {
                viewSpace.P3Here = false;
            }
            BoardSpaces[GameClass.PlayerList[2].PlayerPos].P3Here = true;
        }
        else if (playerPos == 4)
        {
            foreach (BoardSpaceViewRecord viewSpace in BoardSpaces)
            {
                viewSpace.P4Here = false;
            }
            BoardSpaces[GameClass.PlayerList[3].PlayerPos].P4Here = true;
        }
        else
        {
            ErrorSendBack("Error, Please Try Again"); //Should be fine not awaiting
            return;
        }
    }

    private void UpdateAllPos()
    {
        int lenPL = GameClass.PlayerList.Count;
        foreach (int n in Enumerable.Range(1, lenPL))
        {
            UpdatePos(n);
        }
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
