using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Reflection;
using InventoryApp.Models.Base;
using System.ComponentModel;
using System.Windows.Input;
using Microsoft.VisualStudio.PlatformUI;

namespace InventoryApp.ViewModels.Base
{
    class BaseQuery : ViewModelsBase
    {
        //private string connectionString = ConfigurationManager.ConnectionStrings["DataBase"].ConnectionString;
        private SqlConnection connection;
        private SqlCommand command;

        public T Fill<T>() where T : class
        {
            try
            {
                var classType = BaseModel.GetClass<T>();
                ObservableCollection<T> model = new ObservableCollection<T>();

                //connection = new SqlConnection(connectionString);
                command = new SqlCommand("ViewInfo", connection);
                command.Parameters.Add(new SqlParameter("@tablename", SqlDbType.VarChar)).Value = "Client";
                command.CommandType = CommandType.StoredProcedure;
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
            }
            //TODO custom materialdesign MessageBox
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            finally
            {
                connection.Close();
            }
            return (T)Activator.CreateInstance(typeof(T));
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

        public ICommand ExitApp
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
