using InventoryApp.Models.Base;
using InventoryApp.Security;
using InventoryApp.ViewModels.Base;
using InventoryApp.ViewModels.Service;
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
            get { return string.IsNullOrEmpty(Properties.Settings.Default.UserName) ? BaseModel.GenerateUserName() : Properties.Settings.Default.UserName; }
            set
            {
                if (Properties.Settings.Default.UserName != value && value.Length > 7)
                {
                    Properties.Settings.Default.UserName = value; Properties.Settings.Default.Save(); OnPropertyChanged(nameof(UserName));
                }
                else { Properties.Settings.Default.UserName = BaseModel.GenerateUserName(); }
            }
        }
        #endregion

        #region Functions
        private void SignIn(object param)
        {
            var signInQuery = $"UPDATE [ManagerLogInfo] SET UserName = '{UserName}' WHERE UserName = '{UserName}' and UserPass = '{PasswordSecurity.PasswordEncrypt(param as PasswordBox)}'";
            bool isSignedIn = new BaseQuery().ExecuteQuery<LoginViewModel>(signInQuery);
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
            var signUpQuery = $"INSERT INTO [ManagerLogInfo] (UserName, UserPass) VALUES ('{UserName}', '{PasswordSecurity.PasswordEncrypt(param as PasswordBox)}')";
            bool isSignedUp = new BaseQuery().ExecuteQuery<LoginViewModel>(signUpQuery);
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
