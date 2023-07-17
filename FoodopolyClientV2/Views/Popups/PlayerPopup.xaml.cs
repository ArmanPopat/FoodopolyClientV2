using CommunityToolkit.Maui.Views;
using FoodopolyClientV2.ViewModels.Popups;

namespace FoodopolyClientV2.Views.Popups;

public partial class PlayerPopup : Popup
{
	public PlayerPopup(PlayerPopupViewModel viewModel)
	{
		InitializeComponent();
	    BindingContext = viewModel;
	}
}