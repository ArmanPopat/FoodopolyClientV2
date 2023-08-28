using CommunityToolkit.Maui.Views;
using FoodopolyClientV2.ViewModels.Popups;

namespace FoodopolyClientV2.Views.Popups;

public partial class TradesPopup : Popup
{
	public TradesPopup(TradesPopupViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}