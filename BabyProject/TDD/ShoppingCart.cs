using System.Collections.Generic;

namespace BabyProject.TDD
{
    internal class ShoppingCart
    {
        public ShoppingCart()
        {
            Items = new List<Item>();
        }

        public List<Item> Items { get; set; }
    }
}