using CommunityToolkit.Maui.Views;
using FoodopolyClientV2.ViewModels.Popups;

namespace FoodopolyClientV2.Views.Popups;

public partial class SeeSpecificTradePopup : Popup
{
	public SeeSpecificTradePopup(SeeSpecififcTradePopupViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}