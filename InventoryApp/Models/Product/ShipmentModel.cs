using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace InventoryApp.Models.Product
{
    class ShipmentModel
    {
        private DateTime date = DateTime.Now;

        public int Id { get; set; }
        public DateTime Date
        {
            get => DateTime.ParseExact(date.Date.ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);
            set
            {
                if (value != date)
                {
                    date = value;
                }
            }
        }
        [Required(ErrorMessage = "Amount must be more than zero")]
        [Range(1, Int32.MaxValue)]
        public int Amount { get; set; }
        [Required(ErrorMessage = "Please select product")]
        public ProductModel Product { get; set; }
        [Required(ErrorMessage = "Please select client")]
        public User.ClientModel Client { get; set; }
    }
}
