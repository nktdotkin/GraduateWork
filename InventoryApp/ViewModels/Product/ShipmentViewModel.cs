using InventoryApp.Models.Product;
using InventoryApp.ViewModels.Base;
using System.Collections.ObjectModel;

namespace InventoryApp.ViewModels.Product
{
    class ShipmentViewModel : ViewModelsBase
    {
        private const string CommandToExecute = "GetShipment";
        public ObservableCollection<ShipmentModel> ShipmentModels { get; set; }

        public ShipmentViewModel()
        {
            ShipmentModels = new ObservableCollection<ShipmentModel>();
            ShipmentModels = new BaseQuery().Fill<ShipmentModel>(CommandToExecute);
        }
    }
}
