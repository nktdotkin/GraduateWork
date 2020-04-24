using InventoryApp.Models.User;
using InventoryApp.ViewModels.Base;
using InventoryControl.Models.Base;
using System.Collections.ObjectModel;
using System.Linq;

namespace InventoryApp.ViewModels.User
{
    class ProviderViewModel : ViewModelsBase
    {
        private const string CommandToExecute = "GetProvider";
        private const string TableName = "Provider";
        private string searchText;
        public string ProviderLocationSource { get; set; }

        public ObservableCollection<ProviderModel> ProviderModels { get; set; }
        public RelayCommand DeleteCommand { get; set; }

        private ProviderModel selectedItem;
        public ProviderModel SelectedItem
        {
            get => selectedItem;
            set
            {
                if (value != selectedItem)
                {
                    selectedItem = value;
                    OnPropertyChanged(nameof(SearchText));
                    if (ProviderLocationSource != selectedItem.Adress)
                    {
                        ProviderLocationSource = new BaseQuery().GetAdress(selectedItem.Adress);
                        OnPropertyChanged(nameof(ProviderLocationSource));
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

        public ProviderViewModel()
        {
            ProviderModels = new ObservableCollection<ProviderModel>();
            DeleteCommand = new RelayCommand((obj) => Delete());
            Update();
            //TODO Replace with current location
            ProviderLocationSource = new BaseQuery().GetAdress(null);
        }

        private void Update()
        {
            ProviderModels = new BaseQuery().Fill<ProviderModel>(CommandToExecute);
            OnPropertyChanged(nameof(ProviderModels));
        }

        private void Delete()
        {
            new BaseQuery().Delete(TableName, SelectedItem.Id);
            ProviderModels.Remove(SelectedItem);
        }

        private void Find(string searchText)
        {
            var searchResult = ProviderModels.Where(items =>
            items.Name.Contains(searchText) ||
            items.Surname.Contains(searchText) ||
            items.Company.Contains(searchText) ||
            items.Adress.Contains(searchText)
            ).ToList();
            if (!ProviderModels.SequenceEqual(searchResult))
            {
                ProviderModels.Clear();
                foreach (var items in searchResult)
                {
                    ProviderModels.Add(items);
                }
                OnPropertyChanged(nameof(ProviderModels));
            }
        }
    }
}
