using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Models.Product
{
    class GroupsModel
    {
        public int Id { get; set; }
        public string Group { get; set; }
        [Required(ErrorMessage = "Tax must be more than zero")]
        [Range(0.1, double.MaxValue)]
        public decimal Tax { get; set; }
    }
}
