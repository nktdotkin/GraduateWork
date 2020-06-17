using InventoryApp.Models.Product;
using InventoryApp.Models.User;
using InventoryApp.Service;
using InventoryApp.ViewModels.Base;
using LiveCharts;
using LiveCharts.Configurations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace InventoryApp.ViewModels.Common
{
    internal class StatsViewModel : ViewModelsBase
    {
        public ObservableCollection<ShipmentModel> ShipmentModels { get; set; }
        public ChartValues<ShipmentModel> ShipmentStats { get; set; } = new ChartValues<ShipmentModel>();

        public Func<double, string> FormatterSold { get; set; } = value => value + ". штук";
        public Func<double, string> FormatterEarned { get; set; } = value => value + " BYN";

        public object MapperShipmentAmount { get; set; }
        public object MapperShipmentPrice { get; set; }

        public List<string> SoldDateList { get; set; }
        public List<string> SoldProductList { get; set; }
        public Dictionary<ProductModel, int> TopProductList { get; set; } = new Dictionary<ProductModel, int>();
        public Dictionary<ClientModel, int> TopClientList { get; set; } = new Dictionary<ClientModel, int>();
        public List<string> MessageList { get; set; } = LogService.ReadFromFile();

        public RelayCommand GetDay { get; set; }
        public RelayCommand GetWeek { get; set; }
        public RelayCommand GetMonth { get; set; }

        public StatsViewModel()
        {
            MessageList.Reverse();
            MapperShipmentAmount = Mappers.Xy<ShipmentModel>().X((sold, index) => index).Y(sold => sold.Amount);
            MapperShipmentPrice = Mappers.Xy<ShipmentModel>().X((sold, index) => index).Y(sold => (double)sold.TotalPrice);
            GetDay = new RelayCommand((obj) => GetStatsByDate('d'));
            GetWeek = new RelayCommand((obj) => GetStatsByDate('w'));
            GetMonth = new RelayCommand((obj) => GetStatsByDate('m'));
            Update();
        }

        private void Update()
        {
            ShipmentModels = new BaseQueryService().Fill<ShipmentModel>(($"Get{DataBaseTableNames.Shipment}"));
            SoldDateList = ShipmentModels.Select(item => item.Date.ToString("dd MMM yyyy")).ToList();
            SoldProductList = ShipmentModels.Select(item => item.Product.Name).ToList();
            GetStatsByDate('w');
            SelectTop();
        }

        private void GetStatsByDate(char c)
        {
            ShipmentStats.Clear();
            SoldDateList.Clear();
            SoldProductList.Clear();
            switch (c)
            {
                case 'd':
                    foreach (var model in ShipmentModels.Where(item => item.Date.DayOfYear.Equals(DateTime.Now.DayOfYear) && item.Date.Year.Equals(DateTime.Now.Year)))
                    {
                        ModelToList(model);
                    }
                    break;

                case 'w':
                    foreach (var model in ShipmentModels.Where(item => item.Date.DayOfWeek.Equals(DateTime.Now.DayOfWeek) && item.Date.Year.Equals(DateTime.Now.Year)))
                    {
                        ModelToList(model);
                    }
                    break;

                case 'm':
                    foreach (var model in ShipmentModels.Where(item => item.Date.Month.Equals(DateTime.Now.Month) && item.Date.Year.Equals(DateTime.Now.Year)))
                    {
                        ModelToList(model);
                    }
                    break;
            }
        }

        private void ModelToList(ShipmentModel model)
        {
            ShipmentStats.Add(model);
            SoldDateList.Add(model.Date.ToString("dd MMM yyyy"));
            SoldProductList.Add(model.Product.Name);
        }

        private void SelectTop()
        {
            foreach (var values in ShipmentModels.GroupBy(item => item.Client.Id).
                Select(g => new { Value = g.Sum(s => s.Amount), Key = g.First().Client }).
                Take(3).OrderByDescending(c => c.Value))
            {
                TopClientList.Add(values.Key, values.Value);
            }
            foreach (var values in ShipmentModels.GroupBy(item => item.Product.Id).
                Select(g => new { Value = g.Sum(s => s.Amount), Key = g.First().Product }).
                Take(3).OrderByDescending(c => c.Value))
            {
                TopProductList.Add(values.Key, values.Value);
            }
        }
    }
}