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
            DeleteCommand = new RelayCommand((obj) => Delete());
            AddCommand = new RelayCommand((obj) => Add());
            AddNewProvider = new ProviderModel();
            ProviderLocationSource = BaseService.GetAddress(null);
            Notification = new NotificationServiceViewModel();
            BaseQueryService = new BaseQueryService();
            Task.Run(() => Update());
        }

        #region Properties
        public string ProviderLocationSource { get; set; }
        public ObservableCollection<ProviderModel> ProviderModels { get; set; }

        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand AddCommand { get; set; }

        public NotificationServiceViewModel Notification { get; set; }

        private BaseQueryService BaseQueryService;

        public ProviderModel AddNewProvider { get; set; }

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
                        ProviderLocationSource = BaseService.GetAddress(selectedItem?.Address);
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
                    OnPropertyChanged(nameof(SearchText));
                }
            }
        }
        #endregion

        #region Functions
        public void Update()
        {
            ProviderModels = BaseQueryService.Fill<ProviderModel>(($"Get{DataBaseTableNames.Provider}"));
            OnPropertyChanged(nameof(ProviderModels));
        }

        private void Delete()
        {
            if (SelectedItem?.Id != null)
            {
                bool isCompleted = BaseQueryService.Delete(DataBaseTableNames.Provider, SelectedItem.Id);
                if (isCompleted)
                {
                    Notification.ShowNotification("Инфо: Поставщик удален.");
                    Task.Run(() => Update());
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
        }

        private void Add()
        {
            var errorList = new ValidationService<ProviderModel>().ValidateFields(AddNewProvider);
            if (errorList.Any())
            {
                Notification.ShowListNotification(errorList);
            }
            else
            {
                bool isCompleted = BaseQueryService.Add(DataBaseTableNames.Provider, AddNewProvider);
                if (isCompleted)
                {
                    Notification.ShowNotification($"Инфо: Поставщик {AddNewProvider.Company} добавлен.");
                    Task.Run(() => Update());
                }
                else
                {
                    Notification.ShowNotification($"Ошибка: Поставщик {AddNewProvider.Company} не добавлен.");
                }
            }
        }

        private void Find(string searchText)
        {
            var searchResult = ProviderModels.Where(items =>
            items.Name.Contains(searchText) ||
            items.Surname.Contains(searchText) ||
            items.Company.Contains(searchText) ||
            items.Email.Contains(searchText) ||
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
