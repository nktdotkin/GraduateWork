using InventoryApp.Models.Product;
using InventoryApp.Models.User;
using InventoryApp.Service;
using InventoryApp.ViewModels.Base;
using InventoryApp.ViewModels.Common;
using InventoryApp.ViewModels.User;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryApp.ViewModels.Product
{
    class SupplyViewModel : ViewModelsBase
    {
        public SupplyViewModel()
        {
            GC.Collect(1, GCCollectionMode.Forced);
            SupplyModels = new ObservableCollection<SupplyModel>();
            DeleteCommand = new RelayCommand((obj) => Delete());
            AddCommand = new RelayCommand((obj) => Add());
            AddNewSupply = new SupplyModel();
            Notification = new NotificationServiceViewModel();
            ModelValidation = new ValidationService<SupplyModel>();
            Task.Run(() => Update());
        }

        #region Properties
        private const string TableName = "Supply";
        public ObservableCollection<SupplyModel> SupplyModels { get; set; }
        public ObservableCollection<ProviderModel> ProviderModels { get; set; }
        public ObservableCollection<ProductModel> ProductModels { get; set; }

        private ValidationService<SupplyModel> ModelValidation { get; set; }
        public NotificationServiceViewModel Notification { get; set; }

        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand AddCommand { get; set; }

        private SupplyModel selectedItem;
        public SupplyModel SelectedItem
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

        public SupplyModel AddNewSupply { get; set; }

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

        private ProviderModel searchByProvider;
        public ProviderModel SearchByProvider
        {
            get => searchByProvider;
            set
            {
                if (value != searchByProvider)
                {
                    searchByProvider = value;
                    OnPropertyChanged(nameof(SearchByProvider));
                    if (!string.IsNullOrWhiteSpace(searchByProvider.Company))
                    {
                        //var updateTask = Task.Run(() => Update());
                        //Task.WaitAll(updateTask);
                        Find(searchByProvider.Company);
                    }
                    else
                    {
                        Task.Run(() => Update());
                    }
                }
            }
        }

        private bool spinnerVisibility;
        public bool SpinnerVisibility
        {
            get => spinnerVisibility;
            set
            {
                if (value != spinnerVisibility)
                {
                    spinnerVisibility = value;
                    OnPropertyChanged(nameof(SpinnerVisibility));
                }
            }
        }
        #endregion

        #region Functions
        private void Update(bool onlySupply = false)
        {
            if (!onlySupply)
            {
                ProviderModels = new ProviderViewModel().Update();
                ProductModels = new ProductViewModel().Update(true);
            }
            SupplyModels = new BaseQueryService().Fill<SupplyModel>(($"Get{TableName}"));
            OnPropertyChanged(nameof(SupplyModels));
            OnPropertyChanged(nameof(ProviderModels));
            OnPropertyChanged(nameof(ProductModels));
        }

        private void Delete()
        {
            if (SelectedItem?.Id != null)
            {
                bool isCompleted = new BaseQueryService().Delete(TableName, SelectedItem.Id);
                if (isCompleted)
                {
                    Notification.ShowNotification("Инфо: Информация удалена.");
                }
                else
                {
                    Notification.ShowNotification("Ошибка: Удаление завершилось с ошибкой.");
                }
            }
            else
            {
                Notification.ShowNotification("Ошибка: Выберите информацию для удаления.");
            }
            Task.Run(() => Update());
        }

        private void CreateDocument()
        {
            var exportMessage = new DocumentService().ExportInformationToFile(AddNewSupply, "Поставки");
            Notification.ShowNotification(exportMessage);
        }

        private void HideSpinner()
        {
            this.spinnerVisibility = false;
            OnPropertyChanged(nameof(this.SpinnerVisibility));
        }

        private void Add()
        {
            var errorList = ModelValidation.ValidateFields(AddNewSupply);
            if (errorList.Any())
            {
                Notification.ShowListNotification(errorList);
            }
            else
            {
                if ((Properties.Settings.Default.MaxCapacity - Properties.Settings.Default.ActualCapacity) > AddNewSupply.Amount)
                {
                    bool isCompleted = new BaseQueryService().ExecuteQuery<ShipmentModel>($"INSERT INTO {TableName} VALUES (N'{AddNewSupply.Date}', {AddNewSupply.Amount}, {AddNewSupply.Product.Id}, {AddNewSupply.Provider.Id})");
                    if (isCompleted)
                    {
                        Notification.ShowNotification($"Инфо: Поставка для {AddNewSupply.Product.Name} добавлена.");
                        SpinnerVisibility = true;
                        Task.Run(() => CreateDocument());
                        new BaseQueryService().ExecuteQuery<ShipmentModel>($"Update Product set ProductAmount={ProductModels.Where(item => item.Id == AddNewSupply.Product.Id).Select(item => item.Amount).First() + AddNewSupply.Amount} where ProductId = {AddNewSupply.Product.Id}");
                    }
                    Task.Run(() => Update(true));
                    BaseService.DelayAction(300, () => HideSpinner());
                }
                else
                {
                    Notification.ShowNotification("Ошибка: Добавление информации произошло с ошибкой.");
                }
            }
        }

        private void Find(string searchText)
        {
            var searchResult = SupplyModels.Where(items =>
            items.Product.Name.Contains(searchText) ||
            items.Product.Groups.Group.Contains(searchText) ||
            items.Product.Description.Contains(searchText) ||
            items.Provider.Name.Contains(searchText) ||
            items.Provider.Phone.Contains(searchText) ||
            items.Provider.Surname.Contains(searchText)
            ).ToList();
            if (!SupplyModels.SequenceEqual(searchResult))
            {
                SupplyModels.Clear();
                foreach (var items in searchResult)
                {
                    SupplyModels.Add(items);
                }
                OnPropertyChanged(nameof(SupplyModels));
            }
        }
        #endregion
    }
}
