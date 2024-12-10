using System.ComponentModel.DataAnnotations;

namespace Internet_1.Models
{
    public class RegisterViewModel
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Rol { get; set; } = "User";


        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }
    }
}
