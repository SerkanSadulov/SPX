using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class LoginEntity
    {

        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;


    }
}
