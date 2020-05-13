using InventoryApp.Models.Product;
using InventoryApp.Models.User;
using InventoryApp.Service;
using InventoryApp.ViewModels.Common;
using InventoryApp.Views.Main;
using System.Collections.ObjectModel;
using System.Windows;

namespace InventoryApp.ViewModels.Base
{
    class SettingsViewModel : ViewModelsBase
    {
        public SettingsViewModel()
        {
            MainWindowCommand = new RelayCommand((obj) => MainWindow());
            AddNewStatusCommand = new RelayCommand((obj) => AddNewStatus());
            AddNewGroupCommand = new RelayCommand((obj) => AddNewGroup());
            AddNewStoreCommand = new RelayCommand((obj) => AddNewStore());
            Notification = new NotificationServiceViewModel();
            GetFromDatabase();
        }

        public RelayCommand MainWindowCommand { get; set; }
        public ObservableCollection<StatusesModel> StatusesModels { get; set; }
        public ObservableCollection<GroupsModel> GroupsModels { get; set; }
        public ObservableCollection<StoretypesModel> StoretypesModels { get; set; }

        public NotificationServiceViewModel Notification { get; set; }

        public RelayCommand AddNewStatusCommand { get; set; }
        public RelayCommand AddNewGroupCommand { get; set; }
        public RelayCommand AddNewStoreCommand { get; set; }

        private bool isCompleted = false;

        private void GetFromDatabase()
        {
            GroupsModels = new BaseQueryService().Fill<GroupsModel>(($"GetGroups"));
            StatusesModels = new BaseQueryService().Fill<StatusesModel>(($"GetStatuses"));
            StoretypesModels = new BaseQueryService().Fill<StoretypesModel>(($"GetStoreTypes"));
        }

        private void UpdateGroups()
        {
            foreach (var updatedRows in GroupsModels)
            {
                isCompleted = new BaseQueryService().ExecuteQuery<GroupsModel>($"Update [ProductGroups] SET GroupType = N'{updatedRows.Group}', Tax = {updatedRows.Tax} WHERE GroupId = {updatedRows.Id}");
                if (!isCompleted)
                {
                    isCompleted = new BaseQueryService().ExecuteQuery<GroupsModel>($"Insert into [ProductGroups] (GroupType, Tax) VALUES (N'{updatedRows.Group}', {updatedRows.Tax})");
                    isCompleted = false;
                }
            }
        }

        private void UpdateStatuses()
        {
            foreach (var updatedRows in StatusesModels)
            {
                isCompleted = new BaseQueryService().ExecuteQuery<StatusesModel>($"Update [ClientStatuses] SET StatusType = N'{updatedRows.Status}', Discount = {updatedRows.Discount} WHERE StatusId = {updatedRows.StatusId}");
                if (!isCompleted)
                {
                    isCompleted = new BaseQueryService().ExecuteQuery<StatusesModel>($"Insert into [ClientStatuses] (StatusType, Discount) VALUES (N'{updatedRows.Status}', {updatedRows.Discount})");
                    isCompleted = false;
                }
            }
        }

        private void UpdateStoreTypes()
        {
            foreach (var updatedRows in StoretypesModels)
            {
                isCompleted = new BaseQueryService().ExecuteQuery<StoretypesModel>($"Update [ClientStoreTypes] SET StoreType = N'{updatedRows.StoreType}' WHERE StoreId = {updatedRows.StoreId}");
                if (!isCompleted)
                {
                    isCompleted = new BaseQueryService().ExecuteQuery<StoretypesModel>($"Insert into [ClientStoreTypes] (StoreType) VALUES (N'{updatedRows.StoreType}')");
                    isCompleted = false;
                }
            }
        }

        private void SeveSettings()
        {
            Properties.Settings.Default.Save();
        }

        public void AddNewStatus()
        {
            StatusesModels.Add(new StatusesModel() { StatusId = 0, Discount = 0, Status = "Новый статус" });
        }

        public void AddNewGroup()
        {
            GroupsModels.Add(new GroupsModel() { Id = 0, Tax = 0, Group = "Новая группа" });
        }

        public void AddNewStore()
        {
            StoretypesModels.Add(new StoretypesModel() { StoreId = 0, StoreType = "Новый тип" });
        }

        private void MainWindow()
        {
            UpdateInfo();
            new MainWindow().Show();
            Application.Current.Windows[0].Close();
        }

        private void UpdateInfo()
        {
            SeveSettings();
            UpdateGroups();
            UpdateStatuses();
            UpdateStoreTypes();
        }
    }
}
