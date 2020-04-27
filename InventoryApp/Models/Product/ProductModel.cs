using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Models.Product
{
    class ProductModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Uncorrect name")]
        [StringLength(50, MinimumLength = 5)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Uncorrect description")]
        [StringLength(50, MinimumLength = 5)]
        public string Description { get; set; }
        public int ExpirationDateDays { get; set; }
        public int Amount { get; set; }
        public decimal Price { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalPrice { get; set; }
        public int GroupId { get; set; }
        public string ImageLink { get; set; }
        [Required(ErrorMessage = "Please select a group")]
        public string Group { get; set; }
    }
}
