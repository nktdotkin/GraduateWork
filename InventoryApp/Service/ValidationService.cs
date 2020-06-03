using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventoryApp.Service
{
    internal class ValidationService<T> where T : class
    {
        public List<ValidationResult> ValidateFields(T instanse)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(instanse);
            Validator.TryValidateObject(instanse, context, results, true);
            return results;
        }
    }
}