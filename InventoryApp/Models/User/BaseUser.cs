using InventoryApp.Models.Base;
using InventoryApp.ViewModels.Base;
using System.Text.RegularExpressions;

namespace InventoryApp.Models.User
{
    public class BaseUser : ViewModelsBase
    {
        private string _phone;
        private string _email;

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Adress { get; set; }
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
        public string Email
        {
            get
            {
                if (_phone == null)
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
