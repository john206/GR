using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace GR
{
    public class Program
    {
        public IList<Item> Items { get; set; }

        private static void Main(string[] args)
        {
            Console.WriteLine("Welcome");

            var app = new Program()
            {
                Items = new List<Item>
                {
                    new Item {Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20},
                    new AppreciatingItem {Name = "Aged Brie", SellIn = 2, Quality = 0},
                    new Item {Name = "Elixir of the Mongoose", SellIn = 5, Quality = 7},
                    new LegendaryItem {Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80},
                    new BackstagePassItem
                    {
                        Name = "Backstage passes to a TAFKAL80ETC concert",
                        SellIn = 15,
                        Quality = 20
                    },
                    new ConjuredItem {Name = "Conjured Mana Cake", SellIn = 3, Quality = 6}
                }
            };

            app.UpdateInventory();

            var filename = $"inventory_{DateTime.Now:yyyyddMM-HHmmss}.txt";
            var inventoryOutput = JsonConvert.SerializeObject(app.Items, Formatting.Indented);
            File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename), inventoryOutput);

            Console.ReadKey();
        }

        public void UpdateInventory()
        {
            Console.WriteLine("Updating inventory");
            foreach (var item in this.Items)
            {
                item.UpdateItem();           
            }
            Console.WriteLine("Inventory update complete");
        }
    }

    // TODO: Consider moving each Item type to its own file.

    public class Item
    {
        private int quality;

        protected const int DegradeIncrement = 1;
        protected const int MinQuality = 0;
        protected const int MaxQuality = 50;
                
        // TODO: Consider disallowing hardcoded strings.
        public string Name { get; set; }

        public int SellIn { get; set; }

        public virtual int Quality 
        { 
            get { return this.quality; }
            set 
            {
                if (value < MinQuality)
                {
                    this.quality = MinQuality;
                }
                else if (value > MaxQuality)
                {
                    this.quality = MaxQuality;
                }
                else
                {
                    this.quality = value;
                }
            }
        }

        public void UpdateItem()
        {
            this.UpdateSellIn();
            this.UpdateQuality();
        }

        protected virtual void UpdateSellIn()
        {
            --this.SellIn;
        }

        protected virtual void UpdateQuality()
        {
            this.Quality -= this.DegradeRate;
        }

        protected virtual int DegradeRate => this.SellIn >= 0 ? DegradeIncrement : 2 * DegradeIncrement;
    }

    public class LegendaryItem : Item
    {
        // TODO: Parameterize for other types of legendary items.
        public override int Quality => 80;

        // Legendary items never have to be sold.
        protected override void UpdateSellIn()
        {
            return;
        }

        // Legendary items remain constant value.
        protected override void UpdateQuality()
        {
            return;
        }
    }

    public class AppreciatingItem : Item
    {
        private const int Increment = 1;
        
        // Appreciating items don't have a valid sell by date because they increase in value over time.
        protected override void UpdateSellIn()
        {
            return;
        }

        protected override void UpdateQuality()
        {
            this.Quality += Increment;
        }
    }

    public class ConjuredItem : Item
    {
        protected override int DegradeRate => 2 * base.DegradeRate;
    }

    public class BackstagePassItem : Item
    {
        protected override void UpdateQuality()
        {
            const int IncrementMoreThanTenDays = 1;
            const int IncrementBetweenTenAndFiveDays = 2;
            const int IncrementLessThanFiveDays = 3;
            
            if (this.SellIn > 10)
            {
                this.Quality += IncrementMoreThanTenDays;              
            }
            else if (this.SellIn <= 10 && this.SellIn > 5)
            {
                this.Quality += IncrementBetweenTenAndFiveDays;
            }
            else if (this.SellIn <= 5 && this.SellIn >= 0)
            {
                this.Quality += IncrementLessThanFiveDays;
            }
            else
            {
                this.Quality = 0;
            }
        }
    }
}