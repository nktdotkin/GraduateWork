using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Models.User
{
    class ProviderModel : BaseUser
    {
        [Required(ErrorMessage = "Выберите название компании")]
        [StringLength(50, MinimumLength = 5)]
        public string Company { get; set; }
    }
}
