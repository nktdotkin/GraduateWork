using InventoryApp.Models.Product;
using InventoryApp.Service;
using InventoryApp.ViewModels.Base;
using InventoryApp.ViewModels.Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
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
            ImportCommand = new RelayCommand((obj) => GetProductFromFile());
            AddProductImageCommand = new RelayCommand((obj) => AddProductImage());
            AddNewProduct = new ProductModel();
            Notification = new NotificationServiceViewModel();
            BaseQueryService = new BaseQueryService();
            Task.Run(() => Update(false, true));
        }

        #region Properties
        public ObservableCollection<ProductModel> ProductModels { get; set; }
        public ObservableCollection<ProductModel> OutdatedProductModels { get; set; }
        public ObservableCollection<GroupsModel> GroupsModels { get; set; }

        private BaseQueryService BaseQueryService;

        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand AddCommand { get; set; }
        public RelayCommand AddProductImageCommand { get; set; }
        public RelayCommand ImportCommand { get; set; }

        public ProductModel AddNewProduct { get; set; }
        public NotificationServiceViewModel Notification { get; set; }

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
                        var updateTask = Task.Run(() => Update(true));
                        Task.WaitAll(updateTask);
                        Find(searchText);
                    }
                    else
                    {
                        Task.Run(() => Update(true));
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
                        var updateTask = Task.Run(() => Update(true));
                        Task.WaitAll(updateTask);
                        Find(searchByGroup.Group);
                    }
                    else
                    {
                        Task.Run(() => Update(true));
                    }
                }
            }
        }
        #endregion

        #region Functions
        public void Update(bool onlyProduct = false, bool isFirstStart = false)
        {
            if (!onlyProduct)
            {
                GroupsModels = BaseQueryService.Fill<GroupsModel>($"Get{DataBaseTableNames.Groups}");
                OutdatedProductModels = BaseQueryService.Fill<ProductModel>($"Get{DataBaseTableNames.OutdatedProduct}");
                OnPropertyChanged(nameof(GroupsModels));
                OnPropertyChanged(nameof(OutdatedProductModels));
            }
            ProductModels = BaseQueryService.Fill<ProductModel>($"Get{DataBaseTableNames.Product}");
            OnPropertyChanged(nameof(ProductModels));
            if (isFirstStart)
            {
                CheckStorageCapacity();
                Task.Run(() => DeleteOutdatingProducts());
            }
        }

        private void Delete()
        {
            if (SelectedItem?.Id != null)
            {
                bool isCompleted = BaseQueryService.Delete(DataBaseTableNames.Product, SelectedItem.Id);
                if (isCompleted)
                {
                    Notification.ShowNotification("Инфо: Товар удален.");
                    Task.Run(() => Update());
                }
                else
                {
                    Notification.ShowNotification("Ошибка: Удаление завершилось с ошибкой.");
                }
            }
            else
            {
                Notification.ShowNotification("Ошибка: Выберите товар.");
            }
        }

        private void Add()
        {
            var errorList = new ValidationService<ProductModel>().ValidateFields(AddNewProduct);
            if (errorList.Any())
            {
                Notification.ShowListNotification(errorList);
            }
            else
            {
                if (CheckStorageCapacity())
                {
                    AddNewProduct.GroupId = GroupsModels.Where(items => items.Group == AddNewProduct.Groups.Group).First().Id;
                    bool isCompleted = BaseQueryService.Add(DataBaseTableNames.Product, AddNewProduct);
                    if (isCompleted)
                    {
                        Notification.ShowNotification($"Инфо: {AddNewProduct.Name} добавлен.");
                        Task.Run(() => Update());
                    }
                }
                else
                {
                    Notification.ShowNotification("Ошибка: Добавление товара произошло с ошибкой.");
                }
            }
        }

        private void AddProductImage()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.ShowDialog();
            string fileExtension = Path.GetExtension(fileDialog.FileName);
            if (!string.IsNullOrWhiteSpace(fileDialog.FileName) && Regex.IsMatch(fileExtension, @"((\.(?i)(jpg|png|gif|bmp))$)"))
            {
                AddNewProduct.ImageLink = fileDialog.FileName;
                Notification.ShowNotification($"Инфо: Файл {fileDialog.SafeFileName} добавлен.");
            }
            else
            {
                Notification.ShowNotification($"Инфо: Файл не добавлен.");
            }
        }

        private void GetProductFromFile()
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.ShowDialog();
            string fileExtension = Path.GetExtension(fileDialog.FileName);
            if (!string.IsNullOrEmpty(fileDialog.FileName) && fileExtension.Contains("docx"))
            {
                List<string> recordsList = new DocumentService().GetFromDocument(fileDialog.FileName);
                List<string> newProduct = new List<string>();
                int j = 0;
                for (int i = 0; i < recordsList.Count; i++)
                {
                    if (recordsList[i].Contains("Product"))
                    {
                        newProduct.Add(recordsList[j + 1]);
                    }
                    j++;
                }
                int counter = 0;
                foreach (var fields in AddNewProduct.GetType().GetProperties().OrderBy(x => x.MetadataToken))
                {
                    var productProperty = AddNewProduct.GetType().GetProperty(fields.Name);
                    if (fields.Name != "Id" && fields.Name != "GroupId")
                    {
                        if (fields.Name.Contains("Groups"))
                        {
                            productProperty.SetValue(AddNewProduct, Convert.ChangeType(GroupsModels.Where(group => group.Group.Contains(newProduct[counter])).First(), productProperty.PropertyType));
                        }
                        else
                        {
                            productProperty.SetValue(AddNewProduct, Convert.ChangeType(newProduct[counter], productProperty.PropertyType));
                        }
                        counter++;
                    }
                }
                Add();
            }
        }

        private void DeleteOutdatingProducts()
        {
            foreach (var oudatedPorducts in ProductModels.Where(items => (items.ExpirationDateDays.DayOfYear.Equals(DateTime.Now.DayOfYear - 1)) && items.ExpirationDateDays.Year.Equals(DateTime.Now.Year)))
            {
                bool isDeleted = BaseQueryService.Delete(DataBaseTableNames.Product, oudatedPorducts.Id);
                bool isAdded = BaseQueryService.Add("OutdatedProduct", oudatedPorducts);
                if (isDeleted && isAdded)
                {
                    Notification.ShowNotification("Инфо: Товары списаны.");
                    Task.Run(() => Update());
                }
                else
                {
                    Notification.ShowNotification("Ошибка: Списание товаров завершилось с ошибкой.");
                }
            }
        }

        public bool CheckStorageCapacity()
        {
            var productCount = ProductModels.Select(items => items.Amount).Sum();
            var occupiedSpace = (productCount * 100) / Properties.Settings.Default.MaxCapacity;
            Properties.Settings.Default.ActualCapacity = productCount;
            Properties.Settings.Default.Save();
            Notification.ShowNotification($"Инфо: Занятое пространство склада {occupiedSpace}%");
            return occupiedSpace < 99;
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
