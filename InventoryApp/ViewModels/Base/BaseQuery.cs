﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
                        if (fields.PropertyType.FullName.Contains("InventoryApp"))
                        {
                            //do something with this
                            var baseInstance = Activator.CreateInstance(prop.PropertyType);
                            //set values to class object in model
                            foreach (var baseFields in baseInstance.GetType().GetProperties())
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

        public void Delete(string tableName, int id)
        {
            try
            {
                command = new SqlCommand($"DELETE FROM {tableName} WHERE {tableName}Id = {id}", connection);
                connection.Open();
                command.ExecuteNonQuery();
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

        public ObservableCollection<T> Find<T>(ObservableCollection<T> searchInCollection, string searchItem) where T : class
        {
            //var tempCollection = new ObservableCollection<T>();
            //foreach (var fields in searchInCollection)
            //{
            //    var prop = fields.GetType();
            //    if (prop.GetField(prop.Name).GetValue(fields).ToString().Contains(searchItem))
            //    {
            //        tempCollection.Add(prop.GetField(prop.Name).GetValue(fields));
            //    }
            //}
            return searchInCollection;
        }

        //redo because of recover\save DB
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

        //add to configure columns in grid
        public bool BorderVisible()
        {
            return false;
        }
    }
}