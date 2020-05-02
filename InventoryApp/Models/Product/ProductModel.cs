using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace InventoryApp.Models.Product
{
    class ProductModel
    {
        private DateTime expirationDateDays = DateTime.Now;

        public int Id { get; set; }
        [Required(ErrorMessage = "Uncorrect name")]
        [StringLength(50, MinimumLength = 5)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Uncorrect description")]
        [StringLength(50, MinimumLength = 5)]
        public string Description { get; set; }
        [Required(ErrorMessage = "Uncorrect date")]
        public DateTime ExpirationDateDays
        {
            get => DateTime.ParseExact(expirationDateDays.Date.ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);
            set
            {
                if (value != expirationDateDays)
                {
                    expirationDateDays = value;
                }
            }
        }
        public int Amount { get; set; }
        [Required(ErrorMessage = "Price must be more than zero")]
        [Range(0.1, double.MaxValue)]
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public int GroupId { get; set; }
        public string ImageLink { get; set; }
        [Required(ErrorMessage = "Please select a group")]
        public GroupsModel Groups { get; set; }
    }
}
