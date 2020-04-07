namespace InventoryApp.Models.User
{
    //replace statusID with status desc
    public class ClientModel : BaseUser
    {
        private enum StatusEnum
        {
            Default = 0,
            Bronze = 100,
            Silver = 200,
            Gold = 300,
        }

        public string Status { get; set; }
    }
}
