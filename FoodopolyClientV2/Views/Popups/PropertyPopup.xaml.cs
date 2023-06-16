using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using FoodopolyClientV2.ViewModels.Popups;

namespace FoodopolyClientV2.Views.Popups;

public partial class PropertyPopup : Popup
{
	public PropertyPopup(PropertyPopupViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}