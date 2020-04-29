using InventoryApp.Models.User;
using InventoryApp.Service;
using InventoryApp.ViewModels.Base;
using InventoryApp.ViewModels.Common;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace InventoryApp.ViewModels.User
{
    class ProviderViewModel : ViewModelsBase
    {
        public ProviderViewModel()
        {
            GC.Collect(1, GCCollectionMode.Forced);
            ProviderModels = new ObservableCollection<ProviderModel>();
            DeleteCommand = new RelayCommand((obj) => Delete());
            AddCommand = new RelayCommand((obj) => Add());
            AddNewProvider = new ProviderModel();
            ProviderLocationSource = new BaseQueryService().GetAdress(null);
            Notification = new NotificationServiceViewModel();
            ModelValidation = new ValidationService<ProviderModel>();
            Update();
        }

        #region Properties
        private const string TableName = "Provider";
        public string ProviderLocationSource { get; set; }
        public ObservableCollection<ProviderModel> ProviderModels { get; set; }

        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand AddCommand { get; set; }

        public NotificationServiceViewModel Notification { get; set; }
        private ValidationService<ProviderModel> ModelValidation { get; set; }

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
                        ProviderLocationSource = new BaseQueryService().GetAdress(selectedItem?.Adress);
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
        public ObservableCollection<ProviderModel> Update()
        {
            ProviderModels = new BaseQueryService().Fill<ProviderModel>(($"Get{TableName}"));
            OnPropertyChanged(nameof(ProviderModels));
            return ProviderModels;
        }

        private void Delete()
        {
            if (SelectedItem?.Id != null)
            {
                bool isCompleted = new BaseQueryService().Delete(TableName, SelectedItem.Id);
                if (isCompleted)
                {
                    Notification.ShowNotification("Info: Provider is deleted.");
                }
                else
                {
                    Notification.ShowNotification("Error: Deleting provider failed.");
                }
            }
            else
            {
                Notification.ShowNotification("Error: No providers selected.");
            }
            Update();
        }

        private void Add()
        {
            var errorList = ModelValidation.ValidateFields(AddNewProvider);
            if (errorList.Any())
            {
                Notification.ShowListNotification(errorList);
            }
            else
            {
                bool isCompleted = new BaseQueryService().Add(TableName, AddNewProvider);
                if (isCompleted)
                {
                    Notification.ShowNotification($"Info: {AddNewProvider.Name} is added.");
                }
                else
                {
                    Notification.ShowNotification("Error: Adding new provider failed.");
                }
            }
            Update();
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
        #endregion
    }
}
