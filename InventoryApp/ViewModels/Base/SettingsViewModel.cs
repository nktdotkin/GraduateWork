using InventoryApp.Models.Product;
using InventoryApp.Models.User;
using InventoryApp.Service;
using InventoryApp.Views.Main;
using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace InventoryApp.ViewModels.Base
{
    internal class SettingsViewModel : ViewModelsBase
    {
        public SettingsViewModel()
        {
            MainWindowCommand = new RelayCommand((obj) => MainWindow());
            AddNewStatusCommand = new RelayCommand((obj) => AddNewStatus());
            AddNewGroupCommand = new RelayCommand((obj) => AddNewGroup());
            AddNewStoreCommand = new RelayCommand((obj) => AddNewStore());
            GetFromDatabase();
        }

        ~SettingsViewModel()
        {
            GC.Collect(1, GCCollectionMode.Forced);
        }

        public RelayCommand MainWindowCommand { get; set; }
        public ObservableCollection<StatusesModel> StatusesModels { get; set; }
        public ObservableCollection<GroupsModel> GroupsModels { get; set; }
        public ObservableCollection<StoretypesModel> StoretypesModels { get; set; }

        private BaseQueryService BaseQueryService = new BaseQueryService();

        public RelayCommand AddNewStatusCommand { get; set; }
        public RelayCommand AddNewGroupCommand { get; set; }
        public RelayCommand AddNewStoreCommand { get; set; }

        private bool isCompleted = false;

        private void GetFromDatabase()
        {
            GroupsModels = BaseQueryService.Fill<GroupsModel>(($"Get{DataBaseTableNames.Groups}"));
            StatusesModels = BaseQueryService.Fill<StatusesModel>(($"Get{DataBaseTableNames.Statuses}"));
            StoretypesModels = BaseQueryService.Fill<StoretypesModel>(($"Get{DataBaseTableNames.StoreTypes}"));
        }

        private void UpdateGroups()
        {
            foreach (var updatedRows in GroupsModels)
            {
                isCompleted = BaseQueryService.ExecuteQuery<GroupsModel>($"Update [ProductGroups] SET GroupType = N'{updatedRows.Group}', Tax = {updatedRows.Tax} WHERE GroupId = {updatedRows.Id}");
                if (isCompleted) continue;
                isCompleted = BaseQueryService.ExecuteQuery<GroupsModel>($"Insert into [ProductGroups] (GroupType, Tax) VALUES (N'{updatedRows.Group}', {updatedRows.Tax})");
                isCompleted = false;
            }
        }

        private void UpdateStatuses()
        {
            foreach (var updatedRows in StatusesModels)
            {
                isCompleted = BaseQueryService.ExecuteQuery<StatusesModel>($"Update [ClientStatuses] SET StatusType = N'{updatedRows.Status}', Discount = {updatedRows.Discount} WHERE StatusId = {updatedRows.StatusId}");
                if (isCompleted) continue;
                isCompleted = BaseQueryService.ExecuteQuery<StatusesModel>($"Insert into [ClientStatuses] (StatusType, Discount) VALUES (N'{updatedRows.Status}', {updatedRows.Discount})");
                isCompleted = false;
            }
        }

        private void UpdateStoreTypes()
        {
            foreach (var updatedRows in StoretypesModels)
            {
                isCompleted = BaseQueryService.ExecuteQuery<StoretypesModel>($"Update [ClientStoreTypes] SET StoreType = N'{updatedRows.StoreType}' WHERE StoreId = {updatedRows.StoreId}");
                if (isCompleted) continue;
                isCompleted = BaseQueryService.ExecuteQuery<StoretypesModel>($"Insert into [ClientStoreTypes] (StoreType) VALUES (N'{updatedRows.StoreType}')");
                isCompleted = false;
            }
        }

        private void SeveSettings()
        {
            Properties.Settings.Default.Save();
        }

        private void AddNewStatus()
        {
            StatusesModels.Add(new StatusesModel() { StatusId = 0, Discount = 0, Status = "Новый статус" });
        }

        private void AddNewGroup()
        {
            GroupsModels.Add(new GroupsModel() { Id = 0, Tax = 0, Group = "Новая группа" });
        }

        private void AddNewStore()
        {
            StoretypesModels.Add(new StoretypesModel() { StoreId = 0, StoreType = "Новый тип" });
        }

        private void MainWindow()
        {
            UpdateInfo();
            new MainWindow().Show();
            Application.Current.Windows[0]?.Close();
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