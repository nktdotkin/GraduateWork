using InventoryApp.ViewModels.Base;
using System.Collections.ObjectModel;
using InventoryApp.Models.User;
using InventoryControl.Models.Base;
using System.Linq;
using System;

namespace InventoryApp.ViewModels.User
{
    class ClientViewModel : ViewModelsBase
    {
        private const string CommandToExecute = "GetClient";
        private const string TableName = "Client";
        private string searchText;
        public string ClientLocationSource { get; set; }

        public ObservableCollection<ClientModel> ClientModels { get; set; }
        public RelayCommand DeleteCommand { get; set; }

        private ClientModel selectedItem;
        public ClientModel SelectedItem
        {
            get => selectedItem;
            set
            {
                if (value != selectedItem)
                {
                    selectedItem = value;
                    OnPropertyChanged(nameof(SearchText));
                    if(ClientLocationSource != selectedItem.Adress)
                    {
                        ClientLocationSource = new BaseQuery().GetAdress(selectedItem.Adress);
                        OnPropertyChanged(nameof(ClientLocationSource));
                    }
                }
            }
        }

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

        public ClientViewModel()
        {
            ClientModels = new ObservableCollection<ClientModel>();
            DeleteCommand = new RelayCommand((obj) => Delete());
            Update();
            //TODO Replace with current location
            ClientLocationSource = new BaseQuery().GetAdress(null);
        }

        private void Update()
        {
            ClientModels = new BaseQuery().Fill<ClientModel>(CommandToExecute);
            OnPropertyChanged(nameof(ClientModels));
        }

        private void Delete()
        {
            new BaseQuery().Delete(TableName, SelectedItem.Id);
            ClientModels.Remove(SelectedItem);
        }

        private void Find(string searchText)
        {
            var searchResult = ClientModels.Where(items =>
            items.Name.Contains(searchText) ||
            items.Surname.Contains(searchText) ||
            items.Status.Contains(searchText) ||
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
    }
}
