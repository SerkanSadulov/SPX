using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class UserEntity
    {
        [Required]
        public Guid UserId { get; set; } = Guid.Empty;

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(256)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string UserType { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string ProfilePicture { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string PhoneNumber { get; set; } = string.Empty;

    }
}
