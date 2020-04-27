using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Models.User
{
    public class ClientModel : BaseUser
    {
        [Required(ErrorMessage = "Please select status")]
        public string Status { get; set; }
        [Required(ErrorMessage = "Please select store type")]
        public string StoreType { get; set; }
    }
}
