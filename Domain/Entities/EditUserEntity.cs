using System.ComponentModel.DataAnnotations;


namespace Domain.Entities
{
    public class EditUserEntity
    {
        public Guid UserId { get; set; } = Guid.Empty;

        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(256)]
        public string Password { get; set; } = string.Empty;

        public string UserType { get; set; } = string.Empty;

        public string ProfilePicture { get; set; } = string.Empty;
    }
}
