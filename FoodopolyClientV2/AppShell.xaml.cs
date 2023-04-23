using FoodopolyClientV2.Views;

namespace FoodopolyClientV2;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute(nameof(GameJoinPage), typeof(GameJoinPage));
        Routing.RegisterRoute(nameof(GamePage), typeof(GamePage));
    }
}
