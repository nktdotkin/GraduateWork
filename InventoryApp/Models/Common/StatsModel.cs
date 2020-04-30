using InventoryApp.Models.Product;
using InventoryApp.Models.User;
using System;

namespace InventoryApp.Models.Common
{
    class StatsModel
    {
        public DateTime CurrentDate { get => DateTime.Now; }

        public ClientModel Client { get; set; }
        public ProviderModel Provider { get; set; }
        public ProductModel Product { get; set; }
        public ShipmentModel Shipment { get; set; }
        public SupplyModel Supply { get; set; }
    }
}
