using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace InventoryApp.Models.Product
{
    class ProductModel
    {
        private DateTime expirationDateDays = DateTime.Now;

        public int Id { get; set; }
        [Required(ErrorMessage = "Введите назавание товара")]
        [StringLength(50, MinimumLength = 5)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Заполните описание")]
        [StringLength(50, MinimumLength = 5)]
        public string Description { get; set; }
        [Required(ErrorMessage = "Выберите дату")]
        public DateTime ExpirationDateDays
        {
            get => DateTime.ParseExact(expirationDateDays.Date.ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);
            set
            {
                expirationDateDays = value;
            }
        }
        public int Amount { get; set; }
        [Required(ErrorMessage = "Цена должна быть больше 0")]
        [Range(0.1, double.MaxValue)]
        public decimal Price { get; set; }
        public decimal TotalPrice { get; set; }
        public int GroupId { get; set; }
        [Required(ErrorMessage = "Выберите изображение")]
        public string ImageLink { get; set; }
        [Required(ErrorMessage = "Выберите группу")]
        public GroupsModel Groups { get; set; }
    }
}
