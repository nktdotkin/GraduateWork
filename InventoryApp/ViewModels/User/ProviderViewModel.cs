﻿using InventoryApp.Models.User;
using InventoryApp.Service;
using InventoryApp.ViewModels.Base;
using InventoryApp.ViewModels.Common;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryApp.ViewModels.User
{
    internal class ProviderViewModel : ViewModelsBase
    {
        public ProviderViewModel()
        {
            DeleteCommand = new RelayCommand((obj) => Delete());
            AddCommand = new RelayCommand((obj) => Add());
            Task.Run(Update);
        }

        #region Properties

        public string ProviderLocationSource { get; set; } = BaseService.GetAddress(null);
        public ObservableCollection<ProviderModel> ProviderModels { get; set; }

        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand AddCommand { get; set; }

        public NotificationServiceViewModel Notification { get; set; } = new NotificationServiceViewModel();

        private BaseQueryService BaseQueryService = new BaseQueryService();

        public ProviderModel AddNewProvider { get; set; } = new ProviderModel();

        private ProviderModel selectedItem;

        public ProviderModel SelectedItem
        {
            get => selectedItem;
            set
            {
                if (value == selectedItem) return;
                selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
                if (ProviderLocationSource == selectedItem?.Address) return;
                ProviderLocationSource = BaseService.GetAddress(selectedItem?.Address);
                OnPropertyChanged(nameof(ProviderLocationSource));
            }
        }

        private string searchText;

        public string SearchText
        {
            get => searchText;
            set
            {
                if (value == searchText) return;
                searchText = value;
                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    var updateTask = Task.Run(Update);
                    Task.WaitAll(updateTask);
                    Find(searchText);
                }
                else
                {
                    Task.Run(Update);
                }
                OnPropertyChanged(nameof(SearchText));
            }
        }

        #endregion Properties

        #region Functions

        private void Update()
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
                    ProviderModels.Remove(SelectedItem);
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
                    Task.Run(Update);
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
            if (ProviderModels.SequenceEqual(searchResult)) return;
            {
                ProviderModels.Clear();
                foreach (var items in searchResult)
                {
                    ProviderModels.Add(items);
                }
                OnPropertyChanged(nameof(ProviderModels));
            }
        }

        #endregion Functions
    }
}