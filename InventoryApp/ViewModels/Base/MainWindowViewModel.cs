﻿using InventoryApp.Service;
using InventoryApp.ViewModels.Common;
using InventoryApp.Views.Controls;
using InventoryApp.Views.Main;
using InventoryApp.Views.Settings;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace InventoryApp.ViewModels.Base
{
    internal enum TabControl
    {
        Stats,
        Supply,
        Shipment,
        Product,
        Client,
        Provider
    }

    internal class MainWindowViewModel : ViewModelsBase
    {
        public MainWindowViewModel()
        {
            Initialize();
            ClickTabCommand = new RelayCommand((obj) => ClickOnTab());
            BackupCommand = new RelayCommand((obj) => Backup());
            RestoreCommand = new RelayCommand((obj) => Restore());
            SettingsCommand = new RelayCommand((obj) => Settings());
            LogoutCommand = new RelayCommand((obj) => Logout());
            OpenHelperCommand = new RelayCommand((obj) => OpenHelper());
            OpenShipmentFolderCommand = new RelayCommand((obj) => OpenFolder("Отгрузки"));
            OpenSupplyFolderCommand = new RelayCommand((obj) => OpenFolder("Поставки"));
        }

        ~MainWindowViewModel()
        {
            GC.Collect(1, GCCollectionMode.Forced);
        }

        public RelayCommand SettingsCommand { get; set; }
        public RelayCommand ClickTabCommand { get; set; }
        public RelayCommand BackupCommand { get; set; }
        public RelayCommand RestoreCommand { get; set; }
        public RelayCommand LogoutCommand { get; set; }
        public RelayCommand OpenHelperCommand { get; set; }

        public RelayCommand OpenShipmentFolderCommand { get; set; }
        public RelayCommand OpenSupplyFolderCommand { get; set; }

        public NotificationServiceViewModel Notification { get; set; } = new NotificationServiceViewModel();

        private object tablePanel = new Stats();
        private TabControl tabControl;

        public object TablePanel
        {
            get => tablePanel;
            set
            {
                tablePanel = value;
                OnPropertyChanged(nameof(TablePanel));
            }
        }

        public TabControl TabControl
        {
            get => tabControl;
            set
            {
                if (value == tabControl) return;
                tabControl = value;
                OnPropertyChanged(nameof(TabControl));
                OnPropertyChanged(nameof(ProductSwitch));
                OnPropertyChanged(nameof(StatsSwitch));
                OnPropertyChanged(nameof(ShipmentSwitch));
                OnPropertyChanged(nameof(ClientSwitch));
                OnPropertyChanged(nameof(ProviderSwitch));
                OnPropertyChanged(nameof(SupplySwitch));
            }
        }

        public bool ProductSwitch
        {
            get => TabControl == TabControl.Product;
            set => TabControl = value ? TabControl.Product : TabControl;
        }

        public bool StatsSwitch
        {
            get => TabControl == TabControl.Stats;
            set => TabControl = value ? TabControl.Stats : TabControl;
        }

        public bool ShipmentSwitch
        {
            get => TabControl == TabControl.Shipment;
            set => TabControl = value ? TabControl.Shipment : TabControl;
        }

        public bool ClientSwitch
        {
            get => TabControl == TabControl.Client;
            set => TabControl = value ? TabControl.Client : TabControl;
        }

        public bool ProviderSwitch
        {
            get => TabControl == TabControl.Provider;
            set => TabControl = value ? TabControl.Provider : TabControl;
        }

        public bool SupplySwitch
        {
            get => TabControl == TabControl.Supply;
            set => TabControl = value ? TabControl.Supply : TabControl;
        }

        private Client ClientView { get; set; }
        private Provider ProviderView { get; set; }

        private void Initialize()
        {
            ClientView = new Client();
            ProviderView = new Provider();
        }

        private void ClickOnTab()
        {
            switch (TabControl)
            {
                case TabControl.Product:
                    TablePanel = new Views.Controls.Product();
                    break;

                case TabControl.Shipment:
                    TablePanel = new Shipment();
                    break;

                case TabControl.Client:
                    TablePanel = ClientView;
                    break;

                case TabControl.Provider:
                    TablePanel = ProviderView;
                    break;

                case TabControl.Supply:
                    TablePanel = new Supply();
                    break;

                case TabControl.Stats:
                    TablePanel = new Stats();
                    break;
            }
        }

        private void Backup()
        {
            if (Properties.Settings.Default.UserName.Equals("Administrator"))
            {
                var fileDialog = new SaveFileDialog();
                fileDialog.ShowDialog();
                var message = new SnapshotService().CreateSnapshot(fileDialog.FileName);
                Notification.ShowNotification(message);
            }
            else
            {
                Notification.ShowNotification("Ошибка: Доступ запрещен.");
            }
        }

        private void Restore()
        {
            if (Properties.Settings.Default.UserName.Equals("Administrator"))
            {
                var fileDialog = new OpenFileDialog { ValidateNames = false };
                fileDialog.ShowDialog();
                var message = new SnapshotService().RestoreFromSnapshot(fileDialog.SafeFileName);
                Notification.ShowNotification(message);
                BaseService.DelayAction(3000, Logout);
            }
            else
            {
                Notification.ShowNotification("Ошибка: Доступ запрещен.");
            }
        }

        private void OpenHelper()
        {
            try
            {
                Process.Start(Environment.CurrentDirectory + @"\Resources\Help\index.htm");
            }
            catch
            {
                Notification.ShowNotification("Ошибка: Справка не может быть открыта.");
            }
        }

        private void Settings()
        {
            if (Properties.Settings.Default.UserName.Equals("Administrator"))
            {
                new Settings().Show();
                Application.Current.Windows[0]?.Close();
            }
            else
            {
                Notification.ShowNotification("Ошибка: Доступ запрещен.");
            }
        }

        private void OpenFolder(string documentType)
        {
            if (!Directory.Exists(Path.Combine(Environment.CurrentDirectory, documentType)))
            {
                Directory.CreateDirectory(Path.Combine(Environment.CurrentDirectory, documentType));
            }
            Process.Start(Path.Combine(Environment.CurrentDirectory, documentType));
        }

        private void Logout()
        {
            new Login().Show();
            Application.Current.Windows[0]?.Close();
        }
    }
}