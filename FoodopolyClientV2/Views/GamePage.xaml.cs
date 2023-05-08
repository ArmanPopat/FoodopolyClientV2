using FoodopolyClientV2.ViewModels;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;

namespace FoodopolyClientV2.Views;

public partial class GamePage : ContentPage
{
	private GameViewModel gameViewModel;
	public GamePage(GameViewModel viewModel)
	{
		InitializeComponent();
		//gameGrid.HeightRequest = LimitingDimLength; //We need to fix, reckon I'll keep maximised
		//gameGrid.WidthRequest = LimitingDimLength;
		gameViewModel = viewModel;
		BindingContext = gameViewModel;//editted here, see if works
	}
    //The reactive stuff
    public double LimitingDimLength {
		get { return Math.Min(DeviceDisplay.Current.MainDisplayInfo.Width, DeviceDisplay.Current.MainDisplayInfo.Height); }

    }
	//public async Task OnLoad(object sender, EventArgs e)
	//{
	//    await BindingContext.StartConnecting();
	//}
	

	//NEEDS TESTING VOID ASYNC?
	protected override async void OnAppearing()
	{
		base.OnAppearing();
		await gameViewModel.StartConnecting(); // think this would be ok
	}
	protected override async void OnDisappearing()
	{
		base.OnDisappearing();
		await gameViewModel.Disconnecting();
	}


}