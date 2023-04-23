using FoodopolyClientV2.ViewModels;
using System.Security.Cryptography.X509Certificates;

namespace FoodopolyClientV2.Views;

public partial class GameJoinPage : ContentPage
{
	public GameJoinPage(GameJoinViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}