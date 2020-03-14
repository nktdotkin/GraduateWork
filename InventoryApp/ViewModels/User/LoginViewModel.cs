using InventoryApp.ViewModels.Base;
using InventoryControl.Models.Base;
using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Controls;
using InventoryApp.Security;
using System.Windows;
using System.Data;
using InventoryApp.Views.Main;

namespace InventoryApp.ViewModels.User
{
    class LoginViewModel : ViewModelsBase
    {
        public LoginViewModel()
        {
            SignInCommand = new RelayCommand(SignIn);
            SingUpCommand = new RelayCommand(SingUp);
        }

        public RelayCommand SignInCommand { get ; set ; }
        public RelayCommand SingUpCommand { get; set; }

        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DataBase"].ConnectionString;
        private SqlConnection connection;
        private SqlCommand command;
        private SqlDataReader reader;

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

        private void SignIn(object param)
        {
            var passwordBox = param as PasswordBox;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                command = new SqlCommand($"SELECT UserName, UserPass FROM [ManagerLogInfo] WHERE UserName = '{UserName}' and UserPass = '{PasswordSecurity.PasswordEncrypt(passwordBox)}'", connection);
                reader = command.ExecuteReader();
                if (reader.Read())
                {
                    Properties.Settings.Default.CurrentUser = UserName;
                    new MainWindow().Show();
                    Application.Current.Windows[0].Close();
                }
                else
                {
                    MessageBox.Show("Unknown Credentials. Try Again.");
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                reader.Close();
                connection.Close();
            }
        }

        private void SingUp(object param)
        {
            var passwordBox = param as PasswordBox;
            try
            {
                connection = new SqlConnection(connectionString);
                command = new SqlCommand("CreateManager", connection);
                command.Parameters.Add(new SqlParameter("@userName", SqlDbType.VarChar)).Value = UserName;
                command.Parameters.Add(new SqlParameter("@userPass", SqlDbType.VarChar)).Value = PasswordSecurity.PasswordEncrypt(passwordBox);
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                command.ExecuteNonQuery();
                Properties.Settings.Default.UserName = UserName;
                MessageBox.Show("You succesfully registered.");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                connection.Close();
            }
        }

        private string GenerateUserName()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
