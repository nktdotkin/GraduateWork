using InventoryApp.ViewModels.Base;
using System.Collections.ObjectModel;
using InventoryApp.Models.User;
using InventoryApp.Models.Base;
using System.Linq;
using System;

namespace InventoryApp.ViewModels.User
{
    class ClientViewModel : ViewModelsBase
    {
        public ClientViewModel()
        {
            ClientModels = new ObservableCollection<ClientModel>();
            DeleteCommand = new RelayCommand((obj) => Delete());
            AddCommand = new RelayCommand((obj) => Add());
            AddNewClient = new ClientModel();
            ClientLocationSource = new BaseQuery().GetAdress(null);
            Update();
        }

        #region Properties
        private const string TableName = "Client";
        public string ClientLocationSource { get; set; }

        public ObservableCollection<ClientModel> ClientModels { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand AddCommand { get; set; }

        private string notificationMessage;
        public string NotificationMessage
        {
            get => notificationMessage;
            set
            {
                if (value != notificationMessage)
                {
                    notificationMessage = value;
                    OnPropertyChanged(nameof(NotificationMessage));
                }
            }
        }

        private bool isActive;
        public bool IsActive
        {
            get => isActive;
            set
            {
                if (value != isActive)
                {
                    isActive = value;
                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }

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
                        ClientLocationSource = new BaseQuery().GetAdress(selectedItem?.Adress);
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
                        Update();
                        Find(searchText);
                    }
                    else
                    {
                        Update();
                    }
                }
            }
        }
        #endregion

        #region Functions
        private void Update()
        {
            ClientModels = new BaseQuery().Fill<ClientModel>($"Get{TableName}");
            OnPropertyChanged(nameof(ClientModels));
        }

        private void Delete()
        {
            if (SelectedItem?.Id != null)
            {
                bool isCompleted = new BaseQuery().Delete(TableName, SelectedItem.Id);
                ClientModels.Remove(SelectedItem);
                if (isCompleted)
                {
                    NotificationMessage = $"Info: Client is deleted.";
                    IsActive = true;
                }
                else
                {
                    NotificationMessage = "Error: Deleting client failed.";
                    IsActive = true;
                }
            }
            else
            {
                NotificationMessage = "Error: No clients selected.";
                IsActive = true;
            }
            //Set timer as setting
            BaseModel.DelayAction(Properties.Settings.Default.NotificationTimer, () => HideNotification());
        }

        private void Add()
        {
            bool isCompleted = new BaseQuery().Add(TableName, AddNewClient);
            ClientModels.Add(AddNewClient);
            if (isCompleted)
            {
                NotificationMessage = $"Info: {AddNewClient.Name} is added.";
                IsActive = true;
            }
            else
            {
                NotificationMessage = "Error: Adding new client failed.";
                IsActive = true;
            }
            //Set timer as setting
            BaseModel.DelayAction(Properties.Settings.Default.NotificationTimer, () => HideNotification());
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

        private void HideNotification()
        {
            IsActive = false;
        }
        #endregion
    }
}
