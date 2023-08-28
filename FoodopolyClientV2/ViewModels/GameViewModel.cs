using CommunityToolkit.Mvvm.ComponentModel;
using FoodopolyClientV2.Views;
using Microsoft.AspNetCore.SignalR.Client;
using GameClasses;
using ConnectivityLibrary.Services;
using CommunityToolkit.Mvvm.Input;
using SetClasses;
using FoodopolyClasses.PlayerClasses;
using FoodopolyClasses.Records;
using FoodopolyClasses.MultiplayerClasses;
using BoardClasses;
using FoodopolyClientV2.Records;
using System.Xml.Linq;
using System.Collections.ObjectModel;
using Microsoft.Maui.Graphics.Converters;
using FoodopolyClientV2.Views.Popups;
using CommunityToolkit.Maui.Views;
using FoodopolyClientV2.ViewModels.Popups;
using System.Diagnostics;
using FoodopolyClasses.TradeClasses;

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
    [NotifyPropertyChangedFor(nameof(CanAfford))]
    [NotifyPropertyChangedFor(nameof(YourCash))]
    private GameClass? _gameClass;

    private void ManualNotifyGameClassChanged()
    {
        OnPropertyChanged(global::CommunityToolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedArgs.CanRoll);
        OnPropertyChanged(global::CommunityToolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedArgs.CanEnd);
        OnPropertyChanged(global::CommunityToolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedArgs.CurrentTurn);
        OnPropertyChanged(global::CommunityToolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedArgs.CanAfford);
        OnPropertyChanged(global::CommunityToolkit.Mvvm.ComponentModel.__Internals.__KnownINotifyPropertyChangedArgs.YourCash);
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
    public int YourCash
    {
        get
        {
            if (GameClass == null)
            {
                return 0;
            }
            try
            {
                return GameClass.PlayerList.First(o => o.Name == Username).Cash;
            }
            catch
            {
                ErrorSendBack("Error in game. Please Reconnect");
                return 0;
            }
            
        }
    }

    //Checks if can afford, will update with gameclass
    public bool CanAfford
    {
        get
        {
            if (!CurrentTurn)
            {
                return false;
            }
            var undesiderdPos = new int[] { 7, 22, 36, 2, 17, 33, 0, 10, 20, 30, 4, 38 };
            if (undesiderdPos.Any(o => o == GameClass.CurrentTurnPlayer.PlayerPos))
            {
                return false;
            }
            foreach (Station tempProp in GameClass.stations.Properties)
            {
                if (tempProp.BoardPosition == GameClass.CurrentTurnPlayer.PlayerPos)
                {
                    if (GameClass.CurrentTurnPlayer.Cash >= tempProp.Price)
                    {
                        return true;
                    }
                }
            }
            foreach (Utility tempProp in GameClass.utilities.Properties)
            {
                if (tempProp.BoardPosition == GameClass.CurrentTurnPlayer.PlayerPos)
                {
                    if (GameClass.CurrentTurnPlayer.Cash >= tempProp.Price)
                    {
                        return true;
                    }
                }
            }
            foreach (KeyValuePair<string, SetProp> keyValue in GameClass.setsPropDict)
            {
                foreach (Property tempProp in keyValue.Value.Properties)
                {
                    if (tempProp.BoardPosition == GameClass.CurrentTurnPlayer.PlayerPos)
                    {
                        if (GameClass.CurrentTurnPlayer.Cash >= tempProp.Price)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
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
                    UpdateAllViewRecord();
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
            
            //Adjust PlayerColor on Prop and Name
            BoardSpaceViewRecord tempSpaceVar = BoardSpaces[GameClass.CurrentTurnPlayer.PlayerPos];
            //Remember CurrentTurnPos starts from 1
            tempSpaceVar.PlayerOwnerNum = GameClass.CurrentTurnPos;
            tempSpaceVar.playerOwnerName = GameClass.CurrentTurnPlayer.Name;
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
                    UpdateAllViewRecord();
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
                //if (!GameClass.Turn.RollEventDone)
                //{
                //    //Allow Another Roll
                //    //Should already be covered by property
                //}

            });

            
        });

        //Called to upgrade property
        _hubConnection.On<int, int>("Upgrade", async (boardPosition, methodCount) =>
        {
            await Task.Run(async () =>
            {
                foreach (KeyValuePair<string, SetProp> keyValue in GameClass.setsPropDict)
                {
                    foreach (Property property in keyValue.Value.Properties)
                    {
                        if (property.BoardPosition == boardPosition)
                        {

                            //Checking
                            if (property.Owned && keyValue.Value.SetExclusivelyOwned)
                            {
                                if (property.NumOfUpgrades < 5)
                                {
                                    if (keyValue.Value.Properties.All(o => o.NumOfUpgrades >= property.NumOfUpgrades))
                                    {
                                        //Upgrade On Client Side and displays message
                                        Msgs.Add(property.Upgrade());
                                        return;
                                    }
                                }
                            }
                            await ErrorSendBack("Not Synced. Please Reconnect.");
                            return;
                        }
                    }
                }
            });

        });

        //Called to mortgage property=unfinished
        _hubConnection.On<int, int>("Mortgage", async (boardPosition, methodCount) =>
        {
            await Task.Run(async () =>
            {
                if (boardPosition == 12 || boardPosition == 28)
                {
                    Utility utility = GameClass.utilities.Properties.First(o =>  o.BoardPosition == boardPosition);
                    //Checking
                    if (utility.Owned)
                    {
                        Msgs.Add(utility.Mortgage());
                        return;
                        
                    }
                    await ErrorSendBack("Not Synced. Please Reconnect.");
                    return;
                }
                if (boardPosition == 5 || boardPosition == 15 || boardPosition == 25 || boardPosition == 35)
                {
                    Station station = GameClass.stations.Properties.First(o => o.BoardPosition == boardPosition);
                    //Checking
                    if (station.Owned)
                    {
                        Msgs.Add(station.Mortgage());
                        return;

                    }
                    await ErrorSendBack("Not Synced. Please Reconnect.");
                    return;
                }
                foreach (KeyValuePair<string, SetProp> keyValue in GameClass.setsPropDict)
                {
                    foreach (Property property in keyValue.Value.Properties)
                    {
                        if (property.BoardPosition == boardPosition)
                        {

                            //Checking
                            if (property.Owned)
                            {
                                if (property.NumOfUpgrades == 0)
                                {
                                    if (keyValue.Value.Properties.All(o => o.NumOfUpgrades == 0))
                                    {
                                        //Mortgage On Client Side and displays message
                                        Msgs.Add(property.Mortgage());
                                        return;
                                    }
                                }
                            }
                            await ErrorSendBack("Not Synced. Please Reconnect.");
                            return;
                        }
                    }
                }
            });

        });

        _hubConnection.On<int, string, int, InitiateTradeRecord, GameClass>("TradeReceived", (methodCount, initeePlayerUsername, recordKey, tradeRecord, game) =>
        {
            Task.Run(() =>
            {
                if (!(tradeRecord.otherPlayerName == Username))
                {
                    GameClass.TradeRecords.Add(recordKey, tradeRecord);
                    
                    Msgs.Add($"{initeePlayerUsername} has initiated a Trade with {tradeRecord.otherPlayerName}.");
                    return;
                }
                GameClass = game; //unsure why necessary, may remove
                //May had notfication here, Banner?
            });

        });
        _hubConnection.On<int, string, InitiateTradeRecord, GameClass>("TradeError", (methodCount, initeePlayerUsername, tradeRecord, game) =>
        {
            Task.Run(async () =>
            {
                if (!(tradeRecord.myName == Username))
                {
                    return;
                }
                GameClass = game; //resyncs game
                //May add notfication here, Banner? How to show Trade Error? Going to go for alert for now
                await App.Current.MainPage.DisplayAlert("Trade Error", "There has been an error with you trade, please try again.", "OK");
            });

        });
        _hubConnection.On<int, int, InitiateTradeRecord, List<(int BoardPosition, bool ToUnmortgage)>>("TradeConfirmed", (methodCount, tradeRecordKey, tradeRecordSent, mortPropsToDo) =>
        {
            InitiateTradeRecord tradeRecord;
            Task.Run(async () =>
            {
                try
                {
                    tradeRecord = GameClass.TradeRecords[tradeRecordKey];
                    if (tradeRecord != tradeRecordSent)
                    {
                        throw new InvalidDataException();
                    }
                    await Trade.AcceptTradeAsync(tradeRecordKey, tradeRecord, GameClass, mortPropsToDo);
                    Msgs.Add($"{tradeRecord.otherPlayerName} has confirmed a Trade with {tradeRecord.myName}.");
                    if (Username == tradeRecord.myName)
                    {
                        (List<Station> selectedStations, List<Utility> selectedUtilities, List<Property> selectedProperties) = 
                        await Trade.IdentifyTheOwnedStuff(tradeRecord.theirSelectedStationsBoardPos, tradeRecord.theirSelectedUtilitiesBoardPos, tradeRecord.theirSelectedPropertiesBoardPos, GameClass);
                        //ToUnmortgageHERE
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            var popupViewModel = new MortgagedPropertiesTradeAcceptConfirmViewModel(this, selectedStations, selectedUtilities, selectedProperties, tradeRecord, false);
                            var popup = new MortgagedPropertiesTradeAcceptConfirmPopup(popupViewModel);
                            Page.ShowPopup(popup);
                        });
                    }
                }
                catch
                {
                    await ErrorSendBack("Error in trade receipt, please rejoin.");
                    return;
                }
            });
        });

        _hubConnection.On("TradeMortgageFeesDue", async () =>
        {
            await Task.Run(async () =>
            {
                PlayerClass playerClass = await IdentifyPlayer();
                InitiateTradeRecord tradeRecord = playerClass.MorgatgeFeesNotPaidOrUnMortgaged[0];
                (List<Station> selectedStations, List<Utility> selectedUtilities, List<Property> selectedProperties) =
                        await Trade.IdentifyTheOwnedStuff(tradeRecord.theirSelectedStationsBoardPos, tradeRecord.theirSelectedUtilitiesBoardPos, tradeRecord.theirSelectedPropertiesBoardPos, GameClass);
                //ToUnmortgageHERE
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    var popupViewModel = new MortgagedPropertiesTradeAcceptConfirmViewModel(this, selectedStations, selectedUtilities, selectedProperties, tradeRecord, false);
                    var popup = new MortgagedPropertiesTradeAcceptConfirmPopup(popupViewModel);
                    Page.ShowPopup(popup);
                });
            });
        });

        _hubConnection.On<int, InitiateTradeRecord, List<(int BoardPosition, bool ToUnmortgage)>>("UnmortgageOrFeeTradeResponded", async (methodCount, tradeRecordSent, mortPropsToDo) =>
        {
            await Task.Run(async () =>
            {
                try
                {
                    PlayerClass playerClass = await IdentifyPlayer(tradeRecordSent.myName);
                    (List<Station> selectedStations, List<Utility> selectedUtilities, List<Property> selectedProperties) = await Trade.IdentifyTheOwnedStuff(tradeRecordSent.theirSelectedStationsBoardPos,
                        tradeRecordSent.theirSelectedUtilitiesBoardPos, tradeRecordSent.theirSelectedPropertiesBoardPos, GameClass);
                    playerClass.MorgatgeFeesNotPaidOrUnMortgaged.Remove(tradeRecordSent);
                    await Trade.UnMortgageOrPayFee(playerClass, selectedStations, selectedUtilities, selectedProperties, mortPropsToDo);
                    try
                    {
                        playerClass.MorgatgeFeesNotPaidOrUnMortgaged.Remove(tradeRecordSent);
                    }
                    catch
                    {}
                }
                catch
                {
                    await ErrorSendBack("Error in trade receipt, please rejoin.");
                    return;
                }
            });
        });
        _hubConnection.On<int, int, InitiateTradeRecord, GameClass>("TradeNoLongerValid", async (methodCount,tradeRecordkey, tradeRecordSent, game) =>
        {
            await Task.Run(async () =>
            {
                GameClass = game; //resyncs game
                //May add notfication here, Banner? How to show Trade Error? Going to go for alert for now
                await App.Current.MainPage.DisplayAlert("Trade No Longer Valid", "The trade is no longer valid.", "OK");
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


    [RelayCommand]
    async Task InitiateTrade(PlayerClass otherPlayer)
    {
        //Make sure trying not to initiate trade with self
        if (otherPlayer.Name == Username)
        {
            Debug.WriteLine("Error:Trying to trade with self");
            await ErrorSendBack("Please Reload.");
            return;
        }
        //Local PlayerWill be assigned here
        PlayerClass me;
        await Task<PlayerClass>.Run(async () =>
        {
            try
            {
                me = GameClass.PlayerList.First(o => o.Name == Username);
            }
            catch
            {
                Debug.WriteLine("Error:Haven't found local player in player list");
                await ErrorSendBack("Please Resync.");
                return;
            }
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var popupViewModel = new InitiateTradePopupViewModel(this ,me,otherPlayer);
                var popup = new IntiateTradePopup(popupViewModel);
                Page.ShowPopup(popup);
            });
        });
    }

    //Popup command to show my player info
    [RelayCommand]
    async Task ShowPlayerModal(PlayerClass player)
    {
        List<Station> stations = new List<Station>();
        List<Utility> utilities = new List<Utility>();
        List<Property> properties = new List<Property>();
        bool isMe = false;
        await Task.Run(() => 
        {
            var ownedStationsUtilsAndProps = player.GetOwnedPropsAndStuff(GameClass);
            stations = ownedStationsUtilsAndProps.Stations;
            utilities = ownedStationsUtilsAndProps.Utilities;
            properties = ownedStationsUtilsAndProps.Properties;
        });
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var popupViewModel = new PlayerPopupViewModel(this, player, stations, utilities, properties, isMe);
            var popup = new PlayerPopup(popupViewModel);
            Page.ShowPopup(popup);
        });
    }


    static Page Page => Application.Current?.MainPage ?? throw new NullReferenceException();

    //Command to show popup for property
    [RelayCommand]
    async Task ShowPropertyModal(int boardPos)
    {
        if (GameClass == null)
        {
            await ErrorSendBack("Game Not Synced");
            return;
        }

        await Task.Run(() =>
        {
            string nameHere = string.Empty;
            int rent = 0;
            int rentL1 = 0;
            int rentL2 = 0;
            int rentL3 = 0;
            int rentL4 = 0;
            int rentL5 = 0;
            string typeOrSet = string.Empty;
            int theBoardPos = 1000;
            string ownerName = string.Empty;

            //Helps show which should be bolded, i.e. which rent players pay
            bool setExclusivelyOwned = false;
            int rentLevel = 0;

            //Doesn't check if side props are upgraded or enough cash
            bool upgradablePotential = false;

            //ToDo, i.e if button is enables if other set props are upgraded enough, enough cash etc. DIFFERENT TO upgradablePotenial which just ask if possible
            bool upgradeEnabled = false;

            //MortageStuff checks
            bool mortgagePotenial = false;
            bool mortgaged = false;
            if (boardPos == 7 || boardPos == 22 || boardPos == 36)
            {
                nameHere = "Chance";
                theBoardPos = boardPos;
                //return;
            }


            else if (boardPos == 2 || boardPos == 17 || boardPos == 33)
            {
                nameHere = "Chance";
                theBoardPos = boardPos;
                //return;
            }

            else if (boardPos == 0)
            {
                nameHere = "Start;";
                theBoardPos = boardPos;
            }
            else if (boardPos == 10)
            {
                nameHere = "Dieting";
                theBoardPos = boardPos;
            }
            else if (boardPos == 20)
            {
                nameHere = "Buffet";
                theBoardPos = boardPos;
            }
            else if (boardPos == 30)
            {
                nameHere = "Go On A Diet";
                theBoardPos = boardPos;
            }
            else if (boardPos == 4)
            {
                nameHere = "FoodTax";
                theBoardPos = boardPos;
            }
            else if (boardPos == 38)
            {
                nameHere = "FoodWasteTax";
                theBoardPos = boardPos;
            }
            else if (boardPos == 5 || boardPos == 15 || boardPos == 25 || boardPos == 35 )
            {
                Station station = GameClass.stations.Properties.First<Station>(x => x.BoardPosition == boardPos);
                nameHere = station.Name;
                rent = station.RentL1;
                rentL1 = station.RentL2;
                rentL2 = station.RentL3;
                rentL3 = station.RentL4;
                typeOrSet = "Stations";
                theBoardPos = boardPos;
                if(station.Owned)
                {
                    ownerName = station.Owner.Name;

                    //Gets rent level, -1 to start from 0
                    rentLevel = GameClass.stations.Properties.Where(o => o.Owner == station.Owner).Count() - 1;
                    //Shows if set exclusinvely owned
                    setExclusivelyOwned = GameClass.stations.SetExclusivelyOwned;
                }

                if(station.Owned)
                {
                    if (station.Owner.Name == Username && station.Mortgaged == false)
                    {
                        mortgagePotenial = true;
                    }
                }

                mortgaged = station.Mortgaged;
            }
            else if (boardPos == 12 || boardPos == 28)
            {
                Utility utility = GameClass.utilities.Properties.First<Utility>(x => x.BoardPosition == boardPos);
                nameHere = utility.Name;
                rent = utility.RentL1;
                rentL1 = utility.RentL2;
                typeOrSet = "Utilities";
                theBoardPos = boardPos;
                if (utility.Owned)
                {
                    ownerName = utility.Owner.Name;
                    //Gets rent level, -1 to start from 0
                    rentLevel = GameClass.utilities.Properties.Where(o => o.Owner == utility.Owner).Count() - 1;
                    //Shows if set exclusinvely owned
                    setExclusivelyOwned = GameClass.utilities.SetExclusivelyOwned;
                }
                if (utility.Owned)
                {
                    if (utility.Owner.Name == Username && utility.Mortgaged == false)
                    {
                        mortgagePotenial = true;
                    }
                }
                mortgaged = utility.Mortgaged;
            }
            else
            {
                foreach (KeyValuePair<string, SetProp> keyValue in GameClass.setsPropDict)
                {
                    foreach (Property property in keyValue.Value.Properties)
                    {
                        if (property.BoardPosition == boardPos)
                        {
                            nameHere = property.Name;
                            rent = property.Rent;
                            rentL1 = property.RentL1;
                            rentL2 = property.RentL2;
                            rentL3 = property.RentL3;
                            rentL4 = property.RentL4;
                            rentL5 = property.RentL5;
                            typeOrSet = property.SetName;
                            theBoardPos = boardPos;

                            if (property.Owned)
                            {
                                ownerName = property.Owner.Name;
                                //Gets rent level, -1 to start from 0
                                rentLevel = keyValue.Value.Properties.Where(o => o.Owner == property.Owner).Count() - 1;
                                //Shows if set exclusinvely owned
                                setExclusivelyOwned = keyValue.Value.SetExclusivelyOwned;
                            }

                            //Checking Upgradable Property
                            if (property.Owned && keyValue.Value.SetExclusivelyOwned)
                            {
                                if (property.Owner.Name == Username)
                                {
                                    if (property.NumOfUpgrades<5)
                                    {
                                        upgradablePotential = true;
                                        if (property.Owner.Cash >= property.UpgradeCost)
                                        {
                                            if(keyValue.Value.Properties.All(o => o.NumOfUpgrades>=property.NumOfUpgrades))   //check here
                                            {
                                                upgradeEnabled = true;
                                            }
                                        }
                                    }
                                }
                            }
                            if (property.Owned)
                            {
                                if (property.Owner.Name == Username && property.Mortgaged == false)
                                {
                                    mortgagePotenial = true;
                                }
                            }
                            mortgaged = property.Mortgaged;
                        }
                    }
                }
            }
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var popupViewModel = new PropertyPopupViewModel(this ,nameHere, theBoardPos, rent, rentL1, rentL2, rentL3, rentL4, rentL5, typeOrSet, upgradablePotential,
                    upgradeEnabled, ownerName, rentLevel, setExclusivelyOwned, mortgagePotenial, mortgaged);
                var popup = new PropertyPopup(popupViewModel);
                Page.ShowPopup(popup);
            });

        });
    }

    [RelayCommand]
    async Task Upgrade(PropertyPopupViewModel popVM)
    {
        int boardPosition = popVM.BoardPos;
        var undesiderdPos = new int[] { 7, 22, 36, 2, 17, 33, 0, 10, 20, 30, 4, 38, 5, 15, 25, 35, 12, 28};
        if (undesiderdPos.Any(o => o == boardPosition))
        {
            await ErrorSendBack("Error In App, Please Reload.");
            return;
        }
        System.Diagnostics.Debug.WriteLine("IT WorKSSSS");

        //More Validation
        //ToConsider  Skipping Validation at the moment as handled in button showing. Will see.


        //Implementation
        PlayerAuthorisationRecord player = new PlayerAuthorisationRecord(Username, Password);
        await _hubConnection.InvokeAsync("Upgrade", player, TurnMultiplayer.turnMethodCount, boardPosition);

        //Temp measure to increase digit of numofupgrade on popup
        popVM.RentLevel += 1;
        //Maybe add validation here in case rent level goes above 5?
    }

    //Command To mortgage property, most validation is done before as to whether the button is visible 
    [RelayCommand]
    async Task Mortgage(PropertyPopupViewModel popVM)
    {
        int boardPosition = popVM.BoardPos;
        var undesiderdPos = new int[] { 7, 22, 36, 2, 17, 33, 0, 10, 20, 30, 4, 38};
        if (undesiderdPos.Any(o => o == boardPosition))
        {
            await ErrorSendBack("Error In App, Please Reload.");
            return;
        }
        System.Diagnostics.Debug.WriteLine("IT WorKSSSS");


        //Implementation
        PlayerAuthorisationRecord player = new PlayerAuthorisationRecord(Username, Password);
        await _hubConnection.InvokeAsync("Mortgage", player, TurnMultiplayer.turnMethodCount, boardPosition);


    }

    //Func that will be triggered when 'Send Trade' pused on initiate trade popup window VIA ITS VIEWMODEL, pulls another popup if any wanted props are mortgaged
    public async Task StartSendTrade(PlayerClass localPlayer, int mySelectedCash, List<Station> mySelectedStations, List<Utility> mySelectedUtilities, List<Property> mySelectedProperties, int myGODCards,
         PlayerClass otherPlayer, int theirSelectedCash, List<Station> theirSelectedStations, List<Utility> theirSelectedUtilities, List<Property> theirSelectedProperties, int theirGODCards)
    {
        if (theirSelectedStations.Any(o => o.Mortgaged == true) || theirSelectedUtilities.Any(o => o.Mortgaged == true) || theirSelectedProperties.Any(o => o.Mortgaged == true))
        {
            //Should work?? Get's List of names, will have to debug
            //List<String> theirMortgagedOwned = theirSelectedStations.FindAll(o => o.Mortgaged == true).Select(o => o.Name).ToList();
            //Show Mortgaged prop confirm popup
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var popupViewModel = new MortgagedPropertiesTradeConfirmViewModel(this, localPlayer, mySelectedCash, mySelectedStations,
                    mySelectedUtilities, mySelectedProperties, myGODCards, otherPlayer, theirSelectedCash, theirSelectedStations,
                    theirSelectedUtilities, theirSelectedProperties, theirGODCards);
                var popup = new MortgagedPropertiesTradeConfirmPopup(popupViewModel);
                Page.ShowPopup(popup);
            });
            return;
        }
        await SendTrade(localPlayer, mySelectedCash, mySelectedStations, mySelectedUtilities, mySelectedProperties, myGODCards,
         otherPlayer, theirSelectedCash, theirSelectedStations, theirSelectedUtilities, theirSelectedProperties, theirGODCards);
        return;
    }




    //Function that will be triggered when a trade is going to be sent after potential mortgage popup
    public async Task SendTrade(PlayerClass localPlayer, int mySelectedCash, List<Station> mySelectedStations, List<Utility> mySelectedUtilities, List<Property> mySelectedProperties, int myGODCards,
         PlayerClass otherPlayer, int theirSelectedCash, List<Station> theirSelectedStations, List<Utility> theirSelectedUtilities, List<Property> theirSelectedProperties, int theirGODCards)
    {
        //validation-Most validation will be on server side

        //Implementation
        List<int> mySelectedStationsBoardPos = mySelectedStations.Select(o => o.BoardPosition).ToList();
        List<int> mySelectedUtilitiesBoardPos = mySelectedUtilities.Select(o => o.BoardPosition).ToList();
        List<int> mySelectedPropertiesBoardPos = mySelectedProperties.Select(o => o.BoardPosition).ToList();

        List<int> theirSelectedStationsBoardPos = theirSelectedStations.Select(o => o.BoardPosition).ToList();
        List<int> theirSelectedUtilitiesBoardPos = theirSelectedUtilities.Select(o => o.BoardPosition).ToList();
        List<int> theirSelectedPropertiesBoardPos = theirSelectedProperties.Select(o => o.BoardPosition).ToList();

        PlayerAuthorisationRecord player = new PlayerAuthorisationRecord(Username, Password);
        InitiateTradeRecord tradeRecord = new InitiateTradeRecord(Username, otherPlayer.Name, mySelectedCash, mySelectedStationsBoardPos, mySelectedUtilitiesBoardPos,
            mySelectedPropertiesBoardPos, myGODCards, theirSelectedCash, theirSelectedStationsBoardPos, theirSelectedUtilitiesBoardPos, theirSelectedPropertiesBoardPos, theirGODCards);
        await _hubConnection.InvokeAsync("TradeInitiated", player, TurnMultiplayer.turnMethodCount, tradeRecord);


    }

    

    public async Task ShowTrades()
    {
        await Task.Run(() =>
        {
            int numOfIncomingTrades = GameClass.TradeRecords.Values.Where(o => o.otherPlayerName == Username).Count();
            int numOfOutgoingTrades = GameClass.TradeRecords.Values.Where(o => o.myName == Username).Count();

            MainThread.BeginInvokeOnMainThread(() =>
            {
                var popupViewModel = new TradesPopupViewModel(numOfIncomingTrades, numOfOutgoingTrades, this);
                var popup = new TradesPopup(popupViewModel);
                Page.ShowPopup(popup);
            });
        });
            
    }


    //These are used by the initial trades popup
    public async Task SeeIncomingOutgoingTrades(string incomingOrOutgoing)
    {
        await Task.Run(() => 
        {
            List<InitiateTradeRecord> incomingTradeRecords = GameClass.TradeRecords.Values.Where(o => o.otherPlayerName == Username).ToList();
            List<InitiateTradeRecord> outgoingTradeRecords = GameClass.TradeRecords.Values.Where(o => o.myName == Username).ToList();
            List<InitiateTradeRecord> tradeRecords;
            if (incomingOrOutgoing == "Incoming")
            {
                tradeRecords = incomingTradeRecords;
            }
            else if (incomingOrOutgoing == "Outgoing")
            {
                tradeRecords = outgoingTradeRecords;
            }
            else
            {
                Debug.WriteLine(incomingOrOutgoing + " is an invalid argument");
                throw new ArgumentException("This is not a valid input for incomingOrOutgoing.");
            }
            MainThread.BeginInvokeOnMainThread(() =>
            {
                var popupViewModel = new IncomingOutgoingTradesPopupViewModel(tradeRecords, incomingOrOutgoing, this);
                var popup = new IncomingOutgoingTradesPopup(popupViewModel);
                Page.ShowPopup(popup);
            });
        });
    }

    public async Task SeeSpecificTrade(InitiateTradeRecord tradeRecord, string incomingOrOutgoing) {
        //I.e stuff local player is goviong away
        (List<Station> mySelectedStations, List<Utility> mySelectedUtilities, List<Property> mySelectedProperties) = await Trade.IdentifyTheOwnedStuff(tradeRecord.mySelectedStationsBoardPos,
            tradeRecord.mySelectedUtilitiesBoardPos, tradeRecord.mySelectedPropertiesBoardPos, GameClass);
        //I.e stuff localplayer is gwtting
        (List<Station> theirSelectedStations, List<Utility> theirSelectedUtilities, List<Property> theirSelectedProperties) = await Trade.IdentifyTheOwnedStuff(tradeRecord.theirSelectedStationsBoardPos,
            tradeRecord.theirSelectedUtilitiesBoardPos, tradeRecord.theirSelectedPropertiesBoardPos, GameClass);
        MainThread.BeginInvokeOnMainThread(() =>
        {
            var popupViewModel = new SeeSpecififcTradePopupViewModel(this, tradeRecord.myName, tradeRecord.mySelectedCash, mySelectedStations, mySelectedUtilities, mySelectedProperties,
                tradeRecord.myGODCards, tradeRecord.otherPlayerName, tradeRecord.theirSelectedCash, theirSelectedStations, theirSelectedUtilities, theirSelectedProperties, 
                tradeRecord.theirGODCards, tradeRecord, incomingOrOutgoing);
            var popup = new SeeSpecificTradePopup(popupViewModel);
            Page.ShowPopup(popup);
        });
    }

    public async Task StartSendConfirmTrade(InitiateTradeRecord tradeRecord, string initeeName, int cashToBeReceived, List<Station> stationsToBeReceived,
        List<Utility> utilitiesToBeReceived, List<Property> propsToBeRecieved, int gODCardsToBeReceived, string localName, int cashToBeSent,
        List<Station> stationsToBeSent, List<Utility> utilitiesToBeSent, List<Property> propsToBeSent, int gODCardsToBeSent)
    {
        //Validation will be on the server side

        
        await Task.Run(async () =>
        {
            
            if (propsToBeRecieved.Any(o=> o.Mortgaged == true))
            {
                PlayerClass localPlayer = await IdentifyPlayer(localName);
                PlayerClass initeePlayer = await IdentifyPlayer(initeeName);
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    //Done in such a way so that the right ones are checked
                    var popupViewModel = new MortgagedPropertiesTradeConfirmViewModel(this, localPlayer, cashToBeSent, stationsToBeSent,
                        utilitiesToBeSent, propsToBeSent, gODCardsToBeSent, initeePlayer, cashToBeReceived, stationsToBeReceived,
                        utilitiesToBeReceived, propsToBeRecieved, gODCardsToBeReceived, false);
                    var popup = new MortgagedPropertiesTradeConfirmPopup(popupViewModel);
                    Page.ShowPopup(popup);
                });
                return;
            }
            else
            {

                await SendConfirmTrade(tradeRecord, new List<(int BoardPosition, bool ToUnmortgage)>());
                return;
            }
        });

    }
    public async Task SendConfirmTrade(InitiateTradeRecord tradeRecord, List<(int BoardPosition, bool ToUnmortgage)> mortPropsToDo)
    {
        int tradeKey = await Task<int>.Run(async () =>
        {
            //Find key of traderecord or atleast try, NEED TO TEST
            int tradeKey;
            try
            {
                tradeKey = GameClass.TradeRecords.First(o => o.Value == tradeRecord).Key;
            }
            catch
            {
                Debug.WriteLine("Can't find key of that tradeRecord.");
                await App.Current.MainPage.DisplayAlert("Trade Error", "Cannot find that trade, it may have been withdrawn.", "OK");
                return (-1);
            }
            return tradeKey;
        });
        if (tradeKey < 0)
            return;
        PlayerAuthorisationRecord player = new PlayerAuthorisationRecord(Username, Password);
        await _hubConnection.InvokeAsync("TradeConfirmed", player, TurnMultiplayer.turnMethodCount, tradeKey, tradeRecord, mortPropsToDo);
    }

    public async Task RespondUnmortgageOrFeeTrade(InitiateTradeRecord tradeRecord, List<(int BoardPosition, bool ToUnmortgage)> mortPropsToDo)
    {
        PlayerAuthorisationRecord player = new PlayerAuthorisationRecord(Username, Password);
        await _hubConnection.InvokeAsync("UnmortgageOrFeeTradeResponse", player, TurnMultiplayer.turnMethodCount, tradeRecord, mortPropsToDo);
    }

    async Task<PlayerClass> IdentifyPlayer()
    {

        PlayerClass player = GameClass.PlayerList.First<PlayerClass>((player) => (player.Name == Username));
        return player;
    }
    async Task<PlayerClass> IdentifyPlayer(string playerName)
    {

        PlayerClass player = GameClass.PlayerList.First<PlayerClass>((player) => (player.Name == playerName));
        return player;
    }

    //Updates position for player ofr record
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

    //updates all the positions
    private void UpdateAllPos()
    {
        int lenPL = GameClass.PlayerList.Count;
        foreach (int n in Enumerable.Range(1, lenPL))
        {
            UpdatePos(n);
        }
    }


    //updates boardspaceviewrecord on all owners
    private void UpdatePropertyPropsOnViewRecord()
    {
        if (GameClass == null)
        {
            ErrorSendBack("Please Reconnect");
            return;
        }
        //Get a list of props and boardPoses to stramline later
        List<(Property Property, int BoardPos)> PropsAndBoardPos = new();
        foreach (KeyValuePair<string, SetProp> valuePair in GameClass.setsPropDict)
        {
            foreach (var tempP in valuePair.Value.Properties)
            {
                PropsAndBoardPos.Add((tempP, tempP.BoardPosition));
            }
        }
        foreach (BoardSpaceViewRecord viewSpace in BoardSpaces)
        {
            var undesiderdPos = new int[] { 7, 22, 36, 2, 17, 33, 0, 10, 20, 30, 4, 38 };
            var StationPos = new int[] { 5, 15, 25, 35 };
            var UtilityPos = new int[] { 12, 28 };
            
            //Return if undesired pos
            if (undesiderdPos.Any(o => o == viewSpace.BoardPosition))
            {
                return;
            }
            string ownerName;
            int ownerPos;

            List<string> playerNames = new();
            foreach (var player in GameClass.PlayerList)
            {
                playerNames.Add(player.Name);
            }

            


            //Get Station and update
            if (StationPos.Any(o => o == viewSpace.BoardPosition))
            {
                Station station;
                try
                {
                    station = GameClass.stations.Properties.First(o => o.BoardPosition == viewSpace.BoardPosition);
                }
                catch(Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    ErrorSendBack("Please Reconnect, Error in Game");
                    return;
                }
                if (!station.Owned)
                {
                    ownerName = string.Empty;
                    ownerPos = 0;
                }
                else
                {
                    ownerName = station.Owner.Name;
                    try
                    {
                        ownerPos = playerNames.IndexOf(station.Owner.Name) + 1;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        ownerPos = 0;
                    }
                    
                }
            }
            //Get utility and update
            else if (UtilityPos.Any(o => o == viewSpace.BoardPosition))
            {
                Utility utility;
                try
                {
                    utility = GameClass.utilities.Properties.First(o => o.BoardPosition == viewSpace.BoardPosition);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    ErrorSendBack("Please Reconnect, Error in Game");
                    return;
                }
                if (!utility.Owned)
                {
                    ownerName = string.Empty;
                    ownerPos = 0;
                }
                else
                {
                    ownerName = utility.Owner.Name;
                    try
                    {
                        ownerPos = playerNames.IndexOf(utility.Owner.Name) + 1;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        ownerPos = 0;
                    }

                }
            }
            else
            {
                Property property;
                try
                {
                    property = PropsAndBoardPos.First(o => o.BoardPos == viewSpace.BoardPosition).Property;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    ErrorSendBack("Please Reconnect, Error in Game");
                    return;
                }
                if (!property.Owned)
                {
                    ownerName = string.Empty;
                    ownerPos = 0;
                }
                else
                {
                   ownerName = property.Owner.Name;
                    try
                    {
                        ownerPos = playerNames.IndexOf(property.Owner.Name) + 1;
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        ownerPos = 0;
                    }

                }

                viewSpace.PlayerOwnerNum = ownerPos;
                viewSpace.PlayerOwnerName = ownerName;


            }

        }
        
    }



    //Encapsulates all update methods for view records so much only call this method
    private void UpdateAllViewRecord()
    {
        UpdateAllPos();
        UpdatePropertyPropsOnViewRecord();
    }

    //DO UPDATE CLASS FOR ALL SO ONly HAVE To Iterate Once
    
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
