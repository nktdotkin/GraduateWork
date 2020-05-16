using System;
using System.Collections.Generic;
using System.IO;

namespace InventoryApp.Service
{
    static class LogService
    {
        private static string filePath = Environment.CurrentDirectory + @"\Logs";

        public static bool SetToFile(string message)
        {
            bool isCompleted = false;
            try
            {
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                    File.Create(filePath + @"\logs.txt");
                }
                else if (Directory.Exists(filePath))
                {
                    using (StreamWriter sw = new StreamWriter(filePath + @"\logs.txt", true, System.Text.Encoding.UTF8))
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
            try
            {
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                    File.Create(filePath + @"\logs.txt");
                }
                else if (Directory.Exists(filePath))
                {
                    using (StreamReader sr = new StreamReader(filePath + @"\logs.txt", System.Text.Encoding.UTF8))
                    {
                        string tempLine;
                        while ((tempLine = sr.ReadLine()) != null)
                        {
                            messageList.Add(tempLine);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //ignored
            }
            return messageList;
        }
    }
}
