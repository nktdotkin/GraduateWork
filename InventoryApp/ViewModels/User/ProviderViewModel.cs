using InventoryApp.Models.User;
using InventoryApp.ViewModels.Base;
using InventoryControl.Models.Base;
using System.Collections.ObjectModel;

namespace InventoryApp.ViewModels.User
{
    class ProviderViewModel : ViewModelsBase
    {
        private const string CommandToExecute = "GetProvider";
        private const string TableName = "Provider";

        public ObservableCollection<ProviderModel> ProviderModels { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public ProviderModel SelectedItem { get; set; }

        public ProviderViewModel()
        {
            ProviderModels = new ObservableCollection<ProviderModel>();
            ProviderModels = new BaseQuery().Fill<ProviderModel>(CommandToExecute);
            DeleteCommand = new RelayCommand((obj) => Delete());
        }

        private void Delete()
        {
            new BaseQuery().Delete(TableName, SelectedItem.Id);
            ProviderModels.Remove(SelectedItem);
        }
    }
}
