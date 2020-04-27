using InventoryApp.Models.Base;
using InventoryApp.ViewModels.Base;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace InventoryApp.Models.User
{
    public class BaseUser : ViewModelsBase
    {
        private string _phone;
        private string _email;

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
        [Required(ErrorMessage = "Uncorrect phone number")]
        [StringLength(20, MinimumLength = 7)]
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
                        return Regex.Replace(_phone, @"(\d{1})(\d{3})(\d{3})(\d{4})", "$1-$2-$3-$4");
                    default:
                        return "Unknown";
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
        [Required(ErrorMessage = "Uncorrect Email")]
        [StringLength(50, MinimumLength = 7)]
        public string Email
        {
            get
            {
                if (_email == null)
                {
                    return string.Empty;
                }
                else if (BaseModel.EmailCheck(_email))
                {
                    return _email;
                }
                else
                {
                    return "Uncorrect Email";
                }

            }
            set
            {
                if (value != _email)
                {
                    _email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }
    }
}
