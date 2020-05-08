using System;
using System.Collections.Generic;
using System.IO;

namespace InventoryApp.Service
{
    class LogService
    {
        public bool SetToFile(string message)
        {
            bool isCompleted = false;
            using (StreamWriter sw = new StreamWriter(System.Environment.CurrentDirectory + @"\Logs\logs.txt", true, System.Text.Encoding.UTF8))
            {
                sw.WriteLineAsync(DateTime.Now + " - " + Properties.Settings.Default.CurrentUser + " - " + message);
            }
            return isCompleted;
        }

        public List<string> ReadFromFile()
        {
            var messageList = new List<string>();
            using (StreamReader sr = new StreamReader(System.Environment.CurrentDirectory + @"\Logs\logs.txt", System.Text.Encoding.UTF8))
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
