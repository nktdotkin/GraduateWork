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
            try
            {
                using (StreamWriter sw = new StreamWriter(System.Environment.CurrentDirectory + @"\Logs\logs.txt", true, System.Text.Encoding.UTF8))
                {
                    sw.WriteLine(DateTime.Now + " - " + Properties.Settings.Default.CurrentUser + " - " + message);
                    sw.Dispose();
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                //ignored
            }
            return isCompleted;
        }

        public List<string> ReadFromFile(string filePath = @"\Logs\logs.txt")
        {
            var messageList = new List<string>();
            using (StreamReader sr = new StreamReader(System.Environment.CurrentDirectory + filePath, System.Text.Encoding.UTF8))
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
