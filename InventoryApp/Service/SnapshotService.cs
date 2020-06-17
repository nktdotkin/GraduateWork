using System;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace InventoryApp.Service
{
    internal class SnapshotService
    {
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["SQLDirect"].ConnectionString;
        private readonly SqlConnection connection = new SqlConnection(ConnectionString);
        private SqlCommand command;

        public string CreateSnapshot(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return "Создание снепшота не завершено.";
            var snapshotQuery = $"CREATE DATABASE {Path.GetFileName(fileName)} " +
                                   $"ON(name = 'INVENTORYAPP', filename = '{fileName}.ss') " +
                                   $"AS SNAPSHOT of [{Environment.CurrentDirectory}\\DATABASE\\MainDatabase\\INVENTORYAPP.MDF];";
            command = new SqlCommand(snapshotQuery, connection);
            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                return "Создание снепшота завершено.";
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                connection.Close();
            }
        }

        public string RestoreFromSnapshot(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return "Восстановление из снепшота не завершено.";
            var snapshotName = fileName.Remove(fileName.LastIndexOf('.'), 3);
            var snapshotQuery =
                $"ALTER DATABASE[{Environment.CurrentDirectory}\\DATABASE\\MainDatabase\\INVENTORYAPP.MDF] SET OFFLINE WITH ROLLBACK IMMEDIATE;" +
                $" ALTER DATABASE[{Environment.CurrentDirectory}\\DATABASE\\MainDatabase\\INVENTORYAPP.MDF] SET ONLINE USE master;" +
                $" RESTORE DATABASE[{Environment.CurrentDirectory}\\DATABASE\\MainDatabase\\INVENTORYAPP.MDF] from DATABASE_SNAPSHOT = '{snapshotName}'";
            command = new SqlCommand(snapshotQuery, connection);
            try
            {
                connection.Open();
                command.ExecuteNonQuery();
                return "Восстановление из снепшота завершено.";
            }
            catch (Exception e)
            {
                return e.Message;
            }
            finally
            {
                connection.Close();
            }
        }
    }
}