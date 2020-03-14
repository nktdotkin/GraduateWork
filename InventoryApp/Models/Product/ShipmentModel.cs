using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryApp.Models.Product
{
    class ShipmentModel
    {
        public int Id { get; set; }
        public DateTime date { get; set; }
        public int Amount { get; set; }
        public ProductModel product { get; set; }
        public User.ClientModel client { get; set; }
    }
}
