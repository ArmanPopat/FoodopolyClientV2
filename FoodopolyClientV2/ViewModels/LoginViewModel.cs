using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FoodopolyClientV2.Views;
using System.ComponentModel;
using System.Xml.Serialization;

namespace FoodopolyClientV2.ViewModels;
public partial class LoginViewModel : ObservableObject
{

    //public LoginViewModel()
    //{
    //    navigationParams = new Dictionary<string, string>();
    //}

    //[ObservableProperty]
    //Dictionary<string, string> navigationParams;

    [ObservableProperty]
    string username;
    [ObservableProperty]
    string password;

    /*Method on submit button to store our user/pass data*/
    [RelayCommand]
    async Task StoreAndGo() //figure out how to make async
    {
        //navigationParams.Clear();
        //Username= string.Empty;
        //Password= string.Empty;
        if (string.IsNullOrEmpty(Username) || string.IsNullOrEmpty(Password)) 
        {
            await App.Current.MainPage.DisplayAlert("Alert", "Username and Password must not be Empty", "OK");
            return;
        }
        string usernameTrimmed = Username.Trim();
        string passwordTrimmed = Password.Trim();

        if (string.IsNullOrEmpty(usernameTrimmed) || string.IsNullOrEmpty(passwordTrimmed))
        {
            await App.Current.MainPage.DisplayAlert("Alert", "Username and Password must not be Empty", "OK");
            return;
        }
        //navigationParams.Add("Username", Username);
        //navigationParams.Add("Password", Password);

        await Shell.Current.GoToAsync($"{nameof(GameJoinPage)}?Username={usernameTrimmed}&Password={passwordTrimmed}"); 

    }
}

