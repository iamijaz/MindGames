using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace BabyProject
{
    [TestFixture]
    public class ShoppinCartTest
    {
        //These are not old fashioned TDD tests 
        //These are still End To End because shopping cart only require this much of functionality

        [Test]
        public void RemovingAnItemCorrectlyUpdatesTheTotal()
        {
            var shoppingCart = GivenAncartWithThreeItmesAndTotal74();
            WhenIRemoveAnItemPriced4_dot_50(shoppingCart);
            ThenGrandTotalIsreducedTo69_dot_50(shoppingCart);
        }

        private static void ThenGrandTotalIsreducedTo69_dot_50(ShoppingCart shoppingCart)
        {
            Assert.That(shoppingCart.Items.Sum(item => item.Price) == 69.50m);
        }

        private static void WhenIRemoveAnItemPriced4_dot_50(ShoppingCart shoppingCart)
        {
            var itemToRemove = shoppingCart.Items.SingleOrDefault(item => item.Name == "Pencil");
            shoppingCart.Items.Remove(itemToRemove);

        }

        private static ShoppingCart GivenAncartWithThreeItmesAndTotal74()
        {
            var items = new List<Item>()
            {
                new Item() {Name = "Pen", Price = 50.0m},
                new Item() {Name = "Book", Price = 19.5m},
                new Item() {Name = "Pencil", Price = 4.5m}
            };

            var shoppingCart = new ShoppingCart();
            shoppingCart.Items.AddRange(items);

            return shoppingCart;
        }

        [Test]
        public void TwoItemsInShoppingCartShowsCorrectTotal()
        {
            var items=GivenIHaveTwoItemsPenAndBookWithPrice5And7Respectively();            
            var shoppingCart=WhenIAddTheseItemsToTheShoppingCart(items);
            ThenShoppinCartHas2Items(shoppingCart);
            AndShoppingCartHasGrandTotal11(shoppingCart);
        }

        [Test]
        public void AnItemCanBeAddedTotheShoppinCart()
        {
            var item = GivenIHaveAnItemPen();
            var shoppingCart = WhenIAddItemToShoppingCart(item);
            ThenItemIsAddedToTheShoppingCart(shoppingCart, item);
        }

        private static void AndShoppingCartHasGrandTotal11(ShoppingCart shoppingCart)
        {
            Assert.That(shoppingCart.Items.Sum(item => item.Price) == 11.0m);
        }

        private static void ThenShoppinCartHas2Items(ShoppingCart shoppingCart)
        {
            Assert.That(shoppingCart.Items.Count == 2);
        }

        private static ShoppingCart WhenIAddTheseItemsToTheShoppingCart(IEnumerable<Item> items)
        {
            var shoppingCart = new ShoppingCart();
            shoppingCart.Items.AddRange(items);
            return shoppingCart;
        }

        private static IEnumerable<Item> GivenIHaveTwoItemsPenAndBookWithPrice5And7Respectively()
        {
            var items = new List<Item>()
            {
                new Item() {Name = "Pen", Price = 4.0m},
                new Item() {Name = "Book", Price = 7.0m}
            };
            return items;
        }

        private static void ThenItemIsAddedToTheShoppingCart(ShoppingCart shoppingCart, Item item)
        {
            Assert.That(shoppingCart.Items.Contains(item));
        }

        private static ShoppingCart WhenIAddItemToShoppingCart(Item item)
        {
            var shoppingCart = new ShoppingCart();
            shoppingCart.Items.Add(item);
            return shoppingCart;
        }

        private static Item GivenIHaveAnItemPen()
        {
            var item = new Item()
            {
                Name = "Pen"
            };
            return item;
        }
    }
}
