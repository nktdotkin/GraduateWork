using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryApp.Models.Product
{
    class SupplyModel
    {
        public int Id { get; set; }
        public DateTime date { get; set; }
        public int Amount { get; set; }
        public ProductModel product { get; set; }
        public User.ProviderModel provider { get; set; }
    }
}
