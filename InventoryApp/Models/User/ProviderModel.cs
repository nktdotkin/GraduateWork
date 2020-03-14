using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryApp.Models.User
{
    class ProviderModel : BaseUser
    {
        public string Company { get; set; }

        public ProviderModel(int id, string name, string surname, string adress, string phone, string email, string company)
            : base(id, name, surname, adress, phone, email)
        {
            Company = company;
        }
    }
}
