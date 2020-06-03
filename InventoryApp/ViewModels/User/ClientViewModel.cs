using InventoryApp.Models.User;
using InventoryApp.Service;
using InventoryApp.ViewModels.Base;
using InventoryApp.ViewModels.Common;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryApp.ViewModels.User
{
    internal class ClientViewModel : ViewModelsBase
    {
        public ClientViewModel()
        {
            DeleteCommand = new RelayCommand((obj) => Delete());
            AddCommand = new RelayCommand((obj) => Add());
            AddNewClient = new ClientModel();
            ClientLocationSource = BaseService.GetAddress(null);
            Notification = new NotificationServiceViewModel();
            BaseQueryService = new BaseQueryService();
            Task.Run(() => Update());
        }

        #region Properties

        public string ClientLocationSource { get; set; }
        public ObservableCollection<ClientModel> ClientModels { get; set; }
        public ObservableCollection<StoretypesModel> StoretypesModels { get; set; }
        public ObservableCollection<StatusesModel> StatusesModels { get; set; }

        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand AddCommand { get; set; }

        public NotificationServiceViewModel Notification { get; set; }

        private BaseQueryService BaseQueryService;

        public ClientModel AddNewClient { get; set; }

        private ClientModel selectedItem;

        public ClientModel SelectedItem
        {
            get => selectedItem;
            set
            {
                if (value == selectedItem) return;
                selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
                if (ClientLocationSource == selectedItem?.Address) return;
                ClientLocationSource = BaseService.GetAddress(selectedItem?.Address);
                OnPropertyChanged(nameof(ClientLocationSource));
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
                OnPropertyChanged(nameof(SearchText));
            }
        }

        #endregion Properties

        #region Functions

        private void Update(bool onlyClient = false)
        {
            if (!onlyClient)
            {
                StatusesModels = BaseQueryService.Fill<StatusesModel>(($"Get{DataBaseTableNames.Statuses}"));
                StoretypesModels = BaseQueryService.Fill<StoretypesModel>(($"Get{DataBaseTableNames.StoreTypes}"));
                OnPropertyChanged(nameof(StatusesModels));
                OnPropertyChanged(nameof(StoretypesModels));
            }
            ClientModels = BaseQueryService.Fill<ClientModel>($"Get{DataBaseTableNames.Client}");
            OnPropertyChanged(nameof(ClientModels));
        }

        private void Delete()
        {
            if (SelectedItem?.Id != null)
            {
                bool isCompleted = BaseQueryService.Delete(DataBaseTableNames.Client, SelectedItem.Id);
                if (isCompleted)
                {
                    Notification.ShowNotification("Инфо: Клиент удален.");
                    ClientModels.Remove(SelectedItem);
                    Task.Run(() => Update());
                }
                else
                {
                    Notification.ShowNotification("Ошибка: Удаление завершилось с ошибкой.");
                }
            }
            else
            {
                Notification.ShowNotification("Ошибка: Выберите клиента.");
            }
        }

        private void Add()
        {
            var errorList = new ValidationService<ClientModel>().ValidateFields(AddNewClient);
            if (errorList.Any())
            {
                Notification.ShowListNotification(errorList);
            }
            else
            {
                bool isCompleted = BaseQueryService.Add(DataBaseTableNames.Client, AddNewClient);
                Update(true);
                bool isUpdated = BaseQueryService.ExecuteQuery<ClientModel>($"UPDATE {DataBaseTableNames.Client} SET _StatusId = {AddNewClient.Status.StatusId}, _StoreId = {AddNewClient.StoreType.StoreId} WHERE ClientId = {ClientModels.Last().Id}");
                if (isCompleted && isUpdated)
                {
                    Notification.ShowNotification($"Инфо: {AddNewClient.Name} добавлен.");
                    Task.Run(() => Update());
                }
                else
                {
                    Notification.ShowNotification($"Ошибка: {AddNewClient.Name} не добавлен.");
                }
            }
        }

        private void Find(string searchText)
        {
            var searchResult = ClientModels.Where(items =>
            items.Name.Contains(searchText) ||
            items.Surname.Contains(searchText) ||
            items.Status.Status.Contains(searchText) ||
            items.Phone.Contains(searchText) ||
            items.Address.Contains(searchText)
            ).ToList();
            if (ClientModels.SequenceEqual(searchResult)) return;
            {
                ClientModels.Clear();
                foreach (var items in searchResult)
                {
                    ClientModels.Add(items);
                }
                OnPropertyChanged(nameof(ClientModels));
            }
        }

        #endregion Functions
    }
}