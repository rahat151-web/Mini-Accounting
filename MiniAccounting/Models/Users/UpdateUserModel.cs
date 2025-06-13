namespace MiniAccounting.Models.Users
{
    public class UpdateUserModel
    {
        public string Id { get; set; }  // required for update
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RoleName { get; set; }
        public string PhoneNumber { get; set; }
    }
}
