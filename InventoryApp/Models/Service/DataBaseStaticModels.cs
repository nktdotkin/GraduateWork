﻿using System.Collections.Generic;

namespace InventoryApp.Models.Service
{
    //Create with get from database
    class DataBaseStaticModels
    {
        public List<string> Statuses { get; set; }
        public List<string> StoreTypes { get; set; }
        public List<string> Groups { get; set; }

        public DataBaseStaticModels()
        {
            Statuses = new List<string>() { "Default", "Bronze", "Silver", "Gold" };
            StoreTypes = new List<string>() { "Small shop", "Supermarket", "Trade tent", "Warehouse" };
            Groups = new List<string>() { "Default", "Electronics", "Computers", "Appliances", "House", "Auto", "Work" };
        }
    }
}