using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace WalletManagement.Core.DTOs
{
    public class NoNullElementsAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is not IEnumerable list) return true; // Not a list, so validation passes.
            foreach (var item in list)
            {
                if (item == null) return false;
            }
            return true;
        }
    }
}