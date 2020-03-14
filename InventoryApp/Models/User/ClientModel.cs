using InventoryApp.Models.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryApp.Models.User
{
    class ClientModel : BaseUser
    {
        enum Status
        {
            Default = 0,
            Bronze = 100,
            Silver = 200,
            Gold = 300,
        }

        public ClientModel(int id, string name, string surname, string adress, string phone, string email, string status)
            :base(id,name,surname,adress,phone,email)
        {
            Status ClientStatus = status.ToEnum<Status>();
        }
    }
}
