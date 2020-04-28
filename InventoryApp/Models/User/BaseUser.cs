using InventoryApp.ViewModels.Base;
using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Models.User
{
    public class BaseUser : ViewModelsBase
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Uncorrect name")]
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Uncorrect surname")]
        [StringLength(30, MinimumLength = 3)]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Uncorrect adress")]
        [StringLength(50, MinimumLength = 10)]
        public string Adress { get; set; }
        [Required]
        [RegularExpression(@"((\(\d{4}\) ?)|(\d{3}-))?\d{3}-\d{4}", ErrorMessage = "Correct phone format (xxxx)xxx-xxxx")]
        public string Phone { get; set; }
        [Required]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Uncorrect email format")]
        public string Email { get; set; }
    }
}
