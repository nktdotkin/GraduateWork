using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryApp.Models.User
{
    class BaseUser
    {
        public BaseUser(int id, string name, string surname, string adress, string phone, string email)
        {
            Id = id;
            Name = name;
            Surname = surname;
            Adress = adress;
            Phone = phone;
            Email = email;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Adress { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
    }
}
