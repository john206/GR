using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace GR.Tests
{
    public class TestAssemblyTests
    {
        private readonly Program _app;

        public TestAssemblyTests()
        {
            this._app = new Program
            {
                Items = new List<Item>
                {
                    new Item {Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20},
                    new AppreciatingItem {Name = "Aged Brie", SellIn = 2, Quality = 1},
                    new LegendaryItem {Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80},
                    new ConjuredItem {Name = "Conjured Mana Cake", SellIn = 3, Quality = 6},
                    new BackstagePassItem
                    {
                        Name = "Backstage passes to a TAFKAL80ETC concert",
                        SellIn = 15,
                        Quality = 20
                    },
                    new BackstagePassItem
                    {
                        Name = "Backstage passes to a D498FJ9FJ2N concert",
                        SellIn = 10,
                        Quality = 30
                    },
                    new BackstagePassItem
                    {
                        Name = "Backstage passes to a FH38F39DJ39 concert",
                        SellIn = 5,
                        Quality = 33
                    }
                }
            };

            this._app.UpdateInventory();
        }

        [Fact]
        public void PolyItemConstructionIntegrety()
        {
            var items = this._app.Items;
            
            Assert.Equal(7, items.Count);
            Assert.Single(items.Where(item => item.GetType() == typeof(AppreciatingItem)));
            Assert.Single(items.Where(item => item.GetType() == typeof(LegendaryItem)));
            Assert.Single(items.Where(item => item.GetType() == typeof(ConjuredItem)));
            Assert.Equal(3, items.Where(item => item.GetType() == typeof(BackstagePassItem)).Count());
        }

        [Fact]
        public void DexterityVest_SellIn_DecreasesByOne()
        {
            Assert.Equal(9, this._app.Items.First(x => x.Name == "+5 Dexterity Vest").SellIn);
        }

        [Fact]
        public void DexterityVest_Quality_DecreasesByOne()
        {
            Assert.Equal(19, this._app.Items.First(x => x.Name == "+5 Dexterity Vest").Quality);
        }

        // TODO: Since the Program app test has effectively become an integration test, consider moving each following unit test
        // to its own respective subclass' test file.

        // Once the sell by date has passed, Quality degrades twice as fast.
        [Fact]
        public void Item_Quality_DegradeRateDoublesAfterSellByPasses()
        {
            var item = new Item {Name = "GeneralItem", SellIn = 1, Quality = 20};

            item.UpdateItem();

            Assert.Equal(0, item.SellIn);
            Assert.Equal(19, item.Quality);

            item.UpdateItem();

            Assert.Equal(-1, item.SellIn);
            Assert.Equal(17, item.Quality);
        }

        // The Quality of an item is never more than 50.
        [Fact]
        public void Item_Quality_IsNeverAbove50_OnInstantiation()
        {
            var item = new Item { Name = "GeneralItem", SellIn = 10, Quality = 99 };

            Assert.Equal(10, item.SellIn);
            Assert.Equal(50, item.Quality);
        }

        // The Quality of an item is never more than 50.
        [Fact]
        public void AppreciatingItem_Quality_IsNeverAbove50_OnUpdate()
        {
            var item = new AppreciatingItem {Name = "Aged Brie", SellIn = 0, Quality = 49};

            Assert.Equal(0, item.SellIn);
            Assert.Equal(49, item.Quality);

            item.UpdateItem();

            Assert.Equal(0, item.SellIn);
            Assert.Equal(50, item.Quality);

            item.UpdateItem();

            Assert.Equal(0, item.SellIn);
            Assert.Equal(50, item.Quality);
        }

        // The Quality of an item is never negative.
        [Fact]
        public void Item_Quality_IsNeverNegative_OnInstantiation()
        {
            var item = new Item { Name = "GeneralItem", SellIn = 1, Quality = -99 };

            Assert.Equal(1, item.SellIn);
            Assert.Equal(0, item.Quality);            
        }

        // The Quality of an item is never negative.
        [Fact]
        public void Item_Quality_IsNeverNegative_OnUpdate()
        {
            var item = new Item {Name = "GeneralItem", SellIn = 1, Quality = 1};

            Assert.Equal(1, item.SellIn);
            Assert.Equal(1, item.Quality);

            item.UpdateItem();

            Assert.Equal(0, item.SellIn);
            Assert.Equal(0, item.Quality);

            item.UpdateItem();

            Assert.Equal(-1, item.SellIn);
            Assert.Equal(0, item.Quality);
        }        

        // "Sulfuras" is a legendary item and as such its Quality is 80 and it never alters.
        [Fact]
        public void LegendaryItems_Quality_Always80()
        {
            var item = new LegendaryItem {Name = "Sulfuras", SellIn = 1, Quality = 1};            

            Assert.Equal(1, item.SellIn);
            Assert.Equal(80, item.Quality);

            item.UpdateItem();

            Assert.Equal(1, item.SellIn);
            Assert.Equal(80, item.Quality);            
        }

        // "Conjured" items degrade in Quality twice as fast as normal items.
        [Fact]
        public void ConjuredItems_Quality_DegradeRateDoubleNormal_SellByNotPassed()
        {
            var item = new ConjuredItem { Name = "Conjured Mana Cake", SellIn = 10, Quality = 8 };            

            Assert.Equal(10, item.SellIn);
            Assert.Equal(8, item.Quality);

            item.UpdateItem();

            Assert.Equal(9, item.SellIn);
            Assert.Equal(6, item.Quality);
        }

        // "Conjured" items degrade in Quality twice as fast as normal items.
        [Fact]
        public void ConjuredItems_Quality_DegradeRateDoubleNormal_SellByPassed()
        {
            var item = new ConjuredItem {Name = "Conjured Mana Cake", SellIn = 1, Quality = 8};

            Assert.Equal(1, item.SellIn);
            Assert.Equal(8, item.Quality);

            item.UpdateItem();

            Assert.Equal(0, item.SellIn);
            Assert.Equal(6, item.Quality);

            item.UpdateItem();

            Assert.Equal(-1, item.SellIn);
            Assert.Equal(2, item.Quality);
        }

        // "Backstage passes", like aged brie, increase in Quality as SellIn value approaches.
        [Fact]
        public void BackstagePasses_Quality_IncreasesNormal_BeforeTenDaysRemaining()
        {
            var item = new BackstagePassItem {Name = "Backstage Pass Anna1Anna2", SellIn = 20, Quality = 10};

            Assert.Equal(20, item.SellIn);
            Assert.Equal(10, item.Quality);

            item.UpdateItem();

            Assert.Equal(19, item.SellIn);
            Assert.Equal(11, item.Quality);
        }

        // [DEFECT <insert id>] "Backstage passes" do not seem to be increasing in Quality per the specifications as the concert gets closer.
        // Quality increases by 2 when there are 10 days or less.
        [Fact]
        public void BackstagePasses_Quality_IncreasesByTwo_AtTenDaysRemaining()
        {
            var item = new BackstagePassItem { Name = "Backstage Pass Anna1Anna2", SellIn = 11, Quality = 10 };

            Assert.Equal(11, item.SellIn);
            Assert.Equal(10, item.Quality);

            item.UpdateItem();

            Assert.Equal(10, item.SellIn);
            Assert.Equal(12, item.Quality);
        }

        // [DEFECT <insert id>] "Backstage passes" do not seem to be increasing in Quality per the specifications as the concert gets closer.
        // Quality increases by 3 when there are 5 days or less.
        [Fact]
        public void BackstagePasses_Quality_IncreasesByThree_AtFiveDaysRemaining()
        {
            var item = new BackstagePassItem {Name = "Backstage Pass Anna1Anna2", SellIn = 7, Quality = 10};

            item.UpdateItem();

            Assert.Equal(6, item.SellIn);
            Assert.Equal(12, item.Quality);

            item.UpdateItem();

            Assert.Equal(5, item.SellIn);
            Assert.Equal(15, item.Quality);
        }

        // [DEFECT <insert id>] "Backstage passes" do not reduce to a Quality of 0 after the concert passes.
        // Quality drops to 0 after the concert.
        [Fact]
        public void BackstagePasses_Quality_BecomesZeroAfterConcert()
        {
            var item = new BackstagePassItem {Name = "Backstage Pass Anna1Anna2", SellIn = 1, Quality = 10};
            
            item.UpdateItem();

            Assert.Equal(0, item.SellIn);
            Assert.Equal(13, item.Quality);

            item.UpdateItem();

            Assert.Equal(-1, item.SellIn);
            Assert.Equal(0, item.Quality);
        }
    }
}