using InventoryApp.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace InventoryApp.Service
{
    internal class BaseQueryService : ViewModelsBase
    {
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["DataBase"].ConnectionString;
        private SqlConnection connection = new SqlConnection(ConnectionString);
        private SqlCommand command;
        private SqlDataReader reader;

        public ObservableCollection<T> Fill<T>(string commandToExecute) where T : class
        {
            var collection = new ObservableCollection<T>();
            try
            {
                command = new SqlCommand(commandToExecute, connection) { CommandType = CommandType.StoredProcedure };
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
                LogService.SetToFile("Ошибка базы: " + e.Message);
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

        public bool ExecuteQuery<T>(string expression, string parametlessProcedureName = null, T instanse = null, bool parametlessQuery = false, bool isCompleted = false) where T : class
        {
            try
            {
                connection = new SqlConnection(ConnectionString);
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
                                command.Parameters.Add(new SqlParameter($"@{fields.Name}", SqlDbType.NVarChar)).Value = fields.GetValue(instanse);
                            }
                        }
                        break;

                    case false:
                        command = new SqlCommand(expression, connection);
                        break;
                }
                var executedudedCommand = command.ExecuteNonQuery();
                if (executedudedCommand > 0)
                    isCompleted = true;
            }
            catch (Exception e)
            {
                LogService.SetToFile("Ошибка базы: " + e.Message);
            }
            connection.Close();
            return isCompleted;
        }
    }
}