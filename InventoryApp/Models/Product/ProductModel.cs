using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryApp.Models.Product
{
    class ProductModel
    {
        enum Group
        {
            Default = 0,
            Electronics = 5,
            Computers = 10,
            Appliances = 15,
            House = 20,
            Auto = 25,
            Work = 30
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
