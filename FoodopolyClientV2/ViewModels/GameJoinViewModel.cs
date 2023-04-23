using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectivityLibrary.Services;
using ConnectivityLibrary.Records;
using CommunityToolkit.Mvvm.Input;
using FoodopolyClientV2.Views;

namespace FoodopolyClientV2.ViewModels;

//not happyt with name of, guessing because mvvm toolkit makes Username property elsewhere??
[QueryProperty("Username", "Username")]
[QueryProperty("Password", "Password")]
public partial class GameJoinViewModel : ObservableObject
{
    [ObservableProperty]
    string username;
    
    [ObservableProperty]  //Probably doesn't need to be observableprop, could maybe be just property(ie basic c# stuff), addendum, does for queryid to work
    string password;

    [ObservableProperty]
    string gamePasswordCreate;

    [ObservableProperty]
    string gamePasswordJoin;

    [ObservableProperty]
    string gameIdString;

    //Method to try send a post req to api to create game
    [RelayCommand]
    async Task PostCreateGameAPI()
    {
        if (string.IsNullOrEmpty(GamePasswordCreate))
        {
            await App.Current.MainPage.DisplayAlert("Alert", "Game Password Cannot Be Empty", "OK");
            return;
        }
        string gamePasswordCreateTrimmed = GamePasswordCreate.Trim();

        if (string.IsNullOrEmpty(gamePasswordCreateTrimmed))
        {
            await App.Current.MainPage.DisplayAlert("Alert", "Game Password Cannot Be Empty", "OK");
            return;
        }
        var gameRecord = new CreateGameRecord(GamePasswordCreate, Username, Password);
        (bool success, string message, int gameIdOrStatusCode) successAndMessageAndGameIdOrStatusCode;
        
        //ADD TEST FOR CONNECTIVITY AT SOME POINT
        
        try
        {
            successAndMessageAndGameIdOrStatusCode = await APIService.POSTCreateGameAsync(gameRecord);
        }
        catch (Exception ex)
        {

            await App.Current.MainPage.DisplayAlert("Alert", ex.Message, "OK");
            return;
        }
        if (successAndMessageAndGameIdOrStatusCode.success)
        {
            //
            await Shell.Current.GoToAsync($"{nameof(GamePage)}?GameId={successAndMessageAndGameIdOrStatusCode.gameIdOrStatusCode.ToString()}&Username={Username}&Password={Password}");
        }
        else if (successAndMessageAndGameIdOrStatusCode.gameIdOrStatusCode == 400)
        {
            await App.Current.MainPage.DisplayAlert("Alert", successAndMessageAndGameIdOrStatusCode.message, "OK");
            await Shell.Current.GoToAsync(nameof(LoginPage));
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Alert", successAndMessageAndGameIdOrStatusCode.message, "OK");
        }
    }


    //Method to try join game
    [RelayCommand]
    async Task PatchJoinGameAPI()
    {
        string gameIdString2;
        try
        {
            gameIdString2 = GameIdString.Trim();
        }
        catch //There was problem here, gameIdString was null, that's the reason for try catch
        {
            await App.Current.MainPage.DisplayAlert("Alert", "Game ID Cannot Be Empty And Must Be A Whole Number", "OK");
            return;
        }
        bool isGameIdParsable = int.TryParse(gameIdString2, out int gameIdInt);
        if (!isGameIdParsable)
        {
            await App.Current.MainPage.DisplayAlert("Alert", "Game ID Cannot Be Empty And Must Be A Whole Number", "OK");
            return;
        }
        if (string.IsNullOrEmpty(GamePasswordJoin))
        {
            await App.Current.MainPage.DisplayAlert("Alert", "Game Password Cannot Be Empty", "OK");
            return;
        }
        string gamePasswordJoinTrimmed = GamePasswordJoin.Trim();

        if (string.IsNullOrEmpty(gamePasswordJoinTrimmed))
        {
            await App.Current.MainPage.DisplayAlert("Alert", "Game Password Cannot Be Empty", "OK");
            return;
        }

        var gameRecord = new JoinGameRecord(gameIdString2, GamePasswordJoin, Username, Password);
        (bool success, string message, int gameIdOrStatusCode) successAndMessageAndGameIdOrStatusCode;

        //ADD TEST FOR CONNECTIVITY AT SOME POINT

        try
        {
            successAndMessageAndGameIdOrStatusCode = await APIService.PATCHJoinGameAsync(gameRecord, gameIdString2);
        }
        catch (Exception ex)
        {

            await App.Current.MainPage.DisplayAlert("Alert", ex.Message, "OK");
            return;
        }
        if (successAndMessageAndGameIdOrStatusCode.success)
        {
            // what happens with success, pass gameId to it
            await Shell.Current.GoToAsync($"{nameof(GamePage)}?GameId={successAndMessageAndGameIdOrStatusCode.gameIdOrStatusCode}");
        }
        if (successAndMessageAndGameIdOrStatusCode.gameIdOrStatusCode == 400)
        {
            await App.Current.MainPage.DisplayAlert("Alert", successAndMessageAndGameIdOrStatusCode.message, "OK");
            await Shell.Current.GoToAsync(nameof(LoginPage));
            return;
        }
        else if (successAndMessageAndGameIdOrStatusCode.gameIdOrStatusCode == 401)
        {
            await App.Current.MainPage.DisplayAlert("Alert", successAndMessageAndGameIdOrStatusCode.message, "OK");
            if(successAndMessageAndGameIdOrStatusCode.message == "Wrong User Password")
            {
                await Shell.Current.GoToAsync(nameof(LoginPage));
                return;
            }
            
        }
        else
        {
            await App.Current.MainPage.DisplayAlert("Alert", successAndMessageAndGameIdOrStatusCode.message, "OK");
        }
    }
}

