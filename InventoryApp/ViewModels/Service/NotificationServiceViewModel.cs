using InventoryApp.Models.Base;
using InventoryApp.ViewModels.Base;

namespace InventoryApp.ViewModels.Service
{
    class NotificationServiceViewModel : ViewModelsBase
    {
        private string notificationMessage;
        public string NotificationMessage
        {
            get => notificationMessage;
            set
            {
                if (value != notificationMessage)
                {
                    notificationMessage = value;
                    OnPropertyChanged(nameof(NotificationMessage));
                }
            }
        }

        private bool isActive;
        public bool IsActive
        {
            get => isActive;
            set
            {
                if (value != isActive)
                {
                    isActive = value;
                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }

        public void ShowNotification(string message)
        {
            NotificationMessage = message;
            IsActive = true;
            BaseModel.DelayAction(Properties.Settings.Default.NotificationTimer, () => HideNotification());
        }

        private void HideNotification()
        {
            IsActive = false;
        }
    }
}
