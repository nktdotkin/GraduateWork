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
            ClientModels = new ObservableCollection<ClientModel>();
            DeleteCommand = new RelayCommand((obj) => Delete());
            AddCommand = new RelayCommand((obj) => Add());
            AddNewClient = new ClientModel();
            ClientLocationSource = new BaseQueryService().GetAdress(null);
            Notification = new NotificationServiceViewModel();
            DataBaseStaticModels = new DataBaseStaticObjects();
            ModelValidation = new ValidationService<ClientModel>();
            Task.Run(() => Update());
        }

        #region Properties
        private const string TableName = "Client";
        public string ClientLocationSource { get; set; }
        public ObservableCollection<ClientModel> ClientModels { get; set; }

        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand AddCommand { get; set; }

        public NotificationServiceViewModel Notification { get; set; }
        public DataBaseStaticObjects DataBaseStaticModels { get; set; }
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
                    if (ClientLocationSource != selectedItem?.Adress)
                    {
                        ClientLocationSource = new BaseQueryService().GetAdress(selectedItem?.Adress);
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
        public ObservableCollection<ClientModel> Update()
        {
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
                    Notification.ShowNotification("Info: Client is deleted.");
                }
                else
                {
                    Notification.ShowNotification("Error: Deleting client failed.");
                }
            }
            else
            {
                Notification.ShowNotification("Error: No clients selected.");
            }
            Task.Run(() => Update());
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
                bool isCompleted = new BaseQueryService().Add(TableName, AddNewClient);
                if (isCompleted)
                {
                    Notification.ShowNotification($"Info: {AddNewClient.Name} is added.");
                }
                else
                {
                    Notification.ShowNotification("Error: Adding new client failed.");
                }
            }
            Task.Run(() => Update());
        }

        private void Find(string searchText)
        {
            var searchResult = ClientModels.Where(items =>
            items.Name.Contains(searchText) ||
            items.Surname.Contains(searchText) ||
            items.Status.Contains(searchText) ||
            items.Phone.Contains(searchText) ||
            items.Adress.Contains(searchText)
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
