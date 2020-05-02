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
            IsFirstStart();
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
                ManagerAccess(UserName);
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
            byte isAdmin = 0;
            if (Properties.Settings.Default.FirstStart)
            {
                isAdmin = 1;
                Properties.Settings.Default.FirstStart = false;
                Properties.Settings.Default.Save();
            }
            var signUpQuery = $"INSERT INTO [ManagerLogInfo] (UserName, UserPass, UserStatus) VALUES ('{UserName}', '{PasswordSecurityService.PasswordEncrypt(param as PasswordBox)}', {isAdmin})";
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

        private bool IsFirstStart()
        {
            if (Properties.Settings.Default.FirstStart)
            {
                Properties.Settings.Default.UserName = "Administrator";
                Properties.Settings.Default.Save();
                Notification.ShowNotification("Info: Set up admin password.");
            }
            return Properties.Settings.Default.FirstStart;
        }

        private void ManagerAccess(string userName)
        {
            Properties.Settings.Default.CurrentUser = userName;
            if (userName.Equals("Administrator"))
            {
                Properties.Settings.Default.ManagerVisibility = true;
            }
            else
            {
                Properties.Settings.Default.ManagerVisibility = false;
            }
            Properties.Settings.Default.Save();
        }
        #endregion
    }
}
