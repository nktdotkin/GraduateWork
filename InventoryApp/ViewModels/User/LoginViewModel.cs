using InventoryApp.ViewModels.Base;
using System;
using System.Linq;
using System.Windows.Controls;
using InventoryApp.Security;
using System.Windows;
using System.Data;
using InventoryApp.Views.Main;
using InventoryApp.Models.Base;

namespace InventoryApp.ViewModels.User
{
    class LoginViewModel : ViewModelsBase
    {
        public LoginViewModel()
        {
            SignInCommand = new RelayCommand(SignIn);
            SingUpCommand = new RelayCommand(SingUp);
        }

        #region Properties
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

        public RelayCommand SignInCommand { get; set; }
        public RelayCommand SingUpCommand { get; set; }

        public string UserName
        {
            get { return string.IsNullOrEmpty(Properties.Settings.Default.UserName) ? GenerateUserName() : Properties.Settings.Default.UserName; }
            set
            {
                if (Properties.Settings.Default.UserName != value && value.Length > 7)
                {
                    Properties.Settings.Default.UserName = value; Properties.Settings.Default.Save(); OnPropertyChanged(nameof(UserName));
                }
                else { Properties.Settings.Default.UserName = GenerateUserName(); }
            }
        }
        #endregion

        #region Functions
        private void SignIn(object param)
        {
            var signInQuery = $"SELECT UserName, UserPass FROM [ManagerLogInfo] WHERE UserName = '{UserName}' and UserPass = '{PasswordSecurity.PasswordEncrypt(param as PasswordBox)}'";
            bool isSignedIn = new BaseQuery().ExecuteQuery<LoginViewModel>(signInQuery);
            if (isSignedIn)
            {
                Properties.Settings.Default.CurrentUser = UserName;
                new MainWindow().Show();
                Application.Current.Windows[0].Close();
            }
            else
            {
                NotificationMessage = $"Error: Login failed.";
                IsActive = true;
            }
            //Set timer as setting
            BaseModel.DelayAction(Properties.Settings.Default.NotificationTimer, () => HideNotification());
        }

        private void SingUp(object param)
        {
            var signUpQuery = $"INSERTI INTO [ManagerLogInfo] VALUES ({UserName}, {PasswordSecurity.PasswordEncrypt(param as PasswordBox)})";
            bool isSignedUp = new BaseQuery().ExecuteQuery<LoginViewModel>(signUpQuery);
            if (isSignedUp)
            {
                NotificationMessage = $"Info: Successfully registered.";
                IsActive = true;
            }
            else
            {
                NotificationMessage = $"Error: Registration failed.";
                IsActive = true;
            }
            //Set timer as setting
            BaseModel.DelayAction(Properties.Settings.Default.NotificationTimer, () => HideNotification());
        }

        //Remove?
        private string GenerateUserName()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void HideNotification()
        {
            IsActive = false;
        }
        #endregion
    }
}
