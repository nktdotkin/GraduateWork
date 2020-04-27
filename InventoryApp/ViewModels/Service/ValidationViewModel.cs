using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventoryApp.ViewModels.Service
{
    class ValidationViewModel<T> where T : class
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
