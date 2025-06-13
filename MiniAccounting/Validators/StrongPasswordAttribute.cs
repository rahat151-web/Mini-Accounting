using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MiniAccounting.Validators
{
    public class StrongPasswordAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var password = value as string;



            // Example: Minimum 8 characters, at least one upper, one lower, one digit, one special char
            var regex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{8,}$");


            if (!regex.IsMatch(password))
                return new ValidationResult("Password must be at least 8 characters long and contain upper, lower, number, and special character.");

            return ValidationResult.Success;
        }
    }
}
