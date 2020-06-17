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
    internal class ShipmentViewModel : ViewModelsBase
    {
        public ShipmentViewModel()
        {
            DeleteCommand = new RelayCommand((obj) => Delete());
            AddCommand = new RelayCommand((obj) => Add());
            Task.Run(() => Update());
        }

        #region Properties

        public ObservableCollection<ShipmentModel> ShipmentModels { get; set; }
        public ObservableCollection<ClientModel> ClientModels { get; set; }
        public ObservableCollection<ProductModel> ProductModels { get; set; }

        public NotificationServiceViewModel Notification { get; set; } = new NotificationServiceViewModel();

        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand AddCommand { get; set; }

        private BaseQueryService BaseQueryService = new BaseQueryService();

        public ShipmentModel AddNewShipment { get; set; } = new ShipmentModel();

        private ShipmentModel selectedItem;

        public ShipmentModel SelectedItem
        {
            get => selectedItem;
            set
            {
                if (value == selectedItem) return;
                selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }

        public bool SpinnerVisibility { get; set; }

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

        private ClientModel searchByClient;

        public ClientModel SearchByClient
        {
            get => searchByClient;
            set
            {
                if (value == searchByClient) return;
                searchByClient = value;
                OnPropertyChanged(nameof(SearchByClient));
                if (!string.IsNullOrWhiteSpace(searchByClient.Phone))
                {
                    var updateTask = Task.Run(() => Update(true));
                    Task.WaitAll(updateTask);
                    Find(searchByClient.Phone);
                }
                else
                {
                    Task.Run(() => Update(true));
                }
            }
        }

        #endregion Properties

        #region Functions

        private void Update(bool onlyShipment = false)
        {
            if (!onlyShipment)
            {
                ClientModels = BaseQueryService.Fill<ClientModel>(($"Get{DataBaseTableNames.Client}"));
                ProductModels = BaseQueryService.Fill<ProductModel>(($"Get{DataBaseTableNames.Product}"));
                OnPropertyChanged(nameof(ClientModels));
                OnPropertyChanged(nameof(ProductModels));
            }
            ShipmentModels = BaseQueryService.Fill<ShipmentModel>(($"Get{DataBaseTableNames.Shipment}"));
            OnPropertyChanged(nameof(ShipmentModels));
        }

        private void Delete()
        {
            if (SelectedItem?.Id != null)
            {
                bool isCompleted = BaseQueryService.Delete(DataBaseTableNames.Shipment, SelectedItem.Id);
                if (isCompleted)
                {
                    Notification.ShowNotification("Инфо: Информация о заказе удалена.");
                    ShipmentModels.Remove(SelectedItem);
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
            var exportMessage = new DocumentService().ExportInformationToFile(AddNewShipment, "Отгрузки");
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
            var errorList = new ValidationService<ShipmentModel>().ValidateFields(AddNewShipment);
            if (errorList.Any())
            {
                Notification.ShowListNotification(errorList);
            }
            else
            {
                var actualAmount = ProductModels.Where(item => item.Id == AddNewShipment.Product.Id).Select(item => item.Amount).First();
                if (AddNewShipment.Amount > actualAmount)
                {
                    AddOrder(actualAmount, 0);
                }
                else
                {
                    AddOrder(AddNewShipment.Amount, ProductModels.Where(item => item.Id == AddNewShipment.Product.Id).Select(item => item.Amount).First() - AddNewShipment.Amount);
                }
            }
        }

        private void AddOrder(int amount, int setAmount)
        {
            bool isCompleted = BaseQueryService.
    ExecuteQuery<ShipmentModel>($"INSERT INTO {DataBaseTableNames.Shipment} VALUES (N'{AddNewShipment.Date}', {amount}, {(AddNewShipment.Product.TotalPrice - (AddNewShipment.Client.Status.Discount * AddNewShipment.Product.TotalPrice / 100)) * AddNewShipment.Amount}, {AddNewShipment.Product.Id}, {AddNewShipment.Client.Id})");
            if (isCompleted)
            {
                Notification.ShowNotification($"Инфо: Заказ для {AddNewShipment.Client.Name} добавлен.");
                ShowSpinner();
                Task.Run(CreateDocument);
                BaseQueryService.
                    ExecuteQuery<ShipmentModel>($"Update Product set ProductAmount={setAmount} where ProductId = {AddNewShipment.Product.Id}");
                Task.Run(() => Update(true));
                BaseService.DelayAction(300, HideSpinner);
            }
            else
            {
                Notification.ShowNotification("Ошибка: Добавление заказа произошло с ошибкой.");
            }
        }

        private void Find(string searchText)
        {
            var searchResult = ShipmentModels.Where(items =>
            items.Product.Name.Contains(searchText) ||
            items.Product.Groups.Group.Contains(searchText) ||
            items.Product.Description.Contains(searchText) ||
            items.Client.Name.Contains(searchText) ||
            items.Client.Phone.Contains(searchText) ||
            items.Client.Surname.Contains(searchText)
            ).ToList();
            if (ShipmentModels.SequenceEqual(searchResult)) return;
            {
                ShipmentModels.Clear();
                foreach (var items in searchResult)
                {
                    ShipmentModels.Add(items);
                }
                OnPropertyChanged(nameof(ShipmentModels));
            }
        }

        #endregion Functions
    }
}