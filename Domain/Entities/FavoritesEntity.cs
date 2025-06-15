using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class FavoritesEntity
    {
        [Required]
        public Guid FavoriteID { get; set; } = Guid.Empty;

        [Required]
        public Guid UserId { get; set; } = Guid.Empty;

        [Required]
        public Guid ServiceID { get; set; } = Guid.Empty;

        [Required]
        public DateTime AddedOn { get; set; } = DateTime.Now;



    }
}
