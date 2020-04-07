using InventoryApp.ViewModels.Base;
using System.Collections.ObjectModel;
using InventoryApp.Models.Product;
using InventoryControl.Models.Base;

namespace InventoryApp.ViewModels.Product
{
    class ProductViewModel : ViewModelsBase
    {
        private const string CommandToExecute = "GetProduct";
        private const string TableName = "Product";

        public ObservableCollection<ProductModel> ProductModels { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public ProductModel SelectedItem { get; set; }

        public ProductViewModel()
        {
            ProductModels = new ObservableCollection<ProductModel>();
            ProductModels = new BaseQuery().Fill<ProductModel>(CommandToExecute);
            DeleteCommand = new RelayCommand((obj) => Delete());
        }

        private void Delete()
        {
            new BaseQuery().Delete(TableName, SelectedItem.Id);
            ProductModels.Remove(SelectedItem);
        }
    }
}
