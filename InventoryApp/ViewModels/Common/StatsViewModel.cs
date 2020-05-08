using InventoryApp.Models.Common;
using InventoryApp.Models.Product;
using InventoryApp.Service;
using InventoryApp.ViewModels.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace InventoryApp.ViewModels.Common
{
    class StatsViewModel : ViewModelsBase
    {
        public StatsModel StatsModel { get; set; }
        public ObservableCollection<ShipmentModel> ShipmentModels { get; set; }
        public ObservableCollection<SupplyModel> SupplyModels { get; set; }
        public List<string> messageList { get; set; }
        public StatsViewModel()
        {
            messageList = new LogService().ReadFromFile();
            ShipmentModels = new BaseQueryService().Fill<ShipmentModel>(($"GetShipment"));
        }

        private void GetStatsByDate()
        {

        }
    }
}
