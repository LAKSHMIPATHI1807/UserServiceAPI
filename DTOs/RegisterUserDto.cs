using System.ComponentModel.DataAnnotations;

namespace UserServiceAPI.DTOs
{
    public class RegisterUserDto
    {
        [Required, MaxLength(50)]
        public string UserName { get; set; }

        [Required, MinLength(6), MaxLength(50)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
