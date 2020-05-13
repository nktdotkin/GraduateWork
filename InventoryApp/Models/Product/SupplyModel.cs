﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace InventoryApp.Models.Product
{
    class SupplyModel
    {
        private DateTime date = DateTime.Now;

        public int Id { get; set; }
        public DateTime Date
        {
            get => DateTime.ParseExact(date.Date.ToString("dd-MM-yyyy"), "dd-MM-yyyy", CultureInfo.InvariantCulture);
            set
            {
                date = value;
            }
        }
        [Required(ErrorMessage = "Количество должно быть больше 0")]
        [Range(1, Int32.MaxValue)]
        public int Amount { get; set; }
        [Required(ErrorMessage = "Выберите товар")]
        public ProductModel Product { get; set; }
        [Required(ErrorMessage = "Выберите поставщика")]
        public User.ProviderModel Provider { get; set; }
    }
}
