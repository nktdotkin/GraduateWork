using InventoryApp.ViewModels.Base;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace InventoryApp.Models.User
{
    public class BaseUser : ViewModelsBase
    {
        private string _phone;

        public int Id { get; set; }
        [Required(ErrorMessage = "Введите имя")]
        [StringLength(30, MinimumLength = 3)]
        public string Name { get; set; }
        [Required(ErrorMessage = "Введите фамилию")]
        [StringLength(30, MinimumLength = 3)]
        public string Surname { get; set; }
        [Required(ErrorMessage = "Введите адрес")]
        [StringLength(50, MinimumLength = 10)]
        public string Adress { get; set; }
        [Required(ErrorMessage = "Введите номер телефона")]
        //[RegularExpression(@"((\(\d{4}\) ?)|(\d{3}-))?\d{3}-\d{4}", ErrorMessage = "Correct phone format (xxxx)xxx-xxxx")]
        public string Phone
        {
            get
            {
                if (_phone == null)
                    return string.Empty;

                switch (_phone.Length)
                {
                    case 7:
                        return Regex.Replace(_phone, @"(\d{3})(\d{4})", "$1-$2");
                    case 10:
                        return Regex.Replace(_phone, @"(\d{3})(\d{3})(\d{4})", "($1) $2-$3");
                    case 11:
                        return Regex.Replace(_phone, @"(\d{4})(\d{3})(\d{2})(\d{2})", "$1-$2-$3-$4");
                    default:
                        return "Неверный формат номера";
                }
            }
            set
            {
                if (value != _phone)
                {
                    _phone = value;
                    OnPropertyChanged(nameof(Phone));
                }
            }
        }
        [Required]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Неверный формат E-mail")]
        public string Email { get; set; }
    }
}
