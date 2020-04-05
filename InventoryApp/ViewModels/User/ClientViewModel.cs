using InventoryApp.ViewModels.Base;
using System.Collections.ObjectModel;
using InventoryApp.Models.User;

namespace InventoryApp.ViewModels.User
{
    class ClientViewModel : ViewModelsBase
    {
        private const string CommandToExecute = "GetClient";
        public ObservableCollection<ClientModel> ClientModels { get; set; }

        public ClientViewModel()
        {
            ClientModels = new ObservableCollection<ClientModel>();
            ClientModels = new BaseQuery().Fill<ClientModel>(CommandToExecute);
        }     
    }
}
