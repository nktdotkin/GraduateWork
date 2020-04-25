using InventoryApp.Models.Product;
using InventoryApp.ViewModels.Base;
using InventoryApp.Views.Controls;
using InventoryControl.Models.Base;
using System.Collections.ObjectModel;
using System.Linq;

namespace InventoryApp.ViewModels.Product
{
    class SupplyViewModel : ViewModelsBase
    {
        private const string TableName = "Supply";

        public ObservableCollection<SupplyModel> SupplyModels { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public SupplyModel SelectedItem { get; set; }

        public SupplyViewModel()
        {
            SupplyModels = new ObservableCollection<SupplyModel>();
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
            SupplyModels = new BaseQuery().Fill<SupplyModel>(($"Get{TableName}"));
            OnPropertyChanged(nameof(SupplyModels));
        }

        private void Delete()
        {
            new BaseQuery().Delete(TableName, SelectedItem.Id);
            SupplyModels.Remove(SelectedItem);
        }

        private void Find(string searchText)
        {
            var searchResult = SupplyModels.Where(items =>
            items.Product.Name.Contains(searchText) ||
            items.Product.Group.Contains(searchText) ||
            items.Product.Description.Contains(searchText) ||
            items.Provider.Name.Contains(searchText) ||
            items.Provider.Phone.Contains(searchText) ||
            items.Provider.Surname.Contains(searchText)
            ).ToList();
            if (!SupplyModels.SequenceEqual(searchResult))
            {
                SupplyModels.Clear();
                foreach (var items in searchResult)
                {
                    SupplyModels.Add(items);
                }
                OnPropertyChanged(nameof(SupplyModels));
            }
        }
    }
}
