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
                    var firstLevelInstanse = BaseService.GetClass<T>();
                    foreach (var firstLevelFields in firstLevelInstanse.GetType().GetProperties().OrderBy(x => x.MetadataToken))
                    {
                        var firstLevelProperty = firstLevelInstanse.GetType().GetProperty(firstLevelFields.Name);
                        if (firstLevelFields.PropertyType.FullName.Contains("InventoryApp"))
                        {
                            var secondLevelInstance = Activator.CreateInstance(firstLevelProperty.PropertyType);
                            foreach (var secondLevelFields in secondLevelInstance.GetType().GetProperties().OrderBy(x => x.MetadataToken))
                            {
                                var secondLevelProperty = secondLevelInstance.GetType().GetProperty(secondLevelFields.Name);
                                if (secondLevelProperty.PropertyType.FullName.Contains("InventoryApp"))
                                {
                                    var thirdLevelInstance = Activator.CreateInstance(secondLevelProperty.PropertyType);
                                    foreach (var thirdLevelFields in thirdLevelInstance.GetType().GetProperties().OrderBy(x => x.MetadataToken))
                                    {
                                        var thirdLevelProperty = thirdLevelInstance.GetType().GetProperty(thirdLevelFields.Name);
                                        thirdLevelProperty.SetValue(thirdLevelInstance, Convert.ChangeType(reader.GetValue(readerValueCounter), thirdLevelProperty.PropertyType));
                                        readerValueCounter++;
                                    }
                                    secondLevelProperty.SetValue(secondLevelInstance, Convert.ChangeType(thirdLevelInstance, secondLevelProperty.PropertyType));
                                }
                                else
                                {
                                    secondLevelProperty.SetValue(secondLevelInstance, Convert.ChangeType(reader.GetValue(readerValueCounter), secondLevelProperty.PropertyType));
                                    readerValueCounter++;
                                }
                            }
                            firstLevelProperty.SetValue(firstLevelInstanse, Convert.ChangeType(secondLevelInstance, firstLevelProperty.PropertyType));
                        }
                        else
                        {
                            firstLevelProperty.SetValue(firstLevelInstanse, Convert.ChangeType(reader.GetValue(readerValueCounter), firstLevelProperty.PropertyType));
                            readerValueCounter++;
                        }
                    }
                    collection.Add(firstLevelInstanse);
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

        //public bool AddWithparameters(string tableName, List<string> parameters)
        //{
        //    return ExecuteQuery<BaseQueryService>(null, null, null, false, parameters, tableName, true);
        //}

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
                            if (fields.Name != "Id" && !fields.PropertyType.FullName.Contains("InventoryApp"))
                            {
                                command.Parameters.Add(new SqlParameter($"@{fields.Name}", SqlDbType.VarChar)).Value = fields.GetValue(instanse);
                            }
                        }
                        break;
                    case false:
                        command = new SqlCommand(Expression, connection);
                        //switch (listQuery)
                        //{
                        //    case true:
                        //        command = new SqlCommand($"INSERT INTO {tableName}(Column)VALUES(@Column)", connection);
                        //        command.Parameters.Add("@Column", SqlDbType.VarChar);
                        //        foreach (var value in list)
                        //        {
                        //            command.Parameters["@Column"].Value = value;
                        //        }
                        //        break;
                        //    case false:
                        //        command = new SqlCommand(Expression, connection);
                        //        break;
                        //}
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
