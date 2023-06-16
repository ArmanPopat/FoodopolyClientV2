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
        public void Test()
        {

        }

    }
}