using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Threading;

namespace InventoryApp.Service
{
    static class BaseService
    {
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static T GetClass<T>() where T : class
        {
            return (T)Activator.CreateInstance(typeof(T));
        }

        public static string RemoveSpecialCharacters(string value)
        {
            return Regex.Replace(value, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
        }

        public static void DelayAction(int millisecond, Action action)
        {
            var timer = new DispatcherTimer();
            timer.Tick += delegate
            {
                action.Invoke();
                timer.Stop();
            };
            timer.Interval = TimeSpan.FromMilliseconds(millisecond);
            timer.Start();
        }

        public static string GenerateUserName()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
