using InventoryApp.Models.Product;
using InventoryApp.Service;
using InventoryApp.ViewModels.Base;
using InventoryApp.ViewModels.Common;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryApp.ViewModels.Product
{
    class ProductViewModel : ViewModelsBase
    {
        public ProductViewModel()
        {
            GC.Collect(1, GCCollectionMode.Forced);
            DeleteCommand = new RelayCommand((obj) => Delete());
            AddCommand = new RelayCommand((obj) => Add());
            AddProductImageCommand = new RelayCommand((obj) => AddProductImage());
            AddNewProduct = new ProductModel();
            Notification = new NotificationServiceViewModel();
            ModelValidation = new ValidationService<ProductModel>();
            Task.Run(() => Update(true));
        }

        #region Properties
        private const string TableName = "Product";
        public ObservableCollection<ProductModel> ProductModels { get; set; }
        public ObservableCollection<ProductModel> OutdatedProductModels { get; set; }
        public ObservableCollection<GroupsModel> GroupsModels { get; set; }
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
                        var updateTask = Task.Run(() => Update());
                        Task.WaitAll(updateTask);
                        Find(searchText);
                    }
                    else
                    {
                        Task.Run(() => Update());
                    }
                }
            }
        }

        private GroupsModel searchByGroup;
        public GroupsModel SearchByGroup
        {
            get => searchByGroup;
            set
            {
                if (value != searchByGroup)
                {
                    searchByGroup = value;
                    OnPropertyChanged(nameof(SearchByGroup));
                    if (!string.IsNullOrWhiteSpace(searchByGroup.Group))
                    {
                        var updateTask = Task.Run(() => Update());
                        Task.WaitAll(updateTask);
                        Find(searchByGroup.Group);
                    }
                    else
                    {
                        Task.Run(() => Update());
                    }
                }
            }
        }
        #endregion

        #region Functions
        public ObservableCollection<ProductModel> Update(bool isFirstStart = false)
        {
            GroupsModels = new BaseQueryService().Fill<GroupsModel>(($"GetGroups"));
            OutdatedProductModels = new BaseQueryService().Fill<ProductModel>(($"GetOutdatedProducts"));
            ProductModels = new BaseQueryService().Fill<ProductModel>(($"Get{TableName}"));
            OnPropertyChanged(nameof(ProductModels));
            if (isFirstStart)
            {
                CheckStorageCapacity();
                Task.Run(() => DeleteOutdatingProducts());
            }
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
            Task.Run(() => Update());
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
                if (CheckStorageCapacity())
                {
                    AddNewProduct.GroupId = GroupsModels.Where(items => items.Group == AddNewProduct.Groups.Group).First().Id;
                    bool isCompleted = new BaseQueryService().Add(TableName, AddNewProduct);
                    if (isCompleted)
                    {
                        Notification.ShowNotification($"Info: {AddNewProduct.Name} is added.");
                    }
                }
                else
                {
                    Notification.ShowNotification("Error: Adding new supply information failed.");
                }
            }
            Task.Run(() => Update());
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
                bool isDeleted = new BaseQueryService().Delete(TableName, oudatedPorducts.Id);
                bool isAdded = new BaseQueryService().Add("OutdatedProduct", oudatedPorducts);
                if (isDeleted && isAdded)
                {
                    Notification.ShowNotification("Info: Outdated product is deleted.");
                }
                else
                {
                    Notification.ShowNotification("Error: Deleting outdated product failed.");
                }
            }
            Task.Run(() => Update());
        }

        public bool CheckStorageCapacity()
        {
            var productCount = ProductModels.Select(items => items.Amount).Sum();
            var occupiedSpace = (productCount * 100) / Properties.Settings.Default.MaxCapacity;
            Properties.Settings.Default.ActualCapacity = productCount;
            Notification.ShowNotification($"Info: Used storage space is {occupiedSpace}%");
            return (occupiedSpace < 99) ? true : false;
        }

        private void Find(string searchText)
        {
            var searchResult = ProductModels.Where(items =>
            items.Name.Contains(searchText) ||
            items.Groups.Group.Contains(searchText) ||
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
