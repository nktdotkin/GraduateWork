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
    internal class ProductViewModel : ViewModelsBase
    {
        public ProductViewModel()
        {
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
                if (value == selectedItem) return;
                selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        private string searchText;

        public string SearchText
        {
            get => searchText;
            set
            {
                if (value == searchText) return;
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

        private GroupsModel searchByGroup;

        public GroupsModel SearchByGroup
        {
            get => searchByGroup;
            set
            {
                if (value == searchByGroup) return;
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

        #endregion Properties

        #region Functions

        private void Update(bool onlyProduct = false, bool isFirstStart = false)
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
            if (!isFirstStart) return;
            CheckStorageCapacity();
            Task.Run(DeleteOutdatingProducts);
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
                    AddNewProduct.GroupId = GroupsModels.First(items => items.Group == AddNewProduct.Groups.Group).Id;
                    bool isCompleted = BaseQueryService.Add(DataBaseTableNames.Product, AddNewProduct);
                    if (!isCompleted) return;
                    Notification.ShowNotification($"Инфо: {AddNewProduct.Name} добавлен.");
                    Task.Run(() => Update());
                }
                else
                {
                    Notification.ShowNotification("Ошибка: Добавление товара произошло с ошибкой.");
                }
            }
        }

        private void AddProductImage()
        {
            var fileDialog = new OpenFileDialog();
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
            var fileDialog = new OpenFileDialog();
            fileDialog.ShowDialog();
            string fileExtension = Path.GetExtension(fileDialog.FileName);
            if (string.IsNullOrEmpty(fileDialog.FileName) || !fileExtension.Contains("docx")) return;
            var recordsList = DocumentService.GetFromDocument(fileDialog.FileName);
            var newProduct = new List<string>();
            int j = 0;
            for (int i = 0; i < recordsList.Count; i++)
            {
                if (recordsList.Contains("Product"))
                {
                    newProduct.Add(recordsList[j + 1]);
                }
                j++;
            }
            int counter = 0;
            foreach (var fields in AddNewProduct.GetType().GetProperties().OrderBy(x => x.MetadataToken))
            {
                var productProperty = AddNewProduct.GetType().GetProperty(fields.Name);
                if (fields.Name == "Id" || fields.Name == "GroupId") continue;
                productProperty.SetValue(AddNewProduct,
                    fields.Name.Contains("Groups")
                        ? Convert.ChangeType(GroupsModels.First(group => @group.Group.Contains(newProduct[counter])),
                            productProperty.PropertyType)
                        : Convert.ChangeType(newProduct[counter], productProperty.PropertyType));
                counter++;
            }
            Add();
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

        private bool CheckStorageCapacity()
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
            if (ProductModels.SequenceEqual(searchResult)) return;
            {
                ProductModels.Clear();
                foreach (var items in searchResult)
                {
                    ProductModels.Add(items);
                }
                OnPropertyChanged(nameof(ProductModels));
            }
        }

        #endregion Functions
    }
}