using InventoryApp.Models.Product;
using InventoryApp.Models.User;
using InventoryApp.Service;
using InventoryApp.ViewModels.Common;
using InventoryApp.Views.Main;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace InventoryApp.ViewModels.Base
{
    class SettingsViewModel : ViewModelsBase
    {
        public SettingsViewModel()
        {
            MainWindowCommand = new RelayCommand((obj) => MainWindow());
            Notification = new NotificationServiceViewModel();
            GetFromDatabase();
        }

        public RelayCommand MainWindowCommand { get; set; }
        public ObservableCollection<StatusesModel> StatusesModels { get; set; }
        public ObservableCollection<GroupsModel> GroupsModels { get; set; }
        public ObservableCollection<StoretypesModel> StoretypesModels { get; set; }

        public NotificationServiceViewModel Notification { get; set; }

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
                isCompleted = new BaseQueryService().ExecuteQuery<GroupsModel>($"Update [ProductGroups] SET GroupType = '{updatedRows.Group}', Tax = {updatedRows.Tax} WHERE GroupId = {updatedRows.Id}");
            }
        }

        private void UpdateStatuses()
        {
            foreach (var updatedRows in StatusesModels)
            {
                isCompleted = new BaseQueryService().ExecuteQuery<StatusesModel>($"Update [ClientStatuses] SET StatusType = '{updatedRows.Status}', Discount = {updatedRows.Discount} WHERE StatusId = {updatedRows.StatusId}");
            }
        }

        private void UpdateStoreTypes()
        {
            foreach (var updatedRows in StoretypesModels)
            {
                isCompleted = new BaseQueryService().ExecuteQuery<StoretypesModel>($"Update [ClientStoreTypes] SET StoreType = '{updatedRows.StoreType}' WHERE StatusId = {updatedRows.StoreId}");
            }
        }

        private void SeveSettings()
        {

        }

        private void MainWindow()
        {
            UpdateInfo();
            new MainWindow().Show();
            Application.Current.Windows[0].Close();
        }

        private void UpdateInfo()
        {
            Task.Run(() => UpdateGroups());
            Task.Run(() => UpdateStatuses());
            Task.Run(() => UpdateStoreTypes());
        }
    }
}
