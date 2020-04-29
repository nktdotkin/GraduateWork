using InventoryApp.ViewModels.Base;
using Microsoft.VisualStudio.PlatformUI;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace InventoryApp.Service
{
    class BaseQueryService : ViewModelsBase
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
                    var instanse = BaseService.GetClass<T>();
                    foreach (var fields in instanse.GetType().GetProperties().OrderBy(x => x.MetadataToken))
                    {
                        var prop = instanse.GetType().GetProperty(fields.Name);
                        if (fields.PropertyType.FullName.Contains("InventoryApp"))
                        {
                            var baseInstance = Activator.CreateInstance(prop.PropertyType);
                            foreach (var baseFields in baseInstance.GetType().GetProperties().OrderBy(x => x.MetadataToken))
                            {
                                var baseProp = baseInstance.GetType().GetProperty(baseFields.Name);
                                baseProp.SetValue(baseInstance, Convert.ChangeType(reader.GetValue(readerValueCounter), baseProp.PropertyType));
                                readerValueCounter++;
                            }
                            prop.SetValue(instanse, Convert.ChangeType(baseInstance, prop.PropertyType));
                        }
                        else
                        {
                            prop.SetValue(instanse, Convert.ChangeType(reader.GetValue(readerValueCounter), prop.PropertyType));
                            readerValueCounter++;
                        }
                    }
                    collection.Add(instanse);
                    readerValueCounter = 0;
                }
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
            }
            connection.Close();
            return collection;
        }

        public bool Add<T>(string tableName, T instanse) where T : class
        {
            return ExecuteQuery(null, $"Set{tableName}", instanse, true);
        }

        public bool Delete(string tableName, int id)
        {
            return ExecuteQuery<BaseQueryService>($"DELETE FROM {tableName} WHERE {tableName}Id = {id}");
        }

        public ObservableCollection<T> Find<T>(ObservableCollection<T> searchInCollection, string searchItem) where T : class
        {
            return searchInCollection;
        }

        public bool ExecuteQuery<T>(string Expression, string parametlessProcedureName = null, T instanse = null, bool parametlessQuery = false, bool isCompleted = false) where T : class
        {
            try
            {
                connection = new SqlConnection(connectionString);
                connection.Open();
                switch (parametlessQuery)
                {
                    case true:
                        command = new SqlCommand(parametlessProcedureName, connection);
                        command.CommandType = CommandType.StoredProcedure;
                        foreach (var fields in instanse.GetType().GetProperties())
                        {
                            if (fields.Name != "Id")
                            {
                                command.Parameters.Add(new SqlParameter($"@{fields.Name}", SqlDbType.VarChar)).Value = fields.GetValue(instanse);
                            }
                        }
                        break;
                    case false:
                        command = new SqlCommand(Expression, connection);
                        break;
                }
                var exetudedCommand = command.ExecuteNonQuery();
                if (exetudedCommand > 0)
                    isCompleted = true;
            }
            catch (Exception e)
            {
                //MessageBox.Show(e.Message);
            }
            connection.Close();
            return isCompleted;
        }

        public string GetAdress(string Adress)
        {
            return ("https://www.google.com/maps/search/?api=1&query=" + Adress?.Replace(" ", "+")) ?? "https://www.google.ru/maps";
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
