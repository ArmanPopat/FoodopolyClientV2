using CommunityToolkit.Maui.Views;
using FoodopolyClientV2.ViewModels.Popups;

namespace FoodopolyClientV2.Views.Popups;

public partial class IntiateTradePopup : Popup
{
	public IntiateTradePopup(InitiateTradePopupViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}