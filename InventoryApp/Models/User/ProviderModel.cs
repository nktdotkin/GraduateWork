using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Models.User
{
    internal class ProviderModel : BaseUser
    {
        [Required(ErrorMessage = "Введите название компании")]
        [StringLength(50, MinimumLength = 5)]
        [RegularExpression(@"^[а-яА-Я]+$", ErrorMessage = "Название компании только кириллицей")]
        public string Company { get; set; }
    }
}