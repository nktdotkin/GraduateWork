using System;
using System.Text.RegularExpressions;

namespace InventoryApp.Models.Base
{
    static class BaseModel
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
    }
}
