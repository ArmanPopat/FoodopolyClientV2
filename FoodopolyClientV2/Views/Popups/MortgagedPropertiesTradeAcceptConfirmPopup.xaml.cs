using CommunityToolkit.Maui.Views;
using FoodopolyClientV2.ViewModels.Popups;

namespace FoodopolyClientV2.Views.Popups;

public partial class MortgagedPropertiesTradeAcceptConfirmPopup : Popup
{
	public MortgagedPropertiesTradeAcceptConfirmPopup(MortgagedPropertiesTradeAcceptConfirmViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
		CanBeDismissedByTappingOutsideOfPopup = viewModel.Responding;
	}
}