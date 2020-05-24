using InventoryApp.Models.User;
using InventoryApp.Service;
using InventoryApp.ViewModels.Base;
using InventoryApp.ViewModels.Common;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryApp.ViewModels.User
{
    class ClientViewModel : ViewModelsBase
    {
        public ClientViewModel()
        {
            GC.Collect(1, GCCollectionMode.Forced);
            DeleteCommand = new RelayCommand((obj) => Delete());
            AddCommand = new RelayCommand((obj) => Add());
            AddNewClient = new ClientModel();
            ClientLocationSource = BaseService.GetAddress(null);
            Notification = new NotificationServiceViewModel();
            ModelValidation = new ValidationService<ClientModel>();
            Task.Run(() => Update());
        }

        #region Properties
        private const string TableName = "Client";
        public string ClientLocationSource { get; set; }
        public ObservableCollection<ClientModel> ClientModels { get; set; }
        public ObservableCollection<StoretypesModel> StoretypesModels { get; set; }
        public ObservableCollection<StatusesModel> StatusesModels { get; set; }

        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand AddCommand { get; set; }

        public NotificationServiceViewModel Notification { get; set; }
        private ValidationService<ClientModel> ModelValidation { get; set; }

        private ClientModel addNewClient;
        public ClientModel AddNewClient
        {
            get => addNewClient;
            set
            {
                if (value != addNewClient)
                {
                    addNewClient = value;
                    OnPropertyChanged(nameof(AddNewClient));
                }
            }
        }

        private ClientModel selectedItem;
        public ClientModel SelectedItem
        {
            get => selectedItem;
            set
            {
                if (value != selectedItem)
                {
                    selectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));
                    if (ClientLocationSource != selectedItem?.Address)
                    {
                        ClientLocationSource = BaseService.GetAddress(selectedItem?.Address);
                        OnPropertyChanged(nameof(ClientLocationSource));
                    }
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
        #endregion

        #region Functions
        public ObservableCollection<ClientModel> Update(bool onlyClient = false)
        {
            if (!onlyClient)
            {
                StatusesModels = new BaseQueryService().Fill<StatusesModel>(($"GetStatuses"));
                StoretypesModels = new BaseQueryService().Fill<StoretypesModel>(($"GetStoreTypes"));
                OnPropertyChanged(nameof(StatusesModels));
                OnPropertyChanged(nameof(StoretypesModels));
            }
            ClientModels = new BaseQueryService().Fill<ClientModel>($"Get{TableName}");
            OnPropertyChanged(nameof(ClientModels));
            return ClientModels;
        }

        private void Delete()
        {
            if (SelectedItem?.Id != null)
            {
                bool isCompleted = new BaseQueryService().Delete(TableName, SelectedItem.Id);
                if (isCompleted)
                {
                    Notification.ShowNotification("Инфо: Клиент удален.");
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
            var errorList = ModelValidation.ValidateFields(AddNewClient);
            if (errorList.Any())
            {
                Notification.ShowListNotification(errorList);
            }
            else
            {
                bool isCompleted = new BaseQueryService().Add<ClientModel>(TableName, AddNewClient);
                Update(true);
                bool isUpdated = new BaseQueryService().ExecuteQuery<ClientModel>($"UPDATE {TableName} SET _StatusId = {AddNewClient.Status.StatusId}, _StoreId = {AddNewClient.StoreType.StoreId} WHERE ClientId = {ClientModels.Last().Id}");
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
            if (!ClientModels.SequenceEqual(searchResult))
            {
                ClientModels.Clear();
                foreach (var items in searchResult)
                {
                    ClientModels.Add(items);
                }
                OnPropertyChanged(nameof(ClientModels));
            }
        }
        #endregion
    }
}
