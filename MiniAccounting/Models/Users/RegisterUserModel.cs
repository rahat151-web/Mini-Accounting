using MiniAccounting.Validators;
using System.ComponentModel.DataAnnotations;

namespace MiniAccounting.Models.Users
{
    public class RegisterUserModel
    {

        

        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        //[StrongPassword]
        public string Password { get; set; }

        [Required]
        public string RoleName { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
    }
}
