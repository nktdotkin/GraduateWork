using InventoryApp.Models.Base;
using InventoryApp.Models.User;
using InventoryApp.ViewModels.Base;
using InventoryControl.Models.Base;
using System.Collections.ObjectModel;
using System.Linq;

namespace InventoryApp.ViewModels.User
{
    class ProviderViewModel : ViewModelsBase
    {
        public ProviderViewModel()
        {
            ProviderModels = new ObservableCollection<ProviderModel>();
            DeleteCommand = new RelayCommand((obj) => Delete());
            AddCommand = new RelayCommand((obj) => Add());
            AddNewProvider = new ProviderModel();
            ProviderLocationSource = new BaseQuery().GetAdress(null);
            Update();
        }

        #region Properties
        private const string TableName = "Provider";
        public string ProviderLocationSource { get; set; }

        public ObservableCollection<ProviderModel> ProviderModels { get; set; }
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

        private ProviderModel addNewProvider;
        public ProviderModel AddNewProvider
        {
            get => addNewProvider;
            set
            {
                if (value != addNewProvider)
                {
                    addNewProvider = value;
                    OnPropertyChanged(nameof(AddNewProvider));
                }
            }
        }


        private ProviderModel selectedItem;
        public ProviderModel SelectedItem
        {
            get => selectedItem;
            set
            {
                if (value != selectedItem)
                {
                    selectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));
                    if (ProviderLocationSource != selectedItem?.Adress)
                    {
                        ProviderLocationSource = new BaseQuery().GetAdress(selectedItem?.Adress);
                        OnPropertyChanged(nameof(ProviderLocationSource));
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
            ProviderModels = new BaseQuery().Fill<ProviderModel>(($"Get{TableName}"));
            OnPropertyChanged(nameof(ProviderModels));
        }

        private void Delete()
        {
            if (SelectedItem?.Id != null)
            {
                bool isCompleted = new BaseQuery().Delete(TableName, SelectedItem.Id);
                ProviderModels.Remove(SelectedItem);
                if (isCompleted)
                {
                    NotificationMessage = $"Info: Provider is deleted.";
                    IsActive = true;
                }
                else
                {
                    NotificationMessage = "Error: Deleting provider failed.";
                    IsActive = true;
                }
            }
            else
            {
                NotificationMessage = "Error: No providers selected.";
                IsActive = true;
            }
            //Set timer as setting
            BaseModel.DelayAction(5000, () => HideNotification());
        }

        private void Add()
        {
            bool isCompleted = new BaseQuery().Add(TableName, AddNewProvider);
            ProviderModels.Add(AddNewProvider);
            if (isCompleted)
            {
                NotificationMessage = $"Info: {AddNewProvider.Name} is added.";
                IsActive = true;
            }
            else
            {
                NotificationMessage = "Error: Adding new provider failed.";
                IsActive = true;
            }
            //Set timer as setting
            BaseModel.DelayAction(5000, () => HideNotification());
        }

        private void Find(string searchText)
        {
            var searchResult = ProviderModels.Where(items =>
            items.Name.Contains(searchText) ||
            items.Surname.Contains(searchText) ||
            items.Company.Contains(searchText) ||
            items.Phone.Contains(searchText) ||
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

        public void HideNotification()
        {
            IsActive = false;
        }
        #endregion
    }
}
