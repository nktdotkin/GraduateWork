using InventoryApp.ViewModels.Base;
using System.Collections.ObjectModel;
using InventoryApp.Models.User;
using InventoryControl.Models.Base;

namespace InventoryApp.ViewModels.User
{
    class ClientViewModel : ViewModelsBase
    {
        private const string CommandToExecute = "GetClient";
        private const string TableName = "Client";
        public ObservableCollection<ClientModel> ClientModels { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public ClientModel SelectedItem { get; set; }

        public ClientViewModel()
        {
            ClientModels = new ObservableCollection<ClientModel>();
            ClientModels = new BaseQuery().Fill<ClientModel>(CommandToExecute);
            DeleteCommand = new RelayCommand((obj) => Delete());
        }

        private void Delete()
        {
            new BaseQuery().Delete(TableName, SelectedItem.Id);
            ClientModels.Remove(SelectedItem);
        }
    }
}
