using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Models.User
{
    class ProviderModel : BaseUser
    {
        [Required(ErrorMessage = "Введите название компании")]
        [StringLength(50, MinimumLength = 5)]
        [RegularExpression(@"^[а-яА-Я]+$", ErrorMessage = "Название компании только латиницей")]
        public string Company { get; set; }
    }
}
