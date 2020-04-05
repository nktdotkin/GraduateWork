using System;

namespace InventoryApp.Models.Product
{
    class SupplyModel
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Amount { get; set; }
        public ProductModel Product { get; set; }
        public User.ProviderModel Provider { get; set; }
    }
}
