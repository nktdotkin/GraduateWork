using InventoryApp.Models.Product;
using InventoryApp.Models.User;
using InventoryApp.Service;
using InventoryApp.ViewModels.Base;
using InventoryApp.ViewModels.Common;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryApp.ViewModels.Product
{
    internal class SupplyViewModel : ViewModelsBase
    {
        public SupplyViewModel()
        {
            DeleteCommand = new RelayCommand((obj) => Delete());
            AddCommand = new RelayCommand((obj) => Add());
            Task.Run(() => Update());
        }

        #region Properties

        public ObservableCollection<SupplyModel> SupplyModels { get; set; }
        public ObservableCollection<ProviderModel> ProviderModels { get; set; }
        public ObservableCollection<ProductModel> ProductModels { get; set; }

        public NotificationServiceViewModel Notification { get; set; } = new NotificationServiceViewModel();

        private BaseQueryService BaseQueryService = new BaseQueryService();

        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand AddCommand { get; set; }

        public SupplyModel AddNewSupply { get; set; } = new SupplyModel();

        public bool SpinnerVisibility { get; set; }

        private SupplyModel selectedItem;

        public SupplyModel SelectedItem
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

        private ProviderModel searchByProvider;

        public ProviderModel SearchByProvider
        {
            get => searchByProvider;
            set
            {
                if (value == searchByProvider) return;
                searchByProvider = value;
                OnPropertyChanged(nameof(SearchByProvider));
                if (!string.IsNullOrWhiteSpace(searchByProvider.Company))
                {
                    var updateTask = Task.Run(() => Update(true));
                    Task.WaitAll(updateTask);
                    Find(searchByProvider.Company);
                }
                else
                {
                    Task.Run(() => Update(true));
                }
            }
        }

        #endregion Properties

        #region Functions

        private void Update(bool onlySupply = false)
        {
            if (!onlySupply)
            {
                ProviderModels = BaseQueryService.Fill<ProviderModel>(($"Get{DataBaseTableNames.Provider}"));
                ProductModels = BaseQueryService.Fill<ProductModel>(($"Get{DataBaseTableNames.Product}"));
                OnPropertyChanged(nameof(ProviderModels));
                OnPropertyChanged(nameof(ProductModels));
            }
            SupplyModels = BaseQueryService.Fill<SupplyModel>(($"Get{DataBaseTableNames.Supply}"));
            OnPropertyChanged(nameof(SupplyModels));
        }

        private void Delete()
        {
            if (SelectedItem?.Id != null)
            {
                bool isCompleted = new BaseQueryService().Delete(DataBaseTableNames.Supply, SelectedItem.Id);
                if (isCompleted)
                {
                    Notification.ShowNotification("Инфо: Информация удалена.");
                    SupplyModels.Remove(SelectedItem);
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
        }

        private void CreateDocument()
        {
            var exportMessage = new DocumentService().ExportInformationToFile(AddNewSupply, "Поставки");
            Notification.ShowNotification(exportMessage);
        }

        private void HideSpinner()
        {
            this.SpinnerVisibility = false;
            OnPropertyChanged(nameof(this.SpinnerVisibility));
        }

        private void ShowSpinner()
        {
            this.SpinnerVisibility = true;
            OnPropertyChanged(nameof(this.SpinnerVisibility));
        }

        private void Add()
        {
            var errorList = new ValidationService<SupplyModel>().ValidateFields(AddNewSupply);
            if (errorList.Any())
            {
                Notification.ShowListNotification(errorList);
            }
            else
            {
                if ((Properties.Settings.Default.MaxCapacity - Properties.Settings.Default.ActualCapacity) > AddNewSupply.Amount)
                {
                    bool isCompleted = BaseQueryService.
                        ExecuteQuery<ShipmentModel>($"INSERT INTO {DataBaseTableNames.Supply} VALUES (N'{AddNewSupply.Date}', {AddNewSupply.Amount}, {AddNewSupply.Product.Id}, {AddNewSupply.Provider.Id})");
                    if (isCompleted)
                    {
                        Notification.ShowNotification($"Инфо: Поставка для {AddNewSupply.Product.Name} добавлена.");
                        ShowSpinner();
                        Task.Run(CreateDocument);
                        BaseQueryService.
                            ExecuteQuery<ShipmentModel>($"Update Product set ProductAmount={ProductModels.Where(item => item.Id == AddNewSupply.Product.Id).Select(item => item.Amount).First() + AddNewSupply.Amount} where ProductId = {AddNewSupply.Product.Id}");
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
            items.Provider.Company.Contains(searchText) ||
            items.Provider.Surname.Contains(searchText)
            ).ToList();
            if (SupplyModels.SequenceEqual(searchResult)) return;
            {
                SupplyModels.Clear();
                foreach (var items in searchResult)
                {
                    SupplyModels.Add(items);
                }
                OnPropertyChanged(nameof(SupplyModels));
            }
        }

        #endregion Functions
    }
}