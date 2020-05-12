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
    class ProviderViewModel : ViewModelsBase
    {
        public ProviderViewModel()
        {
            GC.Collect(1, GCCollectionMode.Forced);
            ProviderModels = new ObservableCollection<ProviderModel>();
            DeleteCommand = new RelayCommand((obj) => Delete());
            AddCommand = new RelayCommand((obj) => Add());
            AddNewProvider = new ProviderModel();
            ProviderLocationSource = new BaseQueryService().GetAddress(null);
            Notification = new NotificationServiceViewModel();
            ModelValidation = new ValidationService<ProviderModel>();
            Task.Run(() => Update());
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
                    if (ProviderLocationSource != selectedItem?.Address)
                    {
                        ProviderLocationSource = new BaseQueryService().GetAddress(selectedItem?.Address);
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
                    Notification.ShowNotification("Инфо: Поставщик удален.");
                }
                else
                {
                    Notification.ShowNotification("Ошибка: Удаление произошло с ошибкой.");
                }
            }
            else
            {
                Notification.ShowNotification("Ошибка: Выберите поставщика.");
            }
            Task.Run(() => Update());
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
                    Notification.ShowNotification($"Инфо: {AddNewProvider.Name} добавлен.");
                }
                else
                {
                    Notification.ShowNotification($"Ошибка: {AddNewProvider.Name} не добавлен..");
                }
            }
            Task.Run(() => Update());
        }

        private void Find(string searchText)
        {
            var searchResult = ProviderModels.Where(items =>
            items.Name.Contains(searchText) ||
            items.Surname.Contains(searchText) ||
            items.Company.Contains(searchText) ||
            items.Phone.Contains(searchText) ||
            items.Address.Contains(searchText)
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
