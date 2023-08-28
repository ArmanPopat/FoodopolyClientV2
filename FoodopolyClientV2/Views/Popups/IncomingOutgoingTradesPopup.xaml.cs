using CommunityToolkit.Maui.Views;
using FoodopolyClientV2.ViewModels.Popups;

namespace FoodopolyClientV2.Views.Popups;

public partial class IncomingOutgoingTradesPopup : Popup
{
	public IncomingOutgoingTradesPopup(IncomingOutgoingTradesPopupViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}