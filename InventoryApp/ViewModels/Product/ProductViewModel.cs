using InventoryApp.ViewModels.Base;
using System.Collections.ObjectModel;
using InventoryApp.Models.Product;
using InventoryControl.Models.Base;
using System.Linq;

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
            DeleteCommand = new RelayCommand((obj) => Delete());
            Update();
        }

        private string searchText;
        public string SearchText
        {
            get => searchText;
            set
            {
                if (value != searchText)
                {
                    searchText = value;
                    OnPropertyChanged(nameof(SearchText));
                    if (!string.IsNullOrWhiteSpace(searchText))
                    {
                        Update();
                        Find(searchText);
                    }
                    else
                    {
                        Update();
                    }
                }
            }
        }

        private void Update()
        {
            ProductModels = new BaseQuery().Fill<ProductModel>(CommandToExecute);
            OnPropertyChanged(nameof(ProductModels));
        }

        private void Delete()
        {
            new BaseQuery().Delete(TableName, SelectedItem.Id);
            ProductModels.Remove(SelectedItem);
        }

        private void Find(string searchText)
        {
            var searchResult = ProductModels.Where(items =>
            items.Name.Contains(searchText) ||
            items.Group.Contains(searchText) ||
            items.Description.Contains(searchText)
            ).ToList();
            if (!ProductModels.SequenceEqual(searchResult))
            {
                ProductModels.Clear();
                foreach (var items in searchResult)
                {
                    ProductModels.Add(items);
                }
                OnPropertyChanged(nameof(ProductModels));
            }
        }
    }
}
