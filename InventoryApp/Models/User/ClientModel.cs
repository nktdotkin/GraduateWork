using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Models.User
{
    public class ClientModel : BaseUser
    {
        [Required(ErrorMessage = "Выберите статус")]
        public StatusesModel Status { get; set; }
        [Required(ErrorMessage = "Выберите тип магазина")]
        public StoretypesModel StoreType { get; set; }
    }
}
