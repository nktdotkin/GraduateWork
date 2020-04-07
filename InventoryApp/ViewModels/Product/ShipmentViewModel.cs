using InventoryApp.Models.Product;
using InventoryApp.ViewModels.Base;
using InventoryControl.Models.Base;
using System.Collections.ObjectModel;

namespace InventoryApp.ViewModels.Product
{
    class ShipmentViewModel : ViewModelsBase
    {
        private const string CommandToExecute = "GetShipment";
        private const string TableName = "Shipment";

        public ObservableCollection<ShipmentModel> ShipmentModels { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public ShipmentModel SelectedItem { get; set; }

        public ShipmentViewModel()
        {
            ShipmentModels = new ObservableCollection<ShipmentModel>();
            ShipmentModels = new BaseQuery().Fill<ShipmentModel>(CommandToExecute);
            DeleteCommand = new RelayCommand((obj) => Delete());
        }

        private void Delete()
        {
            new BaseQuery().Delete(TableName, SelectedItem.Id);
            ShipmentModels.Remove(SelectedItem);
        }
    }
}
