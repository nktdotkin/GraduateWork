using InventoryApp.Models.Product;
using InventoryApp.Service;
using InventoryApp.ViewModels.Base;
using InventoryApp.ViewModels.Common;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace InventoryApp.ViewModels.Product
{
    class ProductViewModel : ViewModelsBase
    {
        public ProductViewModel()
        {
            GC.Collect(1, GCCollectionMode.Forced);
            ProductModels = new ObservableCollection<ProductModel>();
            DeleteCommand = new RelayCommand((obj) => Delete());
            AddCommand = new RelayCommand((obj) => Add());
            AddProductImageCommand = new RelayCommand((obj) => AddProductImage());
            AddNewProduct = new ProductModel();
            Notification = new NotificationServiceViewModel();
            DataBaseStaticModels = new DataBaseStaticObjects();
            ModelValidation = new ValidationService<ProductModel>();
            Update();
            DeleteOutdatingProducts();
        }

        #region Properties
        private const string TableName = "Product";
        public ObservableCollection<ProductModel> ProductModels { get; set; }
        public DataBaseStaticObjects DataBaseStaticModels { get; set; }
        private ValidationService<ProductModel> ModelValidation { get; set; }

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
        public ObservableCollection<ProductModel> Update()
        {
            ProductModels = new BaseQueryService().Fill<ProductModel>(($"Get{TableName}"));
            OnPropertyChanged(nameof(ProductModels));
            return ProductModels;
        }

        private void Delete()
        {
            if (SelectedItem?.Id != null)
            {
                bool isCompleted = new BaseQueryService().Delete(TableName, SelectedItem.Id);
                if (isCompleted)
                {
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
            Update();
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
                bool isCompleted = new BaseQueryService().Add(TableName, AddNewProduct);
                if (isCompleted)
                {
                    Notification.ShowNotification($"Info: {AddNewProduct.Name} is added.");
                }
                else
                {
                    Notification.ShowNotification("Error: Adding new product failed.");
                }
            }
            Update();
        }

        private void AddProductImage()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.ShowDialog();
            if (!string.IsNullOrWhiteSpace(fileDialog.FileName))
            {
                AddNewProduct.ImageLink = fileDialog.FileName;
                Notification.ShowNotification($"Info: File {fileDialog.SafeFileName} is added.");
            }
            else
            {
                Notification.ShowNotification($"Info: File is not added.");
            }
        }

        private void DeleteOutdatingProducts()
        {
            foreach (var oudatedPorducts in ProductModels.Where(items => (items.ExpirationDateDays.DayOfYear.Equals(DateTime.Now.DayOfYear - 1)) && items.ExpirationDateDays.Year.Equals(DateTime.Now.Year)))
            {
                bool isCompleted = new BaseQueryService().Delete(TableName, oudatedPorducts.Id);
                if (isCompleted)
                {
                    Notification.ShowNotification("Info: Outdated product is deleted.");
                }
                else
                {
                    Notification.ShowNotification("Error: Deleting outdated product failed.\n\t It can be referenced in Shipment");
                }
            }
            Update();
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
