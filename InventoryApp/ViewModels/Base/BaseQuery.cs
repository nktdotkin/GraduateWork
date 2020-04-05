using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Input;
using InventoryApp.Models.Base;
using Microsoft.VisualStudio.PlatformUI;

namespace InventoryApp.ViewModels.Base
{
    class BaseQuery : ViewModelsBase
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["DataBase"].ConnectionString;
        private SqlConnection connection = new SqlConnection(connectionString);
        private SqlCommand command;
        private SqlDataReader reader;

        public ObservableCollection<T> Fill<T>(string commandToExecute) where T : class
        {
            var collection = new ObservableCollection<T>();
            try
            {
                command = new SqlCommand(commandToExecute, connection);
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                reader = command.ExecuteReader();
                int readerValueCounter = 0;
                while (reader.Read())
                {
                    var instanse = BaseModel.GetClass<T>();
                    foreach (var fields in instanse.GetType().GetProperties())
                    {
                        var prop = instanse.GetType().GetProperty(fields.Name);
                        prop.SetValue(instanse, Convert.ChangeType(reader.GetValue(readerValueCounter), prop.PropertyType));
                        readerValueCounter++;
                    }
                    collection.Add(instanse);
                    readerValueCounter = 0;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                connection.Close();
            }
            return collection;
        }

        public void Add()
        {

        }

        public void Delete()
        {

        }

        public void Find()
        {

        }

        public bool ExecuteQuery(string Expression)
        {
            bool isCompleted = false;
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                command = new SqlCommand(Expression, connection);              
                var exetudedCommand = command.ExecuteReader();
                isCompleted = exetudedCommand.HasRows;
                connection.Close();
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message + e.HelpLink);
            }
            return isCompleted;
        }

        public ICommand ExitCommand
        {
            get
            {
                return new DelegateCommand(() =>
                {
                    Application.Current.Shutdown();
                });
            }
        }
    }
}
