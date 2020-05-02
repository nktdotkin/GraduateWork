using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Models.User
{
    public class ClientModel : BaseUser
    {
        [Required(ErrorMessage = "Please select status")]
        public StatusesModel Status { get; set; }
        [Required(ErrorMessage = "Please select store type")]
        public StoretypesModel StoreType { get; set; }
    }
}
