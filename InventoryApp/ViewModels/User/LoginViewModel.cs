using InventoryApp.Service;
using InventoryApp.ViewModels.Base;
using InventoryApp.ViewModels.Common;
using InventoryApp.Views.Main;
using System.Windows;
using System.Windows.Controls;

namespace InventoryApp.ViewModels.User
{
    class LoginViewModel : ViewModelsBase
    {
        public LoginViewModel()
        {
            SignInCommand = new RelayCommand(SignIn);
            SingUpCommand = new RelayCommand(SingUp);
            Notification = new NotificationServiceViewModel();
        }

        #region Properties

        public RelayCommand SignInCommand { get; set; }
        public RelayCommand SingUpCommand { get; set; }
        public NotificationServiceViewModel Notification { get; set; }

        public string UserName
        {
            get { return string.IsNullOrEmpty(Properties.Settings.Default.UserName) ? BaseService.GenerateUserName() : Properties.Settings.Default.UserName; }
            set
            {
                if (Properties.Settings.Default.UserName != value && value.Length > 7)
                {
                    Properties.Settings.Default.UserName = value; Properties.Settings.Default.Save(); OnPropertyChanged(nameof(UserName));
                }
                else { Properties.Settings.Default.UserName = BaseService.GenerateUserName(); }
            }
        }
        #endregion

        #region Functions
        private void SignIn(object param)
        {
            var signInQuery = $"UPDATE [ManagerLogInfo] SET UserName = '{UserName}' WHERE UserName = '{UserName}' and UserPass = '{PasswordSecurityService.PasswordEncrypt(param as PasswordBox)}'";
            bool isSignedIn = new BaseQueryService().ExecuteQuery<LoginViewModel>(signInQuery);
            if (isSignedIn)
            {
                Properties.Settings.Default.CurrentUser = UserName;
                new MainWindow().Show();
                Application.Current.Windows[0].Close();
            }
            else
            {
                Notification.ShowNotification("Error: Login failed.");
            }
        }

        private void SingUp(object param)
        {
            var signUpQuery = $"INSERT INTO [ManagerLogInfo] (UserName, UserPass) VALUES ('{UserName}', '{PasswordSecurityService.PasswordEncrypt(param as PasswordBox)}')";
            bool isSignedUp = new BaseQueryService().ExecuteQuery<LoginViewModel>(signUpQuery);
            if (isSignedUp)
            {
                Notification.ShowNotification("Info: Successfully registered.");
            }
            else
            {
                Notification.ShowNotification("Error: Registration failed.");
            }
        }
        #endregion
    }
}
