using InventoryApp.Models.Base;
using InventoryApp.Models.Service;
using InventoryApp.Models.User;
using InventoryApp.ViewModels.Base;
using InventoryApp.ViewModels.Service;
using System;
using System.Collections.ObjectModel;
using System.Linq;

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
            Notification = new NotificationServiceViewModel();
            DataBaseStaticModels = new DataBaseStaticModels();
            ModelValidation = new ValidationViewModel<ClientModel>();
            Update();
        }

        #region Properties
        private const string TableName = "Client";
        public string ClientLocationSource { get; set; }

        public ObservableCollection<ClientModel> ClientModels { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand AddCommand { get; set; }

        public NotificationServiceViewModel Notification { get; set; }
        public DataBaseStaticModels DataBaseStaticModels { get; set; }
        private ValidationViewModel<ClientModel> ModelValidation { get; set; }

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
            ClientModels.Clear();
            ClientModels = new BaseQuery().Fill<ClientModel>($"Get{TableName}");
            OnPropertyChanged(nameof(ClientModels));
        }

        private void Delete()
        {
            if (SelectedItem?.Id != null)
            {
                bool isCompleted = new BaseQuery().Delete(TableName, SelectedItem.Id);
                if (isCompleted)
                {
                    ClientModels.Remove(SelectedItem);
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
                bool isCompleted = new BaseQuery().Add(TableName, AddNewClient);
                if (isCompleted)
                {
                    ClientModels.Add(AddNewClient);
                    Notification.ShowNotification($"Info: {AddNewClient.Name} is added.");
                }
                else
                {
                    Notification.ShowNotification("Error: Adding new client failed.");
                }
            }
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
