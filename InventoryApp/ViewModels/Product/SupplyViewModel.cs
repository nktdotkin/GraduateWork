using InventoryApp.Models.Product;
using InventoryApp.ViewModels.Base;
using System.Collections.ObjectModel;

namespace InventoryApp.ViewModels.Product
{
    class SupplyViewModel : ViewModelsBase
    {
        private const string CommandToExecute = "GetSupply";
        public ObservableCollection<SupplyModel> SupplyModels { get; set; }

        public SupplyViewModel()
        {
            SupplyModels = new ObservableCollection<SupplyModel>();
            SupplyModels = new BaseQuery().Fill<SupplyModel>(CommandToExecute);
        }
    }
}
