using CommunityToolkit.Maui.Views;
using FoodopolyClientV2.ViewModels.Popups;

namespace FoodopolyClientV2.Views.Popups;

public partial class MortgagedPropertiesTradeConfirmPopup : Popup
{
	public MortgagedPropertiesTradeConfirmPopup(MortgagedPropertiesTradeConfirmViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}