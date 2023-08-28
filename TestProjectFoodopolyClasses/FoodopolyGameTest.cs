using BoardClasses;
using GameClasses;

namespace TestProjectFoodopolyClasses
{
    public class FoodopolyGameTest
    {
        [Fact]
        public void TestPropertyUpgradeChange()
        {
            GameClass game = new GameClass(1, "password");
            //CChest cChest = new CChest("Community Chest", 10);

            game.setsPropDict.TryGetValue("Brown", out var value);
            Property prop = (Property)value.Properties[0];
            prop.Upgrade();
            prop.Upgrade();


            game.setsPropDict.TryGetValue("Brown", out var actValue);
            Property prop2 = (Property)actValue.Properties[0];
            Assert.Equal(2, prop2.NumOfUpgrades);

        }
        [Fact]
        public void TestIfMorgatgedPickedUp()
        {
            GameClass game2 = new GameClass(2, "password");
            game2.setsPropDict.TryGetValue("Brown", out var value);
            Property prop = (Property)value.Properties[0];
            prop.Owned = true;
            prop.Mortgaged = true;
            List<Property> list = new List<Property>();
            list.Add(prop);
            List<String> theirMortgagedOwned = list.FindAll(o => o.Mortgaged == true).Select(o => o.Name).ToList();
            Assert.Equal("Salad", theirMortgagedOwned[0]);
        }

    }
}