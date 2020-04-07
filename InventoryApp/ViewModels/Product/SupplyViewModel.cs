using InventoryApp.Models.Product;
using InventoryApp.ViewModels.Base;
using InventoryApp.Views.Controls;
using InventoryControl.Models.Base;
using System.Collections.ObjectModel;

namespace InventoryApp.ViewModels.Product
{
    class SupplyViewModel : ViewModelsBase
    {
        private const string CommandToExecute = "GetSupply";
        private const string TableName = "Supply";

        public ObservableCollection<SupplyModel> SupplyModels { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public SupplyModel SelectedItem { get; set; }

        public SupplyViewModel()
        {
            SupplyModels = new ObservableCollection<SupplyModel>();
            SupplyModels = new BaseQuery().Fill<SupplyModel>(CommandToExecute);
            DeleteCommand = new RelayCommand((obj) => Delete());
        }

        private void Delete()
        {
            new BaseQuery().Delete(TableName, SelectedItem.Id);
            SupplyModels.Remove(SelectedItem);
        }
    }
}
