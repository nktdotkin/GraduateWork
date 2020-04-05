using System;

namespace InventoryApp.Models.Product
{
    class ShipmentModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Amount { get; set; }
        public ProductModel Product { get; set; }
        public User.ClientModel Client { get; set; }
    }
}
