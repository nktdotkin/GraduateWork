using InventoryApp.Models.Base;
using InventoryApp.Models.Product;
using InventoryApp.Models.Service;
using InventoryApp.ViewModels.Base;
using InventoryApp.ViewModels.Service;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Linq;

namespace InventoryApp.ViewModels.Product
{
    class ProductViewModel : ViewModelsBase
    {
        public ProductViewModel()
        {
            ProductModels = new ObservableCollection<ProductModel>();
            DeleteCommand = new RelayCommand((obj) => Delete());
            AddCommand = new RelayCommand((obj) => Add());
            AddProductImageCommand = new RelayCommand((obj) => AddProductImage());
            AddNewProduct = new ProductModel();
            Notification = new NotificationServiceViewModel();
            DataBaseStaticModels = new DataBaseStaticModels();
            ModelValidation = new ValidationViewModel<ProductModel>();
            Update();
        }

        #region Properties
        private const string TableName = "Product";
        public ObservableCollection<ProductModel> ProductModels { get; set; }
        public DataBaseStaticModels DataBaseStaticModels { get; set; }
        private ValidationViewModel<ProductModel> ModelValidation { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand AddCommand { get; set; }
        public RelayCommand AddProductImageCommand { get; set; }

        private ProductModel selectedItem;
        public ProductModel SelectedItem
        {
            get => selectedItem;
            set
            {
                if (value != selectedItem)
                {
                    selectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));
                }
            }
        }

        public ProductModel AddNewProduct { get; set; }
        public NotificationServiceViewModel Notification { get; set; }

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
        #endregion

        #region Functions
        private void Update()
        {
            ProductModels.Clear();
            ProductModels = new BaseQuery().Fill<ProductModel>(($"Get{TableName}"));
            OnPropertyChanged(nameof(ProductModels));
        }

        private void Delete()
        {
            if (SelectedItem?.Id != null)
            {
                bool isCompleted = new BaseQuery().Delete(TableName, SelectedItem.Id);
                if (isCompleted)
                {
                    ProductModels.Remove(SelectedItem);
                    Notification.ShowNotification("Info: Product is deleted.");
                }
                else
                {
                    Notification.ShowNotification("Error: Deleting product failed.");
                }
            }
            else
            {
                Notification.ShowNotification("Error: No products selected.");
            }
        }

        private void Add()
        {
            var errorList = ModelValidation.ValidateFields(AddNewProduct);
            if (errorList.Any())
            {
                Notification.ShowListNotification(errorList);
            }
            else
            {
                bool isCompleted = new BaseQuery().Add(TableName, AddNewProduct);
                if (isCompleted)
                {
                    ProductModels.Add(AddNewProduct);
                    Notification.ShowNotification($"Info: {AddNewProduct.Name} is added.");
                }
                else
                {
                    Notification.ShowNotification("Error: Adding new product failed.");
                }
            }
        }

        private void AddProductImage()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.ShowDialog();
            AddNewProduct.ImageLink = fileDialog.FileName;
            Notification.ShowNotification($"Info: File {fileDialog.SafeFileName} is added.");
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
        #endregion
    }
}
