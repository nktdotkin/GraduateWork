﻿using InventoryApp.Models.Product;
using InventoryApp.ViewModels.Base;
using InventoryControl.Models.Base;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System;
using System.Data;
using System.Threading.Tasks;

namespace InventoryApp.ViewModels.Product
{
    class ShipmentViewModel : ViewModelsBase
    {
        private const string CommandToExecute = "GetShipment";
        private const string TableName = "Shipment";

        public ObservableCollection<ShipmentModel> ShipmentModels { get; set; }
        public RelayCommand DeleteCommand { get; set; }
        public ShipmentModel SelectedItem { get; set; }

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

        public ShipmentViewModel()
        {
            ShipmentModels = new ObservableCollection<ShipmentModel>();
            DeleteCommand = new RelayCommand((obj) => Delete());
            Update();
        }

        private void Delete()
        {
            new BaseQuery().Delete(TableName, SelectedItem.Id);
            ShipmentModels.Remove(SelectedItem);;
        }

        private void Update()
        {
            ShipmentModels = new BaseQuery().Fill<ShipmentModel>(CommandToExecute);
            OnPropertyChanged(nameof(ShipmentModels));
        }

        private void Find(string searchText)
        {
            var searchResult = ShipmentModels.Where(items =>
            items.Product.Name.Contains(searchText) ||
            items.Product.Group.Contains(searchText) ||
            items.Product.Description.Contains(searchText) ||
            items.Client.Name.Contains(searchText) ||
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
    }
}