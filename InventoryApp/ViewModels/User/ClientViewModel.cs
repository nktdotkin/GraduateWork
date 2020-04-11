using InventoryApp.ViewModels.Base;
using System.Collections.ObjectModel;
using InventoryApp.Models.User;
using InventoryControl.Models.Base;
using System.Linq;

namespace InventoryApp.ViewModels.User
{
    class ClientViewModel : ViewModelsBase
    {
        private const string CommandToExecute = "GetClient";
        private const string TableName = "Client";
        public ObservableCollection<ClientModel> ClientModels { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public ClientModel SelectedItem { get; set; }

        public ClientViewModel()
        {
            ClientModels = new ObservableCollection<ClientModel>();
            DeleteCommand = new RelayCommand((obj) => Delete());
            Update();
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
