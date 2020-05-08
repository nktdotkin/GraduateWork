using InventoryApp.Service;
using InventoryApp.ViewModels.Product;
using InventoryApp.ViewModels.User;
using InventoryApp.Views.Controls;
using InventoryApp.Views.Main;
using InventoryApp.Views.Settings;
using System.Windows;

namespace InventoryApp.ViewModels.Base
{
    enum TabControl
    {
        Stats,
        Supply,
        Shipment,
        Product,
        Client,
        Provider
    }

    class MainWindowViewModel : ViewModelsBase
    {
        public MainWindowViewModel()
        {
            tablePanel = new Stats();
            ClickTabCommand = new RelayCommand((obj) => ClickOnTab());
            BackupCommand = new RelayCommand((obj) => Backup());
            RestoreCommand = new RelayCommand((obj) => Restore());
            SettingsCommand = new RelayCommand((obj) => Settings());
            LogoutCommand = new RelayCommand((obj) => Logout());
        }

        public RelayCommand SettingsCommand { get; set; }
        public RelayCommand ClickTabCommand { get; set; }
        public RelayCommand BackupCommand { get; set; }
        public RelayCommand RestoreCommand { get; set; }
        public RelayCommand LogoutCommand { get; set; }


        private object tablePanel;
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
                if (value != tabControl)
                {
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
        }

        public bool ProductSwitch
        {
            get { return TabControl == TabControl.Product; }
            set { TabControl = value ? TabControl.Product : TabControl; }
        }

        public bool StatsSwitch
        {
            get { return TabControl == TabControl.Stats; }
            set { TabControl = value ? TabControl.Stats : TabControl; }
        }

        public bool ShipmentSwitch
        {
            get { return TabControl == TabControl.Shipment; }
            set { TabControl = value ? TabControl.Shipment : TabControl; }
        }

        public bool ClientSwitch
        {
            get { return TabControl == TabControl.Client; }
            set { TabControl = value ? TabControl.Client : TabControl; }
        }

        public bool ProviderSwitch
        {
            get { return TabControl == TabControl.Provider; }
            set { TabControl = value ? TabControl.Provider : TabControl; }
        }

        public bool SupplySwitch
        {
            get { return TabControl == TabControl.Supply; }
            set { TabControl = value ? TabControl.Supply : TabControl; }
        }

        private void ClickOnTab()
        {
            switch (TabControl)
            {
                case TabControl.Product:
                    TablePanel = new Views.Controls.Product();
                    break;
                case TabControl.Shipment:
                    TablePanel = new Views.Controls.Shipment();
                    break;
                case TabControl.Client:
                    TablePanel = new Views.Controls.Client();
                    break;
                case TabControl.Provider:
                    TablePanel = new Views.Controls.Provider();
                    break;
                case TabControl.Supply:
                    TablePanel = new Views.Controls.Supply();
                    break;
                case TabControl.Stats:
                    TablePanel = new Views.Controls.Stats();
                    break;
            }
        }

        private void Backup()
        {
            if (new BaseQueryService().ExecuteQuery<MainWindowViewModel>(""))
            {
                MessageBox.Show("Backup complete. Path to backup folder: " + Properties.Settings.Default.RestorePath);
            }
            else
            {
                MessageBox.Show("Backup failed.");
            }
        }

        private void Restore()
        {
            if (new BaseQueryService().ExecuteQuery<MainWindowViewModel>(""))
            {
                MessageBox.Show("Restore complete. Please reload application.");
            }
            else
            {
                MessageBox.Show("Restore failed.");
            }
        }

        private void Settings()
        {
            new Settings().Show();
            Application.Current.Windows[0].Close();
        }

        private void Logout()
        {
            new Login().Show();
            Application.Current.Windows[0].Close();
        }
    }
}
