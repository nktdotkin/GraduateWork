using InventoryApp.ViewModels.Base;
using System.Collections.ObjectModel;
using InventoryApp.Models.Product;

namespace InventoryApp.ViewModels.Product
{
    class ProductViewModel : ViewModelsBase
    {
        private const string CommandToExecute = "GetProduct";
        public ObservableCollection<ProductModel> ProductModels { get; set; }

        public ProductViewModel()
        {
            ProductModels = new ObservableCollection<ProductModel>();
            ProductModels = new BaseQuery().Fill<ProductModel>(CommandToExecute);
        }
    }
}
