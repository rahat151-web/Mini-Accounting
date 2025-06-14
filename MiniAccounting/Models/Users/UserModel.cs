using System.ComponentModel.DataAnnotations;

namespace MiniAccounting.Models.Users
{
    public class UserModel
    {
        [Required]
        public string Id { get; set; }  // required for update

        
        public string? UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string? Password { get; set; }

        [Required]
        public string RoleName { get; set; }

        [Required] 
        public string PhoneNumber { get; set; }

    }
}
