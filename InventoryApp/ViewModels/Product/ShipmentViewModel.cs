using InventoryApp.Models.Product;
using InventoryApp.Models.User;
using InventoryApp.Service;
using InventoryApp.ViewModels.Base;
using InventoryApp.ViewModels.Common;
using InventoryApp.ViewModels.User;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryApp.ViewModels.Product
{
    class ShipmentViewModel : ViewModelsBase
    {
        public ShipmentViewModel()
        {
            GC.Collect(1, GCCollectionMode.Forced);
            ShipmentModels = new ObservableCollection<ShipmentModel>();
            DeleteCommand = new RelayCommand((obj) => Delete());
            AddCommand = new RelayCommand((obj) => Add());
            AddNewShipment = new ShipmentModel();
            Notification = new NotificationServiceViewModel();
            ModelValidation = new ValidationService<ShipmentModel>();
            Task.Run(() => Update());
        }

        #region Properties
        private const string TableName = "Shipment";
        public ObservableCollection<ShipmentModel> ShipmentModels { get; set; }
        public ObservableCollection<ClientModel> ClientModels { get; set; }
        public ObservableCollection<ProductModel> ProductModels { get; set; }

        private ValidationService<ShipmentModel> ModelValidation { get; set; }
        public NotificationServiceViewModel Notification { get; set; }

        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand AddCommand { get; set; }
        public RelayCommand AddProductImageCommand { get; set; }

        private ShipmentModel selectedItem;
        public ShipmentModel SelectedItem
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

        public ShipmentModel AddNewShipment { get; set; }

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

        private ClientModel searchByClient;
        public ClientModel SearchByClient
        {
            get => searchByClient;
            set
            {
                if (value != searchByClient)
                {
                    searchByClient = value;
                    OnPropertyChanged(nameof(SearchByClient));
                    if (!string.IsNullOrWhiteSpace(searchByClient.Phone))
                    {
                        //var updateTask = Task.Run(() => Update());
                        //Task.WaitAll(updateTask);
                        Find(searchByClient.Phone);
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
        private void Update(bool onlyShipment = false)
        {
            if (!onlyShipment)
            {
                ClientModels = new ClientViewModel().Update(true);
                ProductModels = new ProductViewModel().Update(true);
            }
            ShipmentModels = new BaseQueryService().Fill<ShipmentModel>(($"Get{TableName}"));
            OnPropertyChanged(nameof(ShipmentModels));
            OnPropertyChanged(nameof(ClientModels));
            OnPropertyChanged(nameof(ProductModels));
        }

        private void Delete()
        {
            if (SelectedItem?.Id != null)
            {
                bool isCompleted = new BaseQueryService().Delete(TableName, SelectedItem.Id);
                if (isCompleted)
                {
                    Notification.ShowNotification("Инфо: Информация о заказе удалена.");
                    Task.Run(() => Update());
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
            this.spinnerVisibility = false;
            OnPropertyChanged(nameof(this.SpinnerVisibility));
        }

        private void Add()
        {
            var errorList = ModelValidation.ValidateFields(AddNewShipment);
            if (errorList.Any())
            {
                Notification.ShowListNotification(errorList);
            }
            else
            {
                var actualAmount = ProductModels.Where(item => item.Id == AddNewShipment.Product.Id).Select(item => item.Amount).First();
                if (AddNewShipment.Amount > actualAmount)
                {
                    bool isCompleted = new BaseQueryService().ExecuteQuery<ShipmentModel>($"INSERT INTO {TableName} VALUES (N'{AddNewShipment.Date}', {actualAmount}, {(AddNewShipment.Product.TotalPrice - (AddNewShipment.Client.Status.Discount * AddNewShipment.Product.TotalPrice / 100)) * actualAmount} , {AddNewShipment.Product.Id}, {AddNewShipment.Client.Id})");
                    if (isCompleted)
                    {
                        Notification.ShowNotification($"Инфо: Заказ для {AddNewShipment.Client.Name} на {actualAmount} шт. добавлен.");
                        SpinnerVisibility = true;
                        Task.Run(() => CreateDocument());
                        new BaseQueryService().ExecuteQuery<ShipmentModel>($"Update Product set ProductAmount={0} where ProductId = {AddNewShipment.Product.Id}");
                        Task.Run(() => Update(true));
                        BaseService.DelayAction(100, () => HideSpinner());
                    }
                    else
                    {
                        Notification.ShowNotification("Ошибка: Добавление заказа произошло с ошибкой.");
                    }
                }
                else
                {
                    bool isCompleted = new BaseQueryService().ExecuteQuery<ShipmentModel>($"INSERT INTO {TableName} VALUES (N'{AddNewShipment.Date}', {AddNewShipment.Amount}, {(AddNewShipment.Product.TotalPrice - (AddNewShipment.Client.Status.Discount * AddNewShipment.Product.TotalPrice / 100)) * AddNewShipment.Amount}, {AddNewShipment.Product.Id}, {AddNewShipment.Client.Id})");
                    if (isCompleted)
                    {
                        Notification.ShowNotification($"Инфо: Заказ для {AddNewShipment.Client.Name} добавлен.");
                        SpinnerVisibility = true;
                        Task.Run(() => CreateDocument());
                        new BaseQueryService().ExecuteQuery<ShipmentModel>($"Update Product set ProductAmount={ProductModels.Where(item => item.Id == AddNewShipment.Product.Id).Select(item => item.Amount).First() - AddNewShipment.Amount} where ProductId = {AddNewShipment.Product.Id}");
                        Task.Run(() => Update(true));
                        BaseService.DelayAction(300, () => HideSpinner());
                    }
                    else
                    {
                        Notification.ShowNotification("Ошибка: Добавление заказа произошло с ошибкой.");
                    }
                }
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
            if (!ShipmentModels.SequenceEqual(searchResult))
            {
                ShipmentModels.Clear();
                foreach (var items in searchResult)
                {
                    ShipmentModels.Add(items);
                }
                OnPropertyChanged(nameof(ShipmentModels));
            }
        }
        #endregion
    }
}
