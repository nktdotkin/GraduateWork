using InventoryApp.Models.User;
using InventoryApp.ViewModels.Base;
using System.Collections.ObjectModel;

namespace InventoryApp.ViewModels.User
{
    class ProviderViewModel
    {
        private const string CommandToExecute = "GetProvider";
        public ObservableCollection<ProviderModel> ProviderModels { get; set; }

        public ProviderViewModel()
        {
            ProviderModels = new ObservableCollection<ProviderModel>();
            var userCollectionModels = new BaseQuery().Fill<ProviderModel>(CommandToExecute);
            foreach (var userFiled in userCollectionModels)
            {
                ProviderModels.Add(userFiled);
            }
        }
    }
}
