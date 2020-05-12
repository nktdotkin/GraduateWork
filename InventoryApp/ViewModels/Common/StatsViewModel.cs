using InventoryApp.Models.Common;
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
    class StatsViewModel : ViewModelsBase
    {
        public StatsModel StatsModel { get; set; }
        public ObservableCollection<ShipmentModel> ShipmentModels { get; set; }
        public ChartValues<ShipmentModel> ShipmentStats { get; set; }

        public Func<double, string> FormatterSold { get; set; }
        public Func<double, string> FormatterEarned { get; set; }

        public object MapperShipmentAmount { get; set; }
        public object MapperShipmentPrice { get; set; }

        public List<string> SoldDateList { get; set; }
        public List<string> SoldProductList { get; set; }
        public Dictionary<ProductModel, int> TopProductList { get; set; }
        public Dictionary<ClientModel, int> TopClientList { get; set; }
        public List<string> MessageList { get; set; }

        public RelayCommand GetWeek { get; set; }
        public RelayCommand GetMonth { get; set; }
        public RelayCommand GetYear { get; set; }

        public StatsViewModel()
        {
            MessageList = new LogService().ReadFromFile();
            MessageList.Reverse();
            FormatterSold = value => value + ". штук";
            FormatterEarned = value => value + "$";
            ShipmentStats = new ChartValues<ShipmentModel>();
            MapperShipmentAmount = Mappers.Xy<ShipmentModel>().X((sold, index) => index).Y(sold => sold.Amount);
            MapperShipmentPrice = Mappers.Xy<ShipmentModel>().X((sold, index) => index).Y(sold => (double)sold.TotalPrice);
            GetWeek = new RelayCommand((obj) => GetStatsByDate('w'));
            GetMonth = new RelayCommand((obj) => GetStatsByDate('m'));
            GetYear = new RelayCommand((obj) => GetStatsByDate('y'));
            Update();
        }

        private void Update()
        {
            ShipmentModels = new BaseQueryService().Fill<ShipmentModel>(($"GetShipment"));
            SoldDateList = ShipmentModels.Select(item => item.Date.ToString("dd MMM yyyy")).ToList();
            SoldProductList = ShipmentModels.Select(item => item.Product.Name).ToList();
            GetStatsByDate('d');
            SelectTop();
        }

        private void GetStatsByDate(char c)
        {
            ShipmentStats.Clear();
            SoldDateList.Clear();
            SoldProductList.Clear();
            switch (c)
            {
                case 'w':
                    foreach (var model in ShipmentModels.Where(item => item.Date.DayOfWeek.Equals(DateTime.Now.DayOfWeek)))
                    {
                        ModelToList(model);
                    }
                    break;
                case 'm':
                    foreach (var model in ShipmentModels.Where(item => item.Date.Month.Equals(DateTime.Now.Month)))
                    {
                        ModelToList(model);
                    }
                    break;
                case 'y':
                    foreach (var model in ShipmentModels.Where(item => item.Date.Year.Equals(DateTime.Now.Year)))
                    {
                        ModelToList(model);
                    }
                    break;
                case 'd':
                    foreach (var model in ShipmentModels)
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
            TopClientList = new Dictionary<ClientModel, int>();
            TopProductList = new Dictionary<ProductModel, int>();
            foreach (var values in ShipmentModels.GroupBy(item => item.Client.Id).Select(g => new { Value = g.Sum(s => s.Amount), Key = g.First().Client }).Take(3))
            {
                TopClientList.Add(values.Key, values.Value);
            }
            foreach (var values in ShipmentModels.GroupBy(item => item.Product.Id).Select(g => new { Value = g.Sum(s => s.Amount), Key = g.First().Product }).Take(3))
            {
                TopProductList.Add(values.Key, values.Value);
            }
        }
    }
}
