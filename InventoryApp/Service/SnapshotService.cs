using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace InventoryApp.Service
{
    class SnapshotService
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["SQLDirect"].ConnectionString;
        private SqlConnection connection = new SqlConnection(connectionString);
        private SqlCommand command;

        public string CreateSnapshot(string fileName)
        {
            string message = null;
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                string snapshotQuery = $"CREATE DATABASE {Path.GetFileName(fileName)} " +
                    $"ON(name = 'INVENTORYAPP', filename = '{fileName}.ss') " +
                    $"AS SNAPSHOT of [{Environment.CurrentDirectory}\\DATABASE\\MainDatabase\\INVENTORYAPP.MDF];";
                command = new SqlCommand(snapshotQuery, connection);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    message = "Создание снепшота завершено.";
                }
                catch (Exception e)
                {
                    message = e.Message;
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                message = "Создание снепшота не завершено.";
            }
            return message;
        }

        public string RestoreFromSnapshot(string fileName)
        {
            string message = null;
            if (!string.IsNullOrWhiteSpace(fileName))
            {
                string snapshotName = fileName.Remove(fileName.LastIndexOf('.'), 3);
                string snapshotQuery = $"ALTER DATABASE[{Environment.CurrentDirectory}\\DATABASE\\MainDatabase\\INVENTORYAPP.MDF] SET OFFLINE WITH ROLLBACK IMMEDIATE;" +
                    $" ALTER DATABASE[{Environment.CurrentDirectory}\\DATABASE\\MainDatabase\\INVENTORYAPP.MDF] SET ONLINE USE master;" +
                    $" RESTORE DATABASE[{Environment.CurrentDirectory}\\DATABASE\\MainDatabase\\INVENTORYAPP.MDF] from DATABASE_SNAPSHOT = '{snapshotName}'";
                command = new SqlCommand(snapshotQuery, connection);
                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    message = "Восстановление из снепшота завершено.";
                }
                catch (Exception e)
                {
                    message = e.Message;
                }
                finally
                {
                    connection.Close();
                }
            }
            else
            {
                message = "Восстановление из снепшота не завершено.";
            }
            return message;
        }
    }
}
