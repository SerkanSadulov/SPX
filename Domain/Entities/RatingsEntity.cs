using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class RatingsEntity
    {
        [Required]
        public Guid RatingID { get; set; } = Guid.Empty;

        [Required]
        public Guid ServiceID { get; set; } = Guid.Empty;

        [Required]
        public Guid ClientID { get; set; } = Guid.Empty;

        [Required]
        public byte Rating { get; set; }

        [StringLength(int.MaxValue)]
        public string? Comment { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.Now;
    }
}
