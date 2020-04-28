using InventoryApp.Models.Base;
using InventoryApp.Models.Product;
using InventoryApp.Models.User;
using InventoryApp.ViewModels.Base;
using InventoryApp.ViewModels.Service;
using InventoryApp.ViewModels.User;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

namespace InventoryApp.ViewModels.Product
{
    class ShipmentViewModel : ViewModelsBase
    {
        public ShipmentViewModel()
        {
            GC.Collect(1, GCCollectionMode.Forced);
            ShipmentModels = new ObservableCollection<ShipmentModel>();
            DeleteCommand = new RelayCommand((obj) => Delete());
            AddCommand = new RelayCommand((obj) => Add());
            AddNewShipment = new ShipmentModel();
            Notification = new NotificationServiceViewModel();
            ModelValidation = new ValidationViewModel<ShipmentModel>();
            Update();
        }

        #region Properties
        private const string TableName = "Shipment";
        public ObservableCollection<ShipmentModel> ShipmentModels { get; set; }
        public ObservableCollection<ClientModel> ClientModels { get; set; }
        public ObservableCollection<ProductModel> ProductModels { get; set; }

        private ValidationViewModel<ShipmentModel> ModelValidation { get; set; }
        public NotificationServiceViewModel Notification { get; set; }

        public RelayCommand DeleteCommand { get; set; }
        public RelayCommand AddCommand { get; set; }
        public RelayCommand AddProductImageCommand { get; set; }

        private ShipmentModel selectedItem;
        public ShipmentModel SelectedItem
        {
            get => selectedItem;
            set
            {
                if (value != selectedItem)
                {
                    selectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));
                }
            }
        }

        public ShipmentModel AddNewShipment { get; set; }

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
            ClientModels = new ClientViewModel().Update();
            ProductModels = new ProductViewModel().Update();
            ShipmentModels = new BaseQuery().Fill<ShipmentModel>(($"Get{TableName}"));
            OnPropertyChanged(nameof(ShipmentModels));
        }

        private void Delete()
        {
            if (SelectedItem?.Id != null)
            {
                bool isCompleted = new BaseQuery().Delete(TableName, SelectedItem.Id);
                if (isCompleted)
                {
                    Notification.ShowNotification("Info: Shipment information is deleted.");
                }
                else
                {
                    Notification.ShowNotification("Error: Deleting information failed.");
                }
            }
            else
            {
                Notification.ShowNotification("Error: No shipment information selected.");
            }
            Update();
        }

        private void Add()
        {
            var errorList = ModelValidation.ValidateFields(AddNewShipment);
            if (errorList.Any())
            {
                Notification.ShowListNotification(errorList);
            }
            else
            {
                if (AddNewShipment.Amount > ProductModels.Where(item => item.Id == AddNewShipment.Product.Id).Select(item => item.Amount).First())
                {
                    Notification.ShowNotification($"Info: Not enougth in stock (or nothing selected).");
                }
                else
                {
                    bool isCompleted = new BaseQuery().ExecuteQuery<ShipmentModel>($"INSERT INTO {TableName} VALUES ('{AddNewShipment.Date}', {AddNewShipment.Amount}, {AddNewShipment.Product.Id}, {AddNewShipment.Client.Id})");
                    if (isCompleted)
                    {
                        Notification.ShowNotification($"Info: Shipment for {AddNewShipment.Client.Name} is added.");
                        new BaseQuery().ExecuteQuery<ShipmentModel>($"Update Product set ProductAmount={ProductModels.Where(item => item.Id == AddNewShipment.Product.Id).Select(item => item.Amount).First() - AddNewShipment.Amount} where ProductId = {AddNewShipment.Product.Id}");
                    }
                    else
                    {
                        Notification.ShowNotification("Error: Adding new shipment information failed.");
                    }
                }
            }
            Update();
        }

        private void Find(string searchText)
        {
            var searchResult = ShipmentModels.Where(items =>
            items.Product.Name.Contains(searchText) ||
            items.Product.Group.Contains(searchText) ||
            items.Product.Description.Contains(searchText) ||
            items.Client.Name.Contains(searchText) ||
            items.Client.Phone.Contains(searchText) ||
            items.Client.Surname.Contains(searchText)
            ).ToList();
            if (!ShipmentModels.SequenceEqual(searchResult))
            {
                ShipmentModels.Clear();
                foreach (var items in searchResult)
                {
                    ShipmentModels.Add(items);
                }
                OnPropertyChanged(nameof(ShipmentModels));
            }
        }
        #endregion
    }
}
