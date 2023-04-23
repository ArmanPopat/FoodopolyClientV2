using ConnectivityLibrary;

namespace FoodopolyClientV2;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();
        APIHelper.InitialiseClient(); //Initialise https class

        MainPage = new AppShell();
    }
}
