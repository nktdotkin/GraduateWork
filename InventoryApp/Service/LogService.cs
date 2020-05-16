using System;
using System.Collections.Generic;
using System.IO;

namespace InventoryApp.Service
{
    static class LogService
    {
        private static string filePath = Environment.CurrentDirectory + @"\Logs\logs.txt";

        public static bool SetToFile(string message)
        {
            bool isCompleted = false;
            try
            {
                if (!File.Exists(filePath))
                {
                    File.Create(filePath);
                }
                else if (File.Exists(filePath))
                {
                    using (StreamWriter sw = new StreamWriter(filePath, true, System.Text.Encoding.UTF8))
                    {
                        sw.WriteLine(DateTime.Now + " - " + Properties.Settings.Default.CurrentUser + " - " + message);
                        sw.Dispose();
                        sw.Close();
                    }
                }
            }
            catch (Exception e)
            {
                //ignored
            }
            return isCompleted;
        }

        public static List<string> ReadFromFile()
        {
            var messageList = new List<string>();
            using (StreamReader sr = new StreamReader(filePath, System.Text.Encoding.UTF8))
            {
                string tempLine;
                while ((tempLine = sr.ReadLine()) != null)
                {
                    messageList.Add(tempLine);
                }
            }
            return messageList;
        }
    }
}
