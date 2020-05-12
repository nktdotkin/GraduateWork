using InventoryApp.Service;
using InventoryApp.ViewModels.Base;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace InventoryApp.ViewModels.Common
{
    class NotificationServiceViewModel : ViewModelsBase
    {
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

        private SnackbarMessageQueue notificationMessageQueue;
        public SnackbarMessageQueue NotificationMessageQueue
        {
            get => notificationMessageQueue;
            set
            {
                if (value != notificationMessageQueue)
                {
                    notificationMessageQueue = value;
                    OnPropertyChanged(nameof(NotificationMessageQueue));
                }
            }
        }

        public void ShowListNotification(List<ValidationResult> Messages)
        {
            Task.Run(() => ShowNotificationAsync(Messages));
        }

        private void ShowNotificationAsync(List<ValidationResult> Messages)
        {
            NotificationMessageQueue = new SnackbarMessageQueue(TimeSpan.FromMilliseconds(3000));
            IsActive = true;
            foreach (var message in Messages)
            {
                NotificationMessageQueue.Enqueue(message);
                new LogService().SetToFile(message.ToString());
            }
        }

        public void ShowNotification(string message)
        {
            Task.Run(() => ShowListNotification(new List<ValidationResult>() { new ValidationResult(message) }));
        }
    }
}
